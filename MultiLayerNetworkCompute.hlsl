#define LayerInputSizeIx 0
#define LayerInputStartIx 1
#define LayerWeightStartIx 2
#define LayerOutputStartIx 3
#define MAX_LAYERS 16

cbuffer consts {
	int dispatchDim_x;
	int totalImageCount;
	int totalLayerCount;
	int currentLayerIx;
	int currentImageIx;
	int runBackprop;
	int bpCurrentLayerSize;
	int finalOutputCount;

	//max layer count of 16 for now -- easy to adjust -- minimal memory impact in constant buffer
	int4 allLayers[MAX_LAYERS];
};

cbuffer backprop {
	float bpLearningRate;
	float bpMomentum;
	float correctTarget;
	float incorrectTarget;
};


RWStructuredBuffer<float> image_data;
RWStructuredBuffer<float> weight_data;
RWStructuredBuffer<float> in_out_data;
RWStructuredBuffer<float> node_error_data;
RWStructuredBuffer<int> image_guesses;
RWStructuredBuffer<int> image_labels;

#define groupDim_x 512
groupshared float sdata[groupDim_x];
groupshared int mdata[groupDim_x];


float bipolarSigmoid(float val)
{
	return 2/(1 + exp(-val)) - 1;
}

float plainSigmoid(float val)
{
	return 1/(1 + exp(-val));
}

void finishSharedDataSum(int tid, RWStructuredBuffer<float> outputArray, int outputIx, bool activate)
{
	if (groupDim_x>= 512) { if (tid < 256) { sdata[tid] += sdata[tid + 256]; } GroupMemoryBarrierWithGroupSync(); }
	
	if (groupDim_x >= 256) { if (tid < 128) { sdata[tid] += sdata[tid + 128]; } GroupMemoryBarrierWithGroupSync(); }
	
	if (groupDim_x >= 128) { 
		if (tid < 64) { 
			sdata[tid] += sdata[tid + 64]; 
		} 
		GroupMemoryBarrierWithGroupSync(); 
	}
	

	if (groupDim_x >= 64){
		if (tid < 32){
			sdata[tid] += sdata[tid + 32];
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}

	if (groupDim_x >= 32){
		if (tid < 16){
			sdata[tid] += sdata[tid + 16];
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}

	if (groupDim_x >= 16){
		if (tid < 8){
			sdata[tid] += sdata[tid + 8];
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}

	if (groupDim_x >= 8){
		if (tid < 4){
			sdata[tid] += sdata[tid + 4];
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}

	if (groupDim_x >= 4){
		if (tid < 2){
			sdata[tid] += sdata[tid + 2];
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}

	if (groupDim_x >= 2){
		if (tid < 1){
			sdata[tid] += sdata[tid + 1];
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}
	
	if (tid == 0) {
		if(activate)
			outputArray[outputIx] = plainSigmoid(sdata[0]);
		else
			outputArray[outputIx] = sdata[0];
	}
}

//printing trick -- put inside read loop of layer to printout stuff
//if(groupIdx.x == 0)
//{
//	count++;
//	node_error_data[tid] = weightArray[weightIx];
//}
	 	




//here we simply proceed to run a layer for each thread
void runLayer(	uint3 threadIdx, 
				uint3 groupIdx,
				int4 layerDefinition,
				RWStructuredBuffer<float> inputArray,
				RWStructuredBuffer<float> weightArray,
				RWStructuredBuffer<float> outputArray) 
{
	int tid = (int)threadIdx.x;

	//what is the size of the inputs -- 785 if reading from mnist images --- or however many features at the next layers
	int layerInputSize = layerDefinition[LayerInputSizeIx];

	//where do we start reading our inputs
	int input_startIx = layerDefinition[LayerInputStartIx];

	//where are we indexed in for the beginning of our weight array
	int weight_startIx = layerDefinition[LayerWeightStartIx];

	//where do we write to for this layer?
	int output_startIx = layerDefinition[LayerOutputStartIx];

	//for each layer, input_startIx is always the same -- because we're processing the same input
	//for weights, which weights are read is based on groupID -- that's because
	//we launch # of groups == layer network size -- so if we have 3,000 features this layer, 
	//we dispatch 3,000 groups
	int startWeightIx = layerInputSize*(groupIdx.x) + weight_startIx;

	int weightIx = startWeightIx + tid;
	int inputIx = input_startIx + tid;

	int dispatchSize= groupDim_x*2*dispatchDim_x;

	//end of the line
	int inputsFinished = input_startIx + layerInputSize;

	int outputIx = output_startIx + groupIdx.x;
	
	sdata[tid] = 0;
	int etid = tid;
	do{
	 	sdata[tid] += weightArray[weightIx]*inputArray[inputIx] + weightArray[weightIx+groupDim_x]*inputArray[inputIx+groupDim_x]; 

		inputIx += dispatchSize; 
		weightIx += dispatchSize; 

	} while (inputIx < inputsFinished);

	GroupMemoryBarrierWithGroupSync();
	
	//now that its been read into shared memory, finish the sum normally, storing into outputIx
	
	//we do want activation after the calculation
	finishSharedDataSum(tid, outputArray, outputIx, true);

}

void calculateLayerError(	
				uint3 threadIdx, 
				uint3 groupIdx,
				int4 layerDefinition,
				int currentLayerSize,
				RWStructuredBuffer<float> inputArray,
				RWStructuredBuffer<float> weightArray,
				RWStructuredBuffer<float> outputArray) 
{
	//grab our thread identifier!
	int tid = (int)threadIdx.x;

	//what is the size of the inputs -- 785 if reading from mnist images --- or however many features at the next layers
	int layerInputSize = layerDefinition[LayerInputSizeIx];

	//where do we start reading our inputs
	int input_startIx = layerDefinition[LayerInputStartIx];

	//where are we indexed in for the beginning of our weight array
	int weight_startIx = layerDefinition[LayerWeightStartIx];

	//where do we write to for this layer?
	int output_startIx = layerDefinition[LayerOutputStartIx];

	//For backprop, we're reading ACROSS networks -- therefore each groupID represents
	//the weight location to read from across featurse 
	//that is if you're propograting weights for node 0 in the layer
	//look at 0, 0+layerSize, 0 + 2*layerSize ... etc
	int startWeightIx = (groupIdx.x) + weight_startIx;

	int weightIx = startWeightIx + layerInputSize*tid;
	int inputIx = input_startIx + tid;

	int dispatchSize= groupDim_x*2*dispatchDim_x;

	int outputIx = output_startIx + groupIdx.x;
	
	sdata[tid] = 0;

	do{
	 	sdata[tid] += weightArray[weightIx]*inputArray[inputIx] 
	 	+ weightArray[weightIx + groupDim_x*layerInputSize]*inputArray[inputIx + groupDim_x]; 

		inputIx += dispatchSize; 
		weightIx += dispatchSize*layerInputSize; 
	} while (inputIx < input_startIx + currentLayerSize);

	GroupMemoryBarrierWithGroupSync();
	
	//now that its been read into shared memory, finish the sum normally, storing into outputIx
	finishSharedDataSum(tid, outputArray, outputIx, false);
}

//here we simply proceed to run a layer for each thread
void propogateErrorDeltas(	
				uint3 threadIdx, 
				uint3 groupIdx,
				float learningRate,
				float momentum,
				int layerNetworkSize,
				int4 layerDefinition,
				RWStructuredBuffer<float> inputArray,
				RWStructuredBuffer<float> weightArray) 
{
	int tid = (int)threadIdx.x;

	//Nodes for sale! I got your nodes for sale here
	//number of groups == number of nodes => groupIdx.x == nodeID inside the network
	int nodeID = groupIdx.x;

	//this is the error caclulation we need to propogate -- read from the same place all the time
	float errorCalculation = learningRate*node_error_data[nodeID];

	//where are we indexed in for the beginning of our weight array
	int weight_startIx = layerDefinition[LayerWeightStartIx];

	//now we know where to index in
	int weightBaseIx = weight_startIx + nodeID*layerNetworkSize;

	//what is the size of the inputs -- 785 if reading from mnist images --- or however many features at the next layers
	int layerInputSize = layerDefinition[LayerInputSizeIx];

	//where do we start reading our inputs
	int input_startIx = layerDefinition[LayerInputStartIx];

	//what weight should we write to?
	int weightIx = weightBaseIx + tid;

	//grab our input
	int inputIx = input_startIx + tid;

	//how far apart are teh reads
	int dispatchSize= groupDim_x*2*dispatchDim_x;

	//mostly doing writes here
	do{

		//update the weight array accordingly
		weightArray[weightIx] += errorCalculation*inputArray[inputIx];
		weightArray[weightIx + groupDim_x] += errorCalculation*inputArray[inputIx + groupDim_x];

		inputIx += dispatchSize; 
		weightIx += dispatchSize; 

	} while (inputIx < input_startIx + layerInputSize);
}

void parallelMax(	
				uint3 threadIdx, 
				uint3 groupIdx,
				int finalLayerSize,
				int4 layerDefinition,
				RWStructuredBuffer<float> inputArray,
				RWStructuredBuffer<float> outputArray )
{

	//as usual, get our thread info
	int tid = threadIdx.x;

	//we need to figure out the max across a number of individuals
	int checkCount = 0;
	float fBest, lastCheck, vOne, vTwo;

	int lastIx, bestIx;
	
	//where do we start reading our inputs -- why at the END of the layer of course --
	//this is also the place we will eventually write to as well, derrrr
	int input_startIx = layerDefinition[LayerOutputStartIx];

	//input == output -- ya hear?
	int inputIx = input_startIx + tid;

	//how far apart are the read/writes
	int dispatchSize= groupDim_x*2*dispatchDim_x;

	//we go till the end
	int inputsFinished = inputIx + finalLayerSize;
	
	//inputs and outputs are the same here -- just reading and writing to different arrays at the same location
	int outputIx = inputIx;

	int readInputIx = inputIx;
	int writeInputIx = inputIx;
	
	//send in label data plz!
	int targetIx = image_labels[currentImageIx];

	//we will read two inputs at a time -- then decide which is better and store that index/value
	//we will continue to read them against each other and choose max, until we have a final ix as max
	//then we compare against max -- and set the errors accordingly

	mdata[tid] = 0;
	sdata[tid] = 0;

	do{
		vOne = inputArray[readInputIx];
		vTwo = inputArray[readInputIx + groupDim_x];

		bestIx = (vTwo > vOne ? readInputIx + groupDim_x : readInputIx);
		fBest = (vTwo > vOne ? vTwo : vOne);

		if(checkCount > 0)
		{
			mdata[tid] =  (lastCheck > fBest ? lastIx : bestIx);
			sdata[tid] =  (lastCheck > fBest ? lastCheck : fBest);
		}
		else{
		 	mdata[tid] = (vTwo > vOne ? readInputIx + groupDim_x : readInputIx);
		 	sdata[tid] = (vTwo > vOne ? vTwo : vOne);
		}

		checkCount++;

	 	lastCheck = fBest;
	 	lastIx = bestIx;

		readInputIx += dispatchSize; 

	} while (readInputIx < inputsFinished);
	
	//outputArray[tid] = currentImageIx;// tid;//sdata[tid];

	//sync up -- now we have to reduce our mdata/sdata according to maximums
	GroupMemoryBarrierWithGroupSync();

	float sd, sd2;

	if (groupDim_x>= 512) { 
	if (tid < 256) { 
		sd = sdata[tid]; sd2 = sdata[tid + 256];
		if(sd2 > sd)
		{
			sdata[tid] = sd2;
			mdata[tid] = mdata[tid + 256];
		}
		else
		{
			sdata[tid] = sd;
			mdata[tid] = mdata[tid];
		}
	} GroupMemoryBarrierWithGroupSync(); }
	
	if (groupDim_x >= 256) { if (tid < 128) { sd = sdata[tid]; sd2 = sdata[tid + 128];
		if(sd2 > sd)
		{
			sdata[tid] = sd2;
			mdata[tid] = mdata[tid + 128];
		}
		else
		{
			sdata[tid] = sd;
			mdata[tid] = mdata[tid];
		}} GroupMemoryBarrierWithGroupSync(); }
	
	if (groupDim_x >= 128) { 
		if (tid < 64) { 
			sd = sdata[tid]; sd2 = sdata[tid + 64];
		if(sd2 > sd)
		{
			sdata[tid] = sd2;
			mdata[tid] = mdata[tid + 64];
		}
		else
		{
			sdata[tid] = sd;
			mdata[tid] = mdata[tid];
		}
		} 
		GroupMemoryBarrierWithGroupSync(); 
	}
	

	if (groupDim_x >= 64){
		if (tid < 32){
			sd = sdata[tid]; sd2 = sdata[tid + 32];
		if(sd2 > sd)
		{
			sdata[tid] = sd2;
			mdata[tid] = mdata[tid + 32];
		}
		else
		{
			sdata[tid] = sd;
			mdata[tid] = mdata[tid];
		}
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}

	if (groupDim_x >= 32){
		if (tid < 16){
			sd = sdata[tid]; sd2 = sdata[tid + 16];
		if(sd2 > sd)
		{
			sdata[tid] = sd2;
			mdata[tid] = mdata[tid + 16];
		}
		else
		{
			sdata[tid] = sd;
			mdata[tid] = mdata[tid];
		}
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}

	if (groupDim_x >= 16){
		if (tid < 8){
			sd = sdata[tid]; sd2 = sdata[tid + 8];
		if(sd2 > sd)
		{
			sdata[tid] = sd2;
			mdata[tid] = mdata[tid + 8];
		}
		else
		{
			sdata[tid] = sd;
			mdata[tid] = mdata[tid];
		}
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}

	if (groupDim_x >= 8){
		if (tid < 4){
			sd = sdata[tid]; sd2 = sdata[tid + 4];
		if(sd2 > sd)
		{
			sdata[tid] = sd2;
			mdata[tid] = mdata[tid + 4];
		}
		else
		{
			sdata[tid] = sd;
			mdata[tid] = mdata[tid];
		}
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}

	if (groupDim_x >= 4){
		if (tid < 2){
			sd = sdata[tid]; sd2 = sdata[tid + 2];
		if(sd2 > sd)
		{
			sdata[tid] = sd2;
			mdata[tid] = mdata[tid + 2];
		}
		else
		{
			sdata[tid] = sd;
			mdata[tid] = mdata[tid];
		}
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}

	if (groupDim_x >= 2){
		if (tid < 1){
			sd = sdata[tid]; sd2 = sdata[tid + 1];
		if(sd2 > sd)
		{
			sdata[tid] = sd2;
			mdata[tid] = mdata[tid + 1];
		}
		else
		{
			sdata[tid] = sd;
			mdata[tid] = mdata[tid];
		}
		}
		//GroupMemoryBarrierWithGroupSync(); 
	}
	
	//when all is said and done mdata[0] is the max index! 
	GroupMemoryBarrierWithGroupSync(); 

	if(tid == 0)
	{
		//raw ix of maximum guess
		image_guesses[currentImageIx] = mdata[0] - input_startIx;
	}


	//now we can set error for each input! == TARGET (are you correct) - actual output (read from input array!)
	do{

		//are you the max AND the correct choice?
		float fTarget = (writeInputIx - input_startIx == targetIx ? correctTarget : incorrectTarget);
		outputArray[outputIx] = min(1.0, max(-1, fTarget - inputArray[outputIx]));
		
		//are you the max AND you're the correct choice?
		fTarget = (writeInputIx + groupDim_x  - input_startIx == targetIx ? correctTarget : incorrectTarget);
		outputArray[outputIx + groupDim_x] = min(1.0, max(-1, fTarget - inputArray[outputIx + groupDim_x]));
	
		writeInputIx += dispatchSize; 
		outputIx += dispatchSize; 

	} while (writeInputIx < inputsFinished);
	
}

[numthreads( groupDim_x, 1, 1)]
void runNetwork(uint index : SV_GroupIndex, uint3 threadIdx: SV_GroupThreadID, uint3 groupIdx: SV_GroupID)
{
	//this information can change according to which layer we run -- special care is taken for the 0th -- input layer
	//how we are to behave this run
	int4 layerDefinition = allLayers[currentLayerIx];

	if(runBackprop == 0)
	{
		//every thread takes this path when true
	
		//our arrays are set according to layer - I would do and if/else statement and set a pointer
		//put shader code doesn't allow this
		if(currentLayerIx == 0)
			runLayer(threadIdx, groupIdx, layerDefinition, image_data, weight_data, in_out_data);
		else
			runLayer(threadIdx, groupIdx, layerDefinition, in_out_data, weight_data, in_out_data);

	}
	else if(runBackprop == 1)
	{

		//always grab the last layer -- this contains all the info we need for parallel summations
		layerDefinition = allLayers[totalLayerCount - 1];

		//need to start the error propogation process with the outputs -- plz
		parallelMax(	
					threadIdx, 
					groupIdx,
					finalOutputCount,
					layerDefinition, //read from layer def of the last layer -- output layer
					in_out_data, //our "input" is the networks' output found within the last location of in_out_data -- layerdef will guide us
					node_error_data);

	}
	else if(runBackprop == 2)
	{
		//we are running backprop on this layer -- but just to sum up the error rates
	//	if(currentLayerIx != totalLayerCount - 1)
	//	{
			//special case when we're the first
			calculateLayerError(	
					threadIdx, 
					groupIdx,
					layerDefinition,
					bpCurrentLayerSize,
					node_error_data, //read and write to node error all the same for these non-end layers
					weight_data,
					node_error_data);
	//	}
	//	else
	//	{
	//		calculateLayerError(	
	//				threadIdx, 
	//				groupIdx,
	//				layerDefinition,
	//				bpCurrentLayerSize,
	//				in_out_data, //special case here reading from in_out -- normally we would read from prev layer error
	//				weight_data,
	//				node_error_data);
	//	}
	}
	else if(runBackprop == 3)
	{
		//final piece, we calculate and write all of the values back to our weight array
		int groupID = groupIdx.x; 

		int layerID = 0;
		int startLayer = 0;
		int endLayer= 0;
		//now we find out what layer this node is from
		for(int i=0; i <  totalLayerCount; i++)
		{
			//guessing...
			layerID = i; 

			//there are no other options, this is our layer
			if(i == totalLayerCount -1)
				break;

			//check the next layer up
			endLayer += allLayers[i + 1][LayerInputSizeIx];
			
			//are you less than layer sum and greater than start?? then you are in this layer!
			if(groupID < endLayer && groupID >= startLayer)
				break;

			//we weren't in this layer, maybe next time -- increment please
			startLayer += allLayers[i + 1][LayerInputSizeIx];
		}

		//how are we defined! We know what node we're investigating so we know what layer it comes from
		int4 bpLayerDefinition = allLayers[layerID];
		int nextLayerSize;

		if(layerID != totalLayerCount - 1)
			nextLayerSize = allLayers[layerID + 1][LayerInputSizeIx];
		else
			nextLayerSize = finalOutputCount;

		if(layerID == 0){

			//we start at a particular image
			bpLayerDefinition[LayerInputStartIx] = currentImageIx*bpLayerDefinition[LayerInputSizeIx];

			propogateErrorDeltas(threadIdx, groupIdx, 
							bpLearningRate, bpMomentum, 
							nextLayerSize, //send in the input size of the next layer -- i.e. outputcount
							bpLayerDefinition, 
							image_data,
							weight_data);
		}
		else
		{
			propogateErrorDeltas(threadIdx, groupIdx, 
							bpLearningRate, bpMomentum, 
							nextLayerSize, //send in the input size of the next layer -- i.e. outputcount
							bpLayerDefinition, 
							in_out_data,
							weight_data);
		}




	}
}
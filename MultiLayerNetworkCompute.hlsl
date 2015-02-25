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
	float extra1;
	float extra2;
};


RWStructuredBuffer<float> image_data;
RWStructuredBuffer<float> weight_data;
RWStructuredBuffer<float> in_out_data;
RWStructuredBuffer<float> node_error_data;

#define groupDim_x 512
groupshared float sdata[groupDim_x];

void finishSharedDataSum(int tid, RWStructuredBuffer<float> outputArray, int outputIx)
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

	int count = 0;
	int etid = tid;
	do{
	 	sdata[tid] += weightArray[weightIx]*inputArray[inputIx] + weightArray[weightIx+groupDim_x]*inputArray[inputIx+groupDim_x]; 

	 	
		inputIx += dispatchSize; 
		weightIx += dispatchSize; 

	} while (inputIx < inputsFinished);

	GroupMemoryBarrierWithGroupSync();
	
	//now that its been read into shared memory, finish the sum normally, storing into outputIx
	finishSharedDataSum(tid, outputArray, outputIx);
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

	//for each layer, input_startIx is always the same -- because we're processing the same input
	//for weights, which weights are read is based on groupID -- that's because
	//we launch # of groups == layer network size -- so if we have 3,000 features this layer, 
	//we dispatch 3,000 groups
	int startWeightIx = layerInputSize*(groupIdx.x) + weight_startIx;

	int weightIx = startWeightIx + tid;
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
	finishSharedDataSum(tid, outputArray, outputIx);
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

[numthreads( groupDim_x, 1, 1)]
void runNetwork(uint index : SV_GroupIndex, uint3 threadIdx: SV_GroupThreadID, uint3 groupIdx: SV_GroupID)
{
	//this information can change according to which layer we run -- special care is taken for the 0th -- input layer
	//how we are to behave this run
	int4 layerDefinition = allLayers[currentLayerIx];

	if(runBackprop == 0)
	{
		//every thread takes this path when true
		if(currentLayerIx == 0)
		{	
			//we start at a particular image
			layerDefinition[LayerInputStartIx] = currentImageIx*layerDefinition[LayerInputSizeIx];
		}

		//our arrays are set according to layer - I would do and if/else statement and set a pointer
		//put shader code doesn't allow this
		if(currentLayerIx == 0)
			runLayer(threadIdx, groupIdx, layerDefinition, image_data, weight_data, in_out_data);
		else
			runLayer(threadIdx, groupIdx, layerDefinition, in_out_data, weight_data, in_out_data);

	}
	else if(runBackprop == 1)
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
	else if(runBackprop == 2)
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
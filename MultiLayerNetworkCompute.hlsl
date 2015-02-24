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
	int bpWeightsToRead;
	int extra3;

	//max layer count of 16 for now -- easy to adjust -- minimal memory impact in constant buffer
	int4 allLayers[MAX_LAYERS];
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

	int weightIx = weight_startIx + tid;
	int inputIx = input_startIx + tid;

	int dispatchSize= groupDim_x*2*dispatchDim_x;

	int outputIx = output_startIx + groupIdx.x;
	
	sdata[tid] = 0;

	do{
	 	sdata[tid] += weightArray[weightIx]*inputArray[inputIx] 
	 	+ weightArray[weightIx+groupDim_x]*inputArray[inputIx+groupDim_x]; 

		inputIx += dispatchSize; 
		weightIx += dispatchSize; 

	} while (inputIx < input_startIx + layerInputSize);

	GroupMemoryBarrierWithGroupSync();
	
	//now that its been read into shared memory, finish the sum normally, storing into outputIx
	finishSharedDataSum(tid, outputArray, outputIx);
}

void calculateLayerError(	
				uint3 threadIdx, 
				uint3 groupIdx,
				int4 layerDefinition,
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

	//however many reads * 2 * inputLayerSize is the amount we have to skip ahead
	int weightIx = weight_startIx + 2*dispatchDim_x*layerInputSize*tid;

	//where do we read our input?
	int inputIx = input_startIx + groupIdx.x;

	//go ahead an make the read
	float targetError = inputArray[inputIx];

	//how large is teh gap between reads
	int weightDispatchSize = layerInputSize*2;

	//where are we storing this ifnromatino?
	int outputIx = output_startIx + groupIdx.x;
	

	sdata[tid] = 0;

	int readCount = 0;

	do{
	 	sdata[tid] += weightArray[weightIx]*inputArray[inputIx] 
	 	+ weightArray[weightIx + groupDim_x*inputLayerSize]*inputArray[inputIx + groupDim_x]; 

		inputIx += dispatchSize; 
		weightIx += dispatchSize*inputLayerSize; 
	} while (inputIx < input_startIx + layerInputSize);


	//do{
	// 	sdata[tid] += weightArray[weightIx]*targetError
	// 	+ weightArray[weightIx + layerInputSize]*targetError; 

	//	weightIx += weightDispatchSize; 

	//} while (readCount < dispatchDim_x);




	GroupMemoryBarrierWithGroupSync();
	
	//now that its been read into shared memory, finish the sum normally, storing into outputIx
	finishSharedDataSum(tid, outputArray, outputIx);
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
		if(currentLayerIx == totalLayerCount - 1)
		{
			//special case when we're the first
		}
	}
}
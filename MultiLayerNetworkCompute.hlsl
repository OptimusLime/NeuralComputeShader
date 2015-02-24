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
	int extra;
	int extra2;
	int extra3;

	//max layer count of 16 for now -- easy to adjust -- minimal memory impact in constant buffer
	int4 allLayers[MAX_LAYERS];
};


RWStructuredBuffer<float> image_data;
RWStructuredBuffer<float> weight_data;
RWStructuredBuffer<float> in_out_data;

#define groupDim_x 512
groupshared float sdata[groupDim_x];


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
	//we launch # of groups == layer network size -- so if we have 3,000 features this layer, we dispatch 3,000 groups
	int startWeightIx = inputLayerSize*(groupIdx.x) + weight_startIx;

	int weightIx = weight_startIx + tid;
	int inputIx = input_startIx + tid;

	int dispatchSize= groupDim_x*2*dispatchDim_x;

	int outputIx = output_startIx + groupIdx.x;
	
	sdata[tid] = 0;

	do{
	 	sdata[tid] += weightArray[weightIx]*inputArray[inputIx] 
	 	+ weightArray[weightIx+groupDim_x]*inputArray[inputIx+groupDim_x]; 

		i += dispatchSize; 
	} while (i < n);

	GroupMemoryBarrierWithGroupSync();
	
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

[numthreads( groupDim_x, 1, 1)]
void runNetwork(uint index : SV_GroupIndex, uint3 threadIdx: SV_GroupThreadID, uint3 groupIdx: SV_GroupID)
{
	//this information can change according to which layer we run -- special care is taken for the 0th -- input layer
	RWStructuredBuffer<float> inputArray;
	//how we are to behave this run
	int4 layerDefinition = allLayers[currentLayerIx];

	//every thread takes this path when true
	if(currentLayerIx == 0)
	{	
		inputArray = image_data;
		//we start at a particular image
		layerDefinition[LayerInputStartIx] = currentImageIx*layerDefinition[LayerInputSizeIx];
	}
	else
		inputArray = in_out_data;


	//our arrays are set - now we must determine a little more information
	runLayer(threadIdx, groupIdx, layerDefinition, inputArray, weight_data, in_out_data);
}

[numthreads( groupDim_x, 1, 1)]
void reduceArrayEfficient(uint index : SV_GroupIndex, uint3 threadIdx: SV_GroupThreadID, uint3 groupIdx: SV_GroupID)
{
	int tid = (int)threadIdx.x;

	int startWeightIx = paddingSize*(groupIdx.y + networkStartIx);
	//paddingSize*(groupIx/imageCount + networkIx);
	int startImageIx = paddingSize*(groupIdx.x + imageStartIx);//paddingSize*(groupIx % imageCount);

	//groupIdx.x*(groupDim_x*2) +
	int baseIx = index;

	int weightIx = startWeightIx + baseIx;
	int i = startImageIx + baseIx;

	int dispatchSize= groupDim_x*2*dispatchDim_x;

	int outputIx = outputStartIx + groupIdx.x + imageCount*groupIdx.y;
	
	sdata[tid] = 0;

	do{
	 	sdata[tid] += weight_data[weightIx]*g_idata[i] + weight_data[weightIx+groupDim_x]*g_idata[i+groupDim_x]; 
		i += dispatchSize; 
	} while (i < n);

	GroupMemoryBarrierWithGroupSync();
	
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
		g_odata[outputIx] = sdata[0];
	}
}


[numthreads( groupDim_x, 1, 1)]
void reduceNonOptimal(uint3 threadIdx: SV_GroupThreadID,
			uint3 groupIdx: SV_GroupID)
{
	// each thread loadsone element from global to shared mem
	unsigned int tid = threadIdx.x;
	unsigned int i= groupIdx.x*groupDim_x+ threadIdx.x;
	sdata[tid] = g_idata[i];

	GroupMemoryBarrierWithGroupSync();
	// do reduction in shared mem
	for(unsigned int s=1; s < groupDim_x; s *= 2) {
		if(tid % (2*s) == 0){
			sdata[tid] += sdata[tid + s];
		}
		GroupMemoryBarrierWithGroupSync();
	}
	// write result for this block to global mem
	if(tid == 0) 
		g_odata[groupIdx.x] = sdata[0];
}

[numthreads( groupDim_x, 1, 1)]
void simpleMultiply(uint tid: SV_GroupIndex, 
	uint3 threadIdx: SV_GroupThreadID,
	uint3 groupIdx: SV_GroupID
	)
{
	//just take in n * dispatchDim_x* existing value
	//unsigned int n = val[0];
	//unsigned int dispatchDim_x = val[1];
	unsigned int i= groupIdx.x*groupDim_x+ threadIdx.x;

	float value = g_idata[i];
	//very simple -- multiple whats there times n * dispatchDim_x
	g_idata[i] = dispatchDim_x;

	if (tid == 0) 
		g_odata[tid] =  n;
}
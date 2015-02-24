cbuffer consts {
	int n;
	int dispatchDim_x;
	int imageCount;
	int paddingSize;
	int imageStartIx;
	int networkStartIx;
	int outputStartIx;
	int extra3;
};

RWStructuredBuffer<float> weight_data;
RWStructuredBuffer<float> g_idata;
RWStructuredBuffer<float> g_odata;

#define groupDim_x 512
groupshared float sdata[groupDim_x];


[numthreads( groupDim_x, 1, 1)]
void reduceArray(uint index : SV_GroupIndex, uint3 threadIdx: SV_GroupThreadID, uint3 groupIdx: SV_GroupID)
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
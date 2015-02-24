// Indexing in constant buffers works on 16-byte boundaries --
// even though the input data is a float[16], declaring the
// cbuffer as such will misalign the data. 16 individual floats
// would work properly, but such a structure cannot be indexed
// at runtime. But an array of four float4 objects allows runtime
// indexing and satisfies the alignment issues.
//cbuffer inputs {
//	float4 gInput[4];
//}

cbuffer inputs {
	uint n;
	uint dispatch;
	uint val;
	uint extra;
}

RWStructuredBuffer<float> gOutput;
RWStructuredBuffer<float> gMore;

// Utilize 16 threads per group. The thread ID will be used
// to index into the constant buffer.
//[numthreads(16, 1, 1)]
//void main(uint3 threadId : SV_DispatchThreadID)
//{
//	// The complex indexing is required due to the inability to
//	// simply declare gInput as a float[16].
//	gOutput[threadId.x] = 2 * gInput[threadId.x / 4][threadId.x % 4];
//}

#define groupDim_x 16

[numthreads( groupDim_x, 1, 1)]
void simpleMultiply(uint3 threadId : SV_DispatchThreadID, uint tid: SV_GroupIndex, uint3 groupIdx: SV_GroupID)
{
	// The complex indexing is required due to the inability to
	// simply declare gInput as a float[16].
	gOutput[threadId.x] = 2 * n* dispatch;//gInput[threadId.x / 4];//[threadId.x % 4];
	
	//just take in n * dispatchDim_x* existing value
	//unsigned int n = val[0];
	//unsigned int dispatchDim_x = val[1];

	//float value = g_idata[tid];
	//very simple -- multiple whats there times n * dispatchDim_x
	//g_idata[tid] = value*n*dispatchDim_x;
}


[numthreads(groupDim_x, 1, 1)]
void testStructure(uint3 threadId : SV_DispatchThreadID, uint tid: SV_GroupIndex, uint3 groupIdx: SV_GroupID)
{
	//multiple everything by two in place
	gOutput[threadId.x] = n * dispatch* gOutput[tid];
}
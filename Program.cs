﻿using System;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using SlimDX.Helper;
using Device = SlimDX.Direct3D11.Device;
using Buffer = SlimDX.Direct3D11.Buffer;
using MapFlags = SlimDX.Direct3D11.MapFlags;
using GPUTools;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ComputeShader11
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            MultiLayerNetwork();
            //MainReduce();
            //if (MainReduce())
                //return;

            //if (MainSimple())
                //return;

            //Device device = new Device(DriverType.Hardware);//, DeviceCreationFlags.Debug);//, FeatureLevel.Level_11_0);

            //bool forceReload = true;

            //load up our efficient array computation booger
            //ComputeShader computeShader = Helper.LoadComputeShader(device, "EfficientArrayCompute.hlsl", "reduceArray", forceReload);
            //ComputeShader computeShader = Helper.LoadComputeShader(device, "EfficientArrayCompute.hlsl", "simpleMultiply", forceReload);
            //ComputeShader compute = Helper.LoadComputeShader(device, "SimpleCompute.hlsl", "main", forceReload);

            //if (computeShader == null)
            //    return;

            //const int bufferIntCount = 4;
            //const int constBufferSizeInBytes = bufferIntCount * sizeof(uint);

            ////how many objects are we sending into the GPU
            //const int elementFloatCount = 128;
            //const int outputFloatsBufferSizeInBytes = elementFloatCount * sizeof(float);

            ////I HAVE NO IDEA WHAT THESE NUMBERS ARE YET
            //uint n = 32;
            //uint dimensionXSize = 128;

            //BufferDescription inputBufferDescription = new BufferDescription
            //{
            //    BindFlags = BindFlags.ConstantBuffer,
            //    CpuAccessFlags = CpuAccessFlags.Write,
            //    OptionFlags = ResourceOptionFlags.None,
            //    SizeInBytes = constBufferSizeInBytes,
            //    StructureByteStride = sizeof(uint),
            //    Usage = ResourceUsage.Dynamic,
            //};

            //Buffer inputBuffer = new Buffer(device, inputBufferDescription);
            //DataBox input = device.ImmediateContext.MapSubresource(inputBuffer, MapMode.WriteDiscard, MapFlags.None);//(inputBuffer, 0, constBufferSizeInBytes, MapMode.WriteDiscard, MapFlags.None);

            //input.Data.Write(n);
            //input.Data.Write(dimensionXSize);
            ////going to send in this input buffer 
            ////I have no idea what unmap subresource means...
            //device.ImmediateContext.UnmapSubresource(inputBuffer, 0);

            ////need to send information into the buffer
            //BufferDescription cpuToGPUBufferDescription = new BufferDescription
            //{
            //    BindFlags = BindFlags.UnorderedAccess,
            //    CpuAccessFlags = CpuAccessFlags.Write,
            //    OptionFlags = ResourceOptionFlags.StructuredBuffer,
            //    SizeInBytes = outputFloatsBufferSizeInBytes,
            //    StructureByteStride = sizeof(float),
            //    Usage = ResourceUsage.Staging,
            //};

            ////we need to be able 
            //Buffer cpuToGPUBuffer = new Buffer(device, cpuToGPUBufferDescription);
            //DataBox cpuToGpuIn = device.ImmediateContext.MapSubresource(cpuToGPUBuffer, MapMode.WriteDiscard, MapFlags.None);//(inputBuffer, 0, constBufferSizeInBytes, MapMode.WriteDiscard, MapFlags.None);
            //for (var i = 0; i < elementFloatCount; i++)
            //    cpuToGpuIn.Data.Write((float)i);

            //// A staging buffer is used to copy data between the CPU and GPU; the output
            //// buffer (which gets the computation results) cannot be mapped directly.
            //BufferDescription gpuToCPUstagingBufferDescription = new BufferDescription
            //{
            //    BindFlags = BindFlags.None,
            //    CpuAccessFlags = CpuAccessFlags.None,
            //    OptionFlags = ResourceOptionFlags.StructuredBuffer,
            //    SizeInBytes = outputFloatsBufferSizeInBytes,
            //    StructureByteStride = sizeof(float),
            //    Usage = ResourceUsage.Staging,
            //};
            //Buffer gpuStagingBuffer = new Buffer(device, gpuToCPUstagingBufferDescription);

            //// The output buffer itself, and the view required to bind it to the pipeline.
            //BufferDescription outputBufferDescription = new BufferDescription
            //{
            //    BindFlags = BindFlags.UnorderedAccess | BindFlags.ShaderResource,
            //    OptionFlags = ResourceOptionFlags.StructuredBuffer,
            //    SizeInBytes = outputFloatsBufferSizeInBytes,
            //    StructureByteStride = sizeof(float),
            //    Usage = ResourceUsage.Default,
            //};
            //Buffer outputBuffer = new Buffer(device, outputBufferDescription);
            //UnorderedAccessViewDescription outputViewDescription = new UnorderedAccessViewDescription
            //{
            //    ElementCount = elementFloatCount,
            //    Format = SlimDX.DXGI.Format.Unknown,
            //    Dimension = UnorderedAccessViewDimension.Buffer
            //};

            //UnorderedAccessView outputView = new UnorderedAccessView(device, outputBuffer, outputViewDescription);

            //// Compile the shader.
            //device.ImmediateContext.ComputeShader.Set(computeShader);
            //device.ImmediateContext.ComputeShader.SetUnorderedAccessView(outputView, 0);
            //device.ImmediateContext.ComputeShader.SetConstantBuffer(inputBuffer, 0);

            ////copy our CPU resources inside cputogpu buffer onto the GPU 
            //device.ImmediateContext.CopyResource(cpuToGPUBuffer, outputBuffer);

            //// Compute shaders execute on multiple threads at the same time. Those execution
            //// threads are grouped; Dispatch() indicates how many groups in the X, Y and Z
            //// dimension will be utilized. The shader itself specified how many threads per
            //// group (also in X, Y and Z dimensions) to use via the [numthreads] attribute.
            //// In this sample, one thread group will be used with 16 threads, each thread
            //// will process one element of the input data.
            //device.ImmediateContext.Dispatch(1, 1, 1);

            //device.ImmediateContext.CopyResource(outputBuffer, gpuStagingBuffer);
            //DataBox output = device.ImmediateContext.MapSubresource(gpuStagingBuffer, MapMode.Read, MapFlags.None);

            //Console.Write("Results:");
            //for (int index = 0; index < elementFloatCount; ++index)
            //    Console.Write(" {0}", output.Data.Read<float>());


            //device.ImmediateContext.UnmapSubresource(cpuToGPUBuffer, 0);
            //device.ImmediateContext.UnmapSubresource(outputBuffer, 0);

            //Console.WriteLine();






            ////Texture2D uavTexture;
            ////UnorderedAccessView computeResult = Helper.CreateUnorderedAccessView(device, 1024, 1024, Format.R8G8B8A8_UNorm, out uavTexture);
            
            ////device.ImmediateContext.ComputeShader.Set(compute);
            ////device.ImmediateContext.ComputeShader.SetUnorderedAccessView(computeResult, 0);
            ////device.ImmediateContext.Dispatch(32, 32, 1);

            ////Texture2D.ToFile(device.ImmediateContext, uavTexture, ImageFileFormat.Png, "uav.png");

            ////uavTexture.Dispose();
            ////computeResult.Dispose();



            //device.Dispose();
        }



        
        #region Layer Definition Helpers

        static int[] ConstructFullBuffer(int dispatchSize, 
            int totalTestImages, 
            int inputPixelSize,  
            int currentLayerIx, 
            int currentImageIx, 
            int runningBackprop,
            int currentPaddedLayerSize,
            int finalPaddedOutputSize,
            int[] layerDefinitions)
        {
            //need to make a new mega array -- for writing to the buffer
            
            //structure of the buffer at time of writing this code
            //cbuffer consts {
            //    int dispatchDim_x;
            //    int totalImageCount;
            //    int totalLayerCount;
            //    int currentLayerIx;
            //    int currentImageIx;
            //    int extra;
            //    int extra2;
            //    int extra3;

            //    //max layer count of 16 for now -- easy to adjust -- minimal memory impact in constant buffer
            //    int4 allLayers[16];
            //};

            //start with our default information
            var allValues = new List<int>() { 
                dispatchSize, 
                totalTestImages, 
                layerDefinitions.Length / 4,
                currentLayerIx, 
                currentImageIx,
                runningBackprop,
                currentPaddedLayerSize,
                finalPaddedOutputSize
            };

            //add in the layer definitions at the end
            allValues.AddRange(layerDefinitions);

            //we're ready to submit this as an array
            return allValues.ToArray();
        }

        static int PadInt(int val, int paddingSize)
        {
            return (int)(Math.Ceiling((float)val / paddingSize) *paddingSize);
        }

        static int[] PadDefaultSizes(int[] layerSize, int paddingSize)
        {
            int[] cloneLayers = (int[])layerSize.Clone();
            for (int i = 0; i < cloneLayers.Length; i++)
                cloneLayers[i] = PadInt(cloneLayers[i], paddingSize);
            return cloneLayers;
        }

        static float[] PaddedInputArray(float[][] inputs, int paddedSizePerImage)
        {
            float[] finalArray = new float[paddedSizePerImage * inputs.Length];
            int startIx = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                float[] img = inputs[i];
                for (int w = 0; w < img.Length; w++)
                    finalArray[startIx + w] = img[w];

                startIx += paddedSizePerImage;
            }

            //all padded out!
            return finalArray;
        }


        static int[] ConstructLayerDefinitions(int[] layerSizes, int inputPixelSize, int totalTestImages, out int totalWeightSize, out int totalNetworkSize)
        {
            //plus one for implied input layer
            int fullLayerCount = layerSizes.Length;
            
            //4 ints per layer
            int[] finalDefinitions = new int[fullLayerCount * 4];

            int inputStartIx = 0;
            int weightStartIx = 0;
            int outputStartIx = 0;

            //#define LayerInputSizeIx 0
            //#define LayerInputStartIx 1
            //#define LayerWeightStartIx 2
            //#define LayerOutputStartIx 3
            for (int i = 0; i < fullLayerCount; i++)
            {
                int LayerInputSizeIx = 4 * i;
                int LayerInputStartIx = 4 * i + 1;
                int LayerWeightStartIx = 4 * i + 2;
                int LayerOutputStartIx = 4 * i + 3;

                int layerInputSize;
                //set the number of inputs -- changes according to layer -- special case input layer
                //mostly it's just the numbero f features in the layer above you
                if (i == 0)
                    //how large are the inputs for 0th layer? well the input size of course!
                    layerInputSize = inputPixelSize;
                else //otherwise the inputs == the featuer size of the last layer!
                    layerInputSize = layerSizes[i - 1];

                //grab the true input size
                finalDefinitions[LayerInputSizeIx] = layerInputSize;

                //input starts at particular locations -- this is basically the sum of the number of features from before
                finalDefinitions[LayerInputStartIx] = inputStartIx;

                //however, note that the 0th layer reads from the inputs, therefore it doesn't increment any counts
                if (i != 0)
                    inputStartIx += layerSizes[i];
                

                //where do we read the weights from?
                finalDefinitions[LayerWeightStartIx] = weightStartIx;

                //now we do some incrementing always!/
                //the weights for each layer are laid out as such:
                //[network{0-0}, network{0-1}, ....network{0-layerSize[0]}, network{1-0}, network{1-1}, ...network{1-layerSize[1]}...
                //all the way to where flc = (fullLayerCount-1)  ... network{flc - 0}, network{flc - 1}, ....network{flc - layerSize[flc]}
                //== [layer0Size X pixelInputSize, layer1Size X layer0Size, layer2Size X layer1Size, ... layer{FLC}Size x layer{FLC-1}Size
                weightStartIx += (layerInputSize * layerSizes[i]);

                //set the layer definition output start to be 
                finalDefinitions[LayerOutputStartIx] = outputStartIx;

                //increment the size of the array -- this where everything is stored!
                outputStartIx += layerSizes[i];
            }

            //these are the total sizes of both the number of nodes -- network size
            //as well as the total number of required weights -- connection size!
            totalWeightSize = weightStartIx;
            totalNetworkSize = outputStartIx;

            return finalDefinitions;
        }

        static int[] ConstructBackpropLayerDefinitions(
            int[] paddedLayerSizes,  
            int paddedInputSize,
            int totalNodeCount,
            int totalWeightCount
            )
        {
            //plus one for implied input layer
            int fullLayerCount = paddedLayerSizes.Length;

            //4 ints per layer -- even though we don't use the earliest layer -- we still reserve space fro all the layers
            int[] finalDefinitions = new int[fullLayerCount* 4];

            int inputStartIx = totalNodeCount;
            int weightStartIx = totalWeightCount;
            int outputStartIx = 0;

            //#define LayerInputSizeIx 0
            //#define LayerInputStartIx 1
            //#define LayerWeightStartIx 2
            //#define LayerOutputStartIx 3
            //backwards we go for backprop -- topsy turvyyyyy
            //not that we do not need to calculate error for input nodes -- so we do 1 less layer than forwards propogation
            for (int i = fullLayerCount-1; i > 0; i--)
            {
                int LayerInputSizeIx = 4 * i;
                int LayerInputStartIx = 4 * i + 1;
                int LayerWeightStartIx = 4 * i + 2;
                int LayerOutputStartIx = 4 * i + 3;

                //read this layer's output count -- in backprop THESE are the inputs
                int layerInputSize = paddedLayerSizes[i];

                //grab the true input size
                finalDefinitions[LayerInputSizeIx] = layerInputSize;
                
                //where do we read from? The layer input count -- duh
                inputStartIx -= layerInputSize;

                //input starts at particular locations -- this is basically the sum of the number of features from before
                finalDefinitions[LayerInputStartIx] = inputStartIx;

                int previuosLayerCount = paddedLayerSizes[i - 1];

                //go back however many weights x however many outputs we had this layer -- simple
                weightStartIx -= layerInputSize * previuosLayerCount;

                //where do we read the weights from?
                finalDefinitions[LayerWeightStartIx] = weightStartIx;

                //go back a litter farther, would you? Backprop is calculating the error for the previous layers nodes
                //i.e. we start writing previousLayerCount number of nodes back in the array
                outputStartIx = inputStartIx - previuosLayerCount;
                
                //set the layer definition output start to be 
                finalDefinitions[LayerOutputStartIx] = outputStartIx;
            }

            return finalDefinitions;
        }

        #endregion

        #region Random/Empty Input Array Helpers
        static int[] emptyIntArray(int size, int? defaultValue = null)
        {
            int[] finalSize = new int[size];
            if (!defaultValue.HasValue)
                return finalSize;

            for (int i = 0; i < size; i++)
                finalSize[i] = defaultValue.Value;

            return finalSize;
        }
        static float[] emptyArray(int size, float? defaultValue = null)
        {
            float[] finalSize = new float[size];
            if (!defaultValue.HasValue)
                return finalSize;

            for (int i = 0; i < size; i++)
                finalSize[i] = defaultValue.Value;

            return finalSize;
        }
        //send back random input images for the looking
        static int[] DefaultRandomImageLabels(int outputCount, int imageCount, int? overwriteInput = null, int? seed = null)
        {
            Random r;
            if (seed.HasValue)
                r = new Random(seed.Value);
            else
                r = new Random();


            int[] labels = new int[imageCount];
            for(int i=0; i < imageCount; i++)
            {
                labels[i] = (overwriteInput.HasValue ? overwriteInput.Value : r.Next(imageCount));
            }

            //send it back!
            return labels;
        }
        //send back random input images for the looking
        static float[][] DefaultRandomInputImages(int pixelCount, int imageCount, float? overwriteInput = null, int? seed = null)
        {
            Random r;
            if (seed.HasValue)
                r = new Random(seed.Value);
            else
                r = new Random();

            float[][] rInputs = new float[imageCount][];
            for (int i = 0; i < imageCount; i++)
            {
                float[] rImage = new float[pixelCount];

                for (int w = 0; w < pixelCount; w++)
                    rImage[w] = (overwriteInput.HasValue ? (i+1)*overwriteInput.Value : (float)r.NextDouble());

                rInputs[i] = rImage;
            }

            //all done!
            return rInputs;
        }

        static float[] DefaultRandomWeightArray(int[] layerSizes, int[] paddedSizes, int trueInputSize, int paddedInputSize, int fullSize, float? overwriteWeight = null, int? seed = null)
        {
            Random r;
            if (seed.HasValue)
                r = new Random(seed.Value);
            else
                r = new Random();

            float[] finalWeights = new float[fullSize];

            int startIx = 0;
            for (int i = 0; i < layerSizes.Length; i++)
            {
                int original = layerSizes[i];
                int padded = paddedSizes[i];

                int lastLayerSize = (i == 0 ? paddedInputSize : paddedSizes[i - 1]);
                int lastTrueLayerSize = (i == 0 ? trueInputSize : layerSizes[i - 1]);

                //for however many are in this layer
                for (int w = 0; w < padded; w++)
                {
                    //only set weights in non-padded areas
                    if (w >= original) //(c + 1)*
                    {
                        startIx += (padded - original)*lastLayerSize;
                        break;
                    }
                    
                    for (int c = 0; c < lastLayerSize; c++)
                    {
                        if(c < lastTrueLayerSize)
                            finalWeights[startIx] = (overwriteWeight.HasValue ? (w+1)*overwriteWeight.Value : (float)r.NextDouble());

                        //always increment start location
                         startIx++;
                    }
                }
                //increment the layer amout and carry on!
            }

            return finalWeights;
        }

        static int MaxValue(float[] vals)
        {
            float mv = float.MinValue;
            int maxIx = 0;
            for (int i = 0; i < vals.Length; i++)
            {
                float v = vals[i];
                if (v > mv)
                {
                    maxIx = i;
                    mv = v;
                }
            }
            //send back the index of the max value
            return maxIx;
        }

        static float CorrectTarget(int ix, int correctIx, float? correct, float? incorrect)
        {
            bool isCorrect = (ix == correctIx);
            
            if(isCorrect)
                return correct.HasValue ? correct.Value : 1.0f;
            else
                return (incorrect.HasValue ? incorrect.Value : 0.0f);

        }
        static float[] CorrectFloatErrors(float[] networkOutputs, int correctIx, float? correct, float? incorrect)
        {
            float[] errors = new float[networkOutputs.Length];
            for (int i = 0; i < networkOutputs.Length; i++)
            {
                errors[i] = CorrectTarget(i, correctIx, correct, incorrect) - networkOutputs[i];
            }
            return errors;
        }

        #endregion

        //how many threads are launched for the main entry point
        //hardcoded in shader
        const int computeShaderHardcodedThreadSize = 512;

        //defined in shader as well
        const int MAX_LAYERS = 16;


        static int[] defaultLayerSizes = new int[] { 2500, 10 };

        static void MultiLayerNetwork()
        {

            Device device = new Device(DriverType.Hardware);//, DeviceCreationFlags.Debug);//, FeatureLevel.Level_11_0);
            bool forceReload = true;

            ComputeShader computeShader = Helper.LoadComputeShader(device, "MultiLayerNetworkCompute.hlsl", "runNetwork", forceReload);

            if (computeShader == null)
                return;

            //inputs we are sending in (must be >= 4 ints for whatever reason
            const int bufferIntCount = 8 + 4 * MAX_LAYERS;
            const int constBufferSizeInBytes = bufferIntCount * sizeof(int);

            //add the inputs
            Buffer inputBuffer = CreateBuffer(device, constBufferSizeInBytes);
            const int floatBufferCount = 4;
            const int constFloatBufferSizeInBytes = floatBufferCount * sizeof(float);

            //buffer for pushing in backprop related variables
            Buffer backpropBuffer = CreateBuffer(device, constFloatBufferSizeInBytes);

            //these are our input sizes
            int inputPixelSize = 785;
            int totalImageCount = 60000;

            int[] layerSizes = defaultLayerSizes;

            //send in random input images
            //float? overwriteInputs = 1.0f;
            float? overwriteInputs = null;// 1.0f;
            float[][] randomInputImages = DefaultRandomInputImages(inputPixelSize, totalImageCount, overwriteInputs);
            int? overwriteLabel = 1;
            int[] randomInputLabels = DefaultRandomImageLabels(layerSizes.Last(), totalImageCount, overwriteLabel);

            //send it to shader 
            RunShader(device, computeShader, 
                inputBuffer, constBufferSizeInBytes, 
                backpropBuffer, constFloatBufferSizeInBytes,
                inputPixelSize, totalImageCount, layerSizes, randomInputImages, randomInputLabels);
        }

     
        static void RunShader(Device device, 
            ComputeShader computeShader,
            Buffer inputBuffer,
            int constBufferSizeInBytes,
            Buffer backpropBuffer, 
            int constFloatBufferSizeInBytes,
            int inputPixelSize,
            int totalImageCount, 
            int[] layerSizes, 
            float[][] inputImages,
            int[] imageLabels)
        {
            //pad by dispatch dimension size == 2*number of threads
            //this is for efficient read purposes -- we don't want to read out of bounds, so we pad by dispatch size (stride length of shader)
            int paddingSize = 2 * computeShaderHardcodedThreadSize;

            //what will our inputs be like padded, i wonder???
            int paddedInputSize = PadInt(inputPixelSize, paddingSize);

            //now convert our input float doublearray into a single long float array
            float[] fullPaddedInputValues = PaddedInputArray(inputImages, paddedInputSize);

            //pad out the layer sizes
            int[] paddedLayers = PadDefaultSizes(layerSizes, paddingSize);
            int finalPaddedOutputCount = paddedLayers.Last();
            int trueOutputCount = layerSizes.Last();

            //how much data will we load? padded values x total imagecount
            int totalInputCapacity = fullPaddedInputValues.Length;

            //how many connections do we need for this big object
            int fullWeightArraySize = 0;

            //how many nodes exist inside the network?
            int fullInputOutputArraySize = 0;

            //use our layer sizes to create a full definiton of the network that can be executed on the GPU
            //we also measure node and connection sizes with this function -- mind you these are the full padded sizes
            int[] layerDefinitions = ConstructLayerDefinitions(paddedLayers, paddedInputSize, totalImageCount, out fullWeightArraySize, out fullInputOutputArraySize);
            
            //we use the new information to create the backprop definitions as well -- so we can go backwards with our network
            int[] backpropLayerDefinitions = ConstructBackpropLayerDefinitions(paddedLayers, paddedInputSize, fullInputOutputArraySize, fullWeightArraySize);

            //finally, we create our empty node array -- this will store our network node values during network execution
            float[] inputOutputNodeValues = emptyArray(fullInputOutputArraySize);
            int[] networkGuesses = emptyIntArray(totalImageCount);


            //set up our constant buffer with our inputs -- yazoooooooo
            const int inputBufferIx = 0;
            const int backpropBufferIx = 1;

            //how many dispatches? Only every 1 at a time (so we say)
            int groupsToDispatch = 1;

            if (groupsToDispatch > 1)
                throw new NotImplementedException("I don't believe this is verified to work with more than 1 dispatch group size right now");

            //create a random set of weights (or choose to overwrite with default weight
            //currently useing default weight of 1 for debug purposes
            //float? overWrite = 1.0f; //set to null if you want true random
            float? overWrite = null;// 1.0f; //set to null if you want true random
            float[] randomWeights = DefaultRandomWeightArray(layerSizes, paddedLayers, inputPixelSize, paddedInputSize, fullWeightArraySize, overWrite);

            //Create GPUList of particles using the immediate context
            GPUList<float> inputImageValues = new GPUList<float>(device.ImmediateContext);
            inputImageValues.Capacity = totalInputCapacity;

            //Create list to hold input image labels
            GPUList<int> inputImageLabels = new GPUList<int>(device.ImmediateContext);
            inputImageLabels.Capacity = totalImageCount;

            //Create list to hold input image labels
            GPUList<int> networkImageGuesses = new GPUList<int>(device.ImmediateContext);
            networkImageGuesses.Capacity = totalImageCount;

            //copy in the weights into a structure weight arary -- prepare it for the correct size
            GPUList<float> weightValues = new GPUList<float>(device.ImmediateContext);
            weightValues.Capacity = randomWeights.Length;

            //finally, we create the network node array
            GPUList<float> networkNodeValues = new GPUList<float>(device.ImmediateContext);
            networkNodeValues.Capacity = fullInputOutputArraySize;

            //finally, we create the network node array
            GPUList<float> backpropErrorNodeValues = new GPUList<float>(device.ImmediateContext);
            backpropErrorNodeValues.Capacity = fullInputOutputArraySize;

            int subIx = 0;
            //DataBox constantInputDataBox = DataboxMapResource(device, inputBuffer, inputBufferIx);
            //DataBox constantFloatInputDataBox = DataboxMapResource(device, backpropBuffer, backpropBufferIx);

            //get ready to record these objects-- this time includes the time to copy everything
            //separate gpu clacle for setup
            Stopwatch gpuTotal = new Stopwatch();
            gpuTotal.Start();

            //set ALL of the input images -- in reality -- this call is made ONCE for all networks ever
            inputImageValues.AddRange(fullPaddedInputValues);

            //mark our labels as well, plz!
            inputImageLabels.AddRange(imageLabels);

            //send in empty guess array
            networkImageGuesses.AddRange(networkGuesses);

            //set the weights inside the gpu -- this is for the full network
            weightValues.AddRange(randomWeights);

            //finally, set the default in/out nodes -- this might not be necessary
            networkNodeValues.AddRange(inputOutputNodeValues);

            //copy in for backprop nodes as well
            backpropErrorNodeValues.AddRange(inputOutputNodeValues);

            //Ordering of lists in shader -- should match unorderd access for directx compute shaders
            //RWStructuredBuffer<float> image_data;
            //RWStructuredBuffer<float> weight_data;
            //RWStructuredBuffer<float> in_out_data;

            //now set the memory for our gpu shader -- in correct order - image, weight, in_out
            //Run the compute shader
            device.ImmediateContext.ComputeShader.Set(computeShader);
            int unorderIx = 0;
            device.ImmediateContext.ComputeShader.SetUnorderedAccessView(inputImageValues.UnorderedAccess, unorderIx++);
            device.ImmediateContext.ComputeShader.SetUnorderedAccessView(weightValues.UnorderedAccess, unorderIx++);
            device.ImmediateContext.ComputeShader.SetUnorderedAccessView(networkNodeValues.UnorderedAccess, unorderIx++);
            device.ImmediateContext.ComputeShader.SetUnorderedAccessView(backpropErrorNodeValues.UnorderedAccess, unorderIx++);
            device.ImmediateContext.ComputeShader.SetUnorderedAccessView(networkImageGuesses.UnorderedAccess, unorderIx++);
            device.ImmediateContext.ComputeShader.SetUnorderedAccessView(inputImageLabels.UnorderedAccess, unorderIx++);
            
            //set contstant buffers
            device.ImmediateContext.ComputeShader.SetConstantBuffer(inputBuffer, inputBufferIx);
            device.ImmediateContext.ComputeShader.SetConstantBuffer(backpropBuffer, backpropBufferIx);
            
            //separate gpu clacle for setup
            Stopwatch gpu = new Stopwatch();
            gpu.Start();

            Stopwatch sw = null;
            float copyTime = 0;

            float finalGPUTime = 0;

            int startLayerCountIx = 0;

            float[] allNodeCheck = new float[inputOutputNodeValues.Length];
            float learningRate = 0.001f;
            float momentum = 0.0f;

            //int backpropBlockx, backpropBlocky;
            //int psDiv =  (int)(Math.Ceiling((float)randomWeights.Length/(paddingSize*paddingSize)));
            //backpropBlocky = (int)Math.Ceiling(Math.Sqrt(psDiv));
            //backpropBlockx = (int)(Math.Ceiling((float)(psDiv) / backpropBlocky));

            //bool chk = (backpropBlockx * backpropBlocky * paddingSize * paddingSize - randomWeights.Length) >= 0;
            //if (!chk)
            //    throw new Exception("Incorrect breakdown of blocks being sent at last stage of backprop");
            int[] fullInputBuffer;
            float[] networkOutputs = new float[trueOutputCount];
            //DEBUG
            //float[] weightCheck = new float[randomWeights.Length];
            int[] guessCheck = new int[totalImageCount];

            float[] nearEmptyOut = emptyArray(trueOutputCount, 0.0f);
            nearEmptyOut[1] = 1.0f;

            int backpropState = 0;
            for (int currentImageIx = 0; currentImageIx < totalImageCount; currentImageIx++)
            {
                //starts at 0
                startLayerCountIx = 0;

                //not backprop
                backpropState = 0;
                //set current read location
                layerDefinitions[1] = currentImageIx * paddedInputSize;
                //float[] inputs = new float[paddedInputSize];
                //Array.Copy(fullPaddedInputValues, layerDefinitions[1], inputs, 0, paddedInputSize);
  
                if (true)
                {
                    //we need to process this image across all layers
                    for (int currentLayerIx = 0; currentLayerIx < layerSizes.Length; currentLayerIx++)
                    {
                        //how many actual nodes are here -- not whats padded in memory
                        int trueLayerSize = layerSizes[currentLayerIx];

                        //what information do we need to send into the constant buffer to make sure it's correct
                        fullInputBuffer = ConstructFullBuffer(
                                                        groupsToDispatch,
                                                        totalImageCount,
                                                        paddedInputSize,
                                                        currentLayerIx,
                                                        currentImageIx,
                                                        backpropState,
                                                        paddedLayers[currentLayerIx], //send in the padded size of this particular layer
                                                        finalPaddedOutputCount,
                                                        layerDefinitions);

                        //write layer information into the GPU again and again! It needs to know current imnage and current layer
                        WriteUIntArrayToBuffer(device, inputBuffer, inputBufferIx, constBufferSizeInBytes, fullInputBuffer);

                        //dispatch a call for each layer -- no need to read in between
                        //the number of calls we make is the size of the layer for the network
                        device.ImmediateContext.Dispatch(trueLayerSize, 1, 1);

                        //DEBUG
                        //temp copy request
                        //networkNodeValues.CopyRangeTo(startLayerCountIx, trueLayerSize, allNodeCheck, startLayerCountIx);
                        //backpropErrorNodeValues.CopyRangeTo(startLayerCountIx, paddedLayers[currentLayerIx], allNodeCheck, startLayerCountIx);
                        //backpropErrorNodeValues.CopyRangeTo(0, allNodeCheck.Length, allNodeCheck, 0);
                        //networkNodeValues.CopyRangeTo(0, allNodeCheck.Length, allNodeCheck, 0);
                        //networkNodeValues.CopyRangeTo(0, allNodeCheck.Length, allNodeCheck, 0);

                        //going through layer ix
                        startLayerCountIx += paddedLayers[currentLayerIx];
                    }
                }
                if (false)
                {
                    if (true)
                    {
                        //now we copy the image outputs from the nodes -- then set the node error. 
                        networkNodeValues.CopyRangeTo(fullInputOutputArraySize - finalPaddedOutputCount, trueOutputCount, networkOutputs, 0);
                    }
                    //select the max node -- that's the victor!
                    int chosenIx = MaxValue(networkOutputs);

                    //determine if it's right/wrong -- what the correct targets are
                    //this is fake for now
                    float[] corrections = emptyArray(networkOutputs.Length, 1.0f);//CorrectFloatErrors(networkOutputs, 0, networkOutputs[0] + 1, networkOutputs[0]+1);
                    //corrections[0] = 1.0f;

                    if (false)
                    {
                        //now copy into the network errors
                        backpropErrorNodeValues.SetRange(fullInputOutputArraySize - finalPaddedOutputCount, corrections, 0, trueOutputCount);
                    }
                }

                backpropState = 1;

                //create the layer definitions to be send in to finish up backprop weight propogation
                fullInputBuffer = ConstructFullBuffer(
                                        groupsToDispatch,
                                        totalImageCount, //dont need
                                        paddedInputSize, //dont need
                                        0, //dont need 
                                        currentImageIx, //NEED for this calc
                                        backpropState, //NEED to say what we're doing for backprop
                                        paddedLayers[0], //don't need
                                        finalPaddedOutputCount, //NEED this for calculating how many outputs exist
                                        layerDefinitions);//don't need

                //write layer information into the GPU again and again! It needs to know current imnage and current layer
                WriteUIntArrayToBuffer(device, inputBuffer, inputBufferIx, constBufferSizeInBytes, fullInputBuffer);

                //set our floats to a buffer
                WriteFloatsToBuffer(device, backpropBuffer, backpropBufferIx, constFloatBufferSizeInBytes, new float[] { learningRate, momentum, 1.0f, 0.0f });

                //DEBUG
                //Random r = new Random();
                //allNodeCheck = allNodeCheck.Select(x => 0.0f).ToArray();
                //allNodeCheck[allNodeCheck.Length - finalPaddedOutputCount] = 1.0f;
                //allNodeCheck[allNodeCheck.Length - finalPaddedOutputCount + 1] = 4.0f + (float)r.NextDouble();
                //allNodeCheck[allNodeCheck.Length - finalPaddedOutputCount + 2] = 2.0f + (float)r.NextDouble();
                //allNodeCheck[allNodeCheck.Length - finalPaddedOutputCount + 3] = 2.0f;
                //networkNodeValues.SetRange(0, allNodeCheck, 0, allNodeCheck.Length);
                //allNodeCheck = allNodeCheck.Select(x => 0.0f).ToArray();
                //backpropErrorNodeValues.SetRange(0, allNodeCheck, 0, allNodeCheck.Length);

                //launch 1 group to handle the correction of the output layer --- shweet
                device.ImmediateContext.Dispatch(1, 1, 1);

                //DEBUG
                //float[] dcheck = new float[allNodeCheck.Length];
                //networkNodeValues.CopyRangeTo(0, dcheck.Length, dcheck, 0);
                //backpropErrorNodeValues.CopyRangeTo(0, allNodeCheck.Length, allNodeCheck, 0);
                //networkImageGuesses.CopyRangeTo(0, guessCheck.Length, guessCheck, 0);

                //set reasonable backprop values to send backwards
                //backpropErrorNodeValues.SetRange(fullInputOutputArraySize - finalPaddedOutputCount, nearEmptyOut, 0, nearEmptyOut.Length);
                
                //continue;

                //here we run the network backwards to calculate all errors across the nodes
                backpropState = 2;
                if (true)
                {
                    //we have processed the iamges going forward, now we propogate going backwards!!!
                    for (int currentLayerIx = layerSizes.Length - 1; currentLayerIx > 0; currentLayerIx--)
                    {
                        //step 1, set up the input buffer
                        //what information do we need to send into the constant buffer to make sure it's correct
                        fullInputBuffer = ConstructFullBuffer(
                                                        groupsToDispatch,
                                                        totalImageCount,
                                                        paddedInputSize,
                                                        currentLayerIx,
                                                        currentImageIx,
                                                        backpropState,
                                                        paddedLayers[currentLayerIx], //send in the padded size of this particular layer
                                                        finalPaddedOutputCount,
                                                        backpropLayerDefinitions);

                        //step 2, write layer information into the GPU again and again! It needs to know current imnage and current layer
                        WriteUIntArrayToBuffer(device, inputBuffer, inputBufferIx, constBufferSizeInBytes, fullInputBuffer);

                        //the actual previous layer size -- this will determine how many groups to run
                        int previousLayerSize = layerSizes[currentLayerIx - 1];

                        //step 3, run the network for this layer
                        //dispatch a call for each layer -- no need to read in between
                        //the number of calls we make is the size of the previous layer for the network
                        device.ImmediateContext.Dispatch(previousLayerSize, 1, 1);

                        //check health of network -- debugging purposes -- read all the backprop nodes
                        //DEBUG
                        //backpropErrorNodeValues.CopyRangeTo(0, allNodeCheck.Length, allNodeCheck, 0);

                    }
                }

                //the final piece is to run 1 final update -- with as many groups as there are weights
                //this will run a calculate to update the weights
                backpropState = 3;

                //DEBUG -- zero out anything non zero
                //allNodeCheck = allNodeCheck.Select(x => (x != 0 ? 1.0f : 0.0f)).ToArray();
                //allNodeCheck = allNodeCheck.Select(x => 0.0f).ToArray();
                //allNodeCheck[0] = 1.0f;
                //allNodeCheck[1] = -2.0f;
                //backpropErrorNodeValues.SetRange(0, allNodeCheck, 0, allNodeCheck.Length);
                ////allNodeCheck = allNodeCheck.Select(x => (x != 0 ? 2.0f : 0.0f)).ToArray();
                //allNodeCheck = allNodeCheck.Select(x => 0.0f).ToArray();
                //allNodeCheck[0] = 2.0f;
                //allNodeCheck[1] = -2.0f;
                //networkNodeValues.SetRange(0, allNodeCheck, 0, allNodeCheck.Length);

                //create the layer definitions to be send in to finish up backprop weight propogation
                fullInputBuffer = ConstructFullBuffer(
                                        groupsToDispatch,
                                        totalImageCount,
                                        paddedInputSize,
                                        0,
                                        currentImageIx,
                                        backpropState,
                                        paddedLayers[0], //send in the padded size of this particular layer
                                        finalPaddedOutputCount,
                                        layerDefinitions);

                //write layer information into the GPU again and again! It needs to know current imnage and current layer
                WriteUIntArrayToBuffer(device, inputBuffer, inputBufferIx, constBufferSizeInBytes, fullInputBuffer);

                //set our floats to a buffer
                WriteFloatsToBuffer(device, backpropBuffer, backpropBufferIx, constFloatBufferSizeInBytes, new float[] { learningRate, momentum, 1.0f, 0.0f});

                //step 3, run the network for this layer
                //dispatch a call for each layer -- no need to read in between
                //the number of calls we make is the size of the previous layer for the network
                device.ImmediateContext.Dispatch(fullInputOutputArraySize, 1, 1);

                //DEBUG
                //weightValues.CopyRangeTo(0, weightCheck.Length, weightCheck, 0);

            }

            backpropErrorNodeValues.CopyRangeTo(0, allNodeCheck.Length, allNodeCheck, 0);
            finalGPUTime = gpu.ElapsedMilliseconds;

            sw = new Stopwatch();
            sw.Start();

            //we'll figure out the copy locations later -- for now, we're simply happy we get it out quickly
            //sumValue.CopyRangeTo(0, activationCount, finalCalculations, 0);

            copyTime += sw.ElapsedMilliseconds;

            float absoluteTotalTime = gpuTotal.ElapsedMilliseconds;

            Console.WriteLine("GPU Calc Time: " + finalGPUTime + " Copy out time: " + copyTime + " Total Time: " + absoluteTotalTime + " MS Per Image: " + absoluteTotalTime/totalImageCount);
            
            networkImageGuesses.CopyRangeTo(0, guessCheck.Length, guessCheck, 0);
           
            //UH OH we don't dispose of anything
            KillMappedResource(device, inputBuffer, inputBufferIx);
            KillMappedResource(device, backpropBuffer, backpropBufferIx);

            Console.WriteLine("Now cpu calcs...");
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 8;
            po.MaxDegreeOfParallelism = 1;


            float cpuTime = runCPUNetwork(inputImages, paddedInputSize, randomWeights, inputOutputNodeValues, layerSizes, paddedLayers, po);


            Console.WriteLine("Speedup: " + cpuTime / absoluteTotalTime);
            Console.WriteLine("Speedup Test Complete");
        }

        static float runCPUNetwork(float[][] inputImages, int paddedInputSize, 
            float[] weights, float[] inputOutNodeValues, 
            int[] layers, int[] paddedLayers, 
            ParallelOptions po)
        {
            //stop watch please!
            Stopwatch cpuSW = new Stopwatch();
            cpuSW.Start();

            int weightStartIx = 0;
            int nodeStartIx = 0;
            int nodeOutputIx = 0;

            for (int i = 0; i < inputImages.Length; i++)
            {
                nodeStartIx = 0;
                weightStartIx = 0;
                nodeOutputIx = 0;

                for (int c = 0; c < layers.Length; c++)
                {
                    //read in the inputs
                    //if we're the input layer, we do things a little different -- read from a different array
                    float[] layerInputs = (c != 0 ? inputOutNodeValues : inputImages[i]);

                    int iPaddedSize = (c != 0 ? paddedLayers[c - 1] : paddedInputSize);

                    //size of the layer we need to loop over
                    int layerSize = layers[c];
                    int paddedLayerSize = paddedLayers[c];

                    int inputLoopSize = Math.Min(iPaddedSize, layerInputs.Length);
                    
                    //increment before the output process
                    if(c!= 0)
                        nodeOutputIx += paddedLayerSize;

                    //do this in parallel
                    Parallel.For(0, layerSize, po, l =>
                    //for (int l = 0; l < layerSize; l++)
                    {
                        //correct
                        //weight start -- keep incrementing with padded size
                        int weightBegin = weightStartIx + l * iPaddedSize;


                        float sum = 0;
                        //now multiply inputs by other things
                        for (int w = 0; w < inputLoopSize; w++)
                        {
                            //now loop through and multiple
                            sum += layerInputs[nodeStartIx + w] * weights[weightBegin + w];
                        }

                        //set the input/output!
                        inputOutNodeValues[nodeOutputIx + l] = sum;
                    });

                    //increment please! Only if we're not the first layer though -- we use different input
                    if(c!=0)
                        nodeStartIx += paddedLayerSize;

                    //now make sure to update our weight start ix
                    weightStartIx += paddedLayerSize * iPaddedSize;
                }
            }


            float finalCPUTime = cpuSW.ElapsedMilliseconds;

            Console.WriteLine("CPU Summation Time: " + finalCPUTime);

            return finalCPUTime;
        }
        static void WriteFloatsToBuffer(Device device, Buffer floatBuffer, int subresource, int totalByteSize, float[] values)
        {
            DataBox input = DataboxMapResource(device, floatBuffer, subresource);
            WriteFloatsToBuffer(device, floatBuffer, input, totalByteSize, values);
            KillMappedResource(device, floatBuffer, subresource);
        }
        static void WriteFloatsToBuffer(Device device, Buffer floatBuffer, DataBox input, int totalByteSize, float[] values)
        {
            //DataBox input = device.ImmediateContext.MapSubresource(floatBuffer, subresource, MapMode.WriteDiscard, MapFlags.None);//(inputBuffer, 0, constBufferSizeInBytes, MapMode.WriteDiscard, MapFlags.None);

            byte[] finalBytes = new byte[values.Length * sizeof(float)];

            int destinationIx = 0;
            for (int i = 0; i < values.Length; i++)
            {
                var bUint = BitConverter.GetBytes(values[i]);
                int len = bUint.Length;
                Array.Copy(bUint, 0, finalBytes, destinationIx, len);
                destinationIx += len;
            }

            //write everything to the buffer
            input.Data.Position = 0;
            input.Data.Write(finalBytes, 0, finalBytes.Length);

            //unmap that mapped resource
            //device.ImmediateContext.UnmapSubresource(floatBuffer, subresource);
        }


        static Buffer CreateBuffer(Device device, int totalByteSize)
        {
            BufferDescription inputBufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = totalByteSize,
                StructureByteStride = sizeof(int),
                Usage = ResourceUsage.Dynamic,
            };

            //create the buffer accordingly
            return new Buffer(device, inputBufferDescription);

        }
        static Buffer WriteUIntsToBuffer(Device device, Buffer inputBuffer, DataBox input, int totalByteSize, params int[] values)
        {
            return WriteUIntArrayToBuffer(device, inputBuffer, input, totalByteSize, values);
        }

        static DataBox DataboxMapResource(Device device, Buffer inputBuffer, int subresource)
        {
           return device.ImmediateContext.MapSubresource(inputBuffer, subresource, MapMode.WriteDiscard, MapFlags.None);//(inputBuffer, 0, constBufferSizeInBytes, MapMode.WriteDiscard, MapFlags.None);
        }

        static void KillMappedResource(Device device, Buffer inputBuffer, int subresource)
        {
            device.ImmediateContext.UnmapSubresource(inputBuffer, subresource);
        }

        static Buffer WriteUIntArrayToBuffer(Device device, Buffer inputBuffer, int subresource, int totalByteSize, int[] values)
        {
            DataBox input = DataboxMapResource(device, inputBuffer, subresource);
            WriteUIntArrayToBuffer(device, inputBuffer, input, totalByteSize, values);
            KillMappedResource(device, inputBuffer, subresource);
            return inputBuffer;
        }
        static Buffer WriteUIntArrayToBuffer(Device device, Buffer inputBuffer, DataBox input, int totalByteSize, int[] values)
        {
            //DataBox input = device.ImmediateContext.MapSubresource(inputBuffer, subresource, MapMode.WriteDiscard, MapFlags.None);//(inputBuffer, 0, constBufferSizeInBytes, MapMode.WriteDiscard, MapFlags.None);
           //device.ImmediateContext.UpdateSubresource(
            //input.Data.Position = 0;
            byte[] finalBytes = new byte[values.Length * sizeof(int)];
            
            int destinationIx = 0;
            for(int i=0; i < values.Length; i++)
            {
                var bUint = BitConverter.GetBytes(values[i]);
                int len = bUint.Length;
                Array.Copy(bUint, 0, finalBytes, destinationIx, len);
                destinationIx += len;
            }

            //write everything to the buffer
            input.Data.Position = 0;
            input.Data.Write(finalBytes, 0, finalBytes.Length);

            //unmap that mapped resource
            //device.ImmediateContext.UnmapSubresource(inputBuffer, subresource);

            //send back input buffer
            return inputBuffer;
        }


    }
}
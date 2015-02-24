using System;
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
            MainReduce();
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

        static int[] ConstructFullBuffer(int dispatchSize, int totalTestImages, int inputPixelSize,  int currentLayerIx, int currentImageIx, int[] layerDefinitions)
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
                0,
                0,
                0
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
                finalDefinitions[outputStartIx] = outputStartIx;

                //increment the size of the array -- this where everything is stored!
                outputStartIx += layerSizes[i];
            }

            //these are the total sizes of both the number of nodes -- network size
            //as well as the total number of required weights -- connection size!
            totalWeightSize = weightStartIx;
            totalNetworkSize = outputStartIx;

            return finalDefinitions;
        }

        #endregion

        #region Random/Empty Input Array Helpers

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
                    rImage[w] = (overwriteInput.HasValue ? overwriteInput.Value : (float)r.NextDouble());

                rInputs[i] = rImage;
            }

            //all done!
            return rInputs;
        }

        static float[] DefaultRandomWeightArray(int[] layerSizes, int[] paddedSizes, int fullSize, float? overwriteWeight = null, int? seed = null)
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

                for (int w = 0; w < padded; w++)
                    if (w < original)
                        finalWeights[startIx + w] = (overwriteWeight.HasValue ? overwriteWeight.Value : (float)r.NextDouble());

                //increment the layer amout and carry on!
                startIx += padded;
            }

            return finalWeights;
        }

        #endregion

        //how many threads are launched for the main entry point
        //hardcoded in shader
        static const int computeShaderHardcodedThreadSize = 512;

        //defined in shader as well
        static const int MAX_LAYERS = 16;


        static int[] defaultLayerSizes = new int[] { 1000, 1000, 100, 10 };

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
            Buffer inputBuffer = CreateBuffer(device, constBufferSizeInBytes);// WriteUIntsToBuffer(device, bufferIx++, constBufferSizeInBytes, totalFloats, groupsToDispatch);

        
            //these are our input sizes
            int inputPixelSize = 785;
            int totalImageCount = 100;

            int[] layerSizes = defaultLayerSizes;

            //send in random input images
            float? overwriteInputs = 2.0f;
            float[][] randomInputImages = DefaultRandomInputImages(inputPixelSize, totalImageCount, overwriteInputs);

            //send it to shader 
            RunShader(device, computeShader, inputBuffer, constBufferSizeInBytes,
                inputPixelSize, totalImageCount, layerSizes, randomInputImages);
        }

     
        static void RunShader(Device device, 
            ComputeShader computeShader, 
            Buffer inputBuffer, 
            int constBufferSizeInBytes,
            int inputPixelSize,
            int totalImageCount, 
            int[] layerSizes, 
            float[][] inputImages)
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

            //how much data will we load? padded values x total imagecount
            int totalInputCapacity = fullPaddedInputValues.Length;

            //how many connections do we need for this big object
            int fullWeightArraySize = 0;

            //how many nodes exist inside the network?
            int fullInputOutputArraySize = 0;

            //use our layer sizes to create a full definiton of the network that can be executed on the GPU
            //we also measure node and connection sizes with this function -- mind you these are the full padded sizes
            int[] layerDefinitions = ConstructLayerDefinitions(paddedLayers, inputPixelSize, totalImageCount, out fullWeightArraySize, out fullInputOutputArraySize);

            //finally, we create our empty node array -- this will store our network node values during network execution
            float[] inputOutputNodeValues = emptyArray(fullInputOutputArraySize);

            //set up our constant buffer with our inputs -- yazoooooooo
            int inputBufferIx = 0;

            //how many dispatches? Only every 1 at a time (so we say)
            int groupsToDispatch = 1;

            if (groupsToDispatch > 1)
                throw new NotImplementedException("I don't believe this is verified to work with more than 1 dispatch group size right now");



            //create a random set of weights (or choose to overwrite with default weight
            //currently useing default weight of 1 for debug purposes
            float? overWrite = 1.0f; //set to null if you want true random
            float[] randomWeights = DefaultRandomWeightArray(layerSizes, paddedLayers, fullWeightArraySize, overWrite);

            //copy in the weights into a structure weight arary -- prepare it for the correct size
            GPUList<float> weightValues = new GPUList<float>(device.ImmediateContext);
            weightValues.Capacity = randomWeights.Length;

            //Create GPUList of particles using the immediate context
            GPUList<float> inputImageValues = new GPUList<float>(device.ImmediateContext);
            inputImageValues.Capacity = totalInputCapacity;

            //finally, we create the network node array
            GPUList<float> networkNodeValues = new GPUList<float>(device.ImmediateContext);
            networkNodeValues.Capacity = fullInputOutputArraySize;

            //get ready to record these objects-- this time includes the time to copy everything
            //separate gpu clacle for setup
            Stopwatch gpuTotal = new Stopwatch();
            gpuTotal.Start();

            //set ALL of the input images -- in reality -- this call is made ONCE for all networks ever
            inputImageValues.AddRange(fullPaddedInputValues);

            //set the weights inside the gpu -- this is for the full network
            weightValues.AddRange(randomWeights);

            //finally, set the default in/out nodes -- this might not be necessary
            networkNodeValues.AddRange(inputOutputNodeValues);


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
            device.ImmediateContext.ComputeShader.SetConstantBuffer(inputBuffer, 0);
            
            //separate gpu clacle for setup
            Stopwatch gpu = new Stopwatch();
            gpu.Start();

            Stopwatch sw = null;
            float copyTime = 0;

            float finalGPUTime = 0;

            for (int currentImageIx = 0; currentImageIx < totalImageCount; currentImageIx++)
            {
                //we need to process this image across all layers
                for (int currentLayerIx = 0; currentLayerIx < layerSizes.Length; currentLayerIx++)
                {
                    //how many actual nodes are here -- not whats padded in memory
                    int trueLayerSize = layerSizes[currentLayerIx];

                    //what information do we need to send into the constant buffer to make sure it's correct
                    int[] fullInputBuffer = ConstructFullBuffer(groupsToDispatch, totalImageCount, paddedInputSize, currentLayerIx, currentImageIx, layerDefinitions);

                    //write layer information into the GPU again and again! It needs to know current imnage and current layer
                    WriteUIntArrayToBuffer(device, inputBuffer, inputBufferIx, constBufferSizeInBytes, fullInputBuffer);

                    //dispatch a call for each layer -- no need to read in between
                    //the number of calls we make is the size of the layer for the network
                    device.ImmediateContext.Dispatch(trueLayerSize, 1, 1);
                }
            }

            finalGPUTime = gpu.ElapsedMilliseconds;

            sw = new Stopwatch();
            sw.Start();

            //we'll figure out the copy locations later -- for now, we're simply happy we get it out quickly
            //sumValue.CopyRangeTo(0, activationCount, finalCalculations, 0);

            copyTime += sw.ElapsedMilliseconds;

            float absoluteTotalTime = gpuTotal.ElapsedMilliseconds;

            Console.WriteLine("GPU Calc Time: " + finalGPUTime + " Copy out time: " + copyTime + " Total Time: " + absoluteTotalTime);
            Console.WriteLine("Now cpu calcs...");
            
        }

        static Buffer WriteFloatsToBuffer(Device device, int subresource, int totalByteSize, float[] values)
        {
            BufferDescription inputBufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = totalByteSize,
                StructureByteStride = sizeof(float),
                Usage = ResourceUsage.Dynamic,
            };

            //create the buffer accordingly
            Buffer inputBuffer = new Buffer(device, inputBufferDescription);

            DataBox input = device.ImmediateContext.MapSubresource(inputBuffer, subresource, MapMode.WriteDiscard, MapFlags.None);//(inputBuffer, 0, constBufferSizeInBytes, MapMode.WriteDiscard, MapFlags.None);

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
            input.Data.Write(finalBytes, 0, finalBytes.Length);

            //unmap that mapped resource
            device.ImmediateContext.UnmapSubresource(inputBuffer, subresource);

            //send back input buffer
            return inputBuffer;
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
        static Buffer WriteUIntsToBuffer(Device device, Buffer inputBuffer, int subresource, int totalByteSize, params int[] values)
        {
            return WriteUIntArrayToBuffer(device, inputBuffer, subresource, totalByteSize, values);
        }

       static Buffer WriteUIntArrayToBuffer(Device device, Buffer inputBuffer, int subresource, int totalByteSize, int[] values)
        {
            DataBox input = device.ImmediateContext.MapSubresource(inputBuffer, subresource, MapMode.WriteDiscard, MapFlags.None);//(inputBuffer, 0, constBufferSizeInBytes, MapMode.WriteDiscard, MapFlags.None);

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
            input.Data.Write(finalBytes, 0, finalBytes.Length);

            //unmap that mapped resource
            device.ImmediateContext.UnmapSubresource(inputBuffer, subresource);

            //send back input buffer
            return inputBuffer;
        }

       static void MainReduce()
       {
           Device device = new Device(DriverType.Hardware);//, DeviceCreationFlags.Debug);//, FeatureLevel.Level_11_0);
           bool forceReload = true;

           ComputeShader computeShader = Helper.LoadComputeShader(device, "EfficientArrayCompute.hlsl", "runNetwork", forceReload);
           //ComputeShader computeShader = Helper.LoadComputeShader(device, "EfficientArrayCompute.hlsl", "reduceNonOptimal", forceReload);
           //ComputeShader computeShader = Helper.LoadComputeShader(device, "EfficientArrayCompute.hlsl", "reduceNonOptimal", forceReload);
           //ComputeShader computeShader = Helper.LoadComputeShader(device, "EfficientArrayCompute.hlsl", "simpleMultiply", forceReload);

           if (computeShader == null)
               return;

           //inputs we are sending in (must be >= 4 ints for whatever reason
           const int bufferIntCount = 8;
           const int constBufferSizeInBytes = bufferIntCount * sizeof(int);

           //I HAVE NO IDEA WHAT THESE NUMBERS ARE YET
           //uint n = 32;
           //uint dimensionXSize = 2;
           int computeShaderHardcodedThreadSize = 512;
           int totalFloats = 785;

           int networkCount = 300;
           int imageCount = 6000;

           int paddedCount = (int)(Math.Ceiling((float)totalFloats / computeShaderHardcodedThreadSize)*computeShaderHardcodedThreadSize);

           //uint groupsToDispatch = (uint)Math.Ceiling((float)totalFloats / (2 *computeShaderHardcodedThreadSize));
           int groupsToDispatch = (int)Math.Ceiling((float)totalFloats / (2 * computeShaderHardcodedThreadSize));
           //uint groupsToDispatch = 1;

           //at least 1
           groupsToDispatch = Math.Max(1, groupsToDispatch);

           if(groupsToDispatch > 1)
            Console.WriteLine("I don't believe this is verified to work with more than 1 dispatch group size right now");

           //set up our constant buffer with our inputs -- yazoooooooo
           int bufferIx = 0;
          
           Random r = new Random();
           float[] weights = new float[paddedCount * networkCount];
           for (int i = 0; i < weights.Length; i++)
               if (i % paddedCount < totalFloats)
                   weights[i] = (float)Math.Floor((float)i / paddedCount) + 1;// 1.0f;// (float)r.NextDouble();

           //copy in the weights into the constant input buffer
           GPUList<float> weightValues = new GPUList<float>(device.ImmediateContext);

           //Create GPUList of particles using the immediate context
           GPUList<float> allValues = new GPUList<float>(device.ImmediateContext);

           float sum = 0;
           float[] allFloats = new float[paddedCount * imageCount];
           for (int i = 0; i < allFloats.Length; i++)
           {
               if (i % paddedCount < totalFloats)
                   allFloats[i] = (float)Math.Floor((float)i / paddedCount) + 1;//(i % paddedCount) + 1;
                   //allFloats[i] = (float)r.NextDouble(); //(i % paddedCount) + 1;
           }

           //get ready to record these objects-- this time includes the time to copy everything
           //separate gpu clacle for setup
           Stopwatch gpuTotal = new Stopwatch();
           gpuTotal.Start();

           //add the inputs
           Buffer inputBuffer = CreateBuffer(device, constBufferSizeInBytes);// WriteUIntsToBuffer(device, bufferIx++, constBufferSizeInBytes, totalFloats, groupsToDispatch);

           //set the weights inside the gpu
           weightValues.Capacity = weights.Length;
           weightValues.AddRange(weights);

           //add it in for memory purposes
           allValues.Capacity = allFloats.Length;
           allValues.AddRange(allFloats);

           //setting up the breakdown of the network
           int dispatchImageChunks = 5;
           int dispatchNetworkChunks = 5;

           int networkIxStart = 0;
           int imageIxStart = 0;

           int netChunk = networkCount / dispatchNetworkChunks;
           int imageChunk = imageCount / dispatchImageChunks;

           // Create GPUList of particles using the immediate context
           GPUList<float> sumValue = new GPUList<float>(device.ImmediateContext);
           int activationCount = networkCount * imageCount;
           sumValue.Capacity = activationCount;
           float[] az = new float[activationCount];
           for (int i = 0; i < activationCount; i++)
               az[i] = 0f;
           sumValue.AddRange(az);

           //now set it in our gpu
           // Run the compute shader
           device.ImmediateContext.ComputeShader.Set(computeShader);
           int unorderIx = 0;
           device.ImmediateContext.ComputeShader.SetUnorderedAccessView(weightValues.UnorderedAccess, unorderIx++);
           device.ImmediateContext.ComputeShader.SetUnorderedAccessView(allValues.UnorderedAccess, unorderIx++);
           device.ImmediateContext.ComputeShader.SetUnorderedAccessView(sumValue.UnorderedAccess, unorderIx++);
           device.ImmediateContext.ComputeShader.SetConstantBuffer(inputBuffer, 0);

           float[] finalCalculations = new float[activationCount];

           //separate gpu clacle for setup
           Stopwatch gpu = new Stopwatch();
           gpu.Start();

           Stopwatch sw = null;
           float copyTime = 0;

           //int startCopyIx = 0;
           //int networksAtATime = 1;

           //int totalDispatches = (int)(Math.Ceiling((float)networkCount / networksAtATime));

           //WriteUIntsToBuffer(device, 0, constBufferSizeInBytes, totalFloats, groupsToDispatch, (uint)imageCount, paddedCount, (uint)0);// i*paddedCount);
           //device.ImmediateContext.Dispatch(imageCount, networkCount, 1);
           float finalGPUTime = 0;

           int sloppyCopyIx = 0;
           //we need to loop through and run a bunch of dispatch calls
            for (int t = 0; t < dispatchNetworkChunks; t++)
            {
                imageIxStart = 0;
                for (int i = 0; i < dispatchImageChunks; i++)
                {
                    //sw = new Stopwatch();
                    //sw.Start();
                    WriteUIntsToBuffer(device, inputBuffer, 0, constBufferSizeInBytes, totalFloats, 
                        groupsToDispatch, imageChunk, paddedCount, imageIxStart, networkIxStart, sloppyCopyIx);// i*paddedCount);
                    
                    device.ImmediateContext.Dispatch(imageChunk, netChunk, 1);
                   
                    //finalGPUTime += sw.ElapsedMilliseconds;
                    
                    sloppyCopyIx += imageChunk * netChunk;

                    //sw = new Stopwatch();
                    //sw.Start();
                    ////we'll figure out the copy locations later -- for now, we're simply happy we get it out quickly
                    //sumValue.CopyRangeTo(0, imageChunk * netChunk, finalCalculations, sloppyCopyIx);
                    //sloppyCopyIx += imageChunk * netChunk;

                    //copyTime += sw.ElapsedMilliseconds;

                   //for(int n=0; n < netChunk; n++)
                        //sumValue.CopyRangeTo(imageChunk*n, imageChunk, finalCalculations, (networkIxStart + n) * imageCount + imageIxStart);

                   //sumValue.CopyRangeTo(0, imageChunk * netChunk, finalCalculations, networkIxStart * imageCount + imageIxStart);

                   //for (int c = 0; c < sumValue.Count; c++)
                   //    Console.WriteLine("C: " + sumValue[c]);

                   imageIxStart += imageChunk;
               }

               networkIxStart += netChunk;
           }

            finalGPUTime = gpu.ElapsedMilliseconds;

            sw = new Stopwatch();
            sw.Start();
            //we'll figure out the copy locations later -- for now, we're simply happy we get it out quickly
            sumValue.CopyRangeTo(0, activationCount, finalCalculations, 0);

            copyTime += sw.ElapsedMilliseconds;
          
          
           //run it a bunch o' times
           //for (int i = 0; i < totalDispatches; i++)
           //{
           //    int networkStartIx = i * networksAtATime;

           //    int chunkOfNetworks = Math.Min(networksAtATime, networkCount - networkStartIx);

           //    Console.WriteLine("Start: " + i);
           //    WriteUIntsToBuffer(device, 0, constBufferSizeInBytes, totalFloats, groupsToDispatch, (uint)imageCount, paddedCount, (uint) networkStartIx);// i*paddedCount);
           //    //device.ImmediateContext.Dispatch((int)groupsToDispatch, 1, 1);
           //    device.ImmediateContext.Dispatch((int)(imageCount * groupsToDispatch * chunkOfNetworks), 1, 1);
           //    Console.WriteLine("Dispatched: " + i);

           //    sw = new Stopwatch();
           //    sw.Start();

           //    sumValue.CopyRangeTo(0, imageCount * chunkOfNetworks, finalCalculations, startCopyIx);
           //    Console.WriteLine("Copied: " + i);

           //    startCopyIx += imageCount * chunkOfNetworks;
           //    copyTime  += sw.ElapsedMilliseconds;

           //}

           //sumValue.CopyRangeTo(0, activationCount, finalCalculations, 0);

           //float finalGPUTime = gpu.ElapsedMilliseconds;

        
         
           float absoluteTotalTime = gpuTotal.ElapsedMilliseconds;

           Console.WriteLine("GPU Calc Time: " + finalGPUTime + " Copy out time: " + copyTime + " Total Time: " + absoluteTotalTime);
           //for(int i=0; i < doItAbunch; i++)
           //    Console.WriteLine("GPU Summation Time Inner: " + elapsed[i]);


           // Print the particles again
           //Console.WriteLine("\n\nAfter Simple:");
           //foreach (float p in allValues)
           //    Console.WriteLine(p.ToString());

           Stopwatch cpuSW = new Stopwatch();

           cpuSW.Start();

           int startIx = 0;

           float[] cpuCalculations = new float[activationCount];
           int ax = 0;
           //for (int t = 0; t < networkCount; t++)
           ParallelOptions po = new ParallelOptions();
           po.MaxDegreeOfParallelism = 8;

           Parallel.For(0, networkCount, po, t =>
               {
                   //weights start at network start
                   int networkStart = (int)(t * paddedCount);

                   //for each image
                   for (int ix = 0; ix < imageCount; ix++)
                   {
                       int imageStart = (int)(ix * paddedCount);
                       sum = 0;
                       for (int i = 0; i < totalFloats; i++)
                       {
                           sum += weights[networkStart + i] * allFloats[imageStart + i];
                       }

                       //grab the calc for each weight/float combo sum mult
                       cpuCalculations[t*imageCount + ix] = sum;
                   }

                   //skip ahead the padded amount -- we do the sums here
                   //startIx += (int)paddedCount;
               });

           float finalCPUTime = cpuSW.ElapsedMilliseconds;
           Console.WriteLine("CPU Summation Time: " + finalCPUTime);

           Console.WriteLine("Speedup: " + finalCPUTime / absoluteTotalTime);


           for (int i = 0; i < activationCount; i++)
           {
               bool gpuEqualsCPU = Math.Abs(cpuCalculations[i] - finalCalculations[i]) < 0.001;

               Console.WriteLine("They are equal? " + (gpuEqualsCPU ? "YES" : "NO! :(") + " dif: " + Math.Abs(cpuCalculations[i] - finalCalculations[i]));
           }



           //Console.WriteLine("\n\n Each Sum Value:");
           //foreach (float p in sumValue)
           //    Console.WriteLine(p.ToString());


           //Console.WriteLine("\nFinal GPU Value:");
           //float f = sumValue.Sum();
           //Console.WriteLine(f);

           //int halfFloat = (int)totalFloats / 2;
           ////Console.WriteLine("OOps: " + allValues.Sum());// allValues.ToList().GetRange(halfFloat, halfFloat).Sum());
           //Console.WriteLine("True Sum: " + sum);

           //bool gpuEqualsCPU = Math.Abs(sum  - f) < 0.001;
           //Console.WriteLine("They are equal? " +  (gpuEqualsCPU ? "YES" : "NO! :("));

           allValues.ShaderResource.Dispose();
           sumValue.ShaderResource.Dispose();
           inputBuffer.Dispose();
           device.Dispose();
       }

        static bool MainSimple()
        {
            Device device = new Device(DriverType.Hardware);//, DeviceCreationFlags.Debug);//, FeatureLevel.Level_11_0);
            bool forceReload = true;

            ComputeShader computeShader = Helper.LoadComputeShader(device, "SlimExampleCompute.hlsl", "testStructure", forceReload);

            if (computeShader == null)
                return true;

            //inputs we are sending in (must be >= 4 ints for whatever reason
            const int bufferIntCount = 4;
            const int constBufferSizeInBytes = bufferIntCount * sizeof(uint);

            //I HAVE NO IDEA WHAT THESE NUMBERS ARE YET
            int n = 32;
            int dimensionXSize = 2;

            int bufferIx = 0;
            //set up our constant buffer with our inputs -- yazoooooooo
            Buffer inputBuffer = CreateBuffer(device, constBufferSizeInBytes);
            WriteUIntsToBuffer(device, inputBuffer, bufferIx++, constBufferSizeInBytes, n, dimensionXSize);
       
            // Create GPUList of particles using the immediate context
            GPUList<float> allValues = new GPUList<float>(device.ImmediateContext);

            int totalFloats = 16;
            float[] allFloats = new float[totalFloats];
            for (int i = 0; i < totalFloats; i++)
                allFloats[i] = i + 1;

            //add it in for memory purposes
            allValues.AddRange(allFloats);

            //now set it in our gpu
            // Run the compute shader
            device.ImmediateContext.ComputeShader.Set(computeShader);
            device.ImmediateContext.ComputeShader.SetUnorderedAccessView(allValues.UnorderedAccess, 0);
            device.ImmediateContext.ComputeShader.SetConstantBuffer(inputBuffer, 0);
            device.ImmediateContext.Dispatch(1, 1, 1);

            // Print the particles again
            Console.WriteLine("\n\nAfter Integration:");
            foreach (float p in allValues)
                Console.WriteLine(p.ToString());

            return true;


            //// Add some values
            //const int elementCount = 4;
            //const int bufferSizeInBytes = elementCount * sizeof(uint);

            //// The input to the computation will be a constant buffer containing
            //// integers (in floating point representation) from 1 to numberOfElements,
            //// inclusive. The compute shader itself will double these values and write
            //// them to the output buffer.
            //BufferDescription inputBufferDescription = new BufferDescription
            //{
            //    BindFlags = BindFlags.ConstantBuffer,
            //    CpuAccessFlags = CpuAccessFlags.Write,
            //    OptionFlags = ResourceOptionFlags.None,
            //    SizeInBytes = bufferSizeInBytes,
            //    StructureByteStride = sizeof(uint),
            //    Usage = ResourceUsage.Dynamic,
            //};

            //Buffer inputBuffer = new Buffer(device, inputBufferDescription);
            //DataBox input = device.ImmediateContext.MapSubresource(inputBuffer, MapMode.WriteDiscard, MapFlags.None);
            //for (int value = 1; value <= elementCount; ++value)
            //    input.Data.Write((uint)value);
            //device.ImmediateContext.UnmapSubresource(inputBuffer, 0);

            //// A staging buffer is used to copy data between the CPU and GPU; the output
            //// buffer (which gets the computation results) cannot be mapped directly.
            //BufferDescription stagingBufferDescription = new BufferDescription
            //{
            //    BindFlags = BindFlags.None,
            //    CpuAccessFlags = CpuAccessFlags.Read,
            //    OptionFlags = ResourceOptionFlags.StructuredBuffer,
            //    SizeInBytes = bufferSizeInBytes,
            //    StructureByteStride = sizeof(float),
            //    Usage = ResourceUsage.Staging,
            //};
            //Buffer stagingBuffer = new Buffer(device, stagingBufferDescription);

            //// The output buffer itself, and the view required to bind it to the pipeline.
            //BufferDescription outputBufferDescription = new BufferDescription
            //{
            //    BindFlags = BindFlags.UnorderedAccess | BindFlags.ShaderResource,
            //    OptionFlags = ResourceOptionFlags.StructuredBuffer,
            //    SizeInBytes = bufferSizeInBytes,
            //    StructureByteStride = sizeof(float),
            //    Usage = ResourceUsage.Default,
            //};
            //Buffer outputBuffer = new Buffer(device, outputBufferDescription);
            //UnorderedAccessViewDescription outputViewDescription = new UnorderedAccessViewDescription
            //{
            //    ElementCount = elementCount,
            //    Format = SlimDX.DXGI.Format.Unknown,
            //    Dimension = UnorderedAccessViewDimension.Buffer
            //};
            //UnorderedAccessView outputView = new UnorderedAccessView(device, outputBuffer, outputViewDescription);

            //device.ImmediateContext.ComputeShader.Set(computeShader);
            //device.ImmediateContext.ComputeShader.SetUnorderedAccessView(outputView, 0);
            //device.ImmediateContext.ComputeShader.SetConstantBuffer(inputBuffer, 0);

            //// Compute shaders execute on multiple threads at the same time. Those execution
            //// threads are grouped; Dispatch() indicates how many groups in the X, Y and Z
            //// dimension will be utilized. The shader itself specified how many threads per
            //// group (also in X, Y and Z dimensions) to use via the [numthreads] attribute.
            //// In this sample, one thread group will be used with 16 threads, each thread
            //// will process one element of the input data.
            //device.ImmediateContext.Dispatch(1, 1, 1);

            //device.ImmediateContext.CopyResource(outputBuffer, stagingBuffer);
            //DataBox output = device.ImmediateContext.MapSubresource(stagingBuffer, MapMode.Read, MapFlags.None);

            //Console.Write("Results:");
            //for (int index = 0; index < elementCount; ++index)
            //    Console.Write(" {0}", output.Data.Read<float>());
            //device.ImmediateContext.UnmapSubresource(outputBuffer, 0);
            //Console.WriteLine();

            //computeShader.Dispose();
            //outputView.Dispose();
            //outputBuffer.Dispose();
            //stagingBuffer.Dispose();
            //inputBuffer.Dispose();
            //device.Dispose();

            //return true;
        }
    }
}
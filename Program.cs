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

       static Buffer WriteUIntsToBuffer(Device device, int subresource, int totalByteSize, params uint[] values)
        {
            BufferDescription inputBufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = totalByteSize,
                StructureByteStride = sizeof(uint),
                Usage = ResourceUsage.Dynamic,
            };
            
            //create the buffer accordingly
            Buffer inputBuffer = new Buffer(device, inputBufferDescription);

            DataBox input = device.ImmediateContext.MapSubresource(inputBuffer, subresource, MapMode.WriteDiscard, MapFlags.None);//(inputBuffer, 0, constBufferSizeInBytes, MapMode.WriteDiscard, MapFlags.None);

            byte[] finalBytes = new byte[values.Length*sizeof(uint)];
            
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

           ComputeShader computeShader = Helper.LoadComputeShader(device, "EfficientArrayCompute.hlsl", "reduceArray", forceReload);
           //ComputeShader computeShader = Helper.LoadComputeShader(device, "EfficientArrayCompute.hlsl", "reduceNonOptimal", forceReload);
           //ComputeShader computeShader = Helper.LoadComputeShader(device, "EfficientArrayCompute.hlsl", "reduceNonOptimal", forceReload);
           //ComputeShader computeShader = Helper.LoadComputeShader(device, "EfficientArrayCompute.hlsl", "simpleMultiply", forceReload);

           if (computeShader == null)
               return;

           //inputs we are sending in (must be >= 4 ints for whatever reason
           const int bufferIntCount = 4;
           const int constBufferSizeInBytes = bufferIntCount * sizeof(uint);

           //I HAVE NO IDEA WHAT THESE NUMBERS ARE YET
           //uint n = 32;
           //uint dimensionXSize = 2;
           int computeShaderHardcodedThreadSize = 512;
           uint totalFloats = 785;
           int doItAbunch = 1000;
           //uint groupsToDispatch = (uint)Math.Ceiling((float)totalFloats / (2 *computeShaderHardcodedThreadSize));
           uint groupsToDispatch = (uint)Math.Ceiling((float)totalFloats / (2 * computeShaderHardcodedThreadSize));
           //uint groupsToDispatch = 1;

           //at least 1
           groupsToDispatch = Math.Min(1, groupsToDispatch);

           //set up our constant buffer with our inputs -- yazoooooooo
           int bufferIx = 0;
          
           Random r = new Random();
           float[] weights = new float[totalFloats];
           for (int i = 0; i < weights.Length; i++)
               weights[i] = (float)r.NextDouble();

           //copy in the weights into the constant input buffer
           GPUList<float> weightValues = new GPUList<float>(device.ImmediateContext);

          

           // Create GPUList of particles using the immediate context
           GPUList<float> allValues = new GPUList<float>(device.ImmediateContext);

           float sum = 0;
           float[] allFloats = new float[totalFloats];
           for (int i = 0; i < totalFloats; i++)
           {
               allFloats[i] = i + 1;
           }

           //get ready to record these objects-- this time includes the time to copy everything
           Stopwatch gpu = new Stopwatch();
           gpu.Start();

           //add the inputs
           Buffer inputBuffer = WriteUIntsToBuffer(device, bufferIx++, constBufferSizeInBytes, totalFloats, groupsToDispatch);

           //set the weights inside the gpu
           weightValues.AddRange(weights);

           //add it in for memory purposes
           allValues.AddRange(allFloats);

           // Create GPUList of particles using the immediate context
           GPUList<float> sumValue = new GPUList<float>(device.ImmediateContext);
           for (int i = 0; i < groupsToDispatch; i++)
               sumValue.Add(0);

           //now set it in our gpu
           // Run the compute shader
           device.ImmediateContext.ComputeShader.Set(computeShader);
           int unorderIx = 0;
           device.ImmediateContext.ComputeShader.SetUnorderedAccessView(weightValues.UnorderedAccess, unorderIx++);
           device.ImmediateContext.ComputeShader.SetUnorderedAccessView(allValues.UnorderedAccess, unorderIx++);
           device.ImmediateContext.ComputeShader.SetUnorderedAccessView(sumValue.UnorderedAccess, unorderIx++);
           device.ImmediateContext.ComputeShader.SetConstantBuffer(inputBuffer, 0);

           //run it 100 times
           for (uint i = 0; i < doItAbunch; i++)
           {
               WriteUIntsToBuffer(device, 0, constBufferSizeInBytes, totalFloats, groupsToDispatch, i);
               device.ImmediateContext.Dispatch((int)groupsToDispatch, 1, 1);
           }

           float finalGPUTime = gpu.ElapsedMilliseconds;
           Console.WriteLine("GPU Summation Time: " + finalGPUTime);


           // Print the particles again
           //Console.WriteLine("\n\nAfter Simple:");
           //foreach (float p in allValues)
           //    Console.WriteLine(p.ToString());

           Stopwatch c = new Stopwatch();

           c.Start();

           for (int t = 0; t < doItAbunch; t++)
           {
               sum = 0;
               for (int i = 0; i < totalFloats; i++)
               {
                   sum += weightValues[i] * allFloats[i];
               }
           }

           float finalCPUTime = c.ElapsedMilliseconds;
           Console.WriteLine("CPU Summation Time: " + finalCPUTime);

           Console.WriteLine("Speedup: " + finalCPUTime / finalGPUTime);


           Console.WriteLine("\n\n Each Sum Value:");
           foreach (float p in sumValue)
               Console.WriteLine(p.ToString());


           Console.WriteLine("\nFinal GPU Value:");
           float f = sumValue.Sum();
           Console.WriteLine(f);

           int halfFloat = (int)totalFloats / 2;
           //Console.WriteLine("OOps: " + allValues.Sum());// allValues.ToList().GetRange(halfFloat, halfFloat).Sum());
           Console.WriteLine("True Sum: " + sum);

           bool gpuEqualsCPU = Math.Abs(sum  - f) < 0.001;
           Console.WriteLine("They are equal? " +  (gpuEqualsCPU ? "YES" : "NO! :("));

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
            uint n = 32;
            uint dimensionXSize = 2;

            int bufferIx = 0;
            //set up our constant buffer with our inputs -- yazoooooooo
            Buffer inputBuffer = WriteUIntsToBuffer(device,bufferIx++,  constBufferSizeInBytes, n, dimensionXSize);
       
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
using System;
using System.Windows.Forms; //for MessageBox
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;
using SlimDX.D3DCompiler;
using System.IO;

namespace SlimDX.Helper
{
    /// <summary>
    /// Defines several static helper methods that aid in creating shaders, textures, depth stencils, and render targets.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Loads an HLSL shader file into an <see cref="Effect"/> object.  The HLSL code is compiled using Shader Model 5.0 and errors/warnings are shown
        /// in a <see cref="MessageBox"/> window.
        /// </summary>
        /// <param name="device">The current <see cref="Device"/> being used.</param>
        /// <param name="filename">The filename to load.</param>
        /// <param name="technique">The technique name in the shader file to serve as the entry point.</param>
        /// <returns>An <see cref="Effect"/> object representing the shader file.</returns>
        public static Effect LoadShader(Device device, string filename, string technique)
        {
            try
            {
                string errors;
                ShaderBytecode shaderByteCode = ShaderBytecode.CompileFromFile(filename, technique, "fx_5_0", ShaderFlags.None, EffectFlags.None, null, null, out errors);
                if (!string.IsNullOrEmpty(errors))
                {
                    MessageBox.Show(errors, "Shader compilation errors");
                }
                return new Effect(device, shaderByteCode);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    MessageBox.Show(e.Message + " - Inner: " + e.InnerException.Message, e.Source + " Error!");
                else
                    MessageBox.Show(e.Message + "\n\nTrace:" + e.StackTrace + "\n\nData: " + e.Data + "\n\nType: " + e.GetType(), e.Source + " Error!");

                return null;
            }
        }

        /// <summary>
        /// Loads an HLSL compute shader file into a <see cref="ComputeShader"/> object.  The HLSL code is compiled using Compute Shader 5.0 and errors/warnings are shown
        /// in a <see cref="MessageBox"/> window.
        /// </summary>
        /// <param name="device">The current <see cref="Device"/> being used.</param>
        /// <param name="filename">The filename to load.</param>
        /// <param name="entryPoint">The entry point for the compute shader.</param>
        /// <returns>An <see cref="ComputeShader"/> object representing the compute shader.</returns>
        public static ComputeShader LoadComputeShader(Device device, string filename, string entryPoint, bool forceReload)
        {
            try
            {

                //read from file
                string fileString = File.ReadAllText(filename);


                string byteString = filename.Substring(0, filename.LastIndexOf(".")) + ".byte";
                ShaderBytecode shaderByteCode;

                if (File.Exists(byteString) && !forceReload)
                 {
                      var shaderBytes = File.ReadAllBytes(byteString);
                      DataStream ds = new DataStream(shaderBytes.Length, true, true);
                      ds.Write(shaderBytes, 0, shaderBytes.Length);
                      shaderByteCode = new ShaderBytecode(ds);
                 }
                 else
                 {
                     string errors = null;
                     shaderByteCode = ShaderBytecode.Compile(fileString, entryPoint, "cs_5_0", ShaderFlags.None, EffectFlags.None);

                     DataStream ds = shaderByteCode.Data;
                     
                     byte[] allBytes = new byte[ds.Length];
                     ds.Read(allBytes, 0, (int)ds.Length);

                     //write it up
                     File.WriteAllBytes(byteString, allBytes);               
                 }

                //ShaderBytecode shaderByteCode = ShaderBytecode.Compile(fileString, entryPoint, "cs_5_0", ShaderFlags.None, EffectFlags.None, null, null, out errors);
                //ShaderBytecode shaderByteCode = ShaderBytecode.CompileFromFile(filename, entryPoint, "cs_5_0", ShaderFlags.None, EffectFlags.None, null, null, out errors);
                //if (!string.IsNullOrEmpty(errors))
                //{
                //    MessageBox.Show(errors, "Shader compilation errors");
                //}
                return new ComputeShader(device, shaderByteCode);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    MessageBox.Show(e.Message + "\nInner: " + e.InnerException.Message, e.Source + " Error!");
                else
                    MessageBox.Show(e.Message + "\n\nTrace:" + e.StackTrace + "\n\nData: " + e.Data + "\n\nType: " + e.GetType(), e.Source + " Error!");

                return null;
            }
        }

        /// <summary>
        /// Creates a <see cref="Texture2D"/> with the specified width, height, and <see cref="Format"/>.
        /// It uses default values of BindFlags.ShaderResource, CpuAccessFlags.None, ResourceOptionFlags.None, and ResourceUsage.Default
        /// </summary>
        /// <param name="device">The current <see cref="Device"/> being used.</param>
        /// <param name="width">The width of the <see cref="Texture2D"/> being created.</param>
        /// <param name="height">The height of the <see cref="Texture2D"/> being created.</param>
        /// <param name="format">
        /// The <see cref="Format"/> of the <see cref="Texture2D"/> being created.  The device is queried for support of the given format. 
        /// If the current hardware doesn't support the format, then a <see cref="NotSupportedException"/> is thrown.
        /// </param>
        /// <returns>The newly created <see cref="Texture2D"/> object.</returns>
        public static Texture2D CreateTexture2D(Device device, int width, int height, Format format)
        {
            FormatSupport support = device.CheckFormatSupport(format);
            if ((support & FormatSupport.Texture2D) != FormatSupport.Texture2D)
            {
                throw new NotSupportedException("The given format (" + format.ToString() + ") is not supported as a Texture2D on this GPU.");
            }

            Texture2DDescription texDesc = new Texture2DDescription();
            texDesc.ArraySize = 1;
            texDesc.BindFlags = BindFlags.ShaderResource;
            texDesc.CpuAccessFlags = CpuAccessFlags.None;
            texDesc.Format = format;
            texDesc.Height = height;
            texDesc.MipLevels = 1;
            texDesc.OptionFlags = ResourceOptionFlags.None;
            texDesc.SampleDescription = new SampleDescription(1, 0);
            texDesc.Usage = ResourceUsage.Default;
            texDesc.Width = width;

            return new Texture2D(device, texDesc);
        }

        /// <summary>
        /// Creates a <see cref="Texture2D"/> with the specified width, height, <see cref="Format"/>, and <see cref="BindFlags"/>.
        /// It uses default values of CpuAccessFlags.None, ResourceOptionFlags.None, and ResourceUsage.Default
        /// </summary>
        /// <param name="device">The current <see cref="Device"/> being used.</param>
        /// <param name="width">The width of the <see cref="Texture2D"/> being created.</param>
        /// <param name="height">The height of the <see cref="Texture2D"/> being created.</param>
        /// <param name="format">
        /// The <see cref="Format"/> of the <see cref="Texture2D"/> being created.  The device is queried for support of the given format. 
        /// If the current hardware doesn't support the format, then a <see cref="NotSupportedException"/> is thrown.
        /// </param>
        /// <param name="bindFlags">The <see cref="BindFlags"/> of the <see cref="Texture2D"/> being created.</param>
        /// <returns>The newly created <see cref="Texture2D"/> object.</returns>
        public static Texture2D CreateTexture2D(Device device, int width, int height, Format format, BindFlags bindFlags)
        {
            FormatSupport support = device.CheckFormatSupport(format);
            if ((support & FormatSupport.Texture2D) != FormatSupport.Texture2D)
            {
                throw new NotSupportedException("The given format (" + format.ToString() + ") is not supported as a Texture2D on this GPU.");
            }

            Texture2DDescription texDesc = new Texture2DDescription();
            texDesc.ArraySize = 1;
            texDesc.BindFlags = bindFlags;
            texDesc.CpuAccessFlags = CpuAccessFlags.None;
            texDesc.Format = format;
            texDesc.Height = height;
            texDesc.MipLevels = 1;
            texDesc.OptionFlags = ResourceOptionFlags.None;
            texDesc.SampleDescription = new SampleDescription(1, 0);
            texDesc.Usage = ResourceUsage.Default;
            texDesc.Width = width;

            return new Texture2D(device, texDesc);
        }

        /// <summary>
        /// Creates a <see cref="DepthStencilView"/> with the specified width, height, and <see cref="Format"/>.
        /// </summary>
        /// <param name="device">The current <see cref="Device"/> being used.</param>
        /// <param name="width">The width of the <see cref="DepthStencilView"/> being created.</param>
        /// <param name="height">The height of the <see cref="DepthStencilView"/> being created.</param>
        /// <param name="format">
        /// The <see cref="Format"/> of the <see cref="DepthStencilView"/> being created.  The device is queried for support of the given format. 
        /// If the current hardware doesn't support the format, then a <see cref="NotSupportedException"/> is thrown.
        /// </param>
        /// <returns>The newly created <see cref="DepthStencilView"/> object.</returns>
        public static DepthStencilView CreateDepthStencilView(Device device, int width, int height, Format format)
        {
            FormatSupport support = device.CheckFormatSupport(format);
            if ((support & FormatSupport.DepthStencil) != FormatSupport.DepthStencil)
            {
                throw new NotSupportedException("The given format (" + format.ToString() + ") is not supported as a DepthStencil on this GPU.");
            }

            Texture2D depthStencilTexture = CreateTexture2D(device, width, height, format, BindFlags.DepthStencil);

            // Is this necessary?  It seems to work fine without it.
            //DepthStencilViewDescription dsViewDesc = new DepthStencilViewDescription();
            //dsViewDesc.ArraySize = 1;
            //dsViewDesc.Dimension = DepthStencilViewDimension.Texture2D;
            //dsViewDesc.FirstArraySlice = 0;
            //dsViewDesc.Flags = DepthStencilViewFlags.None;
            //dsViewDesc.Format = format;
            //dsViewDesc.MipSlice = 0;

            return new DepthStencilView(device, depthStencilTexture);
        }

        /// <summary>
        /// Creates a <see cref="DepthStencilView"/> with the specified width, height, and <see cref="Format"/>.
        /// </summary>
        /// <param name="device">The current <see cref="Device"/> being used.</param>
        /// <param name="width">The width of the <see cref="DepthStencilView"/> being created.</param>
        /// <param name="height">The height of the <see cref="DepthStencilView"/> being created.</param>
        /// <param name="format">
        /// The <see cref="Format"/> of the <see cref="DepthStencilView"/> being created.  The device is queried for support of the given format. 
        /// If the current hardware doesn't support the format, then a <see cref="NotSupportedException"/> is thrown.
        /// </param>
        /// <param name="depthStencilTexture">The newly created <see cref="Texture2D"/> that is the backing resource for the <see cref="DepthStencilView"/>.</param>
        /// <returns>The newly created <see cref="DepthStencilView"/> object.</returns>
        public static DepthStencilView CreateDepthStencilView(Device device, int width, int height, Format format, out Texture2D depthStencilTexture)
        {
            FormatSupport support = device.CheckFormatSupport(format);
            if ((support & FormatSupport.DepthStencil) != FormatSupport.DepthStencil)
            {
                throw new NotSupportedException("The given format (" + format.ToString() + ") is not supported as a DepthStencil on this GPU.");
            }

            depthStencilTexture = CreateTexture2D(device, width, height, format, BindFlags.DepthStencil);

            // Is this necessary?  It seems to work fine without it.
            //DepthStencilViewDescription dsViewDesc = new DepthStencilViewDescription();
            //dsViewDesc.ArraySize = 1;
            //dsViewDesc.Dimension = DepthStencilViewDimension.Texture2D;
            //dsViewDesc.FirstArraySlice = 0;
            //dsViewDesc.Flags = DepthStencilViewFlags.None;
            //dsViewDesc.Format = format;
            //dsViewDesc.MipSlice = 0;

            return new DepthStencilView(device, depthStencilTexture);
        }

        /// <summary>
        /// Creates a default <see cref="DepthStencilState"/> object.
        /// </summary>
        /// <param name="device">The current <see cref="Device"/> being used.</param>
        /// <returns>The newly created <see cref="DepthStencilState"/> with the default options.</returns>
        public static DepthStencilState CreateDepthStencilState(Device device)
        {
            DepthStencilOperationDescription dsOperation = new DepthStencilOperationDescription();
            dsOperation.Comparison = Comparison.Less;
            dsOperation.DepthFailOperation = StencilOperation.Keep;
            dsOperation.FailOperation = StencilOperation.Keep;
            dsOperation.PassOperation = StencilOperation.Replace;

            DepthStencilStateDescription dsDesc = new DepthStencilStateDescription();
            dsDesc.BackFace = dsOperation;
            dsDesc.DepthComparison = Comparison.Less;
            dsDesc.DepthWriteMask = DepthWriteMask.All;
            dsDesc.FrontFace = dsOperation;
            dsDesc.IsDepthEnabled = true;
            dsDesc.IsStencilEnabled = false;

            return DepthStencilState.FromDescription(device, dsDesc);
        }

        /// <summary>
        /// Creates a <see cref="RenderTargetView"/> with the specified width, height, and <see cref="Format"/>.
        /// </summary>
        /// <param name="device">The current <see cref="Device"/> being used.</param>
        /// <param name="width">The width of the <see cref="RenderTargetView"/> being created.</param>
        /// <param name="height">The height of the <see cref="RenderTargetView"/> being created.</param>
        /// <param name="format">
        /// The <see cref="Format"/> of the <see cref="RenderTargetView"/> being created.  The device is queried for support of the given format. 
        /// If the current hardware doesn't support the format, then a <see cref="NotSupportedException"/> is thrown.
        /// </param>
        /// <returns>The newly created <see cref="RenderTargetView"/> object.</returns>
        public static RenderTargetView CreateRenderTarget(Device device, int width, int height, Format format)
        {
            FormatSupport support = device.CheckFormatSupport(format);
            if ((support & FormatSupport.RenderTarget) != FormatSupport.RenderTarget)
            {
                throw new NotSupportedException("The given format (" + format.ToString() + ") is not supported as a RenderTarget on this GPU.");
            }

            Texture2D targetTexture = CreateTexture2D(device, width, height, format, BindFlags.RenderTarget);

            // Is this necessary?  It seems to work fine without it.
            //RenderTargetViewDescription rtViewDesc = new RenderTargetViewDescription();
            //rtViewDesc.ArraySize = 1;
            //rtViewDesc.DepthSliceCount = 1;
            //rtViewDesc.Dimension = RenderTargetViewDimension.Texture2D;
            //rtViewDesc.ElementOffset = 0;
            //rtViewDesc.ElementWidth = 0;
            //rtViewDesc.FirstArraySlice = 0;
            //rtViewDesc.FirstDepthSlice = 0;
            //rtViewDesc.Format = format;
            //rtViewDesc.MipSlice = 0;

            return new RenderTargetView(device, targetTexture);
        }

        /// <summary>
        /// Creates a <see cref="RenderTargetView"/> with the specified width, height, and <see cref="Format"/>.
        /// </summary>
        /// <param name="device">The current <see cref="Device"/> being used.</param>
        /// <param name="width">The width of the <see cref="RenderTargetView"/> being created.</param>
        /// <param name="height">The height of the <see cref="RenderTargetView"/> being created.</param>
        /// <param name="format">
        /// The <see cref="Format"/> of the <see cref="RenderTargetView"/> being created.  The device is queried for support of the given format. 
        /// If the current hardware doesn't support the format, then a <see cref="NotSupportedException"/> is thrown.
        /// </param>
        /// <param name="targetTexture">The newly created <see cref="Texture2D"/> that is the backing resource for the <see cref="RenderTargetView"/>.</param>
        /// <returns>The newly created <see cref="RenderTargetView"/> object.</returns>
        public static RenderTargetView CreateRenderTarget(Device device, int width, int height, Format format, out Texture2D targetTexture)
        {
            FormatSupport support = device.CheckFormatSupport(format);
            if ((support & FormatSupport.RenderTarget) != FormatSupport.RenderTarget)
            {
                throw new NotSupportedException("The given format (" + format.ToString() + ") is not supported as a RenderTarget on this GPU.");
            }

            targetTexture = CreateTexture2D(device, width, height, format, BindFlags.RenderTarget);

            // Is this necessary?  It seems to work fine without it.
            //RenderTargetViewDescription rtViewDesc = new RenderTargetViewDescription();
            //rtViewDesc.ArraySize = 1;
            //rtViewDesc.DepthSliceCount = 1;
            //rtViewDesc.Dimension = RenderTargetViewDimension.Texture2D;
            //rtViewDesc.ElementOffset = 0;
            //rtViewDesc.ElementWidth = 0;
            //rtViewDesc.FirstArraySlice = 0;
            //rtViewDesc.FirstDepthSlice = 0;
            //rtViewDesc.Format = format;
            //rtViewDesc.MipSlice = 0;

            return new RenderTargetView(device, targetTexture);
        }

        /// <summary>
        /// Creates a <see cref="RenderTargetView"/> with the specified width, height, and <see cref="Format"/>.
        /// The render target is also bound as a shader resource (BindFlags.ShaderResource) and a <see cref="ShaderResourceView"/> is created.
        /// </summary>
        /// <param name="device">The current <see cref="Device"/> being used.</param>
        /// <param name="width">The width of the <see cref="RenderTargetView"/> being created.</param>
        /// <param name="height">The height of the <see cref="RenderTargetView"/> being created.</param>
        /// <param name="format">
        /// The <see cref="Format"/> of the <see cref="RenderTargetView"/> being created.  The device is queried for support of the given format. 
        /// If the current hardware doesn't support the format, then a <see cref="NotSupportedException"/> is thrown.
        /// </param>
        /// <param name="targetTexture">The newly created <see cref="Texture2D"/> that is the backing resource for the <see cref="RenderTargetView"/>.</param>
        /// <param name="shaderView">The newly created <see cref="ShaderResourceView"/> that can be used to bind the render target as input to a shader.</param>
        /// <returns>The newly created <see cref="RenderTargetView"/> object.</returns>
        public static RenderTargetView CreateRenderTarget(Device device, int width, int height, Format format, out Texture2D targetTexture, out ShaderResourceView shaderView)
        {
            FormatSupport support = device.CheckFormatSupport(format);
            if ((support & FormatSupport.RenderTarget) != FormatSupport.RenderTarget)
            {
                throw new NotSupportedException("The given format (" + format.ToString() + ") is not supported as a RenderTarget on this GPU.");
            }

            targetTexture = CreateTexture2D(device, width, height, format, BindFlags.RenderTarget | BindFlags.ShaderResource);

            shaderView = new ShaderResourceView(device, targetTexture);

            // Is this necessary?  It seems to work fine without it.
            //RenderTargetViewDescription rtViewDesc = new RenderTargetViewDescription();
            //rtViewDesc.ArraySize = 1;
            //rtViewDesc.DepthSliceCount = 1;
            //rtViewDesc.Dimension = RenderTargetViewDimension.Texture2D;
            //rtViewDesc.ElementOffset = 0;
            //rtViewDesc.ElementWidth = 0;
            //rtViewDesc.FirstArraySlice = 0;
            //rtViewDesc.FirstDepthSlice = 0;
            //rtViewDesc.Format = format;
            //rtViewDesc.MipSlice = 0;

            return new RenderTargetView(device, targetTexture);
        }

        /// <summary>
        /// Creates an <see cref="UnorderedAccessView"/> with the specified width, height, and <see cref="Format"/>.
        /// </summary>
        /// <param name="device">The current <see cref="Device"/> being used.</param>
        /// <param name="width">The width of the <see cref="UnorderedAccessView"/> being created.</param>
        /// <param name="height">The height of the <see cref="UnorderedAccessView"/> being created.</param>
        /// <param name="format">
        /// The <see cref="Format"/> of the <see cref="UnorderedAccessView"/> being created.  The device is queried for support of the given format. 
        /// If the current hardware doesn't support the format, then a <see cref="NotSupportedException"/> is thrown.
        /// </param>
        /// <param name="unorderedAccessViewTexture">The newly created <see cref="Texture2D"/> that is the backing resource for the <see cref="UnorderedAccessView"/>.</param>
        /// <returns>The newly created <see cref="UnorderedAccessView"/> object.</returns>
        public static UnorderedAccessView CreateUnorderedAccessView(Device device, int width, int height, Format format, out Texture2D unorderedAccessViewTexture)
        {
            FormatSupport support = device.CheckFormatSupport(format);
            if ((support & FormatSupport.UnorderedAccessView) != FormatSupport.UnorderedAccessView)
            {
                throw new NotSupportedException("The given format (" + format.ToString() + ") is not supported as an UnorderedAccessView on this GPU.");
            }

            unorderedAccessViewTexture = CreateTexture2D(device, width, height, format, BindFlags.UnorderedAccess);

            // Is this necessary?
            //UnorderedAccessViewDescription uavViewDesc = new UnorderedAccessViewDescription();
            //uavViewDesc.ArraySize = 1;
            //uavViewDesc.Dimension = DepthStencilViewDimension.Texture2D;
            //uavViewDesc.FirstArraySlice = 0;
            //uavViewDesc.Flags = DepthStencilViewFlags.None;
            //uavViewDesc.Format = format;
            //uavViewDesc.MipSlice = 0;

            return new UnorderedAccessView(device, unorderedAccessViewTexture);
        }
    }
}

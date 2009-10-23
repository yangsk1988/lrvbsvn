using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Collections;
using VirtualBicycle.MathLib;
using D3D = SlimDX.Direct3D9;

namespace VirtualBicycle.Graphics.D3D9
{
    internal sealed class D3D9RenderSystem : RenderSystem
    {
        const int MaxLights = 8;

        Direct3D d3d;
        Device device;

        D3D9RenderStateManager renderStates;

        SlimDX.Matrix matBuffer;

        Light[] lightBuffer;
        D3D.Light[] d3dLgtBuffer;

        SamplerState[] bufferedStates;

        public D3D9RenderSystem(Direct3D d3d, Device device)
            : base(D3D9GraphicsAPIFactory.APIName)
        {
            this.d3d = d3d;
            this.device = device;

            lightBuffer = new Light[MaxLights];
            d3dLgtBuffer = new D3D.Light[MaxLights];

            renderStates = new D3D9RenderStateManager(this);
            base.RenderStates = renderStates;

            bufferedStates = new SamplerState[8];
        }



        public override void Init()
        {
            Capabilities caps = new Capabilities();

            D3D.CompareCaps caps1 = device.Capabilities.AlphaCompareCaps;

            if ((caps1 & D3D.CompareCaps.Always) == D3D.CompareCaps.Always)
            {
                caps.AlphaCompareCaps |= CompareCaps.Always;
            }
            if ((caps1 & D3D.CompareCaps.Equal) == D3D.CompareCaps.Equal)
            {
                caps.AlphaCompareCaps |= CompareCaps.Equal;
            }
            if ((caps1 & D3D.CompareCaps.Greater) == D3D.CompareCaps.Greater)
            {
                caps.AlphaCompareCaps |= CompareCaps.Greater;
            }
            if ((caps1 & D3D.CompareCaps.GreaterEqual) == D3D.CompareCaps.GreaterEqual)
            {
                caps.AlphaCompareCaps |= CompareCaps.GreaterEqual;
            }
            if ((caps1 & D3D.CompareCaps.Less) == D3D.CompareCaps.Less)
            {
                caps.AlphaCompareCaps |= CompareCaps.Less;
            }
            if ((caps1 & D3D.CompareCaps.LessEqual) == D3D.CompareCaps.LessEqual)
            {
                caps.AlphaCompareCaps |= CompareCaps.LessEqual;
            }
            if ((caps1 & D3D.CompareCaps.Never) == D3D.CompareCaps.Never)
            {
                caps.AlphaCompareCaps |= CompareCaps.Never;
            }
            if ((caps1 & D3D.CompareCaps.NotEqual) == D3D.CompareCaps.NotEqual)
            {
                caps.AlphaCompareCaps |= CompareCaps.NotEqual;
            }

            D3D.FilterCaps caps2 = device.Capabilities.CubeTextureFilterCaps;

            if ((caps2 & D3D.FilterCaps.MagAnisotropic) == D3D.FilterCaps.MagAnisotropic)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MagAnisotropic;
            }
            if ((caps2 & D3D.FilterCaps.MagGaussianQuad) == D3D.FilterCaps.MagGaussianQuad)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MagGaussianQuad;
            }
            if ((caps2 & D3D.FilterCaps.MagLinear) == D3D.FilterCaps.MagLinear)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MagLinear;
            }
            if ((caps2 & D3D.FilterCaps.MagPoint) == D3D.FilterCaps.MagPoint)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MagPoint;
            }
            if ((caps2 & D3D.FilterCaps.MagPyramidalQuad) == D3D.FilterCaps.MagPyramidalQuad)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MagPyramidalQuad;
            }
            if ((caps2 & D3D.FilterCaps.MinAnisotropic) == D3D.FilterCaps.MinAnisotropic)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MinAnisotropic;
            }
            if ((caps2 & D3D.FilterCaps.MinGaussianQuad) == D3D.FilterCaps.MinGaussianQuad)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MinGaussianQuad;
            }
            if ((caps2 & D3D.FilterCaps.MinLinear) == D3D.FilterCaps.MinLinear)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MinLinear;
            }
            if ((caps2 & D3D.FilterCaps.MinPoint) == D3D.FilterCaps.MinPoint)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MinPoint;
            }
            if ((caps2 & D3D.FilterCaps.MinPyramidalQuad) == D3D.FilterCaps.MinPyramidalQuad)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MinPyramidalQuad;
            }
            if ((caps2 & D3D.FilterCaps.MipLinear) == D3D.FilterCaps.MipLinear)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MipLinear;
            }
            if ((caps2 & D3D.FilterCaps.MipPoint) == D3D.FilterCaps.MipPoint)
            {
                caps.CubeTextureFilterCaps |= FilterCaps.MipPoint;
            }


            D3D.DeclarationTypeCaps caps3 = device.Capabilities.DeclarationTypes;
            if ((caps3 & D3D.DeclarationTypeCaps.Dec3N) == D3D.DeclarationTypeCaps.Dec3N)
            {
                caps.DeclarationTypes |= DeclarationTypeCaps.Dec3N;
            }
            if ((caps3 & D3D.DeclarationTypeCaps.HalfFour) == D3D.DeclarationTypeCaps.HalfFour)
            {
                caps.DeclarationTypes |= DeclarationTypeCaps.HalfFour;
            }
            if ((caps3 & D3D.DeclarationTypeCaps.HalfTwo) == D3D.DeclarationTypeCaps.HalfTwo)
            {
                caps.DeclarationTypes |= DeclarationTypeCaps.HalfTwo;
            }
            if ((caps3 & D3D.DeclarationTypeCaps.Short2N) == D3D.DeclarationTypeCaps.Short2N)
            {
                caps.DeclarationTypes |= DeclarationTypeCaps.Short2N;
            }
            if ((caps3 & D3D.DeclarationTypeCaps.Short4N) == D3D.DeclarationTypeCaps.Short4N)
            {
                caps.DeclarationTypes |= DeclarationTypeCaps.Short4N;
            }
            if ((caps3 & D3D.DeclarationTypeCaps.UByte4) == D3D.DeclarationTypeCaps.UByte4)
            {
                caps.DeclarationTypes |= DeclarationTypeCaps.UByte4;
            }
            if ((caps3 & D3D.DeclarationTypeCaps.UByte4N) == D3D.DeclarationTypeCaps.UByte4N)
            {
                caps.DeclarationTypes |= DeclarationTypeCaps.UByte4N;
            }
            if ((caps3 & D3D.DeclarationTypeCaps.UDec3) == D3D.DeclarationTypeCaps.UDec3)
            {
                caps.DeclarationTypes |= DeclarationTypeCaps.UDec3;
            }
            if ((caps3 & D3D.DeclarationTypeCaps.UShort2N) == D3D.DeclarationTypeCaps.UShort2N)
            {
                caps.DeclarationTypes |= DeclarationTypeCaps.UShort2N;
            }
            if ((caps3 & D3D.DeclarationTypeCaps.UShort4N) == D3D.DeclarationTypeCaps.UShort4N)
            {
                caps.DeclarationTypes |= DeclarationTypeCaps.UShort4N;
            }

            caps1 = device.Capabilities.DepthCompareCaps;
            if ((caps1 & D3D.CompareCaps.Always) == D3D.CompareCaps.Always)
            {
                caps.DepthCompareCaps |= CompareCaps.Always;
            }
            if ((caps1 & D3D.CompareCaps.Equal) == D3D.CompareCaps.Equal)
            {
                caps.DepthCompareCaps |= CompareCaps.Equal;
            }
            if ((caps1 & D3D.CompareCaps.Greater) == D3D.CompareCaps.Greater)
            {
                caps.DepthCompareCaps |= CompareCaps.Greater;
            }
            if ((caps1 & D3D.CompareCaps.GreaterEqual) == D3D.CompareCaps.GreaterEqual)
            {
                caps.DepthCompareCaps |= CompareCaps.GreaterEqual;
            }
            if ((caps1 & D3D.CompareCaps.Less) == D3D.CompareCaps.Less)
            {
                caps.DepthCompareCaps |= CompareCaps.Less;
            }
            if ((caps1 & D3D.CompareCaps.LessEqual) == D3D.CompareCaps.LessEqual)
            {
                caps.DepthCompareCaps |= CompareCaps.LessEqual;
            }
            if ((caps1 & D3D.CompareCaps.Never) == D3D.CompareCaps.Never)
            {
                caps.DepthCompareCaps |= CompareCaps.Never;
            }
            if ((caps1 & D3D.CompareCaps.NotEqual) == D3D.CompareCaps.NotEqual)
            {
                caps.DepthCompareCaps |= CompareCaps.NotEqual;
            }

            D3D.BlendCaps caps4 = device.Capabilities.DestinationBlendCaps;
            if ((caps4 & D3D.BlendCaps.BlendFactor) == D3D.BlendCaps.BlendFactor)
            {
                caps.DestinationBlendCaps |= BlendCaps.BlendFactor;
            }
            if ((caps4 & D3D.BlendCaps.BothInverseSourceAlpha) == D3D.BlendCaps.BothInverseSourceAlpha)
            {
                caps.DestinationBlendCaps |= BlendCaps.BothInverseSourceAlpha;
            }
            if ((caps4 & D3D.BlendCaps.DestinationAlpha) == D3D.BlendCaps.DestinationAlpha)
            {
                caps.DestinationBlendCaps |= BlendCaps.DestinationAlpha;
            }
            if ((caps4 & D3D.BlendCaps.DestinationColor) == D3D.BlendCaps.DestinationColor)
            {
                caps.DestinationBlendCaps |= BlendCaps.DestinationColor;
            }
            if ((caps4 & D3D.BlendCaps.InverseDestinationAlpha) == D3D.BlendCaps.InverseDestinationAlpha)
            {
                caps.DestinationBlendCaps |= BlendCaps.InverseDestinationAlpha;
            }
            if ((caps4 & D3D.BlendCaps.InverseDestinationColor) == D3D.BlendCaps.InverseDestinationColor)
            {
                caps.DestinationBlendCaps |= BlendCaps.InverseDestinationColor;
            }
            if ((caps4 & D3D.BlendCaps.InverseSourceAlpha) == D3D.BlendCaps.InverseSourceAlpha)
            {
                caps.DestinationBlendCaps |= BlendCaps.InverseSourceAlpha;
            }
            if ((caps4 & D3D.BlendCaps.InverseSourceColor) == D3D.BlendCaps.InverseSourceColor)
            {
                caps.DestinationBlendCaps |= BlendCaps.InverseSourceColor;
            }
            if ((caps4 & D3D.BlendCaps.One) == D3D.BlendCaps.One)
            {
                caps.DestinationBlendCaps |= BlendCaps.One;
            }
            if ((caps4 & D3D.BlendCaps.SourceAlpha) == D3D.BlendCaps.SourceAlpha)
            {
                caps.DestinationBlendCaps |= BlendCaps.SourceAlpha;
            }
            if ((caps4 & D3D.BlendCaps.SourceAlphaSaturated) == D3D.BlendCaps.SourceAlphaSaturated)
            {
                caps.DestinationBlendCaps |= BlendCaps.SourceAlphaSaturated;
            }
            if ((caps4 & D3D.BlendCaps.SourceColor) == D3D.BlendCaps.SourceColor)
            {
                caps.DestinationBlendCaps |= BlendCaps.SourceColor;
            }
            if ((caps4 & D3D.BlendCaps.Zero) == D3D.BlendCaps.Zero)
            {
                caps.DestinationBlendCaps |= BlendCaps.Zero;
            }

            //D3D.DeviceCaps caps5 = device.Capabilities.DeviceCaps;
            //if ((caps5 & D3D.DeviceCaps.CanBlitSysToNonLocal) == D3D.DeviceCaps.CanBlitSysToNonLocal)
            //{
            //    caps.DeviceCaps |= DeviceCaps.CanBlitSysToNonLocal;
            //}
            //if ((caps5 & D3D.DeviceCaps.CanRenderAfterFlip) == D3D.DeviceCaps.CanRenderAfterFlip)
            //{
            //    caps.DeviceCaps |= DeviceCaps.CanRenderAfterFlip;
            //}
            //if ((caps5 & D3D.DeviceCaps.DrawPrimitives2) == D3D.DeviceCaps.DrawPrimitives2)
            //{
            //    caps.DeviceCaps |= DeviceCaps.DrawPrimitives2;
            //}
            //if ((caps5 & D3D.DeviceCaps.DrawPrimitives2Extended) == D3D.DeviceCaps.DrawPrimitives2Extended)
            //{
            //    caps.DeviceCaps |= DeviceCaps.DrawPrimitives2Extended;
            //}
            //if ((caps5 & D3D.DeviceCaps.DrawPrimTLVertex) == D3D.DeviceCaps.DrawPrimTLVertex)
            //{
            //    caps.DeviceCaps |= DeviceCaps.DrawPrimTLVertex;
            //}
            //if ((caps5 & D3D.DeviceCaps.ExecuteSystemMemory) == D3D.DeviceCaps.ExecuteSystemMemory)
            //{
            //    caps.DeviceCaps |= DeviceCaps.ExecuteSystemMemory;
            //}
            //if ((caps5 & D3D.DeviceCaps.ExecuteVideoMemory) == D3D.DeviceCaps.ExecuteVideoMemory)
            //{
            //    caps.DeviceCaps |= DeviceCaps.ExecuteVideoMemory;
            //}
            //if ((caps5 & D3D.DeviceCaps.HWRasterization) == D3D.DeviceCaps.HWRasterization)
            //{
            //    caps.DeviceCaps |= DeviceCaps.HWRasterization;
            //}
            //if ((caps5 & D3D.DeviceCaps.HWTransformAndLight) == D3D.DeviceCaps.HWTransformAndLight)
            //{
            //    caps.DeviceCaps |= DeviceCaps.HWTransformAndLight;
            //}
            //if ((caps5 & D3D.DeviceCaps.NPatches) == D3D.DeviceCaps.NPatches)
            //{
            //    caps.DeviceCaps |= DeviceCaps.NPatches;
            //}
            //if ((caps5 & D3D.DeviceCaps.) == D3D.DeviceCaps.NPatches)
            //{
            //    caps.DeviceCaps |= DeviceCaps.NPatches;
            //}

            D3D.LineCaps lineCaps = device.Capabilities.LineCaps;
            if ((lineCaps & D3D.LineCaps.AlphaCompare) == D3D.LineCaps.AlphaCompare)
            {
                caps.LineCaps |= LineCaps.AlphaCompare;
            }
            if ((lineCaps & D3D.LineCaps.Antialias) == D3D.LineCaps.Antialias)
            {
                caps.LineCaps |= LineCaps.Antialias;
            }
            if ((lineCaps & D3D.LineCaps.Blend) == D3D.LineCaps.Blend)
            {
                caps.LineCaps |= LineCaps.Blend;
            }
            if ((lineCaps & D3D.LineCaps.DepthTest) == D3D.LineCaps.DepthTest)
            {
                caps.LineCaps |= LineCaps.DepthTest;
            }
            if ((lineCaps & D3D.LineCaps.Fog) == D3D.LineCaps.Fog)
            {
                caps.LineCaps |= LineCaps.Fog;
            }
            if ((lineCaps & D3D.LineCaps.Texture) == D3D.LineCaps.Texture)
            {
                caps.LineCaps |= LineCaps.Texture;
            }


            

            RenderSystemCaps = caps;

            device.SetRenderState(RenderState.LocalViewer, true);
            //device = new Device(d3d, adapter, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing);
        }

        internal Device D3DDevice
        {
            get { return device; }
        }

        public override void BeginFrame()
        {
            base.BeginFrame();
            device.BeginScene();
        }

        public override void EndFrame()
        {
            base.EndFrame();
            device.EndScene();
        }

        public override void Clear(ClearFlags flags, int color, float depth, int stencil)
        {
            device.Clear(D3D9Utils.ConvertEnum(flags), color, depth, stencil);
        }

        public unsafe override Matrix GetTransform(TransformState state)
        {
            matBuffer = device.GetTransform((int)state);

            fixed (SlimDX.Matrix* ptr = &matBuffer)
            {
                return *(Matrix*)ptr;
            }
        }

        public override void SetFog(FogMode mode, ColorValue color, float start, float end, float density)
        {
            if (mode == FogMode.None)
            {
                renderStates.FogEnable = false;
            }
            else
            {
                renderStates.FogEnable = true;

                renderStates.FogVertexMode = mode;
                renderStates.FogTableMode = mode;
                renderStates.FogStart = start;
                renderStates.FogEnd = end;
                renderStates.FogDensity = density;
                renderStates.FogColor = color;
            }
        }
        public unsafe override void SetTransform(TransformState state, Matrix matrix)
        {
            device.SetTransform((int)state, *(SlimDX.Matrix*)&matrix);
        }

        public override void SetPointParameters(float size, bool attenuationEnabled, float constant, float linear, float quadratic, float minSize, float maxSize)
        {
            if (attenuationEnabled)
            {
                device.SetRenderState(RenderState.PointScaleEnable, true);
                device.SetRenderState(RenderState.PointScaleA, constant);
                device.SetRenderState(RenderState.PointScaleB, linear);
                device.SetRenderState(RenderState.PointScaleC, quadratic);
            }
            else
            {
                device.SetRenderState(RenderState.PointScaleEnable, false);
            }
            device.SetRenderState(RenderState.PointSize, size);
            device.SetRenderState(RenderState.PointSizeMin, minSize);
            //RenderStates.PointSize = size;
            //RenderStates.PointSizeMin = minSize;

            if (maxSize == 0f)
            {
                maxSize = RenderSystemCaps.MaxPointSize;
            }
            device.SetRenderState(RenderState.PointSizeMax, minSize);

        }
        public override void Render(RenderOperation[] op)
        {
            base.Render(op);

            
        }

        public override Light GetLight(int index)
        {
            return lightBuffer[index];
        }
        public unsafe override void SetLight(int index, Light light)
        {
            if (light == null && lightBuffer[index] != null)
            {
                device.EnableLight(index, false);
            }
            else if (light != null && lightBuffer[index] == null)
            {
                device.EnableLight(index, true);
            }

            Color4F clr = light.Ambient;

            SlimDX.Color4 d3dClr = *(SlimDX.Color4*)&clr;

            d3dLgtBuffer[index].Ambient = d3dClr;
            d3dLgtBuffer[index].Diffuse = d3dClr;
            d3dLgtBuffer[index].Specular = d3dClr;
            d3dLgtBuffer[index].Attenuation0 = light.AttenuationConst;
            d3dLgtBuffer[index].Attenuation1 = light.AttenuationLinear;
            d3dLgtBuffer[index].Attenuation2 = light.AttenuationQuad;

            Vector3 tmpVec = light.Direction;

            d3dLgtBuffer[index].Direction = *(SlimDX.Vector3*)&tmpVec;

            d3dLgtBuffer[index].Falloff = light.SpotFalloff;
            d3dLgtBuffer[index].Phi = light.SpotOuter;
            d3dLgtBuffer[index].Theta = light.SpotInner;
            d3dLgtBuffer[index].Type = D3D9Utils.ConvertEnum(light.Type);

        }
        public override void SetRenderTarget(int index, RenderTarget rt)
        {
            D3D9RenderTarget drt = (D3D9RenderTarget)rt;

            device.SetRenderTarget(index, drt.d3dClrBuffer);
            device.DepthStencilSurface = drt.d3dDepBuffer;
        }
        public override RenderTarget GetRenderTarget(int index)
        {
            D3D.Surface backBuffer = device.GetBackBuffer(index, 0);
            D3D.Surface depthBuffer = device.DepthStencilSurface;

            D3D.SurfaceDescription desc = backBuffer.Description;
            D3D.SurfaceDescription desc2 = depthBuffer.Description;


            return new D3D9RenderTarget(this, backBuffer, depthBuffer, desc.Width, desc.Height, 
                D3D9Utils.ConvertEnum(desc.Format), D3D9Utils.ConvertEnum2(desc2.Format));
        }

        public override void SetStencilFunction(CompareFunction func, int refValue, int mask,
            StencilOperation stencilFailOp, StencilOperation depthFailOp, StencilOperation passOp, bool twoSidedOperation)
        {
            D3D.StencilOperation so;

            // 2-sided operation
            if (twoSidedOperation)
            {
                if ((RenderSystemCaps.StencilCaps & StencilCaps.TwoSided) != StencilCaps.TwoSided)
                    throw new NotSupportedException("2-sided stencils are not supported");
                device.SetRenderState(RenderState.TwoSidedStencilMode, true);

                // Set alternative versions of ops
                // fail op
                so = D3D9Utils.ConvertEnum(stencilFailOp);
                device.SetRenderState<D3D.StencilOperation>(RenderState.CcwStencilFail, so);
                // depth fail op
                device.SetRenderState<D3D.StencilOperation>(RenderState.CcwStencilZFail, so);

                // pass op
                device.SetRenderState<D3D.StencilOperation>(RenderState.CcwStencilPass, so);
            }
            else
            {
                device.SetRenderState(RenderState.TwoSidedStencilMode, false);
            }

            // func
            device.SetRenderState<CompareFunction>(RenderState.StencilFunc, func);

            // reference value
            device.SetRenderState(RenderState.StencilRef, refValue);

            // mask 
            device.SetRenderState(RenderState.StencilMask, mask);

            // fail op
            so = D3D9Utils.ConvertEnum(stencilFailOp);
            device.SetRenderState<D3D.StencilOperation>(RenderState.CcwStencilFail, so);

            // depth fail op
            device.SetRenderState<D3D.StencilOperation>(RenderState.CcwStencilZFail, so);

            // pass op
            device.SetRenderState<D3D.StencilOperation>(RenderState.CcwStencilPass, so);
        }
        public override void SetTexture(int index, Texture texture)
        {
            D3D9Texture tex = (D3D9Texture)texture;

            device.SetTexture(index, tex.baseTexture);
            
        }

        public override SamplerStateCollection GetSamplerStates()
        {
            return new SamplerStateCollection(bufferedStates);
        }

        //public override void SetSamplerState(int sampler, SamplerState type, float value)
        //{
        //    device.SetSamplerState(sampler, D3D9Utils.ConvertEnum(type), value);
        //}
        //public override void SetSamplerState(int sampler, SamplerState type, int value)
        //{
        //    device.SetSamplerState(sampler, D3D9Utils.ConvertEnum(type), value);
        //}
        //public override void SetSamplerState(int sampler, SamplerState type, TextureAddressMode textureAddress)
        //{
        //    device.SetSamplerState(sampler, D3D9Utils.ConvertEnum(type), D3D9Utils.ConvertEnum(textureAddress));
        //}
        //public override void SetSamplerState(int sampler, SamplerState type, TextureFilter textureFilter)
        //{
        //    device.SetSamplerState(sampler, D3D9Utils.ConvertEnum(type), D3D9Utils.ConvertEnum(textureFilter));
        //}


        //public override void SetTextureStageState(int sampler, TextureStage type, float value)
        //{
        //    device.SetTextureStageState(sampler, D3D9Utils.ConvertEnum(type), value);
        //}

        //public override void SetTextureStageState(int sampler, TextureStage type, int value)
        //{
        //    device.SetTextureStageState(sampler, D3D9Utils.ConvertEnum(type), value);
          
        //}
        //public override void SetTextureStageState(int sampler, TextureStage type, TextureArgument arg)
        //{
        //    device.SetTextureStageState(sampler, D3D9Utils.ConvertEnum(type), (int)value);
        //}
        //public override void SetTextureStageState(int sampler, TextureStage type, TextureOperation arg)
        //{
        //    device.SetTextureStageState(sampler, D3D9Utils.ConvertEnum(type), D3D9Utils.ConvertEnum(value));
        //}
        //public override void SetTextureStageState(int sampler, TextureStage type, TextureTransform arg)
        //{
        //    device.SetTextureStageState(sampler, D3D9Utils.ConvertEnum(type), D3D9Utils.ConvertEnum(value));
        //}


        //public override void SetEffect(R3D.GraphicsEngine.EffectSystem.Effect eff)
        //{
        //    throw new NotImplementedException();
        //}

        public override void BindShader(VertexShader shader)
        {
            if (shader == null)
            {
                device.VertexShader = null;
            }
            else
            {
                D3D9VertexShader dvs = (D3D9VertexShader)shader;

                device.VertexShader = dvs.D3DVS;
            }
        }
        public override void BindShader(PixelShader shader)
        {
            if (shader == null)
            {
                device.PixelShader = null;
            }
            else
            {
                D3D9PixelShader dps = (D3D9PixelShader)shader;

                device.PixelShader = dps.D3DPS;
            }
           
                
        }


        public override void SetMaterialColor(Material mat)
        {
            D3D.Material dmat;
            Color4F clr = mat.Ambient;
            dmat.Ambient = new SlimDX.Color4(clr.Alpha, clr.Red, clr.Green, clr.Blue);
            clr = mat.Diffuse;
            dmat.Diffuse = new SlimDX.Color4(clr.Alpha, clr.Red, clr.Green, clr.Blue);
            clr = mat.Specular;
            dmat.Specular = new SlimDX.Color4(clr.Alpha, clr.Red, clr.Green, clr.Blue);
            clr = mat.Emissive;
            dmat.Emissive = new SlimDX.Color4(clr.Alpha, clr.Red, clr.Green, clr.Blue);
            dmat.Power = mat.Power;

            device.Material = dmat;
        }


        public override string ToString()
        {
            return RenderSystemName;
        }
    }
}

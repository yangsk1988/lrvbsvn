using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Collections;
using VirtualBicycle.MathLib;

namespace VirtualBicycle.Graphics
{
    /// <summary>
    /// Applications use the methods of the RenderSystem to perform DrawPrimitive-based rendering, create resources,
    /// work with system-level variables, adjust gamma ramp levels, work with palettes, and create shaders.
    /// </summary>
    public abstract class RenderSystem
    {
        Capabilities caps;

        int batchCount;
        int primitiveCount;
        int vertexCount;

        /// <summary>
        ///  获取渲染场景所用到的批次数量
        /// </summary>
        public int BatchCount
        {
            get { return batchCount; }
        }
        /// <summary>
        ///  获取渲染的场景的图元数量
        /// </summary>
        public int PrimitiveCount
        {
            get { return primitiveCount; }
        }
        /// <summary>
        ///  获取渲染的场景的顶点数量
        /// </summary>
        public int VertexCount
        {
            get { return vertexCount; }
        }

        protected RenderSystem(string renderSystemName)
        {
            RenderSystemName = renderSystemName;
        }

        /// <summary>
        ///  初始化渲染系统
        /// </summary>
        public abstract void Init();


        /// <summary>
        ///  获取硬件对图形API支持的情况
        /// </summary>
        public Capabilities RenderSystemCaps
        {
            get { return caps; }
            protected set { caps = value; }
        }

        /// <summary>
        ///  获取一个<seealso cref="System.String"/>，表示渲染系统的名称
        /// </summary>
        public string RenderSystemName
        {
            get;
            private set;
        }

        /// <summary>
        ///  获取一个<see cref="ObjectFactory"/>，是用来创建图形API对象的抽象工厂
        /// </summary>
        public ObjectFactory ObjectFactory
        {
            get;
            protected set;
        }

        /// <summary>
        ///  获取一个渲染状态管理器，提供渲染状态相关的接口
        /// </summary>
        public RenderStateManager RenderStates
        {
            get;
            protected set;
        }

        /// <summary>
        ///  开始场景的渲染
        /// </summary>
        public virtual void BeginFrame()
        {
            batchCount = 0;
            primitiveCount = 0;
            vertexCount = 0;
        }

        /// <summary>
        ///  结束场景的渲染
        /// </summary>
        public virtual void EndFrame() { }

        /// <summary>
        ///  清除屏幕
        /// </summary>
        /// <param name="flags">表示清除的目标</param>
        /// <param name="color">清除颜色缓冲后的颜色</param>
        /// <param name="depth">清除深度缓冲后的深度</param>
        /// <param name="stencil">清除蒙板缓冲后的蒙板值</param>
        public abstract void Clear(ClearFlags flags, int color, float depth, int stencil);

        /// <summary>
        ///  设置雾效参数
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="color"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="density"></param>
        [Obsolete()]
        public abstract void SetFog(FogMode mode, ColorValue color, float start, float end, float density);

        /// <summary>
        ///  设置光源信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="light"></param>
        public abstract void SetLight(int index, Light light);

        /// <summary>
        ///  获取光源信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract Light GetLight(int index);

        /// <summary>
        ///  设置RenderTarget
        /// </summary>
        /// <param name="index"></param>
        /// <param name="rt"></param>
        public abstract void SetRenderTarget(int index, RenderTarget rt);

        /// <summary>
        ///  获取RenderTarget
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract RenderTarget GetRenderTarget(int index);

        /// <summary>
        ///  设定当前纹理
        /// </summary>
        /// <param name="index"></param>
        /// <param name="texture"></param>
        public abstract void SetTexture(int index, Texture texture);

        /// <summary>
        ///  设置蒙版测试的相关参数
        /// </summary>
        /// <param name="func"></param>
        /// <param name="refValue"></param>
        /// <param name="mask"></param>
        /// <param name="stencilFailOp"></param>
        /// <param name="depthFailOp"></param>
        /// <param name="passOp"></param>
        /// <param name="twoSidedOperation"></param>
        public virtual void SetStencilFunction(CompareFunction func,
            int refValue, int mask,
            StencilOperation stencilFailOp,
            StencilOperation depthFailOp,
            StencilOperation passOp,
            bool twoSidedOperation)
        {

        }

        /// <summary>
        ///  设置绘制点的相关参数
        /// </summary>
        /// <param name="size">点的大小</param>
        /// <param name="attenuationEnabled">是否开启点精灵</param>
        /// <param name="constant"></param>
        /// <param name="linear"></param>
        /// <param name="quadratic"></param>
        /// <param name="minSize"></param>
        /// <param name="maxSize"></param>
        public virtual void SetPointParameters(float size, bool attenuationEnabled,
          float constant, float linear, float quadratic,
          float minSize, float maxSize)
        {
            if (attenuationEnabled)
            {
                RenderStates.PointSpriteEnable = true;
                RenderStates.PointScaleA = constant;
                RenderStates.PointScaleB = linear;
                RenderStates.PointScaleC = quadratic;
                //RenderStates.
                //dDevice.SetRenderState(RenderState.PointScaleEnable, true);
                //dDevice.SetRenderState(RenderState.PointScaleA, constant);
                //dDevice.SetRenderState(RenderState.PointScaleB, linear);
                //dDevice.SetRenderState(RenderState.PointScaleC, quadratic);
            }
            else
            {
                RenderStates.PointSpriteEnable = false;
                //dDevice.SetRenderState(RenderState.PointScaleEnable, false);
            }
            //dDevice.SetRenderState(RenderState.PointSize, size);
            //dDevice.SetRenderState(RenderState.PointSizeMin, minSize);
            RenderStates.PointSize = size;
            RenderStates.PointSizeMin = minSize;

            if (maxSize == 0f)
            {
                maxSize = caps.MaxPointSize;
            }
            RenderStates.PointSizeMax = maxSize;
            //if (maxSize == 0.0f) maxSize = (float)rCapabilities.MaxPointSize;
            //dDevice.SetRenderState(RenderState.PointSizeMax, maxSize);
        }

        //public abstract void SetTextureStageState(int sampler, TextureStage type, TextureArgument arg);
        //public abstract void SetTextureStageState(int sampler, TextureStage type, TextureOperation arg);
        //public abstract void SetTextureStageState(int sampler, TextureStage type, TextureTransform arg);

        //public abstract void SetTextureStageState(int sampler, TextureStage type, int value);
        //public abstract void SetTextureStageState(int sampler, TextureStage type, float value);

        /// <summary>
        ///  获取所有纹理层的Sampler状态
        /// </summary>
        /// <returns></returns>
        public abstract SamplerStateCollection GetSamplerStates();
        //public abstract void SetSamplerState(int sampler, SamplerState type, TextureAddressMode textureAddress);
        //public abstract void SetSamplerState(int sampler, SamplerState type, TextureFilter textureFilter);
        //public abstract void SetSamplerState(int sampler, SamplerState type, int value);
        //public abstract void SetSamplerState(int sampler, SamplerState type, float value);

        /// <summary>
        ///  设置当前的材质
        /// </summary>
        /// <param name="mat"></param>
        public abstract void SetMaterialColor(Material mat);

        //public abstract void SetEffect(Effect eff);

        /// <summary>
        ///  设置顶点Shader
        /// </summary>
        /// <param name="shader"></param>
        public abstract void BindShader(VertexShader shader);

        /// <summary>
        ///  设置像素Shader
        /// </summary>
        /// <param name="shader"></param>
        public abstract void BindShader(PixelShader shader);

        /// <summary>
        ///  执行RenderOperation，渲染图形
        /// </summary>
        /// <param name="op"></param>
        public virtual void Render(RenderOperation[] op)
        {
            if (op == null)
            {
                return;
            }
            for (int i = 0; i < op.Length; i++)
            {
                batchCount++;
                primitiveCount += op[i].Geomentry.PrimCount;
                vertexCount += op[i].Geomentry.VertexCount;
            }


        }

        /// <summary>
        ///  获取变换矩阵
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public abstract Matrix GetTransform(TransformState state);

        /// <summary>
        ///  设置变换矩阵
        /// </summary>
        /// <param name="state"></param>
        /// <param name="matrix"></param>
        public abstract void SetTransform(TransformState state, Matrix matrix);



        //public unsafe void SetTransform(TransformState state, Matrix matrix)
        //{
        //    if (state >= TransformState.World)
        //    {
        //        int idx = (int)(state - TransformState.World);
        //        transformInfo.world[idx] = matrix;
        //        transformInfo.isWorldDirty[idx] = true;
        //        return;
        //    }
        //    if (state >= TransformState.Texture0)
        //    {
        //        int idx = (int)(state - TransformState.Texture0);

        //        transformInfo.texture[idx] = matrix;
        //        transformInfo.isTexDirty[idx] = true;
        //        return;
        //    }
        //    if (state == TransformState.View)
        //    {
        //        transformInfo.view = matrix;
        //        transformInfo.isViewDirty = true;
        //        return;
        //    }
        //    if (state == TransformState.Projection)
        //    {
        //        transformInfo.projection = matrix;
        //        transformInfo.isProjDirty = true;
        //        return;
        //    }
        //}


    }
}

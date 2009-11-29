using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Media;
using Apoc3D.Vfs;

namespace Apoc3D.Graphics
{
    /// <summary>
    ///  定义 Shadow Mapping 所需要的阴影贴图
    /// </summary>
    public class ShadowMap : UnmanagedResource, IDisposable
    {
        struct TestVertex
        {
            public Vector3 pos;
            public float dummy;
            public Vector2 tex1;

            static readonly VertexElement[] elements;

            public static VertexElement[] Elements 
            {
                get { return elements; }
            }

            static TestVertex ()
            {
                elements = new VertexElement[2];
                elements[0] = new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.PositionTransformed);
                elements[1] = new VertexElement(elements[0].Offset, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            }
        }

        //Texture shadowDepthMap;
        RenderTarget shadowRt;

        //RenderTarget shadowRtSurface;

        public const int ShadowMapLength = 1024;

        VertexBuffer pip;

        Viewport smVp;
        RenderSystem renderSys;
        ObjectFactory factory;

        VertexDeclaration pipDecl;

        public DefaultSMGenEffect DefaultSMGen
        {
            get;
            private set;
        }

        public unsafe ShadowMap(RenderSystem dev)
        {
            renderSys = dev;
            factory = dev.ObjectFactory;
            //FileLocation fl = FileSystem.Instance.Locate(FileSystem.CombinePath(Paths.Effects, "StandardShadow.fx"), FileLocateRules.Default);
            //ContentStreamReader sr = new ContentStreamReader(fl);
            //string code = sr.ReadToEnd();

            //string err;
            //effect = Effect.FromString(device, code, null, null, null, ShaderFlags.None, null, out err);
            //sr.Close();

            //mvpParam = new EffectHandle("mvp");

            //genSMTec = new EffectHandle("StandardShadow");
            //effect.Technique = genSMTec;
            //drawSceneTec = new EffectHandle("RenderScene");


            LoadUnmanagedResources();

            smVp.MinZ = 0;
            smVp.MaxZ = 1;
            smVp.Height = ShadowMapLength;
            smVp.Width = ShadowMapLength;
            smVp.X = 0;
            smVp.Y = 0;

            pipDecl = factory.CreateVertexDeclaration(TestVertex.Elements);
            pip = factory.CreateVertexBuffer(4, pipDecl, BufferUsage.Static);

            TestVertex* ptr = (TestVertex*)pip.Lock(0, 0, LockMode.None);
            ptr[0] = new TestVertex { pos = new Vector3(0, 0, 0), tex1 = new Vector2(0, 0), dummy = 0 };
            ptr[1] = new TestVertex { pos = new Vector3(0, 512, 0), tex1 = new Vector2(0, 1), dummy = 0 };
            ptr[2] = new TestVertex { pos = new Vector3(512, 0, 0), tex1 = new Vector2(1, 0), dummy = 0 };
            ptr[3] = new TestVertex { pos = new Vector3(512, 512, 0), tex1 = new Vector2(1, 1), dummy = 0 };

            pip.Unlock();


            DefaultSMGen = new DefaultSMGenEffect(renderSys);
        }

        RenderTarget stdRenderTarget;
        Viewport stdVp;

        public Matrix LightProjection;
        public Matrix ViewTransform;
        public Matrix ViewProj;

        public void End()
        {
            renderSys.SetRenderTarget(0, stdRenderTarget);
            stdRenderTarget = null;
            
            renderSys.Viewport = stdVp;
        }
        public void Begin(Vector3 lightDir, ICamera cam)
        {
            stdVp = renderSys.Viewport;

            renderSys.Viewport = smVp;

            stdRenderTarget = renderSys.GetRenderTarget(0);

            renderSys.SetRenderTarget(0, shadowRt);

            renderSys.Clear(ClearFlags.Target | ClearFlags.DepthBuffer, -1, 1, 0);


            //effect.Begin(FX.DoNotSaveSamplerState | FX.DoNotSaveShaderState | FX.DoNotSaveState);
            //effect.BeginPass(0);
            float zFar = cam.FarPlane;

            Matrix.OrthoRH((float)ShadowMapLength / 10f, (float)ShadowMapLength / 10f, cam.NearPlane, zFar, out LightProjection);

            Vector3 camPos = cam.Position;
            Vector3 up = cam.Front;


            Vector3 lightTarget = camPos;
            Vector3 offset = up;
            Vector3.Multiply(ref offset, 0.5f * zFar, out offset);

            Vector3.Add(ref lightTarget, ref offset, out lightTarget);


            Vector3 lightPos = lightTarget;
            offset = lightDir;
            Vector3.Multiply(ref offset, 0.5f * zFar, out offset);
            Vector3.Subtract(ref lightPos, ref offset, out lightPos);


            //Vector3 v1;
            //Vector3.Subtract(ref camPos, ref lPos, out v1);


            //Vector3.Cross(ref lightDir, ref v1, out up);
            //Vector3.Cross(ref lightDir, ref up, out up);
            up.Y = 0;
            if (up.LengthSquared() == 0)
            {
                up = new Vector3(0.707f, 0, 0.707f);
            }

            up = Vector3.UnitY;


            lightTarget = camPos + (cam.Front - lightDir) * 40;
            lightTarget.Y = camPos.Y;

            lightPos = lightTarget - lightDir * 50;

            Matrix.LookAtRH(ref lightPos, ref lightTarget, ref up, out ViewTransform);




            ViewProj = ViewTransform * LightProjection;

            //Matrix proj = cam.Frustum.proj;
            //Matrix view = cam.Frustum.view;

            //device.SetTransform(TransformState.Projection, proj);
            //device.SetTransform(TransformState.View, view);


            //ShadowTransform = view * proj * TexTransform;
        }

        public Texture ShadowColorMap
        {
            get { return shadowRt; }
        }

        protected override void loadUnmanagedResources()
        {
            //shadowDepthMap = new Texture(renderSys, ShadowMapLength, ShadowMapLength, 1, Usage.DepthStencil, Format.D24X8, Pool.Default);
            shadowRt = factory.CreateRenderTarget(ShadowMapLength, ShadowMapLength, ImagePixelFormat.R32F, DepthFormat.Depth24X8);

            //shadowRtSurface = shadowRt.GetSurfaceLevel(0);
            //shadowDepSurface = shadowDepthMap.GetSurfaceLevel(0);

        }

        protected override void unloadUnmanagedResources()
        {
            //shadowDepthMap.Dispose();
            shadowRt.Dispose();

            //shadowRtSurface.Dispose();
            //shadowDepSurface.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                pip.Dispose();
                DefaultSMGen.Dispose();
            }
        }

    }
}

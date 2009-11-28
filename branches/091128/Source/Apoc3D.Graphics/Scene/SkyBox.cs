using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Config;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Apoc3D.Scene;

namespace Apoc3D.Graphics
{
    /// <summary>
    ///  定义一个天空盒
    /// </summary>
    public class SkyBox : SceneObject, IConfigurable, IDisposable
    {
        struct SkyVertex
        {
            public Vector3 pos;
            public Vector3 texCoord;

            static readonly VertexElement[] elements;

            static SkyVertex() 
            {
                elements = new VertexElement[2];
                elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
                elements[1] = new VertexElement(1, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0);
            }

            public static VertexElement[] Elements 
            {
                get { return elements; }
            }
        }

        bool disposed;

        Texture dayTex;
        Texture nightTex;

        VertexBuffer box;
        IndexBuffer indexBuffer;

        VertexDeclaration vtxDecl;

        RenderSystem renderSystem;
        ObjectFactory factory;

        Effect effect;
        //EffectHandle nightAlpha;
        //EffectHandle day;
        //EffectHandle night;

        public unsafe SkyBox(RenderSystem rs)
            :base(false )
        {
            renderSystem = rs;
            factory = rs.ObjectFactory;

            // sqrt(3)/3
            const float l = 1f / MathEx.Root3;

            vtxDecl = factory.CreateVertexDeclaration(SkyVertex.Elements);
            box = factory.CreateVertexBuffer(8, vtxDecl, BufferUsage.WriteOnly);

            SkyVertex* dst = (SkyVertex*)box.Lock(0, 0, LockMode.None);

            dst[0] = new SkyVertex { pos = new Vector3(-50f, -50f, -50f), texCoord = new Vector3(-l, -l, -l) };
            dst[1] = new SkyVertex { pos = new Vector3(50f, -50f, -50f), texCoord = new Vector3(l, -l, -l) };
            dst[2] = new SkyVertex { pos = new Vector3(-50f, -50f, 50f), texCoord = new Vector3(-l, -l, l) };
            dst[3] = new SkyVertex { pos = new Vector3(50f, -50f, 50f), texCoord = new Vector3(l, -l, l) };
            dst[4] = new SkyVertex { pos = new Vector3(-50f, 50f, -50f), texCoord = new Vector3(-l, l, -l) };
            dst[5] = new SkyVertex { pos = new Vector3(50f, 50f, -50f), texCoord = new Vector3(l, l, -l) };
            dst[6] = new SkyVertex { pos = new Vector3(-50f, 50f, 50f), texCoord = new Vector3(-l, l, l) };
            dst[7] = new SkyVertex { pos = new Vector3(50f, 50, 50f), texCoord = new Vector3(l, l, l) };

            box.Unlock();

            indexBuffer = factory.CreateIndexBuffer(IndexBufferType.Bit16, 36, BufferUsage.WriteOnly);

            ushort* ibDst = (ushort*)indexBuffer.Lock(0, 0, LockMode.None);

            ibDst[0] = 0;
            ibDst[1] = 1;
            ibDst[2] = 3;

            ibDst[3] = 0;
            ibDst[4] = 3;
            ibDst[5] = 2;


            ibDst[6] = 0;
            ibDst[7] = 4;
            ibDst[8] = 5;

            ibDst[9] = 0;
            ibDst[10] = 5;
            ibDst[11] = 1;



            ibDst[12] = 2;
            ibDst[13] = 6;
            ibDst[14] = 4;

            ibDst[15] = 2;
            ibDst[16] = 4;
            ibDst[17] = 0;


            ibDst[18] = 3;
            ibDst[19] = 7;
            ibDst[20] = 6;

            ibDst[21] = 3;
            ibDst[22] = 6;
            ibDst[23] = 2;


            ibDst[24] = 1;
            ibDst[25] = 5;
            ibDst[26] = 7;

            ibDst[27] = 1;
            ibDst[28] = 7;
            ibDst[29] = 3;


            ibDst[30] = 6;
            ibDst[31] = 7;
            ibDst[32] = 5;

            ibDst[33] = 6;
            ibDst[34] = 5;
            ibDst[35] = 4;

            //ibDst[0] = 0;
            //ibDst[1] = 1;
            //ibDst[2] = 3;

            //ibDst[3] = 0;
            //ibDst[4] = 2;
            //ibDst[5] = 3;


            //ibDst[6] = 4;
            //ibDst[7] = 5;
            //ibDst[8] = 7;

            //ibDst[9] = 4;
            //ibDst[10] = 6;
            //ibDst[11] = 7;



            //ibDst[12] = 0;
            //ibDst[13] = 1;
            //ibDst[14] = 4;

            //ibDst[15] = 1;
            //ibDst[16] = 4;
            //ibDst[17] = 5;


            //ibDst[18] = 0;
            //ibDst[19] = 4;
            //ibDst[20] = 2;

            //ibDst[21] = 4;
            //ibDst[22] = 6;
            //ibDst[23] = 2;


            //ibDst[24] = 1;
            //ibDst[25] = 3;
            //ibDst[26] = 5;

            //ibDst[27] = 5;
            //ibDst[28] = 7;
            //ibDst[29] = 3;


            //ibDst[30] = 2;
            //ibDst[31] = 3;
            //ibDst[32] = 6;

            //ibDst[33] = 2;
            //ibDst[34] = 7;
            //ibDst[35] = 3;

            indexBuffer.Unlock();






            FileLocation fl = FileSystem.Instance.Locate(FileSystem.CombinePath(Paths.Effects, "DayNight.fx"), FileLocateRules.Default);
            ContentStreamReader sr = new ContentStreamReader(fl);
            string code = sr.ReadToEnd();
            string err;
            effect = Effect.FromString(renderSystem, code, null, IncludeHandler.Instance, null, ShaderFlags.None, null, out err);

            effect.Technique = new EffectHandle("DayNight");

            nightAlpha = new EffectHandle("nightAlpha");

            day = new EffectHandle("Day");
            night = new EffectHandle("Night");

        }

        public float DayNightLerpParam
        {
            get;
            set;
        }

        /// <summary>
        ///  渲染天空盒
        /// </summary>
        public unsafe void Render()
        {
            if (dayTex != null)
            {
                //Matrix view = renderSystem.GetTransform(TransformState.View);
                //Matrix oldView = view;
                //view.M41 = 0;
                //view.M42 = 0;
                //view.M43 = 0;

                //renderSystem.SetTransform(TransformState.View, view);

                int passCount = effect.Begin(FX.DoNotSaveState | FX.DoNotSaveShaderState | FX.DoNotSaveSamplerState);


                effect.SetTexture(day, dayTex);
                effect.SetTexture(night, nightTex);
                effect.SetValue(nightAlpha, DayNightLerpParam);
                effect.CommitChanges();
      
                for (int i = 0; i < passCount; i++)
                {
                    effect.BeginPass(i);

                    renderSystem.SetRenderState(RenderState.ZEnable, false);
                    renderSystem.SetRenderState(RenderState.ZWriteEnable, false);
                    renderSystem.SetRenderState<Cull>(RenderState.CullMode, Cull.None);

                    renderSystem.SetStreamSource(0, box, 0, sizeof(SkyVertex));
                    renderSystem.VertexFormat = SkyVertex.Format;
                    renderSystem.VertexDeclaration = vtxDecl;

                    renderSystem.Indices = indexBuffer;
                    renderSystem.DrawIndexedPrimitives(RenderPrimitiveType.TriangleList, 0, 0, 8, 0, 12);

                    renderSystem.SetRenderState(RenderState.ZEnable, true);
                    renderSystem.SetRenderState(RenderState.ZWriteEnable, true);

                    renderSystem.SetTransform(TransformState.View, oldView);

                    effect.EndPass();
                }

                effect.End();
            }
        }
        public override void Update(float dt)
        {
            
        }

        public override RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }
        public override bool IsSerializable
        {
            get { return true; }
        }
        public override void Serialize(BinaryDataWriter data)
        {
            base.Serialize(data);
        }
        public override bool IntersectsSelectionRay(ref Ray ray)
        {
            return false;
        }
        

        /// <summary>
        ///  从ResourceLocation加载天空盒纹理
        /// </summary>
        /// <param name="dayTexture"></param>
        /// <param name="nightTexture"></param>
        public void LoadTexture(ResourceLocation dayTexture, ResourceLocation nightTexture)
        {
            if (dayTexture != null)
            {
                if (dayTex != null)
                {
                    dayTex.Dispose();
                    dayTex = null;
                }
                this.dayTex = factory.CreateTexture(dayTexture, TextureUsage.Static);
            }
            if (nightTexture != null)
            {
                if (nightTex != null)
                {
                    nightTex.Dispose();
                    nightTex = null;
                }
                this.nightTex = factory.CreateTexture(nightTexture, TextureUsage.Static);
            }
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            string dayTexFile = sect.GetString("DayTexture", null);
            if (dayTexFile != null)
            {
                FileLocation fl = FileSystem.Instance.Locate(dayTexFile, FileLocateRules.Default);

                dayTex = factory.CreateTexture(fl, TextureUsage.Static);
            }

            string nightTexFile = sect.GetString("NightTexture", null);
            if (nightTexFile != null)
            {
                FileLocation fl = FileSystem.Instance.Locate(nightTexFile, FileLocateRules.Default);

                nightTex = factory.CreateTexture(fl, TextureUsage.Static);
            }
        }

        #endregion

        #region IDisposable 成员
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) 
            {
                if (dayTex != null)
                {
                    dayTex.Dispose();
                    dayTex = null;
                }
                if (nightTex != null)
                {
                    nightTex.Dispose();
                    nightTex = null;
                }
                indexBuffer.Dispose();
                box.Dispose();

            }
        }


        #endregion
    }
}

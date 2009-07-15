using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using MainLogic;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.Logic;

namespace VirtualBicycle.UI
{
    public unsafe class PowerGraph : UIComponent, IDisposable, IUnmanagedResource
    {
        struct InstData
        {
            public float Distance;
            public float HeightRatio;

            public static int Size
            {
                get { return sizeof(float) * 2; }
            }
        }
        struct PBVertex
        {
            public Vector3 pos;
            public Vector2 tex1;

            public static VertexFormat Format
            {
                get { return VertexFormat.Position | VertexFormat.Texture1; }
            }

            public static int Size
            {
                get { return Vector3.SizeInBytes + Vector2.SizeInBytes; }
            }
        }

        public const int BarWidth = 10;
        public const int BarHeight = 125;
        public const int BarCount = 50;

        #region 字段
        GameMainLogic logic;
        IngameUI ingameUI;

        Device device;
        VertexBuffer powerBar;
        VertexBuffer instanceData;
        IndexBuffer idxBuf;

        VertexElement[] elements;
        VertexDeclaration vtxDecl;

        Effect effect;
        //EffectHandle ehBarHeights;
        EffectHandle ehBarPosition;
        EffectHandle ehBarTex;
        EffectHandle ehProj;

        Texture barTex;

        float[] powers = new float[BarCount];

        int barUpdateFrame;


        #endregion

        public Bicycle CurrectBicycle
        {
            get;
            set;
        }
        public PowerGraph(GameMainLogic logic, IngameUI ingameUI, Device device, Game game)
            : base(game)
        {
            this.logic = logic;
            this.ingameUI = ingameUI;

            this.device = device;

            FileLocation fl = FileSystem.Instance.Locate(FileSystem.CombinePath(Paths.Effects, "powerBar.fx"), FileLocateRules.Default);
            ContentStreamReader sr = new ContentStreamReader(fl);

            string err;
            string code = sr.ReadToEnd();
            effect = Effect.FromString(device, code, null, null, null, ShaderFlags.OptimizationLevel3, null, out err);
            sr.Close();

            effect.Technique = new EffectHandle("PowerBar");

            //ehBarHeights = new EffectHandle("barHeights");
            ehBarPosition = new EffectHandle("barPosition");
            ehBarTex = new EffectHandle("barTex");
            ehProj = new EffectHandle("proj");
        }

        protected override void render()
        {

            Size s = Game.Window.ClientSize;
            effect.SetValue<Matrix>(ehProj, Matrix.OrthoOffCenterRH(0, s.Width, s.Height, 0, 0, 10));
            effect.SetValue<Vector2>(ehBarPosition,
                new Vector2(
                    (s.Width - (BarWidth + 1) * BarCount) / 2,
                    s.Height - BarHeight - 30));

            effect.SetTexture(ehBarTex, barTex);
            effect.CommitChanges();

            effect.Begin(FX.DoNotSaveState | FX.DoNotSaveSamplerState | FX.DoNotSaveShaderState);

            effect.BeginPass(0);

            device.SetRenderState<Cull>(RenderState.CullMode, Cull.None);
            device.VertexDeclaration = vtxDecl;
            device.VertexFormat = VertexFormat.None;
            device.Indices = idxBuf;

            device.SetStreamSource(0, powerBar, 0, PBVertex.Size);
            device.SetStreamSourceFrequency(0, BarCount, StreamSource.IndexedData);

            device.SetStreamSource(1, instanceData, 0, InstData.Size);
            device.SetStreamSourceFrequency(1, 1, StreamSource.InstanceData);

            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, 4, 0, 2);

            effect.EndPass();
            effect.End();
        }

        protected override void render(Sprite sprite)
        {

        }

        protected override void update(float dt)
        {

            barUpdateFrame++;

            if (barUpdateFrame == 1)
            {
                for (int i = 1; i < BarCount; i++)
                {
                    powers[i - 1] = powers[i];
                }

                powers[BarCount - 1] += CurrectBicycle.Power;
                powers[BarCount - 1] *= 0.5f;

                barUpdateFrame = 0;
            }
            else
            {
                powers[BarCount - 1] = CurrectBicycle.Power;

            }

            InstData* dst = (InstData*)instanceData.Lock(0, 0, LockFlags.None).DataPointer;

            for (int i = 0; i < BarCount; i++)
            {
                dst->HeightRatio = Math.Min(powers[i] / 600f, 1);
                dst++;
            }

            instanceData.Unlock();
        }


        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                effect.Dispose();

                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion

        protected override void load()
        {
            elements = new VertexElement[5];
            elements[0] = new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0);
            elements[1] = new VertexElement(0, sizeof(float) * 3, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0);
            elements[2] = new VertexElement(1, 0, DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 1);
            elements[3] = new VertexElement(1, sizeof(float), DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 2);

            elements[4] = VertexElement.VertexDeclarationEnd;

            vtxDecl = new VertexDeclaration(device, elements);


            powerBar = new VertexBuffer(device, PBVertex.Size * 4, Usage.None, PBVertex.Format, Pool.Managed);

            PBVertex* ptr = (PBVertex*)powerBar.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

            //Rectangle progArea = new Rectangle(0, 0, 512, 100);
            //ptr[0].dummy = 1.0f;
            ptr[0].pos = new Vector3(0, 0, 0);
            ptr[0].tex1 = new Vector2(0, 0);

            //ptr[1].dummy = 1.0f;
            ptr[1].pos = new Vector3(0, BarHeight, 0);
            ptr[1].tex1 = new Vector2(0, 1);

            //ptr[2].dummy = 1.0f;
            ptr[2].pos = new Vector3(BarWidth, 0, 0);
            ptr[2].tex1 = new Vector2(1, 0);

            //ptr[3].dummy = 1.0f;
            ptr[3].pos = new Vector3(BarWidth, BarHeight, 0);
            ptr[3].tex1 = new Vector2(1, 1);


            powerBar.Unlock();

            idxBuf = new IndexBuffer(device, sizeof(int) * 4, Usage.None, Pool.Managed, false);
            int* idst = (int*)idxBuf.Lock(0, 0, LockFlags.None).DataPointer;
            idst[0] = 0;
            idst[1] = 1;
            idst[2] = 2;
            idst[3] = 3;
            idxBuf.Unlock();

            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.DataUI, "PowerBar.png"), FileLocateRules.Default);
            barTex = TextureLoader.LoadUITexture(device, fl);

            LoadUnmanagedResources();
        }

        protected override void unload()
        {

        }

        #region IUnmanagedResource 成员

        public void LoadUnmanagedResources()
        {
            instanceData = new VertexBuffer(device, InstData.Size * BarCount, Usage.Dynamic, VertexFormat.None, Pool.Default);
            InstData* dst = (InstData*)instanceData.Lock(0, 0, LockFlags.None).DataPointer;

            for (int i = 0; i < BarCount; i++)
            {
                dst->Distance = (BarWidth + 1) * i;
                dst++;
            }

            instanceData.Unlock();
        }

        public void UnloadUnmanagedResources()
        {
            instanceData.Dispose();
        }

        #endregion
    }

}

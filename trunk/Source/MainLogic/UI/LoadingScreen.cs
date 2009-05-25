using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Config;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;

namespace VirtualBicycle.UI
{
    public unsafe class LoadingScreen : UIComponent
    {
        struct PBVertex
        {
            public Vector3 pos;
            public float dummy;
            public int diffuse;

            public static VertexFormat Format
            {
                get { return VertexFormat.PositionRhw | VertexFormat.Diffuse; }
            }

            public static int Size
            {
                get { return Vector4.SizeInBytes + sizeof(int); }
            }
        }

        FileLocation texLocation;
        Texture backGround;

        Rectangle progArea;


        float currentProgress;

        VertexBuffer vbOutline;
        Device device;

        object syncMutex = new object();

        int pbColor;

        Color4 bgColor = Color.White;

        MenuPic prgBar;
        MenuPicDrawPara prgBarPara;
        MenuPicDrawPara prgBarPara2;

        public LoadingScreen(Game game, GameUI gameUI, string configFile)
            : base(game)
        {
            device = gameUI.Device;


            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.Configs, configFile), FileLocateRules.Default);

            VirtualBicycle.Config.Configuration config = ConfigurationManager.Instance.CreateInstance(fl);

            ConfigurationSection sect = config["LoadingScreen"];

            sect.GetRectangle("ProgressBarRegion", out progArea);

            texLocation = FileSystem.Instance.Locate(Path.Combine(Paths.DataUI, sect["Background"]), FileLocateRules.Default);

            pbColor = sect.GetColorRGBInt("ProgressBarColor");



            prgBar = new MenuPic(Game, "prgBar.png", "Progress Bar");

            prgBarPara.Alpha = 0.8f;
            prgBarPara.PosX = progArea.Left;
            prgBarPara.PosY = progArea.Top + progArea.Height / 2;
            prgBarPara.desiredWidth = 0;
            prgBarPara.desiredHeight = progArea.Height;

            prgBarPara2.Alpha = 0.8f;
            prgBarPara2.PosX = progArea.Left + progArea.Width / 2;
            prgBarPara2.PosY = progArea.Top + progArea.Height / 2;
            prgBarPara2.desiredWidth = progArea.Width;
            prgBarPara2.desiredHeight = progArea.Height;

            prgBar.firstDrawPara = prgBarPara;
            prgBar.nextDrawPara = prgBarPara2;
            prgBar.SetCurrentPara(0);
        }

        public void SetProgressValue(int current, int total)
        {

            float progress = (float)current / (float)total;
            this.currentProgress = progress;

            prgBar.SetCurrentPara(progress);
            //PBVertex* ptr1 = (PBVertex*)vb.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

            //ptr1[0].dummy = 1.0f;
            //ptr1[0].pos = new Vector3(progArea.Left, progArea.Top, 0);
            //ptr1[0].diffuse = pbColor;

            //ptr1[1].dummy = 1.0f;
            //ptr1[1].pos = new Vector3(progArea.Left, progArea.Bottom, 0);
            //ptr1[1].diffuse = pbColor;

            //ptr1[2].dummy = 1.0f;
            //ptr1[2].pos = new Vector3(progArea.Left + progArea.Width * currentProgress, progArea.Top, 0);
            //ptr1[2].diffuse = pbColor;

            //ptr1[3].dummy = 1.0f;
            //ptr1[3].pos = new Vector3(progArea.Left + progArea.Width * currentProgress, progArea.Bottom, 0);
            //ptr1[3].diffuse = pbColor;

            //vb.Unlock();

        }

        public override bool IsStacked
        {
            get
            {
                return false;
            }
        }

        protected override void render()
        {
            device.VertexShader = null;
            device.PixelShader = null;
            device.VertexFormat = PBVertex.Format;
            device.Indices = null;

            device.SetStreamSource(0, vbOutline, 0, PBVertex.Size);
            device.DrawPrimitives(PrimitiveType.LineStrip, 0, 4);

        }
        protected override void render(Sprite sprite)
        {
            if (backGround != null)
            {
                sprite.Transform = Matrix.Identity;

                sprite.Draw(backGround, bgColor);
            }

            prgBar.Render(sprite);
        }

        protected override void update(float dt)
        {

        }

        protected override void load()
        {
            vbOutline = new VertexBuffer(device, PBVertex.Size * 5, Usage.None, PBVertex.Format, Pool.Managed);

            PBVertex* ptr = (PBVertex*)vbOutline.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

            ptr[0].dummy = 1.0f;
            ptr[0].pos = new Vector3(progArea.Left - 3, progArea.Top - 3, 0);
            ptr[0].diffuse = pbColor;

            ptr[1].dummy = 1.0f;
            ptr[1].pos = new Vector3(progArea.Right + 3, progArea.Top - 3, 0);
            ptr[1].diffuse = pbColor;

            ptr[2].dummy = 1.0f;
            ptr[2].pos = new Vector3(progArea.Right + 3, progArea.Bottom + 3, 0);
            ptr[2].diffuse = pbColor;

            ptr[3].dummy = 1.0f;
            ptr[3].pos = new Vector3(progArea.Left - 3, progArea.Bottom + 3, 0);
            ptr[3].diffuse = pbColor;

            ptr[4].dummy = 1.0f;
            ptr[4].pos = new Vector3(progArea.Left - 3, progArea.Top - 3, 0);
            ptr[4].diffuse = pbColor;


            vbOutline.Unlock();


            backGround = TextureLoader.LoadUITexture(device, texLocation);
       
        }

        protected override void unload()
        {
            vbOutline.Dispose();
            vbOutline = null;
            backGround.Dispose();
            backGround = null;
        }
    }
}

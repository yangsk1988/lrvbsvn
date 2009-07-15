using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;

namespace VirtualBicycle.UI
{
    public class PictureBox : Control
    {
        public Texture Texture
        {
            get;
            private set;
        }

        public PictureBox(GameUI gameUI, Texture tex)
            : base(gameUI)
        {
            this.Texture = tex;

            SurfaceDescription desc = Texture.GetLevelDescription(0);
            this.Width = desc.Width;
            this.Height = desc.Height;

        }

        public PictureBox(GameUI gameUI, string fileName)
            : base(gameUI)
        {
            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.DataUI, fileName),
                FileLocateRules.Default);
            this.Texture = TextureLoader.LoadUITexture(Device, fl);

            SurfaceDescription desc = Texture.GetLevelDescription(0);
            this.Width = desc.Width;
            this.Height = desc.Height;

        }


        protected override void render(Sprite sprite)
        {


            //sprite.Transform =


            System.Drawing.Color curColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            
            sprite.Draw(Texture, Color.ToArgb());
            sprite.Transform = Matrix.Identity;
        }
    }
}

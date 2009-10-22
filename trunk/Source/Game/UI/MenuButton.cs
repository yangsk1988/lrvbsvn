using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using System.Drawing;

namespace VirtualBicycle.UI
{
    public class MenuButton : IDisposable
    {
        #region Fields
        
        public int lx;
        public int ly;

        Texture texture;

        Device device;
        Sprite sprite;

        Color4 texColor = Color.White;
        Color4 texColor2 = new Color4(0.5f, 1, 1, 1);
        #endregion

        #region 属性
        public string Name
        {
            get;
            private set;
        }

        public bool Selected
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public MenuButton(string name, string fileName,
            int lx, int ly, Device device, Sprite sprite)
        {
            this.Name = name;
            this.lx = lx;
            this.ly = ly;

            this.device = device;
            this.sprite = sprite;

            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.DataUI, fileName),
                FileLocateRules.Default);

            texture = TextureLoader.LoadUITexture(device, fl);
        }
        #endregion

        #region Render
        public void Render()
        {
            sprite.Transform = Matrix.Translation(this.lx, this.ly, 0);

            if (Selected)
            {
                sprite.Draw(texture, texColor);
            }
            else
            {
                sprite.Draw(texture, texColor2);
            }
        }
        #endregion

        #region Update
        public void Update(float dt)
        {
            
        }
        #endregion

        public event EventHandler Activated;

        public void ResetActivateEvent() 
        {
            Activated = null;
        }

        public void Activate()
        {
            if (Activated != null)
            {
                Activated(this, EventArgs.Empty);
            }
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
                Activated = null;
                texture.Dispose();
                texture = null;
                sprite = null;
                device = null;

                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion
    }
}

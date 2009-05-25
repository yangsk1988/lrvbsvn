using System;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.Sound;
using System.Drawing;
using System.IO;

namespace VirtualBicycle.UI
{
    //图片绘制的参数,可以用来做动画
    public struct MenuPicDrawPara
    {
        #region Fields
        public float PosX;
        public float PosY;
        public float Alpha;
        public float desiredWidth;
        public float desiredHeight;

        public readonly static MenuPicDrawPara Zero;

        #endregion

        public Rectangle DesiredRectangle
        {
            get
            {
                return new Rectangle((int)PosX, (int)PosY, (int)desiredWidth, (int)desiredHeight);
            }
        }

        #region Constructor
        public MenuPicDrawPara(float x, float y, float a, float w, float h)
        {
            PosX = x;
            PosY = y;
            Alpha = a;
            desiredWidth = w;
            desiredHeight = h;
        }

        public MenuPicDrawPara(Vector2 pos, float a, Vector2 size)
        {
            PosX = pos.X;
            PosY = pos.Y;
            Alpha = a;
            desiredWidth = size.X;
            desiredHeight = size.Y;
        }

        static MenuPicDrawPara()
        {
            Zero.Alpha = 0f;
            Zero.PosX = 0f;
            Zero.PosY = 0f;
            Zero.desiredHeight = 0f;
            Zero.desiredWidth = 0f;
        }
        #endregion
    }

    //图片绘制的类,可以方便的得到一些绘制参数
    public class MenuPic
    {
        #region Fields
        //文件名
        private Device device;
        private Game Game;

        string fileName;
        public string FileName
        {
            get { return fileName; }
        }

        //细节说明
        string detail;
        public string Detail
        {
            get { return detail; }
        }

        private float width;
        public float Width
        {
            get { return width; }
        }

        private float height;
        public float Height
        {
            get { return height; }
        }

        private Texture texture;

        //图标的纹理
        public Texture Texture
        {
            get
            {
                return texture;
            }
        }

        public MenuPicDrawPara curDrawPara;
        public MenuPicDrawPara firstDrawPara;
        public MenuPicDrawPara nextDrawPara;

        public event EventHandler ActivedHandler;
        #endregion

        #region Constructor
        public MenuPic(Game game, string fileName, string detail)
        {
            this.fileName = fileName;
            this.detail = detail;
            this.device = game.Device;
            this.Game = game;
            this.modColor = new Color3(1, 1, 1);

            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.DataUI, fileName),
                FileLocateRules.Default);
            this.texture = TextureLoader.LoadUITexture(device, fl);

            SurfaceDescription desc = texture.GetLevelDescription(0);
            this.width = desc.Width;
            this.height = desc.Height;

        }

        public MenuPic(Game game, Texture tex, string detail)
        {
            this.device = game.Device;
            this.detail = detail;
            this.texture = tex;
            this.Game = game;

            if (texture != null)
            {
                SurfaceDescription desc = texture.GetLevelDescription(0);
                this.width = desc.Width;
                this.height = desc.Height;
            }
            this.modColor = new Color3(1, 1, 1);
        }
        #endregion

        #region Fields
        public void Unload()
        {
            texture.Dispose();
            texture = null;
        }

        //直接对于当前的CurrentDrawPara进行绘制
        public void Render(Sprite sprite)
        {
            if (this.texture != null)
            {
                float dWidth = GetPositionWidth(curDrawPara.desiredWidth);
                float dHeight = GetPositionHeight(curDrawPara.desiredHeight);
                float posX = GetPositionWidth(curDrawPara.PosX);
                float posY = GetPositionHeight(curDrawPara.PosY);
                float a = curDrawPara.Alpha;

                sprite.Transform = Matrix.Scaling(dWidth / width, dHeight / height, 1) *
                    Matrix.Translation(new Vector3(new Vector2(posX, posY) - new Vector2(dWidth / 2, dHeight / 2), 0f));
                sprite.Draw(texture, new Color4(a, modColor.Red, modColor.Green, modColor.Blue));
                sprite.Transform = Matrix.Identity;
            }
        }

        //根据dPara的值,对于前后的Para进行插值得到新的Para
        public void SetCurrentPara(float dPara)
        {
            curDrawPara.Alpha = firstDrawPara.Alpha + (nextDrawPara.Alpha - firstDrawPara.Alpha) * dPara;
            curDrawPara.PosX = firstDrawPara.PosX + (nextDrawPara.PosX - firstDrawPara.PosX) * dPara;
            curDrawPara.PosY = firstDrawPara.PosY + (nextDrawPara.PosY - firstDrawPara.PosY) * dPara;
            curDrawPara.desiredHeight = firstDrawPara.desiredHeight + (nextDrawPara.desiredHeight - firstDrawPara.desiredHeight) * dPara;
            curDrawPara.desiredWidth = firstDrawPara.desiredWidth + (nextDrawPara.desiredWidth - firstDrawPara.desiredWidth) * dPara;
        }

        private Color3 modColor;
        public Color3 ModColor
        {
            get { return modColor; }
            set { modColor = value; }
        }

        public void Activate()
        {
            if (ActivedHandler != null)
            {
                ActivedHandler(this, EventArgs.Empty);
            }
        }

        public void SetTexture(string fileName)
        {
            this.texture.Dispose();
            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.DataUI, fileName),
                FileLocateRules.Default);
            this.texture = TextureLoader.LoadUITexture(device, fl);

            SurfaceDescription desc = texture.GetLevelDescription(0);
            this.width = desc.Width;
            this.height = desc.Height;
        }

        public void Dispose()
        {
            if (this.texture != null)
            {
                this.texture.Dispose();
            }
        }

        public float GetPositionWidth(float x)
        {
            const float defaultWidth = 1024f;
            float newx = x;
            if (Game != null)
            {
                newx = x / defaultWidth * Game.Window.ClientSize.Width;
            }

            return newx;
        }

        public float GetPositionHeight(float x)
        {
            const float defaultHeight = 768f;
            float newx = x;
            if (Game != null)
            {
                newx = x / defaultHeight * Game.Window.ClientSize.Height;
            }

            return newx;
        }

        public Vector2 GetPosition(Vector2 pos)
        {
            const float defaultWidth = 1024f;
            const float defaultHeight = 768f;
            float x = pos.X / defaultWidth * Game.Window.ClientSize.Width;
            float y = pos.Y / defaultHeight * Game.Window.ClientSize.Height;
            return new Vector2(x, y);
        }
        #endregion
    }
}
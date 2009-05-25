using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;

namespace VirtualBicycle.UI
{
    public class PictureBoxState
    {
        public PictureBoxState()
        {
            Width = 100;
            Height = 100;
            AnimationTime = 1f;
            ModulateColor = Color.White;
        }

        public float X
        {
            get;
            set;
        }
        public float Y
        {
            get;
            set;
        }

        public float Width
        {
            get;
            set;
        }

        public float Height
        {
            get;
            set;
        }

        public Color ModulateColor
        {
            get;
            set;
        }

        public float AnimationTime
        {
            get;
            set;
        }

        public RectangleF DesiredRectangle
        {
            get
            {
                return new RectangleF(X, Y, Width, Height);
            }
        }
    }

    public class UIPictureBox
    {

        #region 字段和属性
        
        Device device;
        Game Game;

        PictureBoxState currentState = new PictureBoxState();

        int animationIndex;
        float stepTimeElasped;

        List<PictureBoxState> animationFrames = new List<PictureBoxState>();


        public string FileName
        {
            get;
            private set;
        }

        public Texture Texture
        {
            get;
            private set;
        }

        public event EventHandler ActivedHandler;
        #endregion

        #region 构造函数
        public UIPictureBox(string fileName, string detail, Device device, Game game)
        {
            this.FileName = fileName;

            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.DataUI, fileName),
                FileLocateRules.Default);
            this.Texture = TextureLoader.LoadUITexture(device, fl);

            this.device = device;
            this.Game = game;
        }

        public UIPictureBox(Texture tex, string detail, Device deivce, Game game)
        {
            this.device = deivce;
            this.Texture = tex;
            this.Game = game;
        }
        #endregion

        #region 方法
        public void Unload()
        {
            Texture.Dispose();
            Texture = null;
        }

        public void Update(float dt)
        {
            if (animationFrames.Count > 0)
            {
                PictureBoxState frame = animationFrames[animationIndex];

                stepTimeElasped += dt;

                if (stepTimeElasped > frame.AnimationTime)
                {
                    animationIndex++;
                    stepTimeElasped = 0;
                }

                if (animationIndex >= animationFrames.Count)
                {
                    animationIndex = 0;
                    stepTimeElasped = 0;
                }

                frame = animationFrames[animationIndex];

                if (animationFrames.Count > 1)
                {
                    int idx2 = animationIndex + 1;
                 
                    PictureBoxState nextFrame = animationFrames[idx2 < animationFrames.Count ? idx2 : 0];

                }
            }

        }

        public void Render(Sprite sprite)
        {
            if (this.Texture != null)
            {
                //float dWidth = GetPositionWidth(curDrawPara.desiredWidth);
                //float dHeight = GetPositionHeight(curDrawPara.desiredHeight);
                //float posX = GetPositionWidth(curDrawPara.PosX);
                //float posY = GetPositionHeight(curDrawPara.PosY);
                //float a = curDrawPara.Alpha;

                sprite.Transform = Matrix.Scaling(dWidth / width, dHeight / height, 1) *
                    Matrix.Translation(new Vector3(new Vector2(posX, posY) - new Vector2(dWidth / 2, dHeight / 2), 0f));
                sprite.Draw(Texture, );
                sprite.Transform = Matrix.Identity;
            }
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
            if (this.Texture != null && !this.Texture.Disposed)
                this.Texture.Dispose();

            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.DataUI, fileName),
                FileLocateRules.Default);
            this.Texture = TextureLoader.LoadUITexture(device, fl);

            SurfaceDescription desc = Texture.GetLevelDescription(0);

        }

        public void Dispose()
        {
            if (this.texture != null)
            {
                this.texture.Dispose();
            }
        }

        #endregion
    }
}

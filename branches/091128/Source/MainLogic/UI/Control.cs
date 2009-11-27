using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.MathLib;

namespace VirtualBicycle.UI
{
    /// <summary>
    /// 控件绘制的参数，可以用来做动画
    /// </summary>
    public class ControlParameter
    {
        #region 字段
        public float X;
        public float Y;

        public float Width;
        public float Height;

        public float Angle;
        public float Scale;

        public Color Color;

        #endregion

        public ControlParameter()
        {
            Width = Control.DefaultWidth;
            Height = Control.DefaultHeight;
            Color = Color.White;
            Scale = 1;
        }

        public Matrix GetTransform(float oWidth, float oHeight)
        {
            return Matrix.RotationZ(MathEx.Degree2Radian(Angle)) *
                   Matrix.Scaling(Scale * Width / oWidth, Scale * Height / oHeight, 1) *
                   Matrix.Translation(X, Y, 0); //(new Vector3(new Vector2(posX, posY) - new Vector2(dWidth / 2, dHeight / 2), 0f));
        }

        public RectangleF Region
        {
            get { return new RectangleF(X, Y, Width, Height); }
        }
    }

    public abstract class Control : IDisposable
    {
        public const int DefaultWidth = 100;
        public const int DefaultHeight = 100;

        public const string DefaultFont = "微软雅黑";

        #region 字段

        Device device;
        GameUI gameUI;

        List<ControlParameter> parameters;

        protected Matrix currentTransform;
        protected Color currentColor;

        

        #endregion


        public event EventHandler ActivedHandler;

        public void Activate()
        {
            if (ActivedHandler != null)
            {
                ActivedHandler(this, EventArgs.Empty);
            }
        }

        protected Control(GameUI gameUI)
        {
            parameters = new List<ControlParameter>();

            this.Width = DefaultWidth;
            this.Height = DefaultHeight;

            this.FrameLength = 30;
            this.Animating = true;

            this.gameUI = gameUI;
            this.device = gameUI.Device;
        }

        public int FrameLength
        {
            get;
            set;
        }
        public int CurrentFrame
        {
            get;
            set;
        }
        public bool Animating
        {
            get;
            set;
        }

        public Device Device
        {
            get { return device; }
        }

        public GameUI GameUI 
        {
            get { return gameUI; }
        }

        public bool IsLoaded
        {
            get;
            private set;
        }

        public bool Disposed
        {
            get;
            private set;
        }

        public float DesiredWidth
        {
            get
            {
                if (parameters.Count == 0)
                {
                    parameters.Add(new ControlParameter());
                }
                return parameters[0].Width;
            }
        }

        public float DesiredHeight
        {
            get
            {
                if (parameters.Count == 0)
                {
                    parameters.Add(new ControlParameter());
                }
                return parameters[0].Height;
            }
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


        public float X
        {
            get
            {
                if (parameters.Count == 0)
                {
                    parameters.Add(new ControlParameter());
                }
                return parameters[0].X;
            }
        }

        public float Y
        {
            get
            {
                if (parameters.Count == 0)
                {
                    parameters.Add(new ControlParameter());
                }
                return parameters[0].Y;
            }
        }

        public Color Color
        {
            get
            {
                if (parameters.Count == 0)
                {
                    parameters.Add(new ControlParameter());
                }
                return parameters[0].Color;
            }
        }

        protected virtual void load()
        {
        }

        protected virtual void unload()
        {
        }

        protected virtual void update(float dt)
        {
            
        }
        protected virtual void render(Sprite sprite) 
        {

        }

        public void Update(float dt)
        {
            if (!IsLoaded)
            {
                load();
                IsLoaded = true;
            }
            update(dt);
        }
        public void Render(Sprite sprite)
        {
            if (!IsLoaded)
            {
                load();
                IsLoaded = true;
            }
            render(sprite);
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (!Disposed)
            {
                if (IsLoaded)
                {
                    unload();
                    IsLoaded = false;
                }

                Disposed = true;
            }
        }

        #endregion
    }
}

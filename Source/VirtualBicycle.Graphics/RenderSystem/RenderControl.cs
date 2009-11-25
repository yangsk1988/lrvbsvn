using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Graphics;
using SD = System.Drawing;

namespace VirtualBicycle.Graphics
{
    /// <summary>
    ///  表示渲染目标所在的控件
    /// </summary>
    public abstract class RenderControl : IDisposable
    {
        RenderTarget renderTarget;

        PresentParameters presentParams;
        RenderSystem renderSystem;

        FpsCounter fpsCounter;

        protected RenderControl(RenderSystem rs, PresentParameters pm, RenderTarget rt)
            : this(rs, pm)
        {
            this.renderTarget = rt;
        }
        protected RenderControl(RenderSystem rs, PresentParameters pm)
        {
            this.presentParams = pm;
            this.renderSystem = rs;
            this.fpsCounter = new FpsCounter();

        }

        public RenderTarget RenderTarget
        {
            get { return renderTarget; }
        }

        protected abstract RenderTarget CreateRenderTarget(RenderSystem rs, PresentParameters pm);

        public virtual void Present()
        {
            fpsCounter.Update();
        }

        public float FPS
        {
            get { return fpsCounter.FPS; }
        }

        public void NotifyBind()
        {
            if (renderTarget == null)
            {
                renderTarget = CreateRenderTarget(renderSystem, presentParams);
            }
        }

        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        ~RenderControl()
        {
            if (!Disposed)
            {
                Dispose(false);
                Disposed = false;
            }
        }

        #endregion
    }

    /// <summary>
    ///  表示渲染窗口
    /// </summary>
    public abstract class RenderWindow : RenderControl
    {

        protected RenderWindow(RenderSystem rs, PresentParameters pm, RenderTarget rt)
            : base(rs, pm, rt)
        {
        }
        protected RenderWindow(RenderSystem rs, PresentParameters pm)
            : base(rs, pm)
        {
        }

    }
}

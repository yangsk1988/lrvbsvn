using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Media;

namespace Apoc3D.Graphics
{
    /// <summary>
    ///  表示渲染目标
    /// </summary>
    public abstract class RenderTarget : IDisposable
    {
        protected BackBuffer colorBuffer;
        protected DepthBuffer depthStencilBuffer;

        protected RenderTarget(RenderSystem renderSystem, int width, int height,
            ImagePixelFormat clrBufFormat, DepthFormat depBufFmt)
        {
            Width = width;
            Height = height;
            ColorBufferFormat = clrBufFormat;
            DepthBufferFormat = depBufFmt;
        }
        protected RenderTarget(RenderSystem renderSystem, int width, int height,
                  ImagePixelFormat clrBufFormat)
        {
            Width = width;
            Height = height;
            ColorBufferFormat = clrBufFormat;
            DepthBufferFormat = DepthFormat.Count;
        }

        public abstract Texture GetColorBufferTexture();
        public abstract Texture GetDepthBufferTexture();       

        public int Width
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            private set;
        }

        public DepthFormat DepthBufferFormat
        {
            get;
            private set;
        }
        public ImagePixelFormat ColorBufferFormat
        {
            get;
            private set;
        }


        #region IDisposable 成员

        protected virtual void Dispose(bool disposing) { }

        public bool Disposed
        {
            get;
            private set;
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

        #endregion
    }
}

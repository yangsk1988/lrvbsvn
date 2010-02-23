﻿using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Media;

namespace VirtualBicycle.Graphics
{
    /// <summary>
    ///  表示渲染目标
    /// </summary>
    public abstract class RenderTarget
    {
        protected BackBuffer colorBuffer;
        protected BackBuffer depthStencilBuffer;

        protected RenderTarget(RenderSystem renderSystem, int width, int height,
            ImagePixelFormat clrBufFormat, DepthFormat depBufFmt)
        {
            Width = width;
            Height = height;
            ColorBufferFormat = clrBufFormat;
            DepthBufferFormat = depBufFmt;
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

    }
}

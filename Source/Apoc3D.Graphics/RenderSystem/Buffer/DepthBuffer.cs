using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Media;

namespace Apoc3D.Graphics
{
    /// <summary>
    ///  表示纹理中的Surface，在这里通常是RenderTarget
    /// </summary>
    public abstract class DepthBuffer : HardwareBuffer
    {
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

        public DepthFormat DepthFormat
        {
            get;
            private set;
        }

        protected DepthBuffer(int width, int height, BufferUsage usage, DepthFormat format)
            : base(usage, PixelFormat.GetMemorySize(width, height, format), false)
        {
            Width = width;
            Height = height;

            DepthFormat = format;
        }

    }
}
 
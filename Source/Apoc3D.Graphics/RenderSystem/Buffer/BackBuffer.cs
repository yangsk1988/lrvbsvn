using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Media;

namespace VirtualBicycle.Graphics
{
    /// <summary>
    ///  表示纹理中的Surface，在这里通常是RenderTarget
    /// </summary>
    public abstract class BackBuffer : HardwareBuffer
    {
        //static Surface()
        //{
        //    const int arrLen = 11;
        //    bytesPP = new int[arrLen];
        //    for (int i = 0; i < arrLen; i++)
        //        bytesPP [i]= -1;

        //    bytesPP[(int)DepthFormat.Depth15Stencil1] = 2;
        //    bytesPP[(int)DepthFormat.Depth16] = 2;
        //    bytesPP[(int)DepthFormat.Depth16Lockable] = 2;
        //    bytesPP[(int)DepthFormat.Depth24] = 4;
        //    bytesPP[(int)DepthFormat.Depth24Stencil4] = 4;
        //    bytesPP[(int)DepthFormat.Depth24Stencil8] = 4;
        //    bytesPP[(int)DepthFormat.Depth24Stencil8Single] = 4;
        //    bytesPP[(int)DepthFormat.Depth32] = 4;
        //    bytesPP[(int)DepthFormat.Depth32Lockable] = 4;
        //    bytesPP[(int)DepthFormat.Depth32Single] = 4;
        //}

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

        public ImagePixelFormat ColorFormat
        {
            get;
            private set;
        }

        protected BackBuffer(int width, int height, BufferUsage usage, PixelFormat format)
            : base(usage, PixelFormat.GetMemorySize(width, height, 1, format), false)
        {
            Width = width;
            Height = height;

            ColorFormat = format;
        }

        //static int[] bytesPP;
        //static int GetMemorySize(int width, int height, DepthFormat format)
        //{
        //    return bytesPP[(int)format] * width * height;
        //}


        //protected override IntPtr @lock(int offset, int size, LockMode mode)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override void unlock()
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Dispose(bool disposing)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
 
using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using D3D = SlimDX.Direct3D9;

namespace VirtualBicycle.Graphics.D3D9
{
    internal class D3D9Surface : Surface
    {
        D3D.Surface surface;

        internal D3D9Surface(D3D9RenderSystem rs, D3D.Surface suf, int width, int height, BufferUsage usage, PixelFormat format)
            : base(width, height, usage, format)
        {
            this.surface = suf;
        }
        //internal D3D9Surface(D3D9RenderSystem rs, D3D.Surface suf, int width, int height, BufferUsage usage, DepthFormat format)
        //    : base(width, height, usage, format)
        //{
        //    this.surface = suf;
        //}

        public D3D9Surface(D3D9RenderSystem rs, int width, int height, BufferUsage usage, PixelFormat format)
            : base(width, height, usage, format)
        {
            surface = D3D.Surface.CreateRenderTarget(
                rs.D3DDevice, 
                width, height,
                D3D9Utils.ConvertEnum(format), 
                MultisampleType.None, 0,
                true);            
        }

        public D3D9Surface(D3D9RenderSystem rs, int width, int height, BufferUsage usage, DepthFormat format)
            : base(width, height, usage, format)
        {
            surface = D3D.Surface.CreateDepthStencil(
                rs.D3DDevice, 
                width, height,
                D3D9Utils.ConvertEnum(format), 
                MultisampleType.None, 0,
                (usage & BufferUsage.Discardable) == BufferUsage.Discardable);
        }

        public D3D.Surface D3DSurface
        {
            get { return surface; }
        }


        protected override IntPtr @lock(int offset, int size, LockMode mode)
        {
            throw new NotImplementedException();
        }

        protected override void unlock()
        {
            
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                surface.Dispose();
            }
            surface = null;                      
        }

    }
}

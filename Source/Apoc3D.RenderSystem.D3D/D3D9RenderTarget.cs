using System;
using System.Collections.Generic;
using System.Text;
using D3D = SlimDX.Direct3D9;

namespace Apoc3D.Graphics.D3D9
{
    //public enum RenderTargetType { }
    internal sealed class D3D9RenderTarget : RenderTarget
    {
        internal D3D.Surface d3dClrBuffer;
        internal D3D.Surface d3dDepBuffer;

        internal D3D9RenderTarget(D3D9RenderSystem rs,
            D3D.Surface backBuffer, D3D.Surface depthBuffer,
            int width, int height,
            PixelFormat clrFmt, PixelFormat depthFmt)
            : base(rs, width, height, clrFmt, depthFmt)
        {
            colorBuffer = new D3D9Surface(rs, backBuffer, width, height, BufferUsage.Static, clrFmt);
            depthStencilBuffer = new D3D9Surface(rs, depthBuffer, width, height, BufferUsage.Static, depthFmt);
           
            d3dClrBuffer = backBuffer;
            d3dDepBuffer = depthBuffer;
        }
        public D3D9RenderTarget(D3D9RenderSystem rs, int width, int height,
            PixelFormat clrFmt, PixelFormat depthFmt)
            : base(rs, width, height, clrFmt, depthFmt)
        {
            D3D9Surface tmp = new D3D9Surface(rs, width, height, BufferUsage.Static, clrFmt);

            d3dClrBuffer = tmp.D3DSurface;
            colorBuffer = tmp;

            tmp = new D3D9Surface(rs, width, height, BufferUsage.Static, depthFmt);
            d3dDepBuffer = tmp.D3DSurface; 
            depthStencilBuffer = tmp;        
        }

        public override Texture GetColorBufferTexture()
        {
            throw new NotImplementedException();
        }

        public override Texture GetDepthBufferTexture()
        {
            throw new NotImplementedException();
        }

    }
}

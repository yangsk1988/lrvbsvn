using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Apoc3D.Graphics.D3D9
{
    /// <summary>
    ///  D3D9RenderControl 就和 SwapChain一样
    /// </summary>
    internal sealed class D3D9RenderControl : RenderControl
    {
        public D3D9RenderControl(D3D9RenderSystem rs, PresentParameters pm, D3D9RenderTarget rt)
            : base(rs, pm, rt)
        {
        }

        public D3D9RenderControl(D3D9RenderSystem rs, PresentParameters pm)
            : base(rs, pm)
        {

        }

        protected override RenderTarget CreateRenderTarget(RenderSystem rs, PresentParameters pm)
        {
            D3D9RenderSystem drs = rs as D3D9RenderSystem;
            if (drs == null)
            {
                throw new InvalidOperationException();
            }
            return new D3D9RenderTarget(drs, pm.BackBufferWidth, pm.BackBufferHeight, pm.BackBufferFormat, pm.DepthFormat);
        }

        public override void Present()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Apoc3D.Graphics.D3D9
{
    internal sealed class D3D9RenderWindow : RenderWindow
    {
        public D3D9RenderWindow(D3D9RenderSystem rs, Form form, PresentParameters pm, D3D9RenderTarget rt)
            : base(rs, form, pm, rt)
        {

        }
        public D3D9RenderWindow(D3D9RenderSystem rs, PresentParameters pm)
            : base(rs, pm)
        {

        }

        protected override RenderTarget CreateRenderTarget(RenderSystem rs, PresentParameters pm)
        {
            return new D3D9RenderTarget((D3D9RenderSystem)rs, pm.BackBufferWidth, pm.BackBufferHeight, pm.BackBufferFormat, pm.DepthFormat);
        }
    }
}

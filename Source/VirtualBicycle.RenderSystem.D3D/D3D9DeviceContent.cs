using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Collections;
using SD = System.Drawing;
using D3D = SlimDX.Direct3D9;

namespace VirtualBicycle.RenderSystem.D3D9
{
    internal sealed class D3D9DeviceContent : DeviceContent
    {
        Direct3D d3d;
        Device device;

        D3D9RenderSystem renderSystem;


        public D3D9DeviceContent()
            : base(true)
        {
            d3d = new Direct3D();
        }

        public override RenderSystem RenderSystem
        {
            get { return renderSystem; }
        }

        protected override RenderControl create(PresentParameters pm)
        {
            RenderControl result;
            if (renderSystem == null)
            {
                D3D.PresentParameters dpm = new D3D.PresentParameters();
                dpm.Windowed = pm.IsWindowed;
                dpm.AutoDepthStencilFormat = D3D9Utils.ConvertEnum(pm.DepthFormat);
                dpm.BackBufferFormat = D3D9Utils.ConvertEnum(pm.BackBufferFormat);
                dpm.BackBufferHeight = pm.BackBufferHeight;
                dpm.BackBufferWidth = pm.BackBufferWidth;
                dpm.EnableAutoDepthStencil = true;
                dpm.SwapEffect = D3D.SwapEffect.Flip;

                // 创建一个RenderWindow
                if (pm.IsFullForm)
                {
                    RenderWindow.RenderForm form =
                        new RenderWindow.RenderForm(new SD.Size(pm.BackBufferWidth, pm.BackBufferHeight));

                    device = new Device(d3d, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, dpm);
                    renderSystem = new D3D9RenderSystem(d3d, device);

                    D3D9RenderTarget defRt =
                        new D3D9RenderTarget(renderSystem,
                            device.GetBackBuffer(0, 0), device.DepthStencilSurface,
                            pm.BackBufferWidth, pm.BackBufferHeight,
                            pm.BackBufferFormat, pm.DepthFormat);
                    D3D9RenderWindow wnd = new D3D9RenderWindow(renderSystem, form, pm, defRt);

                    result = wnd;
                }
                else
                {

                    device = new Device(d3d, 0, DeviceType.Hardware, pm.Control.Handle, CreateFlags.HardwareVertexProcessing, dpm);
                    renderSystem = new D3D9RenderSystem(d3d, device);

                    D3D9RenderTarget defRt =
                        new D3D9RenderTarget(renderSystem,
                            device.GetBackBuffer(0, 0), device.DepthStencilSurface,
                            pm.BackBufferWidth, pm.BackBufferHeight,
                            pm.BackBufferFormat, pm.DepthFormat);

                    D3D9RenderControl ctl = new D3D9RenderControl(renderSystem, pm, defRt);
                    result = ctl;
                }

                renderSystem.Init();
            }
            else
            {
                if (pm.IsFullForm)
                {
                    result = new D3D9RenderWindow(renderSystem, pm);
                }
                else
                {
                    result = new D3D9RenderControl(renderSystem, pm);
                }
            }
            return result;

        }
    }
}

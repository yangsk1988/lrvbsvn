using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Collections;

namespace VirtualBicycle.RenderSystem.D3D9
{
    internal sealed class D3D9GraphicsAPIFactory : GraphicsAPIFactory
    {
        internal static readonly string APIName = "Direct3D9";

        static GraphicsAPIDescription desc;

        static D3D9GraphicsAPIFactory()
        {
            PlatformAPISupport[] platforms = new PlatformAPISupport[1];
            platforms[0] = new PlatformAPISupport(100, "Windows");
            desc = new GraphicsAPIDescription(APIName, new PlatformCollection(platforms));
        }

        public D3D9GraphicsAPIFactory()
            : base(desc)
        {

        }
        public override DeviceContent CreateDeviceContent()
        {
            return new D3D9DeviceContent();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.PluginManager;

namespace VirtualBicycle.RenderSystem.D3D9
{
    public class Plugin : IPlugin
    {

        #region IPlugin 成员

        public void Load()
        {
            GraphicsAPIManager.Instance.RegisterGraphicsAPI(new D3D9GraphicsAPIFactory());
        }

        public void Unload()
        {
            GraphicsAPIManager.Instance.UnregisterGraphicsAPI(D3D9GraphicsAPIFactory.APIName);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using VirtualBicycle.Ide;
using VirtualBicycle;

namespace Plugin.ModelTools
{
    public class Plugin : IPlugin
    {
        #region IPlugin 成员


        public void Load()
        {
            GraphicsDevice.Initialize(Program.MainForm);

            Engine.Initialize(GraphicsDevice.Instance.Device);

            ConverterManager.Instance.Register(new XText2ModelConverter());

            ConverterManager.Instance.Register(new Xml2ModelConverter());
            ConverterManager.Instance.Register(new Xml2ModelConverter2());
        }

        public void Unload()
        {
            DesignerManager.Instance.RegisterDesigner(new ModelDesignerFactory());
        }

        public string Name
        {
            get { return "Model Plugin"; }
        }

        public Icon PluginIcon
        {
            get { return null; }
        }

        public bool IsListed
        {
            get { return true; }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Apoc3D;
using Apoc3D.Ide;

namespace Plugin.DXBased
{
    public class Plugin : IPlugin
    {
        #region IPlugin 成员


        public void Load()
        {
            GraphicsDevice.Initialize(Program.MainForm);

            //Engine.Initialize(GraphicsDevice.Instance.Device);

            DesignerManager.Instance.RegisterDesigner(new ModelDesignerFactory());

            ConverterManager.Instance.Register(new XText2ModelConverter());
        }

        public void Unload()
        {
            DesignerManager.Instance.RegisterDesigner(new ModelDesignerFactory());
        }

        public string Name
        {
            get { return "DX Based Plugin"; }
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

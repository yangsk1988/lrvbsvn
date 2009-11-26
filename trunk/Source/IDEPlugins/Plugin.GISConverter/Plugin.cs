using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Ide;
using System.Drawing;

namespace Plugin.GISConverter
{
    public class Plugin : IPlugin
    {
        #region IPlugin 成员

        DemConverter demConverter;

        public void Load()
        {
            demConverter = new DemConverter();
            ConverterManager.Instance.Register(demConverter);
        }

        public void Unload()
        {
            ConverterManager.Instance.Unregister(demConverter);
        }

        public string Name
        {
            get { return "GIS Converter"; }
        }

        public Icon PluginIcon
        {
            get { return Properties.Resources.PluginIco; }
        }

        public bool IsListed
        {
            get { return true; }
        }

        #endregion
    }
}

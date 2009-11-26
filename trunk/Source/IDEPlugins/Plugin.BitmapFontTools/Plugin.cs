using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using VirtualBicycle.Ide;

namespace Plugin.BitmapFont
{
    public class Plugin : IPlugin
    {
        #region IPlugin 成员

        FontConverter demConverter;

        public void Load()
        {
            demConverter = new FontConverter();
            ConverterManager.Instance.Register(demConverter);
        }

        public void Unload()
        {
            ConverterManager.Instance.Unregister(demConverter);
        }

        public string Name
        {
            get { return "Bitmap Font Tools"; }
        }

        public Icon PluginIcon
        {
            get { return Properties.Resources.font; }
        }

        public bool IsListed
        {
            get { return true; }
        }

        #endregion
    }
}

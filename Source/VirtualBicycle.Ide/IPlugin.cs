using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace VBIDE
{
    public interface IPlugin
    {
        void Load();
        void Unload();

        string Name
        {
            get;
        }

        Icon PluginIcon
        {
            get;
        }

        bool IsListed
        {
            get;
        }
    }
}

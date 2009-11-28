using System;
using System.Collections.Generic;
using System.Text;

namespace Apoc3D.Ide
{
    public static class Paths
    {
        static readonly string pluginPath = "Plugins\\";
        static readonly string configPath = "Config\\";

        public static string PluginPath
        {
            get { return pluginPath; }
        }

        public static string ConfigPath
        {
            get { return configPath; }
        }
    }
}

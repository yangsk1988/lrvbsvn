using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Apoc3D;
using Apoc3D.Config;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Apoc3D.Graphics
{
    public class TextureLibrary : Singleton
    {
        static TextureLibrary singleton;


        public static TextureLibrary Instance
        {
            get { return singleton; }
        }

        public static void Initialize(RenderSystem device)
        {
            singleton = new TextureLibrary(device);
        }

        protected struct Entry
        {
            public ResourceHandle<Texture> colorMap;
            public ResourceHandle<Texture> normalMap;

            public string name;
        }

        RenderSystem device;
        Dictionary<string, Entry> detailedMaps;

        static string defaultMap = "default";

        public static string DefaultMap
        {
            get { return defaultMap; }
            private set { defaultMap = value; }
        }

        public void LoadTextureSet(FileLocation configLoc)
        {
            Configuration config = ConfigurationManager.Instance.CreateInstance(configLoc);

            ConfigurationSection sect = config["DetailedMapsList"];

            ConfigurationSection.ValueCollection entries = sect.Values;

            foreach (string s in entries)
            {
                Entry entry;
                ConfigurationSection texSect = config[s];

                string fileName = texSect["ColorMap"];
                FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.Textures, fileName), FileLocateRules.Default);

                entry.colorMap = TextureManager.Instance.CreateInstance(fl);

                fileName = texSect["NormalMap"];
                fl = FileSystem.Instance.Locate(Path.Combine(Paths.Textures, fileName), FileLocateRules.Default);

                entry.normalMap = TextureManager.Instance.CreateInstance(fl);

                entry.name = s;

                detailedMaps.Add(s, entry);
            }

            string msg = "细节纹理库初始化完毕。加载了{0}种纹理。";

            EngineConsole.Instance.Write(string.Format(msg, detailedMaps.Count.ToString()), ConsoleMessageType.Information);
        }

        private TextureLibrary(RenderSystem device)
        {
            this.device = device;
            this.detailedMaps = new Dictionary<string, Entry>(CaseInsensitiveStringComparer.Instance);
        }

        public Texture GetColorMap(string name)
        {
            return detailedMaps[name].colorMap;
        }

        public Texture GetNormalMap(string name)
        {
            return detailedMaps[name].normalMap;
        }

        public string[] GetNames()
        {
            string[] result = new string[detailedMaps.Count];

            int index = 0;
            foreach (KeyValuePair<string, Entry> e in detailedMaps)
            {
                result[index++] = e.Key;
            }
            return result;
        }

        protected override void dispose()
        {
            foreach (KeyValuePair<string, Entry> e in detailedMaps)
            {
                ResourceHandle<Texture> tex = e.Value.colorMap;
                tex.Dispose();

                tex = e.Value.normalMap;
                tex.Dispose();
            }
            detailedMaps.Clear();

            singleton = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using VirtualBicycle.IO;

namespace VirtualBicycle.Config
{
    public abstract class ConfigurationFormat
    {
        public abstract string[] Filters { get; }

        public abstract Configuration Load(ResourceLocation rl);

        public Configuration Load(string file)
        {
            return Load(new FileLocation(file));
        }
    }
    public class ConfigurationManager : Singleton//<ConfigurationManager>
    {
        static ConfigurationManager singleton;

        public static ConfigurationManager Instance
        {
            get
            {
                return singleton;
            }
        }

        public static void Initialize()
        {
            if (singleton == null)
            {
                singleton = new ConfigurationManager();
            }
        }

        Dictionary<string, ConfigurationFormat> formats = new Dictionary<string, ConfigurationFormat>(CaseInsensitiveStringComparer.Instance);

        private ConfigurationManager() { }


        public void Register(ConfigurationFormat fmt)
        {
            string[] exts = fmt.Filters;
            for (int i = 0; i < exts.Length; i++)
            {
                formats.Add(exts[i], fmt);
            }
        }
        public Configuration CreateInstance(FileLocation  fl)
        {
            string ext = Path.GetExtension(fl.Path);

            ConfigurationFormat fmt;
            if (formats.TryGetValue(ext, out fmt))
            {
                return fmt.Load(fl);
            }
            throw new NotSupportedException(ext);
        }
        public Configuration CreateInstance(string file)
        {
            string ext = Path.GetExtension(file);

            ConfigurationFormat fmt;
            if (formats.TryGetValue(ext, out fmt))
            {
                return fmt.Load(file);
            }
            throw new NotSupportedException(ext);
        }
        public Configuration CreateInstance(ResourceLocation rl, string type)
        {           
            ConfigurationFormat fmt;
            if (formats.TryGetValue(type, out fmt))
            {
                return fmt.Load(rl);
            }
            throw new NotSupportedException(type);
        }

        protected override void dispose()
        {
        }
    }
}

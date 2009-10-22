using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VirtualBicycle.IO;

namespace VirtualBicycle.Media
{
    public class ImageManager : Singleton
    {
        static ImageManager singleton;

        public static ImageManager Instance
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
                singleton = new ImageManager();
            }
        }

        Dictionary<string, ImageFormat> factories;

        private ImageManager()
        {
            factories = new Dictionary<string, ImageFormat>(CaseInsensitiveStringComparer.Instance);
        }

        public void RegisterImageFormat(ImageFormat format)
        {
            string[] exts = format.Filters;
            for (int i = 0; i < exts.Length; i++)
            {
                factories.Add(exts[i], format);
            }
        }
        public void UnregisterImageFormat(ImageFormat format)
        {
            string[] exts = format.Filters;
            for (int i = 0; i < exts.Length; i++)
            {
                factories.Remove(exts[i]);
            }
        }
        public void UnregisterImageFormat(params string[] exts)
        {
            for (int i = 0; i < exts.Length; i++)
            {
                factories.Remove(exts[i]);
            }
        }

        public ImageLoader CreateInstance(string file)
        {
            string ext = Path.GetExtension(file);
            ImageFormat fac;
            if (factories.TryGetValue(ext, out fac))
            {
                return new ImageLoader(fac, file);
            }
            throw new NotSupportedException(ext);
        }

        public ImageLoader CreateInstance(FileLocation fl)
        {
            string ext = Path.GetExtension(fl.Path);
            ImageFormat fac;
            if (factories.TryGetValue(ext, out fac))
            {
                return new ImageLoader(fac, fl);
            }
            throw new NotSupportedException(ext);
        }

        public ImageLoader CreateInstance(ResourceLocation rl, string type)
        {
            ImageFormat fac;
            if (factories.TryGetValue(type, out fac))
            {
                return new ImageLoader(fac, rl);
            }
            throw new NotSupportedException(type);
        }

        public bool Suuports(FileLocation fl)
        {
            string ext = Path.GetExtension(fl.Path);
            return factories.ContainsKey(ext);
        }
        public bool Supports(string file)
        {
            string ext = Path.GetExtension(file);
            return factories.ContainsKey(ext);
        }

        protected override void dispose()
        {
            factories.Clear();
            factories = null;
        }
    }
}

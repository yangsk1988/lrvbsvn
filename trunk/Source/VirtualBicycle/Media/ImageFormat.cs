using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.IO;

namespace VirtualBicycle.Media
{
    //public interface IAnimFactory : IAbstractFactory<AnimBase, ResourceLocation>
    //{
    //    string[] Filters { get; }
    //}

    public abstract class ImageFormat
    {
        //ImageBase CreateInstance(string file);
        //ImageBase CreateInstance(FileLocation fl);

        //string Type { get; }

        public abstract Image LoadImage(ResourceLocation rl);
        public abstract Image LoadImage(string file);

        //public abstract ImageBase LoadImageUnmanaged(ResourceLocation fl);

        public abstract string[] Filters
        {
            get;
        }
        public abstract string Type
        {
            get;
        }
    }

}

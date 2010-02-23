using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.IO;

namespace VirtualBicycle.Media
{
    public class ImageLoader
    {
        ResourceLocation resLoc;

        ImageFormat format;

        public ImageLoader(ImageFormat format, ResourceLocation rl)
        {
            this.resLoc = rl;
            this.format = format;
        }
        public ImageLoader(ImageFormat format, string file)
        {
            this.resLoc = new FileLocation(file);
            this.format = format;
        }

        public Image Load()
        {
            return format.LoadImage(resLoc);
        }
        public int FrameIndex
        {
            get;
            set;
        }


        public override int GetHashCode()
        {
            return resLoc.GetHashCode();
        }
    }
}

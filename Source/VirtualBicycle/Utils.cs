using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace VirtualBicycle
{
    public unsafe static class Utils
    {
        public const int D3DFVF_TEXTUREFORMAT2 = 0;         // Two floating point values
        public const int D3DFVF_TEXTUREFORMAT1 = 3;         // One floating point value
        public const int D3DFVF_TEXTUREFORMAT3 = 1;         // Three floating point values
        public const int D3DFVF_TEXTUREFORMAT4 = 2;         // Four floating point values

        static Random randomizer = new Random();

        public static int GetTexCoordSize3Format(int coordIndex)
        {
            return D3DFVF_TEXTUREFORMAT3 << (coordIndex * 2 + 16);
        }

        public static int GetTexCoordSize2Format(int coordIndex)
        {
            return D3DFVF_TEXTUREFORMAT2;
        }
        public static int GetTexCoordSize1Format(int coordIndex)
        {
            return D3DFVF_TEXTUREFORMAT1 << (coordIndex * 2 + 16);
        }
        public static int GetTexCoordSize4Format(int coordIndex)
        {
            return D3DFVF_TEXTUREFORMAT4 << (coordIndex * 2 + 16);
        }

        static readonly string tempDir = "Temp";

        public static string TempDir 
        {
            get { return tempDir; }
        }

        public static string GetTempFileName()
        {
            string fn;
            do
            {
                fn = Path.Combine(tempDir, "tmp" + randomizer.Next().ToString() + ".tmp");
            }
            while (File.Exists(fn));

            return fn;
        }


        public static string[] EmptyStringArray
        {
            get;
            private set;
        }
        static Utils()
        {
            EmptyStringArray = new string[0];
        }


        public static Texture Bitmap2Texture(Device dev, Bitmap bmp, Usage usage, Pool pool)
        {
            Texture res = new Texture(dev, bmp.Width, bmp.Height, 1, usage, Format.A8R8G8B8, pool);
            DataRectangle rect = res.LockRectangle(0, LockFlags.None);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Memory.Copy(data.Scan0.ToPointer(), rect.Data.DataPointer.ToPointer(), 4 * bmp.Width * bmp.Height);

            bmp.UnlockBits(data);

            res.UnlockRectangle(0);
            return res;
        }
    }
}

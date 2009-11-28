using System;
using System.Collections.Generic;
using System.Text;

namespace Apoc3D.Media
{
    public enum DepthFormat
    {
        Depth15Stencil1 = 0,
        Depth16 = 1,
        Depth16Lockable = 2,
        Depth24X8 = 3,
        Depth24Stencil4 = 4,
        Depth24Stencil8 = 5,
        Depth24Stencil8Single = 6,
        Depth32 = 7,
        Depth32Lockable = 8,
        Depth32Single = 9,
        Count = 10
    }

    /// <summary>
    ///    The pixel format used for images.
    /// </summary>
    public enum ImagePixelFormat
    {
        /// <summary>
        ///    Unknown pixel format.
        /// </summary>
        Unknown = 0,
        /// <summary>
        ///    8-bit pixel format, all bits luminance.
        /// </summary>
        Luminance8 = 1,
        /// <summary>
        ///    8-bit pixel format, all bits alpha.
        /// </summary>
        Alpha8 = 3,

        /// <summary>
        ///    8-bit pixel format, 4 bits alpha, 4 bits luminance.
        /// </summary>
        A4L4 = 4,

        /// <summary>
        ///    8-bit pixel format, 4 bits luminace, 4 bits alpha.
        /// </summary>
        //L4A4 = 5,

        /// <summary>
        ///   8-bit pixel format, 3 bits red, 3 bits green, 2 bits blue.
        /// </summary>
        R3G3B2 = 31,

        /// <summary>
        ///    16-bit pixel format, 5 bits red, 6 bits green, 5 bits blue.
        /// </summary>
        R5G6B5 = 6,

        /// <summary>
        ///    16-bit pixel format, 5 bits blue, 6 bits green, 5 bits red.
        /// </summary>
        B5G6R5 = 7,

        /// <summary>
        ///    16-bit pixel format, 4 bits for alpha, red, green and blue.
        /// </summary>
        A4R4G4B4 = 8,

        /// <summary>
        ///    16-bit pixel format, 4 bits for blue, green, red and alpha.
        /// </summary>
        B4G4R4A4,

        /// <summary>
        ///    16-bit pixel format, 8 bits for alpha, 8 bits for luminance.
        /// </summary>
        A8L8 = 5,

        /// <summary>
        ///    16-bit pixel format, all bits luminance.
        /// </summary>
        Luminance16 = 2,

        /// <summary>
        ///    16-bit pixel format, 1 bit for alpha, 5 bits for blue, green and red.
        /// </summary>
        A1R5G5B5 = 9,

        /// <summary>
        ///    24-bit pixel format, 8 bits for red, green and blue.
        /// </summary>
        R8G8B8 = 10,

        /// <summary>
        ///    24-bit pixel format, 8 bits for blue, green and red.
        /// </summary>
        B8G8R8 = 11,

        /// <summary>
        ///    24-bit pixel format, all bits luminance.
        /// </summary>
        //L24,

        /// <summary>
        ///    32-bit pixel format, 8 bits for alpha, red, green and blue.
        /// </summary>
        A8R8G8B8 = 12,

        /// <summary>
        ///    32-bit pixel format, 8 bits for alpha, blue, green and red`.
        /// </summary>
        A8B8G8R8 = 13,

        /// <summary>
        ///    32-bit pixel format, 8 bits for blue, green, red and alpha.
        /// </summary>
        B8G8R8A8 = 14,

        /// <summary>
        ///    32-bit pixel format, 8 bits for red, green, blue and alpha.
        /// </summary>
        R8G8B8A8 = 28,

        /// <summary>
        ///    32-bit pixel format, 8 bits for red, green and blue.
        ///    like PF_A8R8G8B8, but alpha will get discarded
        /// </summary>
        X8R8G8B8 = 26,

        /// <summary>
        ///    32-bit pixel format, 8 bits for blue, green and red.
        ///		like PF_A8R8G8B8, but alpha will get discarded
        /// </summary>
        X8B8G8R8 = 27,

        /// <summary>
        ///    32-bit pixel format, 2 bits for alpha, 10 bits for red, green and blue.
        /// </summary>
        A2R10G10B10 = 15,

        /// <summary>
        ///    32-bit pixel format, 10 bits for blue, green and red, 2 bits for alpha.
        /// </summary>
        A2B10G10R10 = 16,

        /// <summary>
        ///    DDS (DirectDraw Surface) DXT1 format.
        /// </summary>
        DXT1 = 17,

        /// <summary>
        ///    DDS (DirectDraw Surface) DXT2 format.
        /// </summary>
        DXT2 = 18,

        /// <summary>
        ///    DDS (DirectDraw Surface) DXT3 format.
        /// </summary>
        DXT3 = 19,

        /// <summary>
        ///    DDS (DirectDraw Surface) DXT4 format.
        /// </summary>
        DXT4 = 20,

        /// <summary>
        ///    DDS (DirectDraw Surface) DXT5 format.
        /// </summary>
        DXT5 = 21,

        /// <summary>
        ///		Depth texture format
        /// </summary>
        Depth = 29,

        /// <summary>
        ///    64-bit pixel ABGR format using 16 bits for each channel.
        /// </summary>
        A16B16G16R16 = 30,

        /// <summary>
        ///    16 bit floating point with a single channel (red)
        /// </summary>
        R16F = 32,

        /// <summary>
        ///    48-bit pixel format, 16 bits (float) for red, 16 bits (float) for green, 16 bits (float) for blue
        /// </summary>
        R16G16B16F = 22,

        /// <summary>
        /// 64-bit floating point format using 16 bits for each channel (alpha, blue, green, red).
        /// </summary>
        A16B16G16R16F = 23,

        /// <summary>
        ///		32 bit floating point with a single channel (red)
        /// </summary>
        R32F = 33,

        /// <summary>
        ///    96-bit pixel format, 32 bits (float) for red, 32 bits (float) for green, 32 bits (float) for blue
        /// </summary>
        R32G32B32F = 24,

        /// <summary>
        ///    128-bit floating point format using 32 bits for each channel (alpha, blue,  green, red).
        /// </summary>
        A32B32G32R32F = 25,

        /// <summary>
        ///		32-bit pixel format, 2-channel floating point pixel format, 16 bits (float) for green, 16 bits (float) for red
        /// </summary>
        G16R16F = 35,

        /// <summary>
        ///		64-bit pixel format, 2-channel floating point pixel format, 32 bits (float) for green, 32 bits (float) for red
        /// </summary>
        G32R32F = 36,

        /// <summary>
        /// 32-bit pixel format, 16-bit green, 16-bit red
        /// </summary>
        G16R16 = 34,

        /// <summary>
        /// 48-bit pixel format, 16 bits for red, green and blue
        /// </summary>
        R16G16B16 = 37,

        Count = 38
    }

    public enum ImageType
    {
        Image1D = 0,
        Image2D,
        Image3D,
        CubeImage
    }

    public static class PixelFormat
    {
        static readonly int[] sizeTable = new int[(int)ImagePixelFormat.Count];
        static readonly int[] depSizeTable = new int[(int)DepthFormat.Count];


        static PixelFormat()
        {
            sizeTable[(int)ImagePixelFormat.Unknown] = -1;
            sizeTable[(int)ImagePixelFormat.A16B16G16R16] = 8;
            sizeTable[(int)ImagePixelFormat.A16B16G16R16F] = 8;
            sizeTable[(int)ImagePixelFormat.A1R5G5B5] = 2;
            sizeTable[(int)ImagePixelFormat.A2B10G10R10] = 4;
            sizeTable[(int)ImagePixelFormat.A2R10G10B10] = 4;
            sizeTable[(int)ImagePixelFormat.A32B32G32R32F] = 32;
            sizeTable[(int)ImagePixelFormat.A4L4] = 1;
            sizeTable[(int)ImagePixelFormat.A4R4G4B4] = 2;
            sizeTable[(int)ImagePixelFormat.A8B8G8R8] = 4;
            sizeTable[(int)ImagePixelFormat.A8L8] = 2;
            sizeTable[(int)ImagePixelFormat.A8R8G8B8] = 4;
            sizeTable[(int)ImagePixelFormat.Alpha8] = 1;
            sizeTable[(int)ImagePixelFormat.B4G4R4A4] = 2;
            sizeTable[(int)ImagePixelFormat.B5G6R5] = 2;
            sizeTable[(int)ImagePixelFormat.B8G8R8] = 3;
            sizeTable[(int)ImagePixelFormat.B8G8R8A8] = 4;
            sizeTable[(int)ImagePixelFormat.Depth] = -1;
            sizeTable[(int)ImagePixelFormat.DXT1] = -1;
            sizeTable[(int)ImagePixelFormat.DXT2] = -1;
            sizeTable[(int)ImagePixelFormat.DXT3] = -1;
            sizeTable[(int)ImagePixelFormat.DXT4] = -1;
            sizeTable[(int)ImagePixelFormat.DXT5] = -1;
            sizeTable[(int)ImagePixelFormat.G16R16] = 4;
            sizeTable[(int)ImagePixelFormat.G16R16F] = 4;
            sizeTable[(int)ImagePixelFormat.G32R32F] = 8;
            sizeTable[(int)ImagePixelFormat.Luminance16] = 2;
            sizeTable[(int)ImagePixelFormat.Luminance8] = 1;
            sizeTable[(int)ImagePixelFormat.R16F] = 1;
            sizeTable[(int)ImagePixelFormat.R16G16B16] = 6;
            sizeTable[(int)ImagePixelFormat.R16G16B16F] = 6;
            sizeTable[(int)ImagePixelFormat.R32F] = 4;
            sizeTable[(int)ImagePixelFormat.R32G32B32F] = 12;
            sizeTable[(int)ImagePixelFormat.R3G3B2] = 1;
            sizeTable[(int)ImagePixelFormat.R5G6B5] = 2;
            sizeTable[(int)ImagePixelFormat.R8G8B8] = 3;
            sizeTable[(int)ImagePixelFormat.R8G8B8A8] = 4;
            sizeTable[(int)ImagePixelFormat.X8B8G8R8] = 4;
            sizeTable[(int)ImagePixelFormat.X8R8G8B8] = 4;

            depSizeTable[(int)DepthFormat.Depth15Stencil1] = 2;
            depSizeTable[(int)DepthFormat.Depth16] = 2;
            depSizeTable[(int)DepthFormat.Depth16Lockable] = 2;
            depSizeTable[(int)DepthFormat.Depth24X8] = 4;
            depSizeTable[(int)DepthFormat.Depth24Stencil4] = 4;
            depSizeTable[(int)DepthFormat.Depth24Stencil8] = 4;
            depSizeTable[(int)DepthFormat.Depth24Stencil8Single] = 4;
            depSizeTable[(int)DepthFormat.Depth32] = 4;
            depSizeTable[(int)DepthFormat.Depth32Lockable] = 4;
            depSizeTable[(int)DepthFormat.Depth32Single] = 4;
        }

        public static bool Compressed(ImagePixelFormat format)
        {
            return (format == ImagePixelFormat.DXT1 ||
                    format == ImagePixelFormat.DXT2 ||
                    format == ImagePixelFormat.DXT3 ||
                    format == ImagePixelFormat.DXT4 ||
                    format == ImagePixelFormat.DXT5);
        }
        public static int GetMemorySize(int width, int height, DepthFormat format) 
        {
            int bytepp = depSizeTable[(int)format];
            if (bytepp == -1)
            {
                throw new ArgumentException("Invalid pixel format");
            }
            return width * height * bytepp;
        }

        public static int GetMemorySize(int width, int height, int depth, ImagePixelFormat format)
        {
            switch (format)
            {
                case ImagePixelFormat.DXT1:
                    //Debug.Assert(depth == 1);
                    return ((width * 3) / 4) * ((height * 3) / 4) * 8;
                case ImagePixelFormat.DXT2:
                case ImagePixelFormat.DXT3:
                case ImagePixelFormat.DXT4:
                case ImagePixelFormat.DXT5:
                    //Debug.Assert(depth == 1);
                    return ((width * 3) / 4) * ((height * 3) / 4) * 16;
                default:
                    int bytepp = sizeTable[(int)format];
                    if (bytepp == -1)
                    {
                        throw new ArgumentException("Invalid pixel format");
                    }
                    return width * height * depth * bytepp;

            }
        }
    }
}

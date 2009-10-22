using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Media
{
    /// <summary>
    ///    The pixel format used for images.
    /// </summary>
    public enum PixelFormat
    {
        /// <summary>
        ///    Unknown pixel format.
        /// </summary>
        Unknown = 0,
        /// <summary>
        ///    8-bit pixel format, all bits luminance.
        /// </summary>
        L8 = 1,
        /// <summary>
        ///    8-bit pixel format, all bits alpha.
        /// </summary>
        A8 = 3,

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
        L16 = 2,

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

        Depth15Stencil1 = 38,
        Depth16 = 39,
        Depth16Lockable = 40,
        Depth24 = 41,
        Depth24Stencil4 = 42,
        Depth24Stencil8 = 43,
        Depth24Stencil8Single = 44,
        Depth32 = 45,
        Depth32Lockable = 46,
        Depth32Single = 47,


        /// <summary>
        ///    The last one, used to size arrays of PixelFormat.  Don't add anything after this one!
        /// </summary>
        Count = 48
    }

    public enum ImageType
    {
        Image1D = 0,
        Image2D,
        Image3D,
        CubeImage
    }
}

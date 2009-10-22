using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace VirtualBicycle.Media
{
    ///<summary>
    ///    Class for manipulating bit patterns.
    ///</summary>
    public class Bitwise
    {

        ///<summary>
        ///    Returns the most significant bit set in a value.
        ///</summary>
        public static uint MostSignificantBitSet(uint value)
        {
            uint result = 0;
            while (value != 0)
            {
                ++result;
                value >>= 1;
            }
            return result - 1;
        }

        ///<summary>
        ///    Returns the closest power-of-two number greater or equal to value.
        ///</summary>
        ///<remarks>
        ///   0 and 1 are powers of two, so firstPO2From(0)==0 and firstPO2From(1)==1.
        ///</remarks>
        public static uint FirstPO2From(uint n)
        {
            --n;
            n |= n >> 16;
            n |= n >> 8;
            n |= n >> 4;
            n |= n >> 2;
            n |= n >> 1;
            ++n;
            return n;
        }

        ///<summary>
        ///    Convert N bit colour channel value to P bits. It fills P bits with the
        ///    bit pattern repeated. (this is /((1<<n)-1) in fixed point)
        ///</summary>
        public static uint FixedToFixed(uint value, int n, int p)
        {
            if (n > p)
                // Less bits required than available; this is easy
                value >>= n - p;
            else if (n < p)
            {
                // More bits required than are there, do the fill
                // Use old fashioned division, probably better than a loop
                if (value == 0)
                    value = 0;
                else if (value == ((uint)(1) << n) - 1)
                    value = (1u << p) - 1;
                else
                    value = value * (1u << p) / ((1u << n) - 1u);
            }
            return value;
        }

        ///<summary>
        ///    Convert N bit colour channel value to 8 bits, and return as a byte. It 
        ///    fills P bits with thebit pattern repeated. (this is /((1<<n)-1) in fixed point)
        ///</summary>
        public static byte FixedToByteFixed(uint value, int p)
        {
            return (byte)FixedToFixed(value, 8, p);
        }

        ///<summary>
        ///    Convert floating point colour channel value between 0.0 and 1.0 (otherwise clamped) 
        ///    to integer of a certain number of bits. Works for any value of bits between 0 and 31.
        ///</summary>
        public static uint FloatToFixed(float value, int bits)
        {
            if (value <= 0.0f)
                return 0;
            else if (value >= 1.0f)
                return (1u << bits) - 1;
            else
                return (uint)(value * (1u << bits));
        }

        ///<summary>
        ///    Convert floating point colour channel value between 0.0 and 1.0 (otherwise clamped) 
        ///    to an 8-bit integer, and return as a byte.
        ///</summary>
        public static byte FloatToByteFixed(float value)
        {
            return (byte)FloatToFixed(value, 8);
        }

        ///<summary>
        ///    Fixed point to float
        ///</summary>
        public static float FixedToFloat(uint value, int bits)
        {
            return (float)value / (float)((1u << bits) - 1);
        }

        /**
         * Write a n*8 bits integer value to memory in native endian.
         */
        unsafe public static void IntWrite(byte* dest, int n, uint value)
        {
            switch (n)
            {
                case 1:
                    ((byte*)dest)[0] = (byte)value;
                    break;
                case 2:
                    ((ushort*)dest)[0] = (ushort)value;
                    break;
                case 3:
                    if (BitConverter.IsLittleEndian)
                    {
                        ((byte*)dest)[2] = (byte)((value >> 16) & 0xFF);
                        ((byte*)dest)[1] = (byte)((value >> 8) & 0xFF);
                        ((byte*)dest)[0] = (byte)(value & 0xFF);
                    }
                    else
                    {
                        ((byte*)dest)[0] = (byte)((value >> 16) & 0xFF);
                        ((byte*)dest)[1] = (byte)((value >> 8) & 0xFF);
                        ((byte*)dest)[2] = (byte)(value & 0xFF);
                    }
                    break;
                case 4:
                    ((uint*)dest)[0] = (uint)value;
                    break;
            }
        }

        ///<summary>
        ///    Read a n*8 bits integer value to memory in native endian.
        ///</summary>
        unsafe public static uint IntRead(byte* src, int n)
        {
            switch (n)
            {
                case 1:
                    return ((byte*)src)[0];
                case 2:
                    return ((ushort*)src)[0];
                case 3:
                    if (BitConverter.IsLittleEndian)
                    {
                        return ((uint)((byte*)src)[0]) |
                                ((uint)((byte*)src)[1] << 8) |
                                ((uint)((byte*)src)[2] << 16);
                    }
                    return ((uint)((byte*)src)[0] << 16) |
                        ((uint)((byte*)src)[1] << 8) |
                        ((uint)((byte*)src)[2]);

                case 4:
                    return ((uint*)src)[0];
            }
            return 0; // ?
        }

        private static float[] floatConversionBuffer = new float[] { 0f };
        private static uint[] uintConversionBuffer = new uint[] { 0 };

        ///<summary>
        ///    Convert a float32 to a float16 (NV_half_float)
        ///    Courtesy of OpenEXR
        ///</summary>
        public static ushort FloatToHalf(float f)
        {
            uint i;
            floatConversionBuffer[0] = f;
            unsafe
            {
                fixed (float* pFloat = floatConversionBuffer)
                {
                    i = *((uint*)pFloat);
                }
            }
            return FloatToHalfI(i);
        }

        ///<summary>
        ///    Converts float in uint format to a a half in ushort format
        ///</summary>
        public static ushort FloatToHalfI(uint i)
        {
            int s = (int)(i >> 16) & 0x00008000;
            int e = (int)((i >> 23) & 0x000000ff) - (127 - 15);
            int m = (int)i & 0x007fffff;

            if (e <= 0)
            {
                if (e < -10)
                {
                    return 0;
                }
                m = (m | 0x00800000) >> (1 - e);

                return (ushort)(s | (m >> 13));
            }
            else if (e == 0xff - (127 - 15))
            {
                if (m == 0) // Inf
                {
                    return (ushort)(s | 0x7c00);
                }
                else    // NAN
                {
                    m >>= 13;
                    return (ushort)(s | 0x7c00 | m | (m == 0 ? 1 : 0));
                }
            }
            else
            {
                if (e > 30) // Overflow
                {
                    return (ushort)(s | 0x7c00);
                }

                return (ushort)(s | (e << 10) | (m >> 13));
            }
        }

        ///<summary>
        ///    Convert a float16 (NV_half_float) to a float32
        ///    Courtesy of OpenEXR
        ///</summary>
        public static float HalfToFloat(ushort y)
        {
            uintConversionBuffer[0] = HalfToFloatI(y);
            unsafe
            {
                fixed (uint* pUint = uintConversionBuffer)
                {
                    return *((float*)pUint);
                }
            }
        }

        ///<summary>
        ///    Converts a half in ushort format to a float
        ///    in uint format
        ///</summary>
        public static uint HalfToFloatI(ushort y)
        {
            uint yuint = (uint)y;
            uint s = (yuint >> 15) & 0x00000001;
            uint e = (yuint >> 10) & 0x0000001f;
            uint m = yuint & 0x000003ff;

            if (e == 0)
            {
                if (m == 0) // Plus or minus zero
                    return (s << 31);
                else
                { // Denormalized number -- renormalize it
                    while ((m & 0x00000400) == 0)
                    {
                        m <<= 1;
                        e -= 1;
                    }
                    e += 1;
                    m &= 0xFFFFFBFF; // ~0x00000400;
                }
            }
            else if (e == 31)
            {
                if (m == 0) // Inf
                    return (s << 31) | 0x7f800000;
                else // NaN
                    return (s << 31) | 0x7f800000 | (m << 13);
            }

            e = e + (127 - 15);
            m = m << 13;

            return (s << 31) | (e << 23) | m;
        }

    }

    /// <summary>
    /// Type for R8G8B8/B8G8R8
    /// </summary>
    public struct Col3b
    {
        public byte x, y, z;
        public Col3b(uint a, uint b, uint c)
        {
            x = (byte)a;
            y = (byte)b;
            z = (byte)c;
        }
    }

    /// <summary>
    /// Type for R32B32G32F
    /// </summary>
    public struct Col3f
    {
        public float r, g, b;
        public Col3f(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }

    /// <summary>
    /// Type for A32R32G32B32F
    /// </summary>
    public struct Col4f
    {
        public float a, r, g, b;
        public Col4f(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    ///<summary>
    ///    A class to convert/copy pixels of the same or different formats
    ///</summary>
    public class PixelConversionLoops
    {

        unsafe private static void A8R8G8B8toA8B8G8R8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x000000FF) << 16) | (inp & 0xFF00FF00) | ((inp & 0x00FF0000) >> 16);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void A8R8G8B8toB8G8R8A8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x000000FF) << 24) | ((inp & 0x0000FF00) << 8) | ((inp & 0x00FF0000) >> 8) | ((inp & 0xFF000000) >> 24);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void A8R8G8B8toR8G8B8A8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x00FFFFFF) << 8) | ((inp & 0xFF000000) >> 24);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void A8B8G8R8toA8R8G8B8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x000000FF) << 16) | (inp & 0xFF00FF00) | ((inp & 0x00FF0000) >> 16);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void A8B8G8R8toB8G8R8A8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x00FFFFFF) << 8) | ((inp & 0xFF000000) >> 24);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void A8B8G8R8toR8G8B8A8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x000000FF) << 24) | ((inp & 0x0000FF00) << 8) | ((inp & 0x00FF0000) >> 8) | ((inp & 0xFF000000) >> 24);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void B8G8R8A8toA8R8G8B8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x000000FF) << 24) | ((inp & 0x0000FF00) << 8) | ((inp & 0x00FF0000) >> 8) | ((inp & 0xFF000000) >> 24);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void B8G8R8A8toA8B8G8R8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x000000FF) << 24) | ((inp & 0xFFFFFF00) >> 8);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void B8G8R8A8toR8G8B8A8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x0000FF00) << 16) | (inp & 0x00FF00FF) | ((inp & 0xFF000000) >> 16);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void R8G8B8A8toA8B8G8R8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x000000FF) << 24) | ((inp & 0x0000FF00) << 8) | ((inp & 0x00FF0000) >> 8) | ((inp & 0xFF000000) >> 24);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void R8G8B8A8toB8G8R8A8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x0000FF00) << 16) | (inp & 0x00FF00FF) | ((inp & 0xFF000000) >> 16);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void A8B8G8R8toL8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            byte* dstptr = (byte*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = (byte)(inp & 0x000000FF);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void L8toA8B8G8R8(PixelBox src, PixelBox dst)
        {
            byte* srcptr = (byte*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        byte inp = srcptr[x];
                        dstptr[x] = 0xFF000000 | (((uint)inp) << 0) | (((uint)inp) << 8) | (((uint)inp) << 16);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void A8R8G8B8toL8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            byte* dstptr = (byte*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = (byte)((inp & 0x00FF0000) >> 16);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void L8toA8R8G8B8(PixelBox src, PixelBox dst)
        {
            byte* srcptr = (byte*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        byte inp = srcptr[x];
                        dstptr[x] = 0xFF000000 | (((uint)inp) << 0) | (((uint)inp) << 8) | (((uint)inp) << 16);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void B8G8R8A8toL8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            byte* dstptr = (byte*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = (byte)((inp & 0x0000FF00) >> 8);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void L8toB8G8R8A8(PixelBox src, PixelBox dst)
        {
            byte* srcptr = (byte*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        byte inp = srcptr[x];
                        dstptr[x] = 0x000000FF | (((uint)inp) << 8) | (((uint)inp) << 16) | (((uint)inp) << 24);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void L8toL16(PixelBox src, PixelBox dst)
        {
            byte* srcptr = (byte*)(src.Pointer.ToPointer());
            ushort* dstptr = (ushort*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        byte inp = srcptr[x];
                        dstptr[x] = (ushort)((((uint)inp) << 8) | (((uint)inp)));
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void L16toL8(PixelBox src, PixelBox dst)
        {
            ushort* srcptr = (ushort*)(src.Pointer.ToPointer());
            byte* dstptr = (byte*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        ushort inp = srcptr[x];
                        dstptr[x] = (byte)(inp >> 8);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void R8G8B8toB8G8R8(PixelBox src, PixelBox dst)
        {
            Col3b* srcptr = (Col3b*)(src.Pointer.ToPointer());
            Col3b* dstptr = (Col3b*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        Col3b inp = srcptr[x];
                        dstptr[x].x = inp.z;
                        dstptr[x].y = inp.y;
                        dstptr[x].z = inp.x;
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void B8G8R8toR8G8B8(PixelBox src, PixelBox dst)
        {
            Col3b* srcptr = (Col3b*)(src.Pointer.ToPointer());
            Col3b* dstptr = (Col3b*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        Col3b inp = srcptr[x];
                        dstptr[x].x = inp.z;
                        dstptr[x].y = inp.y;
                        dstptr[x].z = inp.x;
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void A8R8G8B8toR8G8B8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            Col3b* dstptr = (Col3b*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x].x = (byte)((inp >> 16) & 0xFF);
                        dstptr[x].y = (byte)((inp >> 8) & 0xFF);
                        dstptr[x].z = (byte)((inp >> 0) & 0xFF);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void A8R8G8B8toB8G8R8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            Col3b* dstptr = (Col3b*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x].x = (byte)((inp >> 0) & 0xFF);
                        dstptr[x].y = (byte)((inp >> 8) & 0xFF);
                        dstptr[x].z = (byte)((inp >> 16) & 0xFF);
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void X8R8G8B8toA8R8G8B8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = inp | 0xFF000000;
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void X8R8G8B8toA8B8G8R8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x0000FF) << 16) | ((inp & 0xFF0000) >> 16) | (inp & 0x00FF00) | 0xFF000000;
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void X8R8G8B8toB8G8R8A8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x0000FF) << 24) | ((inp & 0xFF0000) >> 8) | ((inp & 0x00FF00) << 8) | 0x000000FF;
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void X8R8G8B8toR8G8B8A8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0xFFFFFF) << 8) | 0x000000FF;
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void X8B8G8R8toA8R8G8B8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x0000FF) << 16) | ((inp & 0xFF0000) >> 16) | (inp & 0x00FF00) | 0xFF000000;
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void X8B8G8R8toA8B8G8R8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = inp | 0xFF000000;
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void X8B8G8R8toB8G8R8A8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0xFFFFFF) << 8) | 0x000000FF;
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void X8B8G8R8toR8G8B8A8(PixelBox src, PixelBox dst)
        {
            uint* srcptr = (uint*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        uint inp = srcptr[x];
                        dstptr[x] = ((inp & 0x0000FF) << 24) | ((inp & 0xFF0000) >> 8) | ((inp & 0x00FF00) << 8) | 0x000000FF;
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void R8G8B8toA8R8G8B8(PixelBox src, PixelBox dst)
        {
            Col3b* srcptr = (Col3b*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int xshift = 16;
            int yshift = 8;
            int zshift = 0;
            int ashift = 24;
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        Col3b inp = srcptr[x];
                        if (BitConverter.IsLittleEndian)
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << zshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << xshift);
                        }
                        else
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << xshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << zshift);
                        }
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void B8G8R8toA8R8G8B8(PixelBox src, PixelBox dst)
        {
            Col3b* srcptr = (Col3b*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int xshift = 0;
            int yshift = 8;
            int zshift = 16;
            int ashift = 24;
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        Col3b inp = srcptr[x];
                        if (BitConverter.IsLittleEndian)
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << zshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << xshift);
                        }
                        else
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << xshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << zshift);
                        }
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void R8G8B8toA8B8G8R8(PixelBox src, PixelBox dst)
        {
            Col3b* srcptr = (Col3b*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int xshift = 0;
            int yshift = 8;
            int zshift = 16;
            int ashift = 24;
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        Col3b inp = srcptr[x];
                        if (BitConverter.IsLittleEndian)
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << zshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << xshift);
                        }
                        else
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << xshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << zshift);
                        }
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void B8G8R8toA8B8G8R8(PixelBox src, PixelBox dst)
        {
            Col3b* srcptr = (Col3b*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int xshift = 8;
            int yshift = 16;
            int zshift = 24;
            int ashift = 0;
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        Col3b inp = srcptr[x];
                        if (BitConverter.IsLittleEndian)
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << zshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << xshift);
                        }
                        else
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << xshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << zshift);
                        }
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void R8G8B8toB8G8R8A8(PixelBox src, PixelBox dst)
        {
            Col3b* srcptr = (Col3b*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int xshift = 8;
            int yshift = 16;
            int zshift = 24;
            int ashift = 0;
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        Col3b inp = srcptr[x];
                        if (BitConverter.IsLittleEndian)
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << zshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << xshift);
                        }
                        else
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << xshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << zshift);
                        }
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        unsafe private static void B8G8R8toB8G8R8A8(PixelBox src, PixelBox dst)
        {
            Col3b* srcptr = (Col3b*)(src.Pointer.ToPointer());
            uint* dstptr = (uint*)(dst.Pointer.ToPointer());
            int xshift = 24;
            int yshift = 16;
            int zshift = 8;
            int ashift = 0;
            int srcSliceSkip = src.SlicePitch;
            int dstSliceSkip = dst.SlicePitch;
            int k = src.Width;
            for (int z = 0; z < src.Depth; z++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < k; x++)
                    {
                        Col3b inp = srcptr[x];
                        if (BitConverter.IsLittleEndian)
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << zshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << xshift);
                        }
                        else
                        {
                            dstptr[x] = ((uint)(0xFF << ashift)) | (((uint)inp.x) << xshift) | (((uint)inp.y) << yshift) | (((uint)inp.z) << zshift);
                        }
                    }
                    srcptr += src.RowPitch;
                    dstptr += dst.RowPitch;
                }
                srcptr += srcSliceSkip;
                dstptr += dstSliceSkip;
            }
        }

        public static bool DoOptimizedConversion(PixelBox src, PixelBox dst)
        {
            switch ((int)dst.Format << 8 + (int)src.Format)
            {
                case ((int)PixelFormat.A8R8G8B8 << 8) + (int)PixelFormat.A8B8G8R8:
                    A8R8G8B8toA8B8G8R8(src, dst);
                    break;
                case ((int)PixelFormat.A8R8G8B8 << 8) + (int)PixelFormat.B8G8R8A8:
                    A8R8G8B8toB8G8R8A8(src, dst);
                    break;
                case ((int)PixelFormat.A8R8G8B8 << 8) + (int)PixelFormat.R8G8B8A8:
                    A8R8G8B8toR8G8B8A8(src, dst);
                    break;
                case ((int)PixelFormat.A8B8G8R8 << 8) + (int)PixelFormat.A8R8G8B8:
                    A8B8G8R8toA8R8G8B8(src, dst);
                    break;
                case ((int)PixelFormat.A8B8G8R8 << 8) + (int)PixelFormat.B8G8R8A8:
                    A8B8G8R8toB8G8R8A8(src, dst);
                    break;
                case ((int)PixelFormat.A8B8G8R8 << 8) + (int)PixelFormat.R8G8B8A8:
                    A8B8G8R8toR8G8B8A8(src, dst);
                    break;
                case ((int)PixelFormat.B8G8R8A8 << 8) + (int)PixelFormat.A8R8G8B8:
                    B8G8R8A8toA8R8G8B8(src, dst);
                    break;
                case ((int)PixelFormat.B8G8R8A8 << 8) + (int)PixelFormat.A8B8G8R8:
                    B8G8R8A8toA8B8G8R8(src, dst);
                    break;
                case ((int)PixelFormat.B8G8R8A8 << 8) + (int)PixelFormat.R8G8B8A8:
                    B8G8R8A8toR8G8B8A8(src, dst);
                    break;
                //case ((int)PixelFormat.R8G8B8A8 << 8) + (int)PixelFormat.A8R8G8B8:
                //    R8G8B8A8toA8R8G8B8(src, dst);
                //    break;
                case ((int)PixelFormat.R8G8B8A8 << 8) + (int)PixelFormat.A8B8G8R8:
                    R8G8B8A8toA8B8G8R8(src, dst);
                    break;
                case ((int)PixelFormat.R8G8B8A8 << 8) + (int)PixelFormat.B8G8R8A8:
                    R8G8B8A8toB8G8R8A8(src, dst);
                    break;
                case ((int)PixelFormat.A8B8G8R8 << 8) + (int)PixelFormat.L8:
                    A8B8G8R8toL8(src, dst);
                    break;
                case ((int)PixelFormat.L8 << 8) + (int)PixelFormat.A8B8G8R8:
                    L8toA8B8G8R8(src, dst);
                    break;
                case ((int)PixelFormat.A8R8G8B8 << 8) + (int)PixelFormat.L8:
                    A8R8G8B8toL8(src, dst);
                    break;
                case ((int)PixelFormat.L8 << 8) + (int)PixelFormat.A8R8G8B8:
                    L8toA8R8G8B8(src, dst);
                    break;
                case ((int)PixelFormat.B8G8R8A8 << 8) + (int)PixelFormat.L8:
                    B8G8R8A8toL8(src, dst);
                    break;
                case ((int)PixelFormat.L8 << 8) + (int)PixelFormat.B8G8R8A8:
                    L8toB8G8R8A8(src, dst);
                    break;
                case ((int)PixelFormat.L8 << 8) + (int)PixelFormat.L16:
                    L8toL16(src, dst);
                    break;
                case ((int)PixelFormat.L16 << 8) + (int)PixelFormat.L8:
                    L16toL8(src, dst);
                    break;
                case ((int)PixelFormat.B8G8R8 << 8) + (int)PixelFormat.R8G8B8:
                    B8G8R8toR8G8B8(src, dst);
                    break;
                case ((int)PixelFormat.R8G8B8 << 8) + (int)PixelFormat.B8G8R8:
                    R8G8B8toB8G8R8(src, dst);
                    break;
                case ((int)PixelFormat.R8G8B8 << 8) + (int)PixelFormat.A8R8G8B8:
                    R8G8B8toA8R8G8B8(src, dst);
                    break;
                case ((int)PixelFormat.B8G8R8 << 8) + (int)PixelFormat.A8R8G8B8:
                    B8G8R8toA8R8G8B8(src, dst);
                    break;
                case ((int)PixelFormat.R8G8B8 << 8) + (int)PixelFormat.A8B8G8R8:
                    R8G8B8toA8B8G8R8(src, dst);
                    break;
                case ((int)PixelFormat.B8G8R8 << 8) + (int)PixelFormat.A8B8G8R8:
                    B8G8R8toA8B8G8R8(src, dst);
                    break;
                case ((int)PixelFormat.R8G8B8 << 8) + (int)PixelFormat.B8G8R8A8:
                    R8G8B8toB8G8R8A8(src, dst);
                    break;
                case ((int)PixelFormat.B8G8R8 << 8) + (int)PixelFormat.B8G8R8A8:
                    B8G8R8toB8G8R8A8(src, dst);
                    break;
                case ((int)PixelFormat.A8R8G8B8 << 8) + (int)PixelFormat.R8G8B8:
                    A8R8G8B8toR8G8B8(src, dst);
                    break;
                case ((int)PixelFormat.A8R8G8B8 << 8) + (int)PixelFormat.B8G8R8:
                    A8R8G8B8toB8G8R8(src, dst);
                    break;
                case ((int)PixelFormat.X8R8G8B8 << 8) + (int)PixelFormat.A8R8G8B8:
                    X8R8G8B8toA8R8G8B8(src, dst);
                    break;
                case ((int)PixelFormat.X8R8G8B8 << 8) + (int)PixelFormat.A8B8G8R8:
                    X8R8G8B8toA8B8G8R8(src, dst);
                    break;
                case ((int)PixelFormat.X8R8G8B8 << 8) + (int)PixelFormat.B8G8R8A8:
                    X8R8G8B8toB8G8R8A8(src, dst);
                    break;
                case ((int)PixelFormat.X8R8G8B8 << 8) + (int)PixelFormat.R8G8B8A8:
                    X8R8G8B8toR8G8B8A8(src, dst);
                    break;
                case ((int)PixelFormat.X8B8G8R8 << 8) + (int)PixelFormat.A8R8G8B8:
                    X8B8G8R8toA8R8G8B8(src, dst);
                    break;
                case ((int)PixelFormat.X8B8G8R8 << 8) + (int)PixelFormat.A8B8G8R8:
                    X8B8G8R8toA8B8G8R8(src, dst);
                    break;
                case ((int)PixelFormat.X8B8G8R8 << 8) + (int)PixelFormat.B8G8R8A8:
                    X8B8G8R8toB8G8R8A8(src, dst);
                    break;
                case ((int)PixelFormat.X8B8G8R8 << 8) + (int)PixelFormat.R8G8B8A8:
                    X8B8G8R8toR8G8B8A8(src, dst);
                    break;
                default:
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    ///    Flags defining some on/off properties of pixel formats
    /// </summary>
    public enum PixelFormatFlags
    {
        // No flags
        None = 0x00000000,
        // This format has an alpha channel
        HasAlpha = 0x00000001,
        // This format is compressed. This invalidates the values in elemBytes,
        // elemBits and the bit counts as these might not be fixed in a compressed format.
        Compressed = 0x00000002,
        // This is a floating point format
        Float = 0x00000004,
        // This is a depth format (for depth textures)
        Depth = 0x00000008,
        // Format is in native endian. Generally true for the 16, 24 and 32 bits
        // formats which can be represented as machine integers.
        NativeEndian = 0x00000010,
        // This is an intensity format instead of a RGB one. The luminance
        // replaces R,G and B. (but not A)
        Luminance = 0x00000020
    }

    /// <summary>
    ///    Pixel component format
    /// </summary>
    public enum PixelComponentType
    {
        Byte = 0,    /// Byte per component (8 bit fixed 0.0..1.0)
        Short = 1,   /// Short per component (16 bit fixed 0.0..1.0))
        Float16 = 2, /// 16 bit float per component
        Float32 = 3, /// 32 bit float per component
        Count = 4    /// Number of pixel types
    }


    ///<summary>
    ///    A class to convert/copy pixels of the same or different formats
    ///</summary>
    public class PixelConverter
    {
        #region Descriptors
        ///<summary>
        ///    A class to convert/copy pixels of the same or different formats
        ///</summary>
        public class PixelFormatDescription
        {

            #region Fields

            // Name of the format, as in the enum
            public string name;
            // The pixel format
            public PixelFormat format;
            // Number of bytes one element (color value) takes.
            public byte elemBytes;
            // Pixel format flags, see enum PixelFormatFlags for the bit field
            // definitions 
            public PixelFormatFlags flags;
            // Component type 
            public PixelComponentType componentType;
            // Component count
            public byte componentCount;
            // Number of bits for red(or luminance), green, blue, alpha
            public byte rbits, gbits, bbits, abits; /*, ibits, dbits, ... */
            // Masks and shifts as used by packers/unpackers */
            public uint rmask, gmask, bmask, amask;
            public byte rshift, gshift, bshift, ashift;

            #endregion Fields

            #region Constructor

            public PixelFormatDescription(string name,
                                          PixelFormat format,
                                          byte elemBytes,
                                          PixelFormatFlags flags,
                                          PixelComponentType componentType,
                                          byte componentCount,
                                          byte rbits,
                                          byte gbits,
                                          byte bbits,
                                          byte abits,
                                          uint rmask,
                                          uint gmask,
                                          uint bmask,
                                          uint amask,
                                          byte rshift,
                                          byte gshift,
                                          byte bshift,
                                          byte ashift)
            {
                this.name = name;
                this.format = format;
                this.elemBytes = elemBytes;
                this.flags = flags;
                this.componentType = componentType;
                this.componentCount = componentCount;
                this.rbits = rbits;
                this.gbits = gbits;
                this.bbits = bbits;
                this.abits = abits;
                this.rmask = rmask;
                this.gmask = gmask;
                this.bmask = bmask;
                this.amask = amask;
                this.rshift = rshift;
                this.gshift = gshift;
                this.bshift = bshift;
            }

            #endregion Constructor
        }


        ///<summary>
        ///    Pixel format database
        ///</summary>
        protected static PixelFormatDescription[] UnindexedPixelFormats = new PixelFormatDescription[] {
			new PixelFormatDescription(
			    "PF_UNKNOWN", 
				PixelFormat.Unknown,
				/* Bytes per element */ 
				0,  
				/* Flags */
				PixelFormatFlags.None,  
				/* Component type and count */
				PixelComponentType.Byte, 0,
				/* rbits, gbits, bbits, abits */
				0, 0, 0, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
  				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_L8",
				PixelFormat.L8,
				/* Bytes per element */ 
				1,  
				/* Flags */
				PixelFormatFlags.Luminance | PixelFormatFlags.NativeEndian,
				/* Component type and count */
				PixelComponentType.Byte, 1,
				/* rbits, gbits, bbits, abits */
				8, 0, 0, 0,
				/* Masks and shifts */
				0xFF, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"L16",
				PixelFormat.L16,
				/* Bytes per element */ 
				2,  
				/* Flags */
				PixelFormatFlags.Luminance | PixelFormatFlags.NativeEndian,  
				/* Component type and count */
				PixelComponentType.Short, 1,
				/* rbits, gbits, bbits, abits */
				16, 0, 0, 0,
				/* Masks and shifts */
				0xFFFF, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_A8",
				PixelFormat.A8,
				/* Bytes per element */ 
				1,  
				/* Flags */
				PixelFormatFlags.HasAlpha | PixelFormatFlags.NativeEndian,
				/* Component type and count */
				PixelComponentType.Byte, 1,
				/* rbits, gbits, bbits, abits */
				0, 0, 0, 8,
				/* Masks and shifts */
				0, 0, 0, 0xFF, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_A4L4",
				PixelFormat.A4L4,
				/* Bytes per element */ 
				1,  
				/* Flags */
				PixelFormatFlags.HasAlpha | PixelFormatFlags.Luminance | PixelFormatFlags.NativeEndian,
				/* Component type and count */
				PixelComponentType.Byte, 2,
				/* rbits, gbits, bbits, abits */
				4, 0, 0, 4,
				/* Masks and shifts */
				0x0F, 0, 0, 0xF0, 0, 0, 0, 4
				),
			//-----------------------------------------------------------------------
  			new PixelFormatDescription(
				 "PF_BYTE_LA",
				 PixelFormat.A8L8,
  				/* Bytes per element */ 
  				2,  
  				/* Flags */
  				PixelFormatFlags.HasAlpha | PixelFormatFlags.Luminance,  
  				/* Component type and count */
  				PixelComponentType.Byte, 2,
  				/* rbits, gbits, bbits, abits */
  				8, 0, 0, 8,
  				/* Masks and shifts */
  				0,0,0,0,0,0,0,0
  				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_R5G6B5",
				PixelFormat.R5G6B5,
				/* Bytes per element */ 
				2,  
				/* Flags */
				PixelFormatFlags.NativeEndian,  
				/* Component type and count */
				PixelComponentType.Byte, 3,
				/* rbits, gbits, bbits, abits */
				5, 6, 5, 0,
				/* Masks and shifts */
				0xF800, 0x07E0, 0x001F, 0, 
				11, 5, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_B5G6R5",
				PixelFormat.B5G6R5,
				/* Bytes per element */ 
				2,  
				/* Flags */
				PixelFormatFlags.NativeEndian,  
				/* Component type and count */
				PixelComponentType.Byte, 3,
				/* rbits, gbits, bbits, abits */
				5, 6, 5, 0,
				/* Masks and shifts */
				0x001F, 0x07E0, 0xF800, 0, 
				0, 5, 11, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_A4R4G4B4",
				PixelFormat.A4R4G4B4,
				/* Bytes per element */ 
				2,  
				/* Flags */
				PixelFormatFlags.HasAlpha | PixelFormatFlags.NativeEndian,  
				/* Component type and count */
				PixelComponentType.Byte, 4,
				/* rbits, gbits, bbits, abits */
				4, 4, 4, 4,
				/* Masks and shifts */
				0x0F00, 0x00F0, 0x000F, 0xF000, 
				8, 4, 0, 12 
				),
			//-----------------------------------------------------------------------
  			new PixelFormatDescription(
				 "PF_A1R5G5B5",
				 PixelFormat.A1R5G5B5,
  				/* Bytes per element */ 
  				2,  
  				/* Flags */
  				PixelFormatFlags.HasAlpha | PixelFormatFlags.NativeEndian,  
  				/* Component type and count */
  				PixelComponentType.Byte, 4,
  				/* rbits, gbits, bbits, abits */
  				5, 5, 5, 1,
  				/* Masks and shifts */
  				0x7C00, 0x03E0, 0x001F, 0x8000, 
  				10, 5, 0, 15
  				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_R8G8B8",
				PixelFormat.R8G8B8,
				/* Bytes per element */ 
				3,  // 24 bit integer -- special
				/* Flags */
				PixelFormatFlags.NativeEndian,  
				/* Component type and count */
				PixelComponentType.Byte, 3,
				/* rbits, gbits, bbits, abits */
				8, 8, 8, 0,
				/* Masks and shifts */
				0xFF0000, 0x00FF00, 0x0000FF, 0, 
				16, 8, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_B8G8R8",
				PixelFormat.B8G8R8,
				/* Bytes per element */ 
				3,  // 24 bit integer -- special
				/* Flags */
				PixelFormatFlags.NativeEndian,  
				/* Component type and count */
				PixelComponentType.Byte, 3,
				/* rbits, gbits, bbits, abits */
				8, 8, 8, 0,
				/* Masks and shifts */
				0x0000FF, 0x00FF00, 0xFF0000, 0, 
				0, 8, 16, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_A8R8G8B8",
				PixelFormat.A8R8G8B8,
				/* Bytes per element */ 
				4,  
				/* Flags */
				PixelFormatFlags.HasAlpha | PixelFormatFlags.NativeEndian,  
				/* Component type and count */
				PixelComponentType.Byte, 4,
				/* rbits, gbits, bbits, abits */
				8, 8, 8, 8,
				/* Masks and shifts */
				0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000,
				16, 8, 0, 24
				),
			//-----------------------------------------------------------------------
  			new PixelFormatDescription(
				 "PF_A8B8G8R8",
				 PixelFormat.A8B8G8R8,
  				/* Bytes per element */ 
  				4,  
  				/* Flags */
  				PixelFormatFlags.HasAlpha | PixelFormatFlags.NativeEndian,  
  				/* Component type and count */
  				PixelComponentType.Byte, 4,
  				/* rbits, gbits, bbits, abits */
  				8, 8, 8, 8,
  				/* Masks and shifts */
  				0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000,
  				0, 8, 16, 24
  				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_B8G8R8A8",
				PixelFormat.B8G8R8A8,
				/* Bytes per element */ 
				4,  
				/* Flags */
				PixelFormatFlags.HasAlpha | PixelFormatFlags.NativeEndian,  
				/* Component type and count */
				PixelComponentType.Byte, 4,
				/* rbits, gbits, bbits, abits */
				8, 8, 8, 8,
				/* Masks and shifts */
				0x0000FF00, 0x00FF0000, 0xFF000000, 0x000000FF,
				8, 16, 24, 0
				),
			//-----------------------------------------------------------------------
  			new PixelFormatDescription(
				 "PF_A2R10G10B10",
				 PixelFormat.A2R10G10B10,
  				/* Bytes per element */ 
  				4,  
  				/* Flags */
  				PixelFormatFlags.HasAlpha | PixelFormatFlags.NativeEndian,  
  				/* Component type and count */
  				PixelComponentType.Byte, 4,
  				/* rbits, gbits, bbits, abits */
  				10, 10, 10, 2,
  				/* Masks and shifts */
  				0x3FF00000, 0x000FFC00, 0x000003FF, 0xC0000000,
  				20, 10, 0, 30
  				),
 		    //-----------------------------------------------------------------------
  			new PixelFormatDescription(
				 "PF_A2B10G10R10",
				 PixelFormat.A2B10G10R10,
  				/* Bytes per element */ 
  				4,  
  				/* Flags */
  				PixelFormatFlags.HasAlpha | PixelFormatFlags.NativeEndian,  
  				/* Component type and count */
  				PixelComponentType.Byte, 4,
  				/* rbits, gbits, bbits, abits */
  				10, 10, 10, 2,
  				/* Masks and shifts */
  				0x000003FF, 0x000FFC00, 0x3FF00000, 0xC0000000,
  				0, 10, 20, 30
  				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_DXT1",
				PixelFormat.DXT1,
				/* Bytes per element */ 
				0,  
				/* Flags */
				PixelFormatFlags.Compressed | PixelFormatFlags.HasAlpha,  
				/* Component type and count */
				PixelComponentType.Byte, 3, // No alpha
				/* rbits, gbits, bbits, abits */
				0, 0, 0, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_DXT2",
				PixelFormat.DXT2,
				/* Bytes per element */ 
				0,  
				/* Flags */
				PixelFormatFlags.Compressed | PixelFormatFlags.HasAlpha,  
				/* Component type and count */
				PixelComponentType.Byte, 4,
				/* rbits, gbits, bbits, abits */
				0, 0, 0, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_DXT3",
				PixelFormat.DXT3,
				/* Bytes per element */ 
				0,  
				/* Flags */
				PixelFormatFlags.Compressed | PixelFormatFlags.HasAlpha,  
				/* Component type and count */
				PixelComponentType.Byte, 4,
				/* rbits, gbits, bbits, abits */
				0, 0, 0, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_DXT4",
				PixelFormat.DXT4,
				/* Bytes per element */ 
				0,  
				/* Flags */
				PixelFormatFlags.Compressed | PixelFormatFlags.HasAlpha,  
				/* Component type and count */
				PixelComponentType.Byte, 4,
				/* rbits, gbits, bbits, abits */
				0, 0, 0, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_DXT5",
				PixelFormat.DXT5,
				/* Bytes per element */ 
				0,  
				/* Flags */
				PixelFormatFlags.Compressed | PixelFormatFlags.HasAlpha,  
				/* Component type and count */
				PixelComponentType.Byte, 4,
				/* rbits, gbits, bbits, abits */
				0, 0, 0, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_R16G16B16F",
				PixelFormat.R16G16B16F,
				/* Bytes per element */ 
				6,  
				/* Flags */
				PixelFormatFlags.Float,  
				/* Component type and count */
				PixelComponentType.Float16, 3,
				/* rbits, gbits, bbits, abits */
				16, 16, 16, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_A16B16G16R16F",
				PixelFormat.A16B16G16R16F,
				/* Bytes per element */ 
				8,  
				/* Flags */
				PixelFormatFlags.Float,  
				/* Component type and count */
				PixelComponentType.Float16, 4,
				/* rbits, gbits, bbits, abits */
				16, 16, 16, 16,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_R32G32B32F",
				PixelFormat.R32G32B32F,
				/* Bytes per element */ 
				12,  
				/* Flags */
				PixelFormatFlags.Float,  
				/* Component type and count */
				PixelComponentType.Float32, 3,
				/* rbits, gbits, bbits, abits */
				32, 32, 32, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_A32B32G32R32F",
				PixelFormat.A32B32G32R32F,
				/* Bytes per element */ 
				16,  
				/* Flags */
				PixelFormatFlags.Float,  
				/* Component type and count */
				PixelComponentType.Float32, 4,
				/* rbits, gbits, bbits, abits */
				32, 32, 32, 32,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
  			new PixelFormatDescription(
				 "PF_X8R8G8B8",
				 PixelFormat.X8R8G8B8,
  				/* Bytes per element */ 
  				4,  
  				/* Flags */
  				PixelFormatFlags.NativeEndian,  
  				/* Component type and count */
  				PixelComponentType.Byte, 3,
  				/* rbits, gbits, bbits, abits */
  				8, 8, 8, 0,
  				/* Masks and shifts */
  				0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000,
  				16, 8, 0, 24
  				),
  			//-----------------------------------------------------------------------
  			new PixelFormatDescription(
				 "PF_X8B8G8R8",
				 PixelFormat.X8B8G8R8,
  				/* Bytes per element */ 
  				4,  
  				/* Flags */
  				PixelFormatFlags.NativeEndian,  
  				/* Component type and count */
  				PixelComponentType.Byte, 3,
  				/* rbits, gbits, bbits, abits */
  				8, 8, 8, 0,
  				/* Masks and shifts */
  				0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000,
  				0, 8, 16, 24
  				),
 			//-----------------------------------------------------------------------
  			new PixelFormatDescription(
				 "PF_R8G8B8A8",
				 PixelFormat.R8G8B8A8,
  				/* Bytes per element */ 
  				4,  
  				/* Flags */
  				PixelFormatFlags.HasAlpha | PixelFormatFlags.NativeEndian,  
  				/* Component type and count */
  				PixelComponentType.Byte, 4,
  				/* rbits, gbits, bbits, abits */
  				8, 8, 8, 8,
  				/* Masks and shifts */
  				0xFF000000, 0x00FF0000, 0x0000FF00, 0x000000FF,
  				24, 16, 8, 0
  				),
 			//-----------------------------------------------------------------------
	  		new PixelFormatDescription(
	             "PF_DEPTH",
	             PixelFormat.Depth,
	  			/* Bytes per element */ 
	  			4,  
	  			/* Flags */
	  			PixelFormatFlags.Depth, 
	  			/* Component type and count */
	  			PixelComponentType.Float32, 1, // ?
	  			/* rbits, gbits, bbits, abits */
	  			0, 0, 0, 0,
	  			/* Masks and shifts */
	  			0, 0, 0, 0, 0, 0, 0, 0
	  			),
            ////-----------------------------------------------------------------------
            new PixelFormatDescription(
                 "PF_A16B16G16R16",
                 PixelFormat.A16B16G16R16,
                /* Bytes per element */ 
                8,  
                /* Flags */
                PixelFormatFlags.HasAlpha,  
                /* Component type and count */
                PixelComponentType.Short, 4,
                /* rbits, gbits, bbits, abits */
                16, 16, 16, 16,
                /* Masks and shifts */
                0, 0, 0, 0, 0, 0, 0, 0
                ),
 			//-----------------------------------------------------------------------
  			new PixelFormatDescription(
				 "PF_R3G3B2",
				 PixelFormat.R3G3B2,
  				/* Bytes per element */ 
  				1,  
  				/* Flags */
  				PixelFormatFlags.NativeEndian,  
  				/* Component type and count */
  				PixelComponentType.Byte, 3,
  				/* rbits, gbits, bbits, abits */
  				3, 3, 2, 0,
  				/* Masks and shifts */
  				0xE0, 0x1C, 0x03, 0, 
  				5, 2, 0, 0 
  				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_R16F",
				PixelFormat.R16F,
				/* Bytes per element */ 
				2,  
				/* Flags */
				PixelFormatFlags.Float,  
				/* Component type and count */
				PixelComponentType.Float16, 1,
				/* rbits, gbits, bbits, abits */
				16, 0, 0, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
				),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_R32F",
				PixelFormat.R32F,
				/* Bytes per element */ 
				4,  
				/* Flags */
				PixelFormatFlags.Float,  
				/* Component type and count */
				PixelComponentType.Float32, 1,
				/* rbits, gbits, bbits, abits */
				32, 0, 0, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
 			    ),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_G16R16",
				PixelFormat.G16R16,
				/* Bytes per element */ 
				4,  
				/* Flags */
				PixelFormatFlags.NativeEndian,  
				/* Component type and count */
				PixelComponentType.Short, 2,
				/* rbits, gbits, bbits, abits */
				16, 16, 0, 0,
				/* Masks and shifts */
				0x0000FFFF, 0xFFFF0000, 0, 0, 0, 16, 0, 0 
 			    ),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_G16R16F",
				PixelFormat.G16R16F,
				/* Bytes per element */ 
				4,  
				/* Flags */
				PixelFormatFlags.Float,  
				/* Component type and count */
				PixelComponentType.Float16, 2,
				/* rbits, gbits, bbits, abits */
				16, 16, 0, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
 			    ),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_G32R32F",
				PixelFormat.G32R32F,
				/* Bytes per element */ 
				4,  
				/* Flags */
				PixelFormatFlags.Float,  
				/* Component type and count */
				PixelComponentType.Float32, 2,
				/* rbits, gbits, bbits, abits */
				32, 32, 0, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
 			    ),
			//-----------------------------------------------------------------------
			new PixelFormatDescription(
				"PF_R16G16B16",
				PixelFormat.R16G16B16,
				/* Bytes per element */ 
				6,  
				/* Flags */
				PixelFormatFlags.None,  
				/* Component type and count */
				PixelComponentType.Short, 3,
				/* rbits, gbits, bbits, abits */
				16, 16, 16, 0,
				/* Masks and shifts */
				0, 0, 0, 0, 0, 0, 0, 0 
 			    ),
			//-----------------------------------------------------------------------
            new PixelFormatDescription(
                "PF_D15S1", 
                PixelFormat.Depth15Stencil1, 
                2, 
                PixelFormatFlags.Depth, 
                PixelComponentType.Byte, 2, 
                0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0),
			//-----------------------------------------------------------------------
            new PixelFormatDescription(
                "PF_D16", 
                PixelFormat.Depth16, 
                2, 
                PixelFormatFlags.Depth, 
                PixelComponentType.Byte, 1,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0),
			//-----------------------------------------------------------------------
            new PixelFormatDescription(
                "PF_D16Lockable", 
                PixelFormat.Depth16Lockable,
                2,
                PixelFormatFlags.Depth,
                PixelComponentType.Byte, 1, 
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0),
			//-----------------------------------------------------------------------
            new PixelFormatDescription(
                "PF_D24X8", 
                PixelFormat.Depth24, 4,
                PixelFormatFlags.Depth, 
                PixelComponentType.Byte, 1,
                0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0),
			//-----------------------------------------------------------------------

            new PixelFormatDescription(
                "PF_D24S4X4", 
                PixelFormat.Depth24Stencil4,
                4,
                PixelFormatFlags.Depth,
                PixelComponentType.Byte, 3,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0),
			//-----------------------------------------------------------------------

            new PixelFormatDescription(
                "PF_D24S8", 
                PixelFormat.Depth24Stencil8,
                4,
                PixelFormatFlags.Depth,
                PixelComponentType.Byte, 2,
                0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0),
			//-----------------------------------------------------------------------

            new PixelFormatDescription(
                "PF_D24FS8", 
                PixelFormat.Depth24Stencil8Single,
                4,
                PixelFormatFlags.Depth,
                PixelComponentType.Float32, 2, 
                0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0),
			//-----------------------------------------------------------------------

            new PixelFormatDescription(
                "PF_D32", PixelFormat.Depth32, 
                4,
                PixelFormatFlags.Depth,
                PixelComponentType.Byte, 1,
                0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0),
			//-----------------------------------------------------------------------

            new PixelFormatDescription(
                "PF_D32Lockable",
                PixelFormat.Depth32Lockable, 
                4,
                PixelFormatFlags.Depth, 
                PixelComponentType.Byte, 1, 
                0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0),
			//-----------------------------------------------------------------------

            new PixelFormatDescription(
                "PF_D32F", 
                PixelFormat.Depth32Single, 
                4,
                PixelFormatFlags.Depth, 
                PixelComponentType.Float32, 1,
                0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0)
 		};
        #endregion
        protected static PixelFormatDescription[] IndexedPixelFormats = null;

        public static void Initialize()
        {
            if (IndexedPixelFormats != null)
                return;
            IndexedPixelFormats = new PixelFormatDescription[(int)PixelFormat.Count];
            foreach (PixelFormatDescription d in UnindexedPixelFormats)
            {
                IndexedPixelFormats[(int)d.format] = d;
            }


        }

        public static PixelFormatDescription GetDescriptionFor(PixelFormat format)
        {
            lock (UnindexedPixelFormats)
            {
                Initialize();
            }
            return IndexedPixelFormats[(int)format];
        }

        #region Static Bulk Conversion Methods

        ///*************************************************************************
        ///   Pixel packing/unpacking utilities
        ///*************************************************************************


        ///<summary>
        ///    Pack a color value to memory
        ///</summary>
        ///<param name="color">The color</param>
        ///<param name="format">Pixel format in which to write the color</param>
        ///<param name="dest">Destination memory location</param>
        public static void PackColor(Color color, PixelFormat format, IntPtr dest)
        {

            unsafe
            {
                PixelConverter.PackColor((uint)color.R, (uint)color.G, (uint)color.B, (uint)color.A, format, (byte*)(dest.ToPointer()));
            }
        }

        ///<summary>
        ///    Pack a color value to memory
        ///</summary>
        ///<param name="r,g,b,a">The four color components, range 0x00 to 0xFF</param>
        ///<param name="format">Pixelformat in which to write the color</param>
        ///<param name="dest">Destination memory location</param>
        unsafe public static void PackColor(uint r, uint g, uint b, uint a, PixelFormat format, byte* dest)
        {
            PixelFormatDescription des = GetDescriptionFor(format);
            if ((des.flags & PixelFormatFlags.NativeEndian) != 0)
            {
                // Shortcut for integer formats packing
                uint value = (((Bitwise.FixedToFixed(r, 8, des.rbits) << des.rshift) & des.rmask) |
                              ((Bitwise.FixedToFixed(g, 8, des.gbits) << des.gshift) & des.gmask) |
                              ((Bitwise.FixedToFixed(b, 8, des.bbits) << des.bshift) & des.bmask) |
                              ((Bitwise.FixedToFixed(a, 8, des.abits) << des.ashift) & des.amask));
                // And write to memory
                Bitwise.IntWrite(dest, des.elemBytes, value);
            }
            else
            {
                // Convert to float
                PackColor((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, (float)a / 255.0f, format, dest);
            }
        }

        ///<summary>
        ///    Pack a color value to memory
        ///</summary>
        ///<param name="r,g,b,a">
        ///    The four color components, range 0.0f to 1.0f
        ///    (an exception to this case exists for floating point pixel
        ///    formats, which don't clamp to 0.0f..1.0f)
        ///</param>
        ///<param name="format">Pixelformat in which to write the color</param>
        ///<param name="dest">Destination memory location</param>
        unsafe protected static void PackColor(float r, float g, float b, float a, PixelFormat format, byte* dest)
        {
            // Catch-it-all here
            PixelFormatDescription des = GetDescriptionFor(format);
            if ((des.flags & PixelFormatFlags.NativeEndian) != 0)
            {
                // Do the packing
                uint value = ((Bitwise.FloatToFixed(r, des.rbits) << des.rshift) & des.rmask) |
                    ((Bitwise.FloatToFixed(g, des.gbits) << des.gshift) & des.gmask) |
                    ((Bitwise.FloatToFixed(b, des.bbits) << des.bshift) & des.bmask) |
                    ((Bitwise.FloatToFixed(a, des.abits) << des.ashift) & des.amask);
                // And write to memory
                Bitwise.IntWrite(dest, des.elemBytes, value);
            }
            else
            {
                switch (format)
                {
                    case PixelFormat.R32F:
                        ((float*)dest)[0] = r;
                        break;
                    case PixelFormat.R32G32B32F:
                        ((float*)dest)[0] = r;
                        ((float*)dest)[1] = g;
                        ((float*)dest)[2] = b;
                        break;
                    case PixelFormat.A32B32G32R32F:
                        ((float*)dest)[0] = r;
                        ((float*)dest)[1] = g;
                        ((float*)dest)[2] = b;
                        ((float*)dest)[3] = a;
                        break;
                    case PixelFormat.R16F:
                        ((ushort*)dest)[0] = Bitwise.FloatToHalf(r);
                        break;
                    case PixelFormat.R16G16B16F:
                        ((ushort*)dest)[0] = Bitwise.FloatToHalf(r);
                        ((ushort*)dest)[1] = Bitwise.FloatToHalf(g);
                        ((ushort*)dest)[2] = Bitwise.FloatToHalf(b);
                        break;
                    case PixelFormat.A16B16G16R16F:
                        ((ushort*)dest)[0] = Bitwise.FloatToHalf(r);
                        ((ushort*)dest)[1] = Bitwise.FloatToHalf(g);
                        ((ushort*)dest)[2] = Bitwise.FloatToHalf(b);
                        ((ushort*)dest)[3] = Bitwise.FloatToHalf(a);
                        break;
                    //   				case PixelFormat.SHORT_RGBA:
                    //   					((ushort*)dest)[0] = Bitwise.FloatToFixed(r, 16);
                    //   					((ushort*)dest)[1] = Bitwise.FloatToFixed(g, 16);
                    //   					((ushort*)dest)[2] = Bitwise.FloatToFixed(b, 16);
                    //   					((ushort*)dest)[3] = Bitwise.FloatToFixed(a, 16);
                    //   					break;
                    //   				case PixelFormat.BYTE_LA:
                    //   					((byte*)dest)[0] = Bitwise.FloatToFixed(r, 8);
                    //   					((byte*)dest)[1] = Bitwise.FloatToFixed(a, 8);
                    //   					break;
                    default:
                        // Not yet supported
                        throw new Exception("Pack to " + format + " not implemented, in PixelUtil.PackColor");
                }
            }
        }

        //           /** Unpack a color value from memory
        //           	@param color	The color is returned here
        //           	@param pf		Pixelformat in which to read the color
        //           	@param src		Source memory location
        //           */
        //   		protected static void UnpackColor(ref System.Drawing.Color color, PixelFormat pf,  IntPtr src) {
        //   			UnpackColor(color.r, color.g, color.b, color.a, pf, src);
        //   		}

        /** Unpack a color value from memory
          @param r,g,b,a	The color is returned here (as byte)
          @param pf		Pixelformat in which to read the color
          @param src		Source memory location
          @remarks 	This function returns the color components in 8 bit precision,
              this will lose precision when coming from A2R10G10B10 or floating
              point formats.  
        */
        unsafe protected static void UnpackColor(ref byte r, ref byte g, ref byte b, ref byte a,
                                                 PixelFormat pf, byte* src)
        {
            PixelFormatDescription des = GetDescriptionFor(pf);
            if ((des.flags & PixelFormatFlags.NativeEndian) != 0)
            {
                // Shortcut for integer formats unpacking
                uint value = Bitwise.IntRead(src, des.elemBytes);
                if ((des.flags & PixelFormatFlags.Luminance) != 0)
                    // Luminance format -- only rbits used
                    r = g = b = (byte)Bitwise.FixedToFixed((value & des.rmask) >> des.rshift, des.rbits, 8);
                else
                {
                    r = (byte)Bitwise.FixedToFixed((value & des.rmask) >> des.rshift, des.rbits, 8);
                    g = (byte)Bitwise.FixedToFixed((value & des.gmask) >> des.gshift, des.gbits, 8);
                    b = (byte)Bitwise.FixedToFixed((value & des.bmask) >> des.bshift, des.bbits, 8);
                }
                if ((des.flags & PixelFormatFlags.HasAlpha) != 0)
                {
                    a = (byte)Bitwise.FixedToFixed((value & des.amask) >> des.ashift, des.abits, 8);
                }
                else
                    a = 255; // No alpha, default a component to full
            }
            else
            {
                // Do the operation with the more generic floating point
                float rr, gg, bb, aa;
                UnpackColor(out rr, out gg, out bb, out aa, pf, src);
                r = Bitwise.FloatToByteFixed(rr);
                g = Bitwise.FloatToByteFixed(gg);
                b = Bitwise.FloatToByteFixed(bb);
                a = Bitwise.FloatToByteFixed(aa);
            }
        }

        unsafe protected static void UnpackColor(out float r, out float g, out float b, out float a,
                                                 PixelFormat pf, byte* src)
        {
            PixelFormatDescription des = GetDescriptionFor(pf);
            if ((des.flags & PixelFormatFlags.NativeEndian) != 0)
            {
                // Shortcut for integer formats unpacking
                uint value = Bitwise.IntRead(src, des.elemBytes);
                if ((des.flags & PixelFormatFlags.Luminance) != 0)
                {
                    // Luminance format -- only rbits used
                    r = g = b = Bitwise.FixedToFloat(
                        (value & des.rmask) >> des.rshift, des.rbits);
                }
                else
                {
                    r = Bitwise.FixedToFloat((value & des.rmask) >> des.rshift, des.rbits);
                    g = Bitwise.FixedToFloat((value & des.gmask) >> des.gshift, des.gbits);
                    b = Bitwise.FixedToFloat((value & des.bmask) >> des.bshift, des.bbits);
                }
                if ((des.flags & PixelFormatFlags.HasAlpha) != 0)
                    a = Bitwise.FixedToFloat((value & des.amask) >> des.ashift, des.abits);
                else
                    a = 1.0f; // No alpha, default a component to full
            }
            else
            {
                switch (pf)
                {
                    case PixelFormat.R32F:
                        r = g = b = ((float*)src)[0];
                        a = 1.0f;
                        break;
                    case PixelFormat.R32G32B32F:
                        r = ((float*)src)[0];
                        g = ((float*)src)[1];
                        b = ((float*)src)[2];
                        a = 1.0f;
                        break;
                    case PixelFormat.A32B32G32R32F:
                        r = ((float*)src)[0];
                        g = ((float*)src)[1];
                        b = ((float*)src)[2];
                        a = ((float*)src)[3];
                        break;
                    case PixelFormat.R16F:
                        r = g = b = Bitwise.HalfToFloat(((ushort*)src)[0]);
                        a = 1.0f;
                        break;
                    case PixelFormat.R16G16B16F:
                        r = Bitwise.HalfToFloat(((ushort*)src)[0]);
                        g = Bitwise.HalfToFloat(((ushort*)src)[1]);
                        b = Bitwise.HalfToFloat(((ushort*)src)[2]);
                        a = 1.0f;
                        break;
                    case PixelFormat.A16B16G16R16F:
                        r = Bitwise.HalfToFloat(((ushort*)src)[0]);
                        g = Bitwise.HalfToFloat(((ushort*)src)[1]);
                        b = Bitwise.HalfToFloat(((ushort*)src)[2]);
                        a = Bitwise.HalfToFloat(((ushort*)src)[3]);
                        break;
                    //   				case PixelFormat.SHORT_RGBA:
                    //   					r = Bitwise.FixedToFloat(((ushort*)src)[0], 16);
                    //   					g = Bitwise.FixedToFloat(((ushort*)src)[1], 16);
                    //   					b = Bitwise.FixedToFloat(((ushort*)src)[2], 16);
                    //   					a = Bitwise.FixedToFloat(((ushort*)src)[3], 16);
                    //   					break;
                    //   				case PixelFormat.BYTE_LA:
                    //   					r = g = b = Bitwise.FixedToFloat(((byte*)src)[0], 8);
                    //   					a = Bitwise.FixedToFloat(((byte*)src)[1], 8);
                    //   					break;
                    default:
                        // Not yet supported
                        throw new Exception("Unpack from " + pf + " not implemented, in PixelUtil.UnpackColor");
                }
            }
        }

        unsafe public static void CopyBytes(IntPtr dst, int dstOffset, IntPtr src, int srcOffset, int count)
        {
            byte* srcBytes = (byte*)src.ToPointer();
            byte* dstBytes = (byte*)dst.ToPointer();
            byte* srcptr = srcBytes + srcOffset;
            byte* dstptr = dstBytes + dstOffset;
            Memory.Copy(srcptr, dstptr, count);
            //for (int i = 0; i < count; i++)
            //    dstBytes[dstOffset + i] = srcBytes[srcOffset + i];
            //for (int i = 0; i < count; i++)
            //    *dstptr++ = *srcptr++;
        }

        ///<summary>
        ///    Convert consecutive pixels from one format to another. No dithering or filtering is being done. 
        ///    Converting from RGB to luminance takes the R channel.  In case the source and destination format match,
        ///    just a copy is done.
        ///</summary>
        ///<param name="srcBytes">Pointer to source region</param>
        ///<param name="srcFormat">Pixel format of source region</param>
        ///<param name="dstBytes">Pointer to destination region</param>
        ///<param name="dstFormat">Pixel format of destination region</param>
        public static void BulkPixelConversion(IntPtr srcBytes, int srcOffset, PixelFormat srcFormat,
                                               IntPtr dstBytes, int dstOffset, PixelFormat dstFormat,
                                               int count)
        {
            PixelBox src = new PixelBox(count, 1, 1, srcBytes, srcFormat);
            //src.Offset = srcOffset;
            PixelBox dst = new PixelBox(count, 1, 1, dstBytes, dstFormat);
            //dst.Offset = dstOffset;
            BulkPixelConversion(src, dst, srcOffset, dstOffset);
        }


 


        ///<summary>
        ///    Convert pixels from one format to another. No dithering or filtering is being done. Converting
        ///    from RGB to luminance takes the R channel. 
        ///</summary>
        ///<param name="src">PixelBox containing the source pixels, pitches and format</param>
        ///<param name="dst">PixelBox containing the destination pixels, pitches and format</param>
        ///<remarks>
        ///    The source and destination boxes must have the same
        ///    dimensions. In case the source and destination format match, a plain copy is done.
        ///</remarks>
        public static void BulkPixelConversion(PixelBox src, PixelBox dst, int srcOfs, int dstOfs)
        {
            //Debug.Assert(src.Width == dst.Width && src.Height == dst.Height && src.Depth == dst.Depth);            

            // Check for compressed formats, we don't support decompression, compression or recoding
            if (src.IsCompressed || dst.IsCompressed)
            {
                if (src.Format == dst.Format)
                {
                    CopyBytes(dst.Pointer, dstOfs, src.Pointer, srcOfs, src.MemorySize);
                    return;
                }
                else
                    throw new Exception("This method can not be used to compress or decompress images, in PixelBox.BulkPixelConversion");
            }

            // The easy case
            if (src.Format == dst.Format)
            {
                // Everything consecutive?
                if (src.Consecutive && dst.Consecutive)
                {
                    CopyBytes(dst.Pointer, dstOfs, src.Pointer, srcOfs, src.MemorySize);
                    return;
                }
                unsafe
                {
                    byte* srcBytes = (byte*)src.Pointer.ToPointer();
                    byte* dstBytes = (byte*)dst.Pointer.ToPointer();
                    byte* srcptr = srcBytes + srcOfs;
                    byte* dstptr = dstBytes + dstOfs;
                    int srcPixelSize = PixelUtil.GetNumElemBytes(src.Format);
                    int dstPixelSize = PixelUtil.GetNumElemBytes(dst.Format);

                    // Calculate pitches+skips in bytes
                    int srcRowPitchBytes = src.RowPitch * srcPixelSize;
                    //int srcRowSkipBytes = src.RowSkip * srcPixelSize;
                    int srcSliceSkipBytes = src.SlicePitch * srcPixelSize;

                    int dstRowPitchBytes = dst.RowPitch * dstPixelSize;
                    //int dstRowSkipBytes = dst.RowSkip * dstPixelSize;
                    int dstSliceSkipBytes = dst.SlicePitch * dstPixelSize;

                    // Otherwise, copy per row
                    int rowSize = src.Width * srcPixelSize;
                    for (int z = 0; z < src.Depth; z++)
                    {
                        for (int y = 0; y < src.Height; y++)
                        {
                            Memory.Copy(srcptr, dstptr, rowSize);
                            //byte* s = srcptr;
                            //byte* d = dstptr;
                            //for (int i = 0; i < rowSize; i++)
                            //    *d++ = *s++;
                            srcptr += srcRowPitchBytes;
                            dstptr += dstRowPitchBytes;
                        }
                        srcptr += srcSliceSkipBytes;
                        dstptr += dstSliceSkipBytes;
                    }
                }
                return;
            }
            //// Converting to X8R8G8B8 is exactly the same as converting to
            //// A8R8G8B8. (same with X8B8G8R8 and A8B8G8R8)
            //// The X8* formats are not currently supported in Axiom
            //// FIXME: These X8 formats are supported now, so I should handle them
            //if ( dst.format == PixelFormat.X8R8G8B8 || dst.format == PixelFormat.X8B8G8R8 )
            //{
            //    // Do the same conversion, with A8R8G8B8, which has a lot of 
            //    // optimized conversions
            //    PixelBox tempdst = new PixelFormat( dst );
            //    tempdst.format = dst.format == PixelFormat.X8R8G8B8 ? PixelFormat.A8R8G8B8 : PixelFormat.A8B8G8R8;
            //    BulkPixelConversion( src, tempdst );
            //    return;
            //}

            //// Converting from X8R8G8B8 is exactly the same as converting from
            //// A8R8G8B8, given that the destination format does not have alpha.
            //if ( ( src.format == PixelFormat.X8R8G8B8 || src.format == PixelFormat.X8B8G8R8 ) &&
            //   !Image.FormatHasAlpha( dst.format ) )
            //{
            //    // Do the same conversion, with A8R8G8B8, which has a lot of 
            //    // optimized conversions
            //    PixelBox tempsrc = src;
            //    tempsrc.format = src.format == PixelFormat.X8R8G8B8 ? PixelFormat.A8R8G8B8 : PixelFormat.A8B8G8R8;
            //    BulkPixelConversion( tempsrc, dst );
            //    return;
            //}

            if (PixelConversionLoops.DoOptimizedConversion(src, dst))
            {
                // If so, good
                return;
            }
            unsafe
            {
                byte* srcBytes = (byte*)src.Pointer.ToPointer();
                byte* dstBytes = (byte*)dst.Pointer.ToPointer();
                byte* srcptr = srcBytes + srcOfs;
                byte* dstptr = dstBytes + dstOfs;
                int srcPixelSize = PixelUtil.GetNumElemBytes(src.Format);
                int dstPixelSize = PixelUtil.GetNumElemBytes(dst.Format);

                // Calculate pitches+skips in bytes
                int srcRowSkipBytes = src.RowPitch * srcPixelSize;
                int srcSliceSkipBytes = src.SlicePitch * srcPixelSize;
                int dstRowSkipBytes = dst.RowPitch * dstPixelSize;
                int dstSliceSkipBytes = dst.SlicePitch * dstPixelSize;

                // The brute force fallback
                float r, g, b, a;
                for (int z = 0; z < src.Depth; z++)
                {
                    for (int y = 0; y < src.Height; y++)
                    {
                        for (int x = 0; x < src.Width; x++)
                        {
                            UnpackColor(out r, out g, out b, out a, src.Format, srcptr);
                            PackColor(r, g, b, a, dst.Format, dstptr);
                            srcptr += srcPixelSize;
                            dstptr += dstPixelSize;
                        }
                        srcptr += srcRowSkipBytes;
                        dstptr += dstRowSkipBytes;
                    }
                    srcptr += srcSliceSkipBytes;
                    dstptr += dstSliceSkipBytes;
                }
            }
        }

        #endregion Static Bulk Conversion Methods
    }

    public class PixelUtil
    {
        /// <summary>
        ///    Returns the size in bytes of an element of the given pixel format.
        /// </summary>
        /// <param name="format">Pixel format to test.</param>
        /// <returns>Size in bytes.</returns>
        public static int GetNumElemBytes(PixelFormat format)
        {
            return PixelConverter.GetDescriptionFor(format).elemBytes;
        }

        //public static bool Compressed(PixelFormat format)
        //{
        //    return (format == PixelFormat.DXT1 ||
        //            format == PixelFormat.DXT2 ||
        //            format == PixelFormat.DXT3 ||
        //            format == PixelFormat.DXT4 ||
        //            format == PixelFormat.DXT5);
        //}

        /// <summary>
        ///    Returns the size in bits of an element of the given pixel format.
        /// </summary>
        /// <param name="format">Pixel format to test.</param>
        /// <returns>Size in bits.</returns>
        public static int GetNumElemBits(PixelFormat format)
        {
            return GetNumElemBytes(format) * 8;
        }

        public static int[] GetBitDepths(PixelFormat format)
        {
            int[] rgba = new int[4];
            rgba[0] = PixelConverter.GetDescriptionFor(format).rbits;
            rgba[1] = PixelConverter.GetDescriptionFor(format).gbits;
            rgba[2] = PixelConverter.GetDescriptionFor(format).bbits;
            rgba[3] = PixelConverter.GetDescriptionFor(format).abits;
            return rgba;
        }

        ///<summary>
        ///    Returns the size in memory of a region with the given extents and pixel
        ///    format with consecutive memory layout.
        ///</summary>
        ///<param name="width">Width of the area</param>
        ///<param name="height">Height of the area</param>
        ///<param name="depth">Depth of the area</param>
        ///<param name="format">Format of the area</param>
        ///<returns>The size in bytes</returns>
        ///<remarks>
        ///    In case that the format is non-compressed, this simply returns
        ///    width * height * depth * PixelConverter.GetNumElemBytes(format). In the compressed
        ///    case, this does serious magic.
        ///</remarks>
        public static int GetMemorySize(int width, int height, int depth, PixelFormat format)
        {
            if (IsCompressed(format))
            {
                switch (format)
                {
                    case PixelFormat.DXT1:
                        //Debug.Assert(depth == 1);
                        return ((width * 3) / 4) * ((height * 3) / 4) * 8;
                    case PixelFormat.DXT2:
                    case PixelFormat.DXT3:
                    case PixelFormat.DXT4:
                    case PixelFormat.DXT5:
                        //Debug.Assert(depth == 1);
                        return ((width * 3) / 4) * ((height * 3) / 4) * 16;
                    default:
                        throw new Exception("Invalid compressed pixel format");
                }
            }
            else
            {
                return width * height * depth * GetNumElemBytes(format);
            }
        }

        public static bool IsCompressed(PixelFormat format)
        {
            return (PixelConverter.GetDescriptionFor(format).flags & PixelFormatFlags.Compressed) != 0;
        }

        public static bool IsFloatingPoint(PixelFormat format)
        {
            return (PixelConverter.GetDescriptionFor(format).flags & PixelFormatFlags.Float) != 0;
        }

        public static bool HasAlpha(PixelFormat format)
        {
            return (PixelConverter.GetDescriptionFor(format).flags & PixelFormatFlags.HasAlpha) != 0;
        }

        public static string GetFormatName(PixelFormat format)
        {
            return PixelConverter.GetDescriptionFor(format).name;
        }

        public static PixelComponentType GetComponentType(PixelFormat format)
        {
            return PixelConverter.GetDescriptionFor(format).componentType;
        }
    }
}

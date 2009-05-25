using System;
using System.Collections.Generic;
using System.Text;
using DevIl;
using VirtualBicycle.IO;

namespace VirtualBicycle.Media
{
    public unsafe abstract class DevIlImageFormat : ImageFormat
    {
        public static bool IsInitialized
        {
            get;
            private set;
        }
        public static void Initialize()
        {
            if (!IsInitialized)
            {
                Il.ilInit();
                IsInitialized = true;
            }
        }


        public static PixelFormat ILFormat2PixelFormat(int format, int bytesPerPixel)
        {
            switch (bytesPerPixel)
            {
                case 1:
                    return PixelFormat.L8;

                case 2:
                    switch (format)
                    {
                        case Il.IL_BGR:
                            return PixelFormat.B5G6R5; // Format.B5G6R5;
                        case Il.IL_RGB:
                            return PixelFormat.R5G6B5; // Format.R5G6B5;
                        case Il.IL_BGRA:
                            return PixelFormat.B4G4R4A4;
                        case Il.IL_RGBA:
                            return PixelFormat.A4R4G4B4;
                    }
                    break;

                case 3:
                    switch (format)
                    {
                        case Il.IL_BGR:
                            return PixelFormat.B8G8R8;
                        case Il.IL_RGB:
                            return PixelFormat.R8G8B8;// Format.R8G8B8;
                    }
                    break;

                case 4:
                    switch (format)
                    {
                        case Il.IL_BGRA:
                            return PixelFormat.A8B8G8R8; // Format.B8G8R8A8;
                        case Il.IL_RGBA:
                            return PixelFormat.A8R8G8B8;//Format.A8R8G8B8;
                        case Il.IL_DXT1:
                            return PixelFormat.DXT1;
                        case Il.IL_DXT2:
                            return PixelFormat.DXT2;
                        case Il.IL_DXT3:
                            return PixelFormat.DXT3;
                        case Il.IL_DXT4:
                            return PixelFormat.DXT4;
                        case Il.IL_DXT5:
                            return PixelFormat.DXT5;
                    }
                    break;
            }

            return PixelFormat.Unknown;
        }

        public static Image LoadImage(ResourceLocation rl, int fileFormat)
        {
            if (!IsInitialized)
                Initialize();

            ContentBinaryReader br = new ContentBinaryReader(rl);

            int ilImageId = Il.ilGenImage();

            byte[] buffer = br.ReadBytes(rl.Size);
            Il.ilBindImage(ilImageId);
            Il.ilLoadL(fileFormat, buffer, buffer.Length);
            br.Close();


            int ilFmt = Il.ilGetInteger(Il.IL_IMAGE_FORMAT);

            int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
            int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
            int depth = Il.ilGetInteger(Il.IL_IMAGE_DEPTH);
            int mipmapCount = Il.ilGetInteger(Il.IL_NUM_MIPMAPS);
            int bytesPerPixel = Il.ilGetInteger(Il.IL_IMAGE_BYTES_PER_PIXEL);
            int dataSize = width * height * bytesPerPixel * depth;

            PixelFormat format = ILFormat2PixelFormat(ilFmt, bytesPerPixel);

            IntPtr dataPtr = Il.ilGetData();

            byte[] decoded = new byte[dataSize];

            fixed (void* dst = &decoded[0])
            {
                Memory.Copy(dataPtr.ToPointer(), dst, dataSize);
            }

            Il.ilDeleteImages(1, ref ilImageId);

            return new Image(width, height, depth, mipmapCount, decoded, format);
        }
    }

    public unsafe sealed class TgaImageFormat : DevIlImageFormat
    {

        public override Image LoadImage(ResourceLocation rl)
        {
            if (!IsInitialized)
                Initialize();

            ContentBinaryReader br = new ContentBinaryReader(rl);

            int ilImageId = Il.ilGenImage();

            byte[] buffer = br.ReadBytes(rl.Size);
            Il.ilBindImage(ilImageId);
            Il.ilLoadL(Il.IL_TGA, buffer, buffer.Length);
            br.Close();

            int ilFmt = Il.ilGetInteger(Il.IL_IMAGE_FORMAT);

            int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
            int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
            int depth = Il.ilGetInteger(Il.IL_IMAGE_DEPTH);

            int bytesPerPixel = Il.ilGetInteger(Il.IL_IMAGE_BYTES_PER_PIXEL);
            PixelFormat format = ILFormat2PixelFormat(ilFmt, bytesPerPixel);

            IntPtr dataPtr = Il.ilGetData();

            int dataSize = width * height * bytesPerPixel * depth;
            byte[] decoded = new byte[dataSize];

            // check to see whether the image is upside down
            if (Il.ilGetInteger(Il.IL_ORIGIN_MODE) == Il.IL_ORIGIN_LOWER_LEFT)
            {
                // if so (probably), put it right side up
                Il.ilEnable(Il.IL_ORIGIN_SET);
                Il.ilSetInteger(Il.IL_ORIGIN_MODE, Il.IL_ORIGIN_UPPER_LEFT);
            }
            // are the color components reversed?
            if (ilFmt == Il.IL_BGR || ilFmt == Il.IL_BGRA)
            {
                // if so (probably), reverse b and r.  this is slower, but it works.
                int newFormat = (ilFmt == Il.IL_BGR) ? Il.IL_RGB : Il.IL_RGBA;
                fixed (void* dst = &decoded[0])
                {
                    Il.ilCopyPixels(0, 0, 0, width, height, depth, newFormat, Il.IL_UNSIGNED_BYTE, new IntPtr(dst));
                }
                ilFmt = newFormat;
            }
            else
            {
                fixed (void* dst = &decoded[0])
                {
                    Memory.Copy(dataPtr.ToPointer(), dst, dataSize);
                }
            }

            Il.ilDeleteImages(1, ref ilImageId);

            return new Image(width, height, depth, decoded, format);
        }

        public override Image LoadImage(string file)
        {
            return LoadImage(new FileLocation(file));
        }

        public override string[] Filters
        {
            get { return new string[] { ".tga" }; }
        }

        public override string Type
        {
            get { return "Targa"; }
        }
    }

    public sealed unsafe class DdsImageFormat : DevIlImageFormat
    {

        public override Image LoadImage(ResourceLocation rl)
        {
            ContentBinaryReader br = new ContentBinaryReader(rl);

            int ilImageId = Il.ilGenImage();

            byte[] buffer = br.ReadBytes(rl.Size);
            Il.ilBindImage(ilImageId);
            Il.ilLoadL(Il.IL_DDS, buffer, buffer.Length);
            br.Close();


            int ilFmt = Il.ilGetInteger(Il.IL_IMAGE_FORMAT);

            int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
            int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
            int depth = Il.ilGetInteger(Il.IL_IMAGE_DEPTH);
            int mipMapCount = Il.ilGetInteger(Il.IL_NUM_MIPMAPS);
            int dxtFormat = Il.ilGetInteger(Il.IL_DXTC_DATA_FORMAT);

            int bytesPerPixel = Il.ilGetInteger(Il.IL_IMAGE_BYTES_PER_PIXEL);
            PixelFormat format = ILFormat2PixelFormat(ilFmt, bytesPerPixel);


            bool hasCubeMap = Il.ilGetInteger(Il.IL_IMAGE_CUBEFLAGS) != 0;

            if (dxtFormat != Il.IL_DXT_NO_COMP)
            {
                // call first with null which returns the size (odd...)
                int dxtSize = Il.ilGetDXTCData(IntPtr.Zero, 0, dxtFormat);
                byte[] decoded = new byte[dxtSize];

                fixed (void* dst = &decoded[0])
                {
                    // get the data into the buffer
                    Il.ilGetDXTCData(new IntPtr(dst), dxtSize, dxtFormat);//.ilGetDXTCData(buffer, dxtSize, dxtFormat);
                }

                if (hasCubeMap)
                {
                    return new Image(width, mipMapCount, decoded, format);
                }
                else
                {
                    return new Image(width, height, depth, mipMapCount, decoded, format);
                }
            }
            else
            {
                int numImagePasses = hasCubeMap ? 6 : 1;
                int imageSize = Il.ilGetInteger(Il.IL_IMAGE_SIZE_OF_DATA);

                byte[] decoded = new byte[imageSize];
                for (int i = 0, offset = 0; i < numImagePasses; i++, offset += imageSize)
                {
                    if (hasCubeMap)
                    {
                        // rebind and set the current image to be active
                        Il.ilBindImage(ilImageId);
                        Il.ilActiveImage(i);
                    }

                    // get the decoded data
                    IntPtr ptr = Il.ilGetData();

                    // copy the data into the byte array, using the offset value if this
                    // data contains multiple images
                    fixed (void* dst = &decoded[0])
                    {
                        Memory.Copy(ptr.ToPointer(), dst, imageSize);
                    }
                } // for

                if (hasCubeMap)
                {
                    return new Image(width, mipMapCount, decoded, format);
                }
                else
                {
                    return new Image(width, height, depth, mipMapCount, decoded, format);
                }
            }
        }

        public override Image LoadImage(string file)
        {
            return LoadImage(new FileLocation(file));
        }

        public override string[] Filters
        {
            get { return new string[] { ".dds" }; }
        }

        public override string Type
        {
            get { return "Direct draw surface"; }
        }
    }

    public sealed unsafe class BmpImageFormat : DevIlImageFormat
    {


        public override string Type
        {
            get { return "Windows Bitmap"; }
        }

        public override string[] Filters
        {
            get { return new string[] { ".bmp", ".dib" }; }
        }


        public override Image LoadImage(ResourceLocation rl)
        {
            if (!IsInitialized)
                Initialize();

            ContentBinaryReader br = new ContentBinaryReader(rl);

            int ilImageId = Il.ilGenImage();

            byte[] buffer = br.ReadBytes(rl.Size);
            Il.ilBindImage(ilImageId);
            Il.ilLoadL(Il.IL_BMP, buffer, buffer.Length);
            br.Close();


            int ilFmt = Il.ilGetInteger(Il.IL_IMAGE_FORMAT);

            int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
            int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
            int depth = Il.ilGetInteger(Il.IL_IMAGE_DEPTH);
            int mipmapCount = Il.ilGetInteger(Il.IL_NUM_MIPMAPS);
            int bytesPerPixel = Il.ilGetInteger(Il.IL_IMAGE_BYTES_PER_PIXEL);
            int dataSize = width * height * bytesPerPixel * depth;

            PixelFormat format = ILFormat2PixelFormat(ilFmt, bytesPerPixel);

            IntPtr dataPtr = Il.ilGetData();

            byte[] decoded = new byte[dataSize];

            // are the color components reversed?
            if (ilFmt == Il.IL_BGR || ilFmt == Il.IL_BGRA)
            {
                // if so (probably), reverse b and r.  this is slower, but it works.
                int newFormat = (ilFmt == Il.IL_BGR) ? Il.IL_RGB : Il.IL_RGBA;
                fixed (void* dst = &decoded[0])
                {
                    Il.ilCopyPixels(0, 0, 0, width, height, depth, newFormat, Il.IL_UNSIGNED_BYTE, new IntPtr(dst));
                }
                ilFmt = newFormat;
            }
            else
            {
                fixed (void* dst = &decoded[0])
                {
                    Memory.Copy(dataPtr.ToPointer(), dst, dataSize);
                }
            }

            Il.ilDeleteImages(1, ref ilImageId);

            return new Image(width, height, depth, mipmapCount, decoded, format);
        }

        public override Image LoadImage(string file)
        {
            return LoadImage(new FileLocation(file));
        }
    }

    public sealed class JpegImageFormat : DevIlImageFormat
    {
        #region IImageFactory 成员

        public override Image LoadImage(ResourceLocation rl)
        {
            return LoadImage(rl, Il.IL_JPG);
        }
        public override Image LoadImage(string file)
        {
            return LoadImage(new FileLocation(file));
        }

        public override string Type
        {
            get { return "Joint Photographic Experts Group"; }
        }
        public override string[] Filters
        {
            get { return new string[] { ".jpg", ".jpeg" }; }
        }
        #endregion
    }

    public sealed class PcxImageFormat : DevIlImageFormat
    {

        public override string Type
        {
            get { return "PCX Format"; }
        }

        public override string[] Filters
        {
            get { return new string[] { ".pcx" }; }
        }

        public override Image LoadImage(ResourceLocation rl)
        {
            return LoadImage(rl, Il.IL_PCX);
        }

        public override Image LoadImage(string file)
        {
            return LoadImage(new FileLocation(file));
        }
    }

    public sealed class PngImageFormat : DevIlImageFormat
    {
        public override string Type
        {
            get { return "Portable Network Graphic Format"; }
        }

        public override string[] Filters
        {
            get { return new string[] { ".png" }; }
        }

        public override Image LoadImage(ResourceLocation rl)
        {
            return LoadImage(rl, Il.IL_PNG);
        }

        public override Image LoadImage(string file)
        {
            return LoadImage(new FileLocation(file));
        }
    }
}

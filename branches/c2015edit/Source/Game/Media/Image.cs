using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace VirtualBicycle.Media
{
    public unsafe class Image : IDisposable
    {
        static int[] bppTable;

        static Image()
        {
            bppTable = new int[(int)PixelFormat.Count];
            for (int i = 0; i < bppTable.Length; i++)
            {
                bppTable[i] = -1;
            }

            //bppTable[(int)PixelFormat.Unknown] = -1;
            bppTable[(int)PixelFormat.L8] = 1;
            bppTable[(int)PixelFormat.A8] = 1;
            bppTable[(int)PixelFormat.A4L4] = 1;
            bppTable[(int)PixelFormat.R3G3B2] = 1;
            bppTable[(int)PixelFormat.R5G6B5] = 2;
            bppTable[(int)PixelFormat.B5G6R5] = 2;
            bppTable[(int)PixelFormat.A4R4G4B4] = 2;
            bppTable[(int)PixelFormat.B4G4R4A4] = 2;
            bppTable[(int)PixelFormat.A8L8] = 2;
            bppTable[(int)PixelFormat.L16] = 2;
            bppTable[(int)PixelFormat.A1R5G5B5] = 2;
            bppTable[(int)PixelFormat.R8G8B8] = 3;
            bppTable[(int)PixelFormat.B8G8R8] = 3;
            bppTable[(int)PixelFormat.A8R8G8B8] = 4;
            bppTable[(int)PixelFormat.A8B8G8R8] = 4;
            bppTable[(int)PixelFormat.B8G8R8A8] = 4;
            bppTable[(int)PixelFormat.R8G8B8A8] = 4;
            bppTable[(int)PixelFormat.X8R8G8B8] = 4;
            bppTable[(int)PixelFormat.X8B8G8R8] = 4;
            bppTable[(int)PixelFormat.A2R10G10B10] = 4;
            bppTable[(int)PixelFormat.A2B10G10R10] = 4;
            bppTable[(int)PixelFormat.A16B16G16R16] = 8;
            bppTable[(int)PixelFormat.R16F] = 2;
            bppTable[(int)PixelFormat.R16G16B16F] = 6;
            bppTable[(int)PixelFormat.A16B16G16R16F] = 8;
            bppTable[(int)PixelFormat.R32F] = 4;
            bppTable[(int)PixelFormat.R32G32B32F] = 12;
            bppTable[(int)PixelFormat.A32B32G32R32F] = 16;
            bppTable[(int)PixelFormat.G16R16F] = 4;
            bppTable[(int)PixelFormat.G32R32F] = 8;
            bppTable[(int)PixelFormat.G16R16] = 4;
            bppTable[(int)PixelFormat.R16G16B16] = 6;
            PixelConverter.Initialize();
        }

        public static int GetBytesPerPixel(PixelFormat format)
        {
            return bppTable[(int)format];
        }

        byte[] data;





        public Image(int width, int height, int depth, int mipCount, byte[] data, PixelFormat format)
        {
            this.BytesPerPixel = GetBytesPerPixel(format);

            this.data = data;
            if (this.data == null)
            {
                this.data = new byte[PixelUtil.GetMemorySize(width, height, depth, format)];
            }

            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.Format = format;
            this.SizeInBytes = this.data.Length;
            
            this.MipmapCount = mipCount;

            if (depth == 1)
            {
                if (width == 1 || height == 1)
                {
                    this.Type = ImageType.Image1D;
                }
                else
                {
                    this.Type = ImageType.Image2D;
                }
            }
            else
            {
                this.Type = ImageType.Image3D;
            }
        }
        
        public Image(int width, int height, int depth, int mipCount, IntPtr data, PixelFormat format)
        {
            this.BytesPerPixel = GetBytesPerPixel(format);

            this.data = new byte[PixelUtil.GetMemorySize(width, height, depth, format)];
            fixed (void* dst = &this.data[0])
            {
                Memory.Copy(data.ToPointer(), dst, this.data.Length);
            }
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.Format = format;
            this.SizeInBytes = this.data.Length;
            this.MipmapCount = mipCount;

            if (depth == 1)
            {
                if (width == 1 || height == 1)
                {
                    this.Type = ImageType.Image1D;
                }
                else
                {
                    this.Type = ImageType.Image2D;
                }
            }
            else
            {
                this.Type = ImageType.Image3D;
            }
        }



        public Image(int length, int mipCount, byte[] data, PixelFormat format)
        {
            this.Type = ImageType.CubeImage;

            this.data = data;
            if (this.data == null)
            {
                this.data = new byte[PixelUtil.GetMemorySize(length, length, 1, format) * 6];
            }

            this.Width = length;
            this.Height = length; 
            this.Depth = 1;
            this.Format = format;
            this.SizeInBytes = this.data.Length;
            this.MipmapCount = mipCount;
        }
       
        public Image(int length, int mipCount, IntPtr data, PixelFormat format)
        {
            this.Type = ImageType.CubeImage;

            this.data = new byte[PixelUtil.GetMemorySize(length, length, 1, format) * 6];

            fixed (void* dst = &this.data[0])
            {
                Memory.Copy(data.ToPointer(), dst, this.data.Length);
            }

            this.Width = length;
            this.Height = length;
            this.Depth = 1;
            this.Format = format;
            this.SizeInBytes = this.data.Length;
            this.MipmapCount = mipCount;
        }



        public Image(int width, int height, int mipCount, byte[] data, PixelFormat format)
        {
            this.BytesPerPixel = GetBytesPerPixel(format);

            this.data = data;
            if (this.data == null)
            {
                this.data = new byte[PixelUtil.GetMemorySize(width, height, 1, format)];
            }

            this.Width = width;
            this.Height = height;
            this.Depth = 1;
            this.Format = format;
            this.SizeInBytes = this.data.Length;
            this.MipmapCount = mipCount;

            if (width == 1 || height == 1)
            {
                this.Type = ImageType.Image1D;
            }
            else
            {
                this.Type = ImageType.Image2D;
            }
        }
        
        public Image(int width, int height, int mipCount, IntPtr data, PixelFormat format)
        {
            this.BytesPerPixel = GetBytesPerPixel(format);

            this.data = new byte[PixelUtil.GetMemorySize(width, height, 1, format)];
            fixed (void* dst = &this.data[0])
            {
                Memory.Copy(data.ToPointer(), dst, this.data.Length);
            }
            this.Width = width;
            this.Height = height;
            this.Depth = 1;
            this.Format = format;
            this.SizeInBytes = this.data.Length;
            this.MipmapCount = mipCount;

            if (width == 1 || height == 1)
            {
                this.Type = ImageType.Image1D;
            }
            else
            {
                this.Type = ImageType.Image2D;
            }
        }

        public Image(int width, int height, int depth, int mipCount, PixelFormat format)
            : this(width, height, depth, mipCount, null, format)
        {
        }



        public Image(int length, int mipCount, PixelFormat format)
            : this(length, mipCount, null, format)
        {
        }
        
        public Image(int width, int height, int mipCount, PixelFormat format)
            : this(width, height, mipCount, null, format)
        {
        }


        public Image(int length, PixelFormat format)
            : this(length, 1, null, format)
        { 
        }

        public void MakeTransparent(Color color)
        {
            if (PixelUtil.HasAlpha(Format) && !PixelUtil.IsCompressed(Format))
            {
                int color2;
                switch (Format)
                {
                    case PixelFormat.A8R8G8B8:
                        color2 = color.ToArgb();
                        break;
                    case PixelFormat.A8B8G8R8:
                        color2 = (color.A << 24) | (color.B << 16) | (color.G << 8) | color.R;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                fixed (byte* ptr = &data[0])
                {
                    int* dst = (int*)ptr;
                    for (int i = 0; i < Width * Height * Depth; i++)
                    {
                        if (dst[i] == color2)
                        {
                            dst[i] = 0;
                        }
                    }
                }

            }

        }


        //public Image(int width, int height, int depth, int mipCount, byte[] data, PixelFormat format, ImageType type)
        //{
        //    this.BytesPerPixel = GetBytesPerPixel(format);

        //    if (BytesPerPixel == -1)
        //    {
        //        throw new InvalidOperationException();
        //    }
        //    this.data = data;
        //    this.Width = width;
        //    this.Height = height;
        //    this.Depth = depth;
        //    this.Format = format;
        //    this.SizeInBytes = data.Length;
        //    this.Type = type;
        //    this.MipmapCount = mipCount;
        //}
        //public Image(int width, int height, int depth, int mipCount, void* data, PixelFormat format, ImageType type)
        //{
        //    this.BytesPerPixel = GetBytesPerPixel(format);

        //    this.data = new byte[PixelUtil.GetMemorySize(width, height, depth, format)];

        //    fixed (void* dst = &this.data[0])
        //    {
        //        Memory.Copy(data, dst, this.data.Length);
        //    }

        //    this.Width = width;
        //    this.Height = height;
        //    this.Depth = depth;
        //    this.Format = format;
        //    this.SizeInBytes = this.data.Length;
        //    this.Type = type;
        //    this.MipmapCount = mipCount;
        //}

        //public Image(int width, int height, int depth, byte[] data, PixelFormat format, ImageType type)
        //{
        //    this.BytesPerPixel = GetBytesPerPixel(format);

        //    //if (data.Length != width * height * depth * BytesPerPixel)
        //    //{
        //    //    throw new ArgumentException();
        //    //}
        //    this.data = data;
        //    this.Width = width;
        //    this.Height = height;
        //    this.Depth = depth;
        //    this.Format = format;
        //    this.SizeInBytes = data.Length;
        //    this.Type = type;
        //    this.MipmapCount = 1;
        //}
        //public Image(int width, int height, int depth, void* data, PixelFormat format, ImageType type)
        //{
        //    this.BytesPerPixel = GetBytesPerPixel(format);

        //    this.data = new byte[PixelUtil.GetMemorySize(width, height, depth, format)];

        //    fixed (void* dst = &this.data[0])
        //    {
        //        Memory.Copy(data, dst, this.data.Length);
        //    }

        //    this.Width = width;
        //    this.Height = height;
        //    this.Depth = depth;
        //    this.Format = format;
        //    this.SizeInBytes = this.data.Length;
        //    this.Type = type;
        //    this.MipmapCount = 1;
        //}
        //public Image(int width, int height, int depth, PixelFormat format, ImageType type)
        //{
        //    this.BytesPerPixel = GetBytesPerPixel(format);

        //    this.data = new byte[PixelUtil.GetMemorySize(width, height, depth, format)];
        //    this.Width = width;
        //    this.Height = height;
        //    this.Depth = depth;
        //    this.Format = format;
        //    this.SizeInBytes = data.Length;
        //    this.Type = type;
        //    this.MipmapCount = 1;
        //}

        //public Image(int width, int height, int depth, byte[] data, PixelFormat format)
        //{
        //    this.BytesPerPixel = GetBytesPerPixel(format);

        //    if (BytesPerPixel == -1)
        //    {
        //        throw new InvalidOperationException();
        //    }
        //    //if (data.Length != width * height * depth * BytesPerPixel)
        //    //{
        //    //    throw new ArgumentException();
        //    //}
        //    this.data = data;
        //    this.Width = width;
        //    this.Height = height;
        //    this.Depth = depth;
        //    this.Format = format;
        //    this.SizeInBytes = data.Length;

        //    if (depth == 1)
        //    {
        //        if (width == 1 || height == 1)
        //        {
        //            Type = ImageType.Image1D;
        //        }
        //        else
        //        {
        //            Type = ImageType.Image2D;
        //        }
        //    }
        //    else
        //    {
        //        Type = ImageType.Image3D;
        //    }
        //    this.MipmapCount = 1;
        //}
        //public Image(int width, int height, int depth, void* data, PixelFormat format)
        //{
        //    this.BytesPerPixel = GetBytesPerPixel(format);

        //    this.data = new byte[PixelUtil.GetMemorySize(width, height, depth, format)];

        //    fixed (void* dst = &this.data[0])
        //    {
        //        Memory.Copy(data, dst, this.data.Length);
        //    }

        //    this.Width = width;
        //    this.Height = height;
        //    this.Depth = depth;
        //    this.Format = format;
        //    this.SizeInBytes = this.data.Length;

        //    if (depth == 1)
        //    {
        //        if (width == 1 || height == 1)
        //        {
        //            Type = ImageType.Image1D;
        //        }
        //        else
        //        {
        //            Type = ImageType.Image2D;
        //        }
        //    }
        //    else
        //    {
        //        Type = ImageType.Image3D;
        //    }
        //    this.MipmapCount = 1;
        //}
        //public Image(int width, int height, int depth, PixelFormat format)
        //{
        //    this.BytesPerPixel = GetBytesPerPixel(format);
           
        //    this.data = new byte[PixelUtil.GetMemorySize(width, height, depth, format)];
        //    this.Width = width;
        //    this.Height = height;
        //    this.Depth = depth;
        //    this.Format = format;
        //    this.SizeInBytes = data.Length;

        //    if (depth == 1)
        //    {
        //        if (width == 1 || height == 1)
        //        {
        //            Type = ImageType.Image1D;
        //        }
        //        else
        //        {
        //            Type = ImageType.Image2D;
        //        }
        //    }
        //    else
        //    {
        //        Type = ImageType.Image3D;
        //    }
        //    this.MipmapCount = 1;
        //}

        //public Image(int width, int height, int depth, PixelFormat format, int mipCount)
        //{
        //    this.BytesPerPixel = GetBytesPerPixel(format);

        //    this.data = new byte[PixelUtil.GetMemorySize(width, height, depth, format)];
        //    this.Width = width;
        //    this.Height = height;
        //    this.Depth = depth;
        //    this.Format = format;
        //    this.SizeInBytes = data.Length;

        //    if (depth == 1)
        //    {
        //        if (width == 1 || height == 1)
        //        {
        //            Type = ImageType.Image1D;
        //        }
        //        else
        //        {
        //            Type = ImageType.Image2D;
        //        }
        //    }
        //    else
        //    {
        //        Type = ImageType.Image3D;
        //    }
        //    this.MipmapCount = mipCount;
        //}
        //public Image(int width, int height, int depth, byte[] data, PixelFormat format, int mipCount)
        //{
        //    this.BytesPerPixel = GetBytesPerPixel(format);

        //    //if (data.Length != width * height * depth * BytesPerPixel)
        //    //{
        //    //    throw new ArgumentException();
        //    //}
        //    this.data = data;
        //    this.Width = width;
        //    this.Height = height;
        //    this.Depth = depth;
        //    this.Format = format;
        //    this.SizeInBytes = data.Length;

        //    if (depth == 1)
        //    {
        //        if (width == 1 || height == 1)
        //        {
        //            Type = ImageType.Image1D;
        //        }
        //        else
        //        {
        //            Type = ImageType.Image2D;
        //        }
        //    }
        //    else
        //    {
        //        Type = ImageType.Image3D;
        //    }
        //    this.MipmapCount = mipCount;
        //}
        //public Image(int width, int height, int depth, void* data, PixelFormat format, int mipCount)
        //{
        //    this.BytesPerPixel = GetBytesPerPixel(format);

        //    this.data = new byte[PixelUtil.GetMemorySize(width, height, depth, format)];

        //    fixed (void* dst = &this.data[0])
        //    {
        //        Memory.Copy(data, dst, this.data.Length);
        //    }

        //    this.Width = width;
        //    this.Height = height;
        //    this.Depth = depth;
        //    this.Format = format;
        //    this.SizeInBytes = this.data.Length;

        //    if (depth == 1)
        //    {
        //        if (width == 1 || height == 1)
        //        {
        //            Type = ImageType.Image1D;
        //        }
        //        else
        //        {
        //            Type = ImageType.Image2D;
        //        }
        //    }
        //    else
        //    {
        //        Type = ImageType.Image3D;
        //    }
        //    this.MipmapCount = mipCount;
        //}
    

        public ImageType Type
        {
            get;
            private set;
        }

        public void CopyData(IntPtr dst, PixelFormat format)
        {
            fixed (void* src = &data[0])
            {
                PixelConverter.BulkPixelConversion(new IntPtr(src), 0, Format, dst, 0, format, Width * Height * Depth);
            }
        }
        public void CopyDataARGB(IntPtr dst)
        {
            fixed (void* src = &data[0])
            {
                PixelConverter.BulkPixelConversion(new IntPtr(src), 0, Format, dst, 0, PixelFormat.A8R8G8B8, Width * Height * Depth);
            }
            //int* d = (int*)dst;
         
            //switch (Format)
            //{
            //    case PixelFormat.A8R8G8B8:
            //        fixed (void* src = &data[0])
            //        {
            //            Memory.Copy(src, dst, Width * Height * Depth * BytesPerPixel);
            //        }
            //        break;
            //    case PixelFormat.A8B8G8R8:
            //        for (int i = 0; i < Width * Height * Depth; i++)
            //        {
            //            d[i] = (data[i * 4] << 24) | (data[i * 4 + 1]) | (data[i * 4 + 2] << 8) | (data[i * 4 + 3] << 16);
            //        }
            //        break;
            //    case PixelFormat.B8G8R8A8:
            //        for (int i = 0; i < Width * Height * Depth; i++)
            //        {
            //            d[i] = (data[i * 4]) | (data[i * 4 + 1] << 8) | (data[i * 4 + 2] << 16) | (data[i * 4 + 3] << 24);
            //        }
            //        break;
            //    case PixelFormat.R8G8B8A8:
            //        for (int i = 0; i < Width * Height * Depth; i++)
            //        {
            //            d[i] = (data[i * 4] << 16) | (data[i * 4 + 1] << 8) | (data[i * 4 + 2]) | (data[i * 4 + 3] << 24);
            //        }
            //        break;
            //    case PixelFormat.X8B8G8R8:
            //        for (int i = 0; i < Width * Height * Depth; i++)
            //        {
            //            d[i] = (0xff << 24) | (data[i * 4 + 1]) | (data[i * 4 + 2] << 8) | (data[i * 4 + 3] << 16);
            //        }
            //        break;
            //    case PixelFormat.X8R8G8B8:
            //        for (int i = 0; i < Width * Height * Depth; i++)
            //        {
            //            d[i] = (0xff << 24) | (data[i * 4 + 1] << 16) | (data[i * 4 + 2] << 8) | (data[i * 4 + 3]);
            //        }
            //        break;
            //    case PixelFormat.R8G8B8:
            //        for (int i = 0; i < Width * Height * Depth; i++)
            //        {
            //            d[i] = (0xff << 24) | (data[i * 3] << 16) | (data[i * 3 + 1] << 8) | (data[i * 3 + 2]);
            //        }
            //        break;
            //    case PixelFormat.B8G8R8:
            //        for (int i = 0; i < Width * Height * Depth; i++)
            //        {
            //            d[i] = (0xff << 24) | (data[i * 3]) | (data[i * 3 + 1] << 8) | (data[i * 3 + 2] << 16);
            //        }
            //        break;
            //    case PixelFormat.L8:
            //        for (int i = 0; i < Width * Height * Depth; i++)
            //        {
            //            d[i] = (data[i] << 24) | (data[i] << 16) | (data[i] << 8) | (data[i]);
            //        }
            //        break;
            //    case PixelFormat.A8:
            //        for (int i = 0; i < Width * Height * Depth; i++)
            //        {
            //            d[i] = (data[i] << 24) | (data[i] << 16) | (data[i] << 8) | (data[i]);
            //        }
            //        break;
            //}
        }
        public byte[] CopyDataARGB()
        {
            byte[] newData = new byte[Width * Height * Depth * 4];
            fixed (void* ptr = &newData[0])
            {
                CopyDataARGB(new IntPtr(ptr));
            }
            return newData;
        }

        public int MipmapCount
        {
            get;
            private set;
        }

        public int Width
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            private set;
        }
        public int Depth
        {
            get;
            private set;
        }

        public byte[] Data
        {
            get { return data; }
        }
        public int BytesPerPixel
        {
            get;
            private set;
        }
        public int SizeInBytes
        {
            get;
            private set;
        }
        public PixelFormat Format
        {
            get;
            private set;
        }

        #region IDisposable 成员

        public  void Dispose()
        {
            data = null;
        }

        #endregion
    }
}

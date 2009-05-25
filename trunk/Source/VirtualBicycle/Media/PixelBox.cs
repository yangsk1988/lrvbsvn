using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Media
{
    public struct PixelBox
    {
        int rowPitch;
        int slicePitch;
        IntPtr pointer;

        int width;
        int height;
        int depth;

        PixelFormat format;

        public PixelBox(int width, int height, int depth, IntPtr pointer, PixelFormat fmt)
        {
            this.pointer = pointer;
            this.rowPitch = width;
            this.slicePitch = width * height;
            this.format = fmt;

            this.width = width;
            this.height = height;
            this.depth = depth;
        }
        //public DataBox(int rowPitch, int slicePitch, IntPtr pointer, PixelFormat fmt)
        //{
        //    this.rowPitch = rowPitch;
        //    this.slicePitch = slicePitch;
        //    this.pointer = pointer;
        //    this.format = fmt;
        //}
        public PixelBox(int rowPitch, int slicePitch, IntPtr pointer, int width, int height, int depth, PixelFormat fmt)
        {
            this.rowPitch = rowPitch;
            this.slicePitch = slicePitch;
            this.pointer = pointer;

            this.width = width;
            this.height = height;
            this.depth = depth;
            this.format = fmt;
        }


        public PixelFormat Format
        {
            get { return format; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Depth
        {
            get { return depth; }
        }

        public bool IsCompressed
        {
            get
            {
                return (PixelConverter.GetDescriptionFor(format).flags & PixelFormatFlags.Compressed) != 0;
            }
        }
        ///<summary>
        ///    Return whether this buffer is laid out consecutive in memory (ie the pitches
        ///    are equal to the dimensions)
        ///</summary>
        public bool Consecutive
        {
            get
            {
                return rowPitch == Width && slicePitch == Width * Height;
            }
        }
        /// <summary>
        ///      Gets the number of bytes of data between two consecutive (1D) rows of data.
        /// </summary>
        public int RowPitch
        {
            get { return rowPitch; }
        }

        /// <summary>
        /// Gets the number of bytes of data between two consecutive (2D) slices of data.
        /// </summary>
        public int SlicePitch
        {
            get { return slicePitch; }
        }
        public IntPtr Pointer
        {
            get { return pointer; }
        }

        public int MemorySize
        {
            get
            {
                return PixelUtil.GetMemorySize(Width, Height, Depth, format);
            }
        }
    }

}

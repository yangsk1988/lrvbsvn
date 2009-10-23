using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.MathLib;
using VirtualBicycle.Media;
using D3D = SlimDX.Direct3D9;
using SD = System.Drawing;
using SDI = System.Drawing.Imaging;

namespace VirtualBicycle.Graphics.D3D9
{
    internal sealed class D3D9Texture : Texture
    {
        static D3D.CubeMapFace[] conv;
        static D3D9Texture()
        {
            conv = new D3D.CubeMapFace[6];
            conv[(int)CubeMapFace.NegativeX] = D3D.CubeMapFace.NegativeX;
            conv[(int)CubeMapFace.NegativeY] = D3D.CubeMapFace.NegativeY;
            conv[(int)CubeMapFace.NegativeZ] = D3D.CubeMapFace.NegativeZ;
            conv[(int)CubeMapFace.PositiveX] = D3D.CubeMapFace.PositiveX;
            conv[(int)CubeMapFace.PositiveY] = D3D.CubeMapFace.PositiveY;
            conv[(int)CubeMapFace.PositiveZ] = D3D.CubeMapFace.PositiveZ;
        }

        static D3D.CubeMapFace ConvertEnum(CubeMapFace face)
        {
            return conv[(int)face];
        }
        static D3D.Pool GetPool(TextureUsage usage)
        {
            if ((usage & TextureUsage.RenderTarget) == TextureUsage.RenderTarget ||
                                   (usage & TextureUsage.Dynamic) == TextureUsage.Dynamic)
            {
                return D3D.Pool.Default;
            }
            return D3D.Pool.Managed;
        }
        

        /// <summary>
        ///     1D/2D normal texture.
        /// </summary>
        D3D.Texture texture;

        /// <summary>
        ///     Cubic texture reference.
        /// </summary>
        D3D.CubeTexture cubeTexture;

        /// <summary>
        ///     3D volume texture.
        /// </summary>
        D3D.VolumeTexture volTexture;


        internal D3D.BaseTexture baseTexture;


        D3D.Device device;
        D3D.Pool pool;

        D3D.VolumeTexture VolumeTexture
        {
            get { return volTexture; }
            set
            {
                volTexture = value;
                baseTexture = value;
            }
        }

        D3D.CubeTexture CubeTexture
        {
            get { return cubeTexture; }
            set
            {
                cubeTexture = value; baseTexture = value;
            }
        }
        D3D.Texture Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                baseTexture = value;
            }
        }

        public unsafe D3D9Texture(D3D9RenderSystem rs, D3D9Surface[] surface, TextureUsage usage)
            : base(rs, surface, usage)
        {
            this.device = rs.D3DDevice;

            if ((usage & TextureUsage.RenderTarget) == TextureUsage.RenderTarget ||
                (usage & TextureUsage.Dynamic) == TextureUsage.Dynamic)
            {
                pool = D3D.Pool.Default;
            }
            else
            {
                pool = D3D.Pool.Managed;
            }

            Texture = new D3D.Texture(device, Width, Height, SurfaceCount, D3D.Usage.None, D3D9Utils.ConvertEnum(Format), pool);


            //D3D.Surface[] dsurface = new D3D.Surface[surface.Length];
            for (int i = 0; i < surface.Length; i++)
            {
                D3D.Surface dsurface = surface[i].D3DSurface;

                SlimDX.DataRectangle rect = texture.LockRectangle(0, D3D.LockFlags.None);

                SlimDX.DataRectangle rect2 = dsurface.LockRectangle(D3D.LockFlags.None);
                                
                Memory.Copy(rect2.Data.DataPointer, rect.Data.DataPointer,  surface[i].Size);

                texture.UnlockRectangle(0);

                dsurface.UnlockRectangle();
            }



        }

        public unsafe D3D9Texture(D3D9RenderSystem rs, D3D9Texture texture)
            : base(rs, texture)
        {
            this.device = rs.D3DDevice;            

            this.pool = texture.pool;
            this.baseTexture = texture.baseTexture;
            this.texture = texture.texture;
            this.volTexture = texture.volTexture;
            this.cubeTexture = texture.cubeTexture;
        }

        public unsafe D3D9Texture(D3D9RenderSystem rs, System.Drawing.Bitmap bmp, TextureUsage usage)
            : base(rs, bmp, usage)
        {
            if ((usage & TextureUsage.RenderTarget) == TextureUsage.RenderTarget ||
               (usage & TextureUsage.Dynamic) == TextureUsage.Dynamic)
            {
                pool = D3D.Pool.Default;
            }
            else
            {
                pool = D3D.Pool.Managed;
            }
            device = rs.D3DDevice;

            Texture = new D3D.Texture(device, Width, Height, SurfaceCount, D3D.Usage.None, D3D.Format.A8R8G8B8, pool);

            SlimDX.DataRectangle rect = texture.LockRectangle(0, D3D.LockFlags.None);

            SDI.BitmapData bmpData = bmp.LockBits(new SD.Rectangle(0, 0, Width, Height), SDI.ImageLockMode.ReadOnly, SDI.PixelFormat.Format32bppArgb);

            if (rect.Pitch == Width * 4)
            {
                Memory.Copy(bmpData.Scan0, rect.Data.DataPointer, ContentSize);
            }
            else
            {
                int rowSize = Width * 4;
                byte* src = (byte*)bmpData.Scan0.ToPointer();
                byte* dst = (byte*)rect.Data.DataPointer.ToPointer();
                for (int i = 0; i < Height; i++)
                {
                    Memory.Copy(src, dst, rowSize);
                    src += rowSize;
                    dst += rowSize;
                }
            } 
            
            bmp.UnlockBits(bmpData);

            texture.UnlockRectangle(0);
        }

        public D3D9Texture(D3D9RenderSystem rs, ImageLoader image, TextureUsage usage)
            : base(rs, image, usage)
        {
            if ((usage & TextureUsage.RenderTarget) == TextureUsage.RenderTarget ||
                (usage & TextureUsage.Dynamic) == TextureUsage.Dynamic)
            {
                pool = D3D.Pool.Default;
            }
            else
            {
                pool = D3D.Pool.Managed;
            } 
            device = rs.D3DDevice;
        }

        public D3D9Texture(D3D9RenderSystem rs, Image image, TextureUsage usage)
            : base(rs, image, usage)
        {
            if ((usage & TextureUsage.RenderTarget) == TextureUsage.RenderTarget ||
                (usage & TextureUsage.Dynamic) == TextureUsage.Dynamic)
            {
                pool = D3D.Pool.Default;
            }
            else
            {
                pool = D3D.Pool.Managed;
            }
            device = rs.D3DDevice;
        }
        
        public D3D9Texture(D3D9RenderSystem rs, int length, int levelCount, TextureUsage usage, PixelFormat format)
            : base(rs, length, levelCount, usage, format)
        {
            pool = GetPool(usage);

            CubeTexture = new D3D.CubeTexture(rs.D3DDevice, length, levelCount, D3D9Utils.ConvertEnum(Usage), D3D9Utils.ConvertEnum(Format), pool);

        }
        public D3D9Texture(D3D9RenderSystem rs, int width, int height, int levelCount, TextureUsage usage, PixelFormat format)
            : base(rs, width, height, 1, levelCount, format, usage)
        {
            pool = GetPool(usage);
            device = rs.D3DDevice;

            Texture = new D3D.Texture(rs.D3DDevice, width, height, levelCount, D3D9Utils.ConvertEnum(Usage), D3D9Utils.ConvertEnum(Format), pool);
        }
        public D3D9Texture(D3D9RenderSystem rs, int width, int height, int depth, int levelCount, TextureUsage usage, PixelFormat format)
            : base(rs, width, height, depth, levelCount, format, usage)
        {
            pool = GetPool(usage);
            device = rs.D3DDevice;

            if (Type == TextureType.Texture3D)
            {
                VolumeTexture = new D3D.VolumeTexture(rs.D3DDevice, width, height, depth, levelCount, D3D9Utils.ConvertEnum(Usage), D3D9Utils.ConvertEnum(Format), pool);
            }
            else
            {
                Texture = new D3D.Texture(rs.D3DDevice, width, height, levelCount, D3D9Utils.ConvertEnum(Usage), D3D9Utils.ConvertEnum(Format), pool);
            }
        }
        protected override void LoadImage(Image image)
        {
            switch (Type)
            {
                case TextureType.CubeTexture:
                    CubeTexture = new D3D.CubeTexture(device, Width, SurfaceCount, D3D9Utils.ConvertEnum(Usage), D3D9Utils.ConvertEnum(Format), pool);

                    break;

                case TextureType.Texture1D:
                case TextureType.Texture2D:
                    Texture = new D3D.Texture(device, Width, Height, SurfaceCount, D3D9Utils.ConvertEnum(Usage), D3D9Utils.ConvertEnum(Format), pool);
                    
                    break;

                case TextureType.Texture3D:
                    VolumeTexture = new D3D.VolumeTexture(device, Width, Height, Depth, SurfaceCount, D3D9Utils.ConvertEnum(Usage), D3D9Utils.ConvertEnum(Format), pool);
                    
                    break;
            }

            throw new NotImplementedException();
        }

        protected override void unload()
        {
            switch (Type)
            {
                case TextureType.CubeTexture:
                    cubeTexture.Dispose();
                    break;

                case TextureType.Texture1D:
                case TextureType.Texture2D:
                    texture.Dispose();
                    break;

                case TextureType.Texture3D:
                    volTexture.Dispose();
                    break;
            }
        }



        protected override DataRectangle @lock(int surface, LockMode mode, Rectangle rect)
        {
            if (Type == TextureType.Texture1D ||
                Type == TextureType.Texture2D)
            {
                SlimDX.DataRectangle drect = texture.LockRectangle(surface, (System.Drawing.Rectangle)rect, D3D9Utils.ConvertEnum(mode, Usage));
                return new DataRectangle(drect.Pitch, drect.Data.DataPointer, rect.Width, rect.Height, Format);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        protected override DataBox @lock(int surface, LockMode mode, Box box)
        {
            if (Type == TextureType.Texture3D)
            {
                D3D.Box d3dbx = new D3D.Box();
                d3dbx.Back = box.Back;
                d3dbx.Bottom = box.Bottom;
                d3dbx.Front = box.Front;
                d3dbx.Left = box.Left;
                d3dbx.Right = box.Right;
                d3dbx.Top = box.Top;

                SlimDX.DataBox dbox = volTexture.LockBox(surface, d3dbx, D3D9Utils.ConvertEnum(mode, Usage));

                return new DataBox(dbox.RowPitch, dbox.SlicePitch, dbox.Data.DataPointer, box.Width, box.Height, box.Depth, Format);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        protected override DataRectangle @lock(int surface, CubeMapFace cubemapFace, LockMode mode, Rectangle rect)
        {
            if (Type == TextureType.CubeTexture)
            {
                SlimDX.DataRectangle drect = cubeTexture.LockRectangle(ConvertEnum(cubemapFace), surface, (System.Drawing.Rectangle)rect, D3D9Utils.ConvertEnum(mode, Usage));

                return new DataRectangle(drect.Pitch, drect.Data.DataPointer, rect.Width, rect.Height, Format);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }


        protected override void unlock(int surface)
        {
            switch (Type)
            {
                case TextureType.CubeTexture:
                    throw new InvalidOperationException();

                case TextureType.Texture1D:
                case TextureType.Texture2D:
                    texture.UnlockRectangle(surface);
                    break;
                case TextureType.Texture3D:
                    volTexture.UnlockBox(surface);
                    break;
            }
        }

        protected override void unlock(CubeMapFace cubemapFace, int surface)
        {
            if (Type == TextureType.CubeTexture)
            {
                cubeTexture.UnlockRectangle(ConvertEnum(cubemapFace), surface);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        protected override void dispose(bool disposing)
        {
            base.dispose(disposing);
            if (disposing)
            {
                if (volTexture != null)
                {
                    volTexture.Dispose();
                }

                if (texture != null)
                {
                    texture.Dispose();
                }

                if (cubeTexture != null)
                {
                    cubeTexture.Dispose();
                }
            }
            volTexture = null;
            texture = null;
            cubeTexture = null;

            baseTexture = null;
        }
    }
}

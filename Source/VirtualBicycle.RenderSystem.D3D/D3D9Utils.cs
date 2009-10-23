using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Media;
using SlimDX.Direct3D9;
using D3D = SlimDX.Direct3D9;

namespace VirtualBicycle.RenderSystem.D3D9
{
    internal sealed class D3D9Utils
    {
        static D3D9Utils()
        {
            fmtConv = new Format[(int)PixelFormat.Count];

            for (int i = 0; i < fmtConv.Length; i++)
            {
                fmtConv[i] = Format.Unknown;
            }

            fmtConv[(int)PixelFormat.A16B16G16R16] = Format.A16B16G16R16;
            fmtConv[(int)PixelFormat.A16B16G16R16F] = Format.A16B16G16R16F;
            fmtConv[(int)PixelFormat.A1R5G5B5] = Format.A1R5G5B5;
            fmtConv[(int)PixelFormat.A2B10G10R10] = Format.A2B10G10R10;
            fmtConv[(int)PixelFormat.A2R10G10B10] = Format.A2R10G10B10;
            fmtConv[(int)PixelFormat.A32B32G32R32F] = Format.A32B32G32R32F;
            fmtConv[(int)PixelFormat.A4L4] = Format.A4L4;
            fmtConv[(int)PixelFormat.A4R4G4B4] = Format.A4R4G4B4;
            fmtConv[(int)PixelFormat.A8] = Format.A8;
            fmtConv[(int)PixelFormat.A8B8G8R8] = Format.A8B8G8R8;
            fmtConv[(int)PixelFormat.A8L8] = Format.A8L8;
            fmtConv[(int)PixelFormat.A8R8G8B8] = Format.A8R8G8B8;
            fmtConv[(int)PixelFormat.DXT1] = Format.Dxt1;
            fmtConv[(int)PixelFormat.DXT2] = Format.Dxt2;
            fmtConv[(int)PixelFormat.DXT3] = Format.Dxt3;
            fmtConv[(int)PixelFormat.DXT4] = Format.Dxt4;
            fmtConv[(int)PixelFormat.DXT5] = Format.Dxt5;
            fmtConv[(int)PixelFormat.G16R16] = Format.G16R16;
            fmtConv[(int)PixelFormat.G16R16F] = Format.G16R16F;
            fmtConv[(int)PixelFormat.G32R32F] = Format.G32R32F;
            fmtConv[(int)PixelFormat.L16] = Format.L16;

            fmtConv[(int)PixelFormat.L8] = Format.L8;
            fmtConv[(int)PixelFormat.R16F] = Format.R16F;
            fmtConv[(int)PixelFormat.R32F] = Format.R32F;
            fmtConv[(int)PixelFormat.R3G3B2] = Format.R3G3B2;
            fmtConv[(int)PixelFormat.R5G6B5] = Format.R5G6B5;
            fmtConv[(int)PixelFormat.R8G8B8] = Format.R8G8B8;
            fmtConv[(int)PixelFormat.X8B8G8R8] = Format.X8B8G8R8;
            fmtConv[(int)PixelFormat.X8R8G8B8] = Format.X8R8G8B8;


            fmtConv[(int)PixelFormat.Depth15Stencil1] = Format.D15S1;
            fmtConv[(int)PixelFormat.Depth16] = Format.D16;
            fmtConv[(int)PixelFormat.Depth16Lockable] = Format.D16Lockable;
            fmtConv[(int)PixelFormat.Depth24] = Format.D24X8;
            fmtConv[(int)PixelFormat.Depth24Stencil4] = Format.D24X4S4;
            fmtConv[(int)PixelFormat.Depth24Stencil8] = Format.D24S8;
            fmtConv[(int)PixelFormat.Depth24Stencil8Single] = Format.D24SingleS8;
            fmtConv[(int)PixelFormat.Depth32] = Format.D32;
            fmtConv[(int)PixelFormat.Depth32Lockable] = Format.D32Lockable;
            fmtConv[(int)PixelFormat.Depth32Single] = Format.D32SingleLockable;


            tsConv = new D3D.TextureStage[33];
            tsConv[(int)TextureStage.AlphaArg0] = D3D.TextureStage.AlphaArg0;
            tsConv[(int)TextureStage.AlphaArg1] = D3D.TextureStage.AlphaArg1;
            tsConv[(int)TextureStage.AlphaArg2] = D3D.TextureStage.AlphaArg2;
            tsConv[(int)TextureStage.AlphaOperation] = D3D.TextureStage.AlphaOperation;
            tsConv[(int)TextureStage.BumpEnvironmentLOffset] = D3D.TextureStage.BumpEnvironmentLOffset;
            tsConv[(int)TextureStage.BumpEnvironmentLScale] = D3D.TextureStage.BumpEnvironmentLScale;
            tsConv[(int)TextureStage.BumpEnvironmentMat00] = D3D.TextureStage.BumpEnvironmentMat00;
            tsConv[(int)TextureStage.BumpEnvironmentMat01] = D3D.TextureStage.BumpEnvironmentMat01;
            tsConv[(int)TextureStage.BumpEnvironmentMat10] = D3D.TextureStage.BumpEnvironmentMat10;
            tsConv[(int)TextureStage.BumpEnvironmentMat11] = D3D.TextureStage.BumpEnvironmentMat11;
            tsConv[(int)TextureStage.ColorArg0] = D3D.TextureStage.ColorArg0;
            tsConv[(int)TextureStage.ColorArg1] = D3D.TextureStage.ColorArg1;
            tsConv[(int)TextureStage.ColorArg2] = D3D.TextureStage.ColorArg2;
            tsConv[(int)TextureStage.ColorOperation] = D3D.TextureStage.ColorOperation;
            tsConv[(int)TextureStage.Constant] = D3D.TextureStage.Constant;
            tsConv[(int)TextureStage.ResultArg] = D3D.TextureStage.ResultArg;
            tsConv[(int)TextureStage.TexCoordIndex] = D3D.TextureStage.TexCoordIndex;
            tsConv[(int)TextureStage.TextureTransformFlags] = D3D.TextureStage.TextureTransformFlags;

        }

        static D3D.LightType[] ltConv = new D3D.LightType[3]
        {
            D3D.LightType.Point,
            D3D.LightType.Directional, 
            D3D.LightType.Spot 
        };

        static D3D.StateBlockType[] sbtConv = new D3D.StateBlockType[3]
        {
            D3D.StateBlockType.All, 
            D3D.StateBlockType.VertexState,
            D3D.StateBlockType.PixelState
        };

        public static D3D.StateBlockType ConvertEnum(StateBlockType type)
        {
            return sbtConv[(int)type];
        }
        public static D3D.ClearFlags ConvertEnum(ClearFlags flags)
        {
            D3D.ClearFlags result = SlimDX.Direct3D9.ClearFlags.None;
            if ((flags & ClearFlags.DepthBuffer) == ClearFlags.DepthBuffer)
            {
                result |= D3D.ClearFlags.ZBuffer;
            }
            if ((flags & ClearFlags.Stencil) == ClearFlags.Stencil)
            {
                result |= D3D.ClearFlags.Stencil;
            }
            if ((flags & ClearFlags.Target) == ClearFlags.Target)
            {
                result |= D3D.ClearFlags.Target;
            }
            return result;
        }

        public static D3D.LightType ConvertEnum(LightType lt)
        {
            return ltConv[(int)lt - 1];
        }

        public static D3D.LockFlags ConvertEnum(LockMode locking, BufferUsage usage)
        {
            D3D.LockFlags ret = 0;
            if (locking == LockMode.Discard)
            {

                // Only add the discard flag for dynamic usgae and default pool
                if ((usage & BufferUsage.Dynamic) != 0 &&
                    (usage & BufferUsage.Discardable) != 0)
                    ret |= D3D.LockFlags.Discard;

            }
            if (locking == LockMode.ReadOnly)
            {
                // D3D debug runtime doesn't like you locking managed buffers readonly
                // when they were created with write-only (even though you CAN read
                // from the software backed version)
                if ((usage & BufferUsage.WriteOnly) == 0)
                    ret |= D3D.LockFlags.ReadOnly;

            }
            if (locking == LockMode.NoOverwrite)
            {
                // Only add the nooverwrite flag for dynamic usgae and default pool
                if ((usage & BufferUsage.Dynamic) != 0 &&
                    (usage & BufferUsage.Discardable) != 0)
                    ret |= D3D.LockFlags.NoOverwrite;

            }

            return ret;
        }
        public static D3D.LockFlags ConvertEnum(LockMode lockmode, TextureUsage usage)
        {
            D3D.LockFlags res = LockFlags.None;

            if (lockmode == LockMode.Discard)
            {

                // Only add the discard flag for dynamic usgae and default pool
                if ((usage & TextureUsage.Dynamic) != 0 &&
                    (usage & TextureUsage.Discardable) != 0)
                    res |= D3D.LockFlags.Discard;

            }
            if (lockmode == LockMode.ReadOnly)
            {
                // D3D debug runtime doesn't like you locking managed buffers readonly
                // when they were created with write-only (even though you CAN read
                // from the software backed version)
                if ((usage & TextureUsage.WriteOnly) == 0)
                    res |= D3D.LockFlags.ReadOnly;

            }
            if (lockmode == LockMode.NoOverwrite)
            {
                // Only add the nooverwrite flag for dynamic usgae and default pool
                if ((usage & TextureUsage.Dynamic) != 0 &&
                    (usage & TextureUsage.Discardable) != 0)
                    res |= D3D.LockFlags.NoOverwrite;

            }

            return res;
        }


        public static D3D.Usage ConvertEnum(BufferUsage usage)
        {
            D3D.Usage ret = 0;

            if ((usage & BufferUsage.Dynamic) == BufferUsage.Dynamic)
            {
                // Only add the dynamic flag for the default pool, and
                // we use default pool when buffer is discardable
                if ((usage & BufferUsage.Discardable) != 0)
                    ret |= D3D.Usage.Dynamic;

            }
            if ((usage & BufferUsage.WriteOnly) == BufferUsage.WriteOnly)
            {
                ret |= D3D.Usage.WriteOnly;
            }
            return ret;
        }

        public static D3D.DeclarationUsage ConvertEnum(VertexElementUsage semantic)
        {
            switch (semantic)
            {
                case VertexElementUsage.BlendIndices:
                    return D3D.DeclarationUsage.BlendIndices;

                case VertexElementUsage.BlendWeight:
                    return D3D.DeclarationUsage.BlendWeight;

                case VertexElementUsage.Color:
                    // index makes the difference (diffuse - 0)
                    return D3D.DeclarationUsage.Color;

                //case VertexElementUsage.Color:
                //    // index makes the difference (specular - 1)
                //    return D3D.DeclarationUsage.Color;

                case VertexElementUsage.Normal:
                    return D3D.DeclarationUsage.Normal;

                case VertexElementUsage.Position:
                    return D3D.DeclarationUsage.Position;

                case VertexElementUsage.TextureCoordinate:
                    return D3D.DeclarationUsage.TextureCoordinate;

                case VertexElementUsage.Binormal:
                    return D3D.DeclarationUsage.Binormal;

                case VertexElementUsage.Tangent:
                    return D3D.DeclarationUsage.Tangent;
            } // switch

            // keep the compiler happy
            return D3D.DeclarationUsage.Position;
        }
        public static D3D.DeclarationType ConvertEnum(VertexElementFormat type)
        {
            // we only need to worry about a few types with D3D
            switch (type)
            {
                case VertexElementFormat.Rgba32:
                //case VertexElementFormat.Color_ARGB:
                case VertexElementFormat.Color:
                    return D3D.DeclarationType.Color;

                case VertexElementFormat.Single:
                    return D3D.DeclarationType.Float1;

                case VertexElementFormat.Vector2:
                    return D3D.DeclarationType.Float2;

                case VertexElementFormat.Vector3:
                    return D3D.DeclarationType.Float3;

                case VertexElementFormat.Vector4:
                    return D3D.DeclarationType.Float4;

                case VertexElementFormat.Short2:
                    return D3D.DeclarationType.Short2;

                case VertexElementFormat.Short4:
                    return D3D.DeclarationType.Short4;

                case VertexElementFormat.Byte4:
                    return D3D.DeclarationType.Ubyte4;

            } // switch

            // keep the compiler happy
            return D3D.DeclarationType.Float3;
        }


        //public static D3D.Pool ConvertEnum(Pool pool)
        //{
        //    switch (pool)
        //    {
        //        case Pool.Default:
        //            return D3D.Pool.Default;
        //        case Pool.Managed:
        //            return D3D.Pool.Managed;
        //        case Pool.Scratch:
        //            return D3D.Pool.Scratch;
        //        case Pool.SystemMemory:
        //            return D3D.Pool.SystemMemory;
        //    }
        //    return D3D.Pool.Managed;
        //}


        public static D3D.Blend ConvertEnum(Blend dv)
        {
            switch (dv)
            {
                case Blend.BlendFactor:
                    return D3D.Blend.BlendFactor;
                case Blend.BothInverseSourceAlpha:
                    return D3D.Blend.BothInverseSourceAlpha;
                case Blend.BothSourceAlpha:
                //device.SetRenderState<D3D.Blend>(RenderState.DestinationBlend, D3D.Blend.BothSourceAlpha);
                //return;
                case Blend.DestinationAlpha:
                    return D3D.Blend.DestinationAlpha;
                case Blend.DestinationColor:
                    return D3D.Blend.DestinationColor;
                case Blend.InverseBlendFactor:
                    return D3D.Blend.InverseBlendFactor;
                case Blend.InverseDestinationAlpha:
                    return D3D.Blend.InverseDestinationAlpha;
                case Blend.InverseDestinationColor:
                    return D3D.Blend.InverseDestinationColor;
                case Blend.InverseSourceAlpha:
                    return D3D.Blend.InverseSourceAlpha;
                case Blend.InverseSourceColor:
                    return D3D.Blend.InverseSourceColor;
                case Blend.One:
                    return D3D.Blend.One;
                case Blend.SourceAlpha:
                    return D3D.Blend.SourceAlpha;
                case Blend.SourceAlphaSaturation:
                    return D3D.Blend.SourceAlphaSaturated;
                case Blend.SourceColor:
                    return D3D.Blend.SourceColor;
                case Blend.Zero:
                    return D3D.Blend.Zero;
            }
            return 0;
        }
        public static Blend ConvertEnum(D3D.Blend dv)
        {
            switch (dv)
            {
                case D3D.Blend.BlendFactor:
                    return Blend.BlendFactor;
                case D3D.Blend.BothInverseSourceAlpha:
                    return Blend.BlendFactor;
                //case D3D.Blend.BothSourceAlpha:
                //    return Blend.BlendFactor;
                case D3D.Blend.DestinationAlpha:
                    return Blend.BlendFactor;
                case D3D.Blend.DestinationColor:
                    return Blend.BlendFactor;
                case D3D.Blend.InverseBlendFactor:
                    return Blend.BlendFactor;
                case D3D.Blend.InverseDestinationAlpha:
                    return Blend.BlendFactor;
                case D3D.Blend.InverseDestinationColor:
                    return Blend.BlendFactor;
                case D3D.Blend.InverseSourceAlpha:
                    return Blend.BlendFactor;
                case D3D.Blend.InverseSourceColor:
                    return Blend.BlendFactor;
                case D3D.Blend.One:
                    return Blend.BlendFactor;
                case D3D.Blend.SourceAlpha:
                    return Blend.BlendFactor;
                case D3D.Blend.SourceAlphaSaturated:
                    return Blend.BlendFactor;
                case D3D.Blend.SourceColor:
                    return Blend.BlendFactor;
                case D3D.Blend.Zero:
                    return Blend.BlendFactor;
            }
            return 0;
        }

        static D3D.Compare[] cmpConv = new D3D.Compare[8] 
        {
            D3D.Compare.Never,
            D3D.Compare.Less,
            D3D.Compare.Equal,
            D3D.Compare.LessEqual,
            D3D.Compare.Greater, 
            D3D.Compare.NotEqual, 
            D3D.Compare.GreaterEqual, 
            D3D.Compare.Always 
        };

        public static D3D.Compare ConvertEnum(CompareFunction dv)
        {
            return cmpConv[(int)dv - 1];
            //switch (dv)
            //{
            //    case CompareFunction.Always:
            //        return Compare.Always;
            //    case CompareFunction.Equal:
            //        return Compare.Equal;
            //    case CompareFunction.Greater:
            //        return Compare.Greater;
            //    case CompareFunction.GreaterEqual:
            //        return Compare.GreaterEqual;
            //    case CompareFunction.Less:
            //        return Compare.Less;
            //    case CompareFunction.LessEqual:
            //        return Compare.LessEqual;
            //    case CompareFunction.Never:
            //        return Compare.Never;
            //    case CompareFunction.NotEqual:
            //        return Compare.NotEqual;
            //}
        }
        public static CompareFunction ConvertEnum(D3D.Compare dv)
        {
            switch (dv)
            {
                case Compare.Always:
                    return CompareFunction.Always;
                case Compare.Equal:
                    return CompareFunction.Equal;
                case Compare.Greater:
                    return CompareFunction.Greater;
                case Compare.GreaterEqual:
                    return CompareFunction.GreaterEqual;
                case Compare.Less:
                    return CompareFunction.Less;
                case Compare.LessEqual:
                    return CompareFunction.LessEqual;
                case Compare.Never:
                    return CompareFunction.Never;
                case Compare.NotEqual:
                    return CompareFunction.NotEqual;
            }
            return 0;
        }

        static D3D.BlendOperation[] bldOpConv = new BlendOperation[] 
        {
            D3D.BlendOperation.Add, 
            D3D.BlendOperation.Subtract,
            D3D.BlendOperation.ReverseSubtract,
            D3D.BlendOperation.Minimum, 
            D3D.BlendOperation.Maximum 
        };

        public static D3D.BlendOperation ConvertEnum(BlendFunction dv)
        {
            return bldOpConv[(int)dv - 1];
            //switch (dv)
            //{
            //    case BlendFunction.Add:
            //        return BlendOperation.Add;
            //    case BlendFunction.Max:
            //        return BlendOperation.Maximum;
            //    case BlendFunction.Min:
            //        return BlendOperation.Minimum;
            //    case BlendFunction.ReverseSubtract:
            //        return BlendOperation.ReverseSubtract;
            //    case BlendFunction.Subtract:
            //        return BlendOperation.Subtract;
            //}
        }
        public static BlendFunction ConvertEnum(D3D.BlendOperation dv)
        {
            switch (dv)
            {
                case BlendOperation.Add:
                    return BlendFunction.Add;
                case BlendOperation.Maximum:
                    return BlendFunction.Max;
                case BlendOperation.Minimum:
                    return BlendFunction.Min;
                case BlendOperation.ReverseSubtract:
                    return BlendFunction.ReverseSubtract;
                case BlendOperation.Subtract:
                    return BlendFunction.Subtract;
            }
            return 0;
        }


        static D3D.StencilOperation[] stenConv = new D3D.StencilOperation[]
        {
            D3D.StencilOperation.Keep, 
            D3D.StencilOperation.Zero, 
            D3D.StencilOperation.Replace,
            D3D.StencilOperation.IncrementSaturate,
            D3D.StencilOperation.DecrementSaturate,
            D3D.StencilOperation.Invert, 
            D3D.StencilOperation.Increment,
            D3D.StencilOperation.Decrement
        };

        public static D3D.StencilOperation ConvertEnum(StencilOperation dv)
        {
            return stenConv[(int)dv - 1];
            //switch (dv)
            //{
            //    case StencilOperation.Decrement:
            //        return D3D.StencilOperation.Decrement;
            //    case StencilOperation.DecrementSaturation:
            //        return D3D.StencilOperation.DecrementSaturate;
            //    case StencilOperation.Increment:
            //        return D3D.StencilOperation.Increment;
            //    case StencilOperation.IncrementSaturation:
            //        return D3D.StencilOperation.IncrementSaturate;
            //    case StencilOperation.Invert:
            //        return D3D.StencilOperation.Invert;
            //    case StencilOperation.Keep:
            //        return D3D.StencilOperation.Keep;
            //    case StencilOperation.Replace:
            //        return D3D.StencilOperation.Replace;
            //    case StencilOperation.Zero:
            //        return D3D.StencilOperation.Zero;
            //}
        }
        public static StencilOperation ConvertEnum(D3D.StencilOperation dv)
        {
            switch (dv)
            {
                case D3D.StencilOperation.Decrement:
                    return StencilOperation.Decrement;
                case D3D.StencilOperation.DecrementSaturate:
                    return StencilOperation.DecrementSaturation;
                case D3D.StencilOperation.Increment:
                    return StencilOperation.Increment;
                case D3D.StencilOperation.IncrementSaturate:
                    return StencilOperation.IncrementSaturation;
                case D3D.StencilOperation.Invert:
                    return StencilOperation.Invert;
                case D3D.StencilOperation.Keep:
                    return StencilOperation.Keep;
                case D3D.StencilOperation.Replace:
                    return StencilOperation.Replace;
                case D3D.StencilOperation.Zero:
                    return StencilOperation.Zero;
            }
            return 0;
        }

        public static D3D.ColorWriteEnable ConvertEnum(ColorWriteChannels dv)
        {
            D3D.ColorWriteEnable result = 0;
            if ((dv & ColorWriteChannels.Red) == ColorWriteChannels.Red)
            {
                result |= ColorWriteEnable.Red;
            }
            if ((dv & ColorWriteChannels.Blue) == ColorWriteChannels.Blue)
            {
                result |= ColorWriteEnable.Blue;
            }
            if ((dv & ColorWriteChannels.Green) == ColorWriteChannels.Green)
            {
                result |= ColorWriteEnable.Green;
            }
            if ((dv & ColorWriteChannels.Alpha) == ColorWriteChannels.Alpha)
            {
                result |= ColorWriteEnable.Alpha;
            }
            return result;
        }
        public static ColorWriteChannels ConvertEnum(D3D.ColorWriteEnable dv)
        {
            ColorWriteChannels result = ColorWriteChannels.None;
            if ((dv & ColorWriteEnable.Red) == ColorWriteEnable.Red)
            {
                result |= ColorWriteChannels.Red;
            }
            if ((dv & ColorWriteEnable.Blue) == ColorWriteEnable.Blue)
            {
                result |= ColorWriteChannels.Blue;
            }
            if ((dv & ColorWriteEnable.Green) == ColorWriteEnable.Green)
            {
                result |= ColorWriteChannels.Green;
            }
            if ((dv & ColorWriteEnable.Alpha) == ColorWriteEnable.Alpha)
            {
                result |= ColorWriteChannels.Alpha;
            }
            return result;
        }

        static D3D.TextureAddress[] taConv = new D3D.TextureAddress[]
        {
            D3D.TextureAddress.Wrap,
            D3D.TextureAddress.Mirror,
            D3D.TextureAddress.Clamp, 
            D3D.TextureAddress.Border, 
            D3D.TextureAddress.MirrorOnce
        };


        public static D3D.TextureAddress ConvertEnum(TextureAddressMode type)
        {
            return taConv[(int)type - 1];
            // convert from ours to D3D
            //switch (type)
            //{
            //    case TextureAddressMode.Wrap:
            //        return D3D.TextureAddress.Wrap;

            //    case TextureAddressMode.Mirror:
            //        return D3D.TextureAddress.Mirror;

            //    case TextureAddressMode.Clamp:
            //        return D3D.TextureAddress.Clamp;

            //    case TextureAddressMode.Border:
            //        return D3D.TextureAddress.Border;

            //    case TextureAddressMode.MirrorOnce:
            //        return D3D.TextureAddress.MirrorOnce;
            //} // end switch

            //return 0;
        }

        public static TextureAddressMode ConvertEnum(D3D.TextureAddress ta) 
        {
            switch (ta)
            {
                case D3D.TextureAddress.Border:
                    return TextureAddressMode.Border;
                case D3D.TextureAddress.Clamp:
                    return TextureAddressMode.Clamp;
                case D3D.TextureAddress.Mirror:
                    return TextureAddressMode.Mirror;
                case D3D.TextureAddress.MirrorOnce:
                    return TextureAddressMode.MirrorOnce;
                case D3D.TextureAddress.Wrap:
                    return TextureAddressMode.Wrap;
            }
            return TextureAddressMode.Clamp;
        }


        /// <summary>
        ///    Converts our Shading enum to the D3D ShadingMode equivalent.
        /// </summary>
        /// <param name="shading"></param>
        /// <returns></returns>
        public static D3D.ShadeMode ConvertEnum(ShadeMode shading)
        {
            if (shading == ShadeMode.Flat)
                return D3D.ShadeMode.Flat;
            return D3D.ShadeMode.Gouraud;
            //switch (shading)
            //{
            //    case ShadeMode.Flat:
            //        return D3D.ShadeMode.Flat;
            //    case ShadeMode.Gouraud:
            //        return D3D.ShadeMode.Gouraud;
            //}

            //return 0;
        }
        public static ShadeMode ConvertEnum(D3D.ShadeMode shading)
        {
            if (shading == D3D.ShadeMode.Flat)
                return ShadeMode.Flat;
            return ShadeMode.Gouraud;
            //switch (shading)
            //{
            //    case D3D.ShadeMode.Flat:
            //        return ShadeMode.Flat;
            //    case D3D.ShadeMode.Gouraud:
            //        return ShadeMode.Gouraud;
            //}

            //return ShadeMode.Flat;
        }

        static D3D.FogMode[] fogConv = new D3D.FogMode[]
        {
            D3D.FogMode.None,
            D3D.FogMode.Exponential,
            D3D.FogMode.ExponentialSquared,
            D3D.FogMode.Linear
        };

        public static D3D.FogMode ConvertEnum(FogMode dv)
        {
            return fogConv[(int)dv];
            //switch (dv)
            //{
            //    case FogMode.Exponent:
            //        return D3D.FogMode.Exponential;
            //    case FogMode.ExponentSquared:
            //        return D3D.FogMode.ExponentialSquared;
            //    case FogMode.Linear:
            //        return D3D.FogMode.Linear;
            //    case FogMode.None:
            //        return D3D.FogMode.None;
            //}
            //return D3D.FogMode.None;
        }
        public static FogMode ConvertEnum(D3D.FogMode dv)
        {
            switch (dv)
            {
                case D3D.FogMode.Exponential:
                    return FogMode.Exponent;
                case D3D.FogMode.ExponentialSquared:
                    return FogMode.ExponentSquared;
                case D3D.FogMode.Linear:
                    return FogMode.Linear;
                case D3D.FogMode.None:
                    return FogMode.None;
            }
            return FogMode.None;
        }

        public static D3D.TextureWrapping ConvertEnum(TextureWrapCoordinates dv)
        {
            TextureWrapping result = TextureWrapping.None;
            if ((dv & TextureWrapCoordinates.Zero) == TextureWrapCoordinates.Zero)
            {
                result |= TextureWrapping.WrapCoordinate0;
            }
            if ((dv & TextureWrapCoordinates.One) == TextureWrapCoordinates.One)
            {
                result |= TextureWrapping.WrapCoordinate1;
            }
            if ((dv & TextureWrapCoordinates.Two) == TextureWrapCoordinates.Two)
            {
                result |= TextureWrapping.WrapCoordinate2;
            }
            if ((dv & TextureWrapCoordinates.Three) == TextureWrapCoordinates.Three)
            {
                result |= TextureWrapping.WrapCoordinate3;
            }
            return result;
        }
        public static TextureWrapCoordinates ConvertEnum(D3D.TextureWrapping dv)
        {
            TextureWrapCoordinates result = TextureWrapCoordinates.None;
            if ((dv & TextureWrapping.WrapCoordinate0) == TextureWrapping.WrapCoordinate0)
            {
                result |= TextureWrapCoordinates.Zero;
            }
            if ((dv & TextureWrapping.WrapCoordinate1) == TextureWrapping.WrapCoordinate1)
            {
                result |= TextureWrapCoordinates.One;
            }
            if ((dv & TextureWrapping.WrapCoordinate2) == TextureWrapping.WrapCoordinate2)
            {
                result |= TextureWrapCoordinates.Two;
            }
            if ((dv & TextureWrapping.WrapCoordinate3) == TextureWrapping.WrapCoordinate3)
            {
                result |= TextureWrapCoordinates.Three;
            }
            return result;
        }

        static D3D.Format[] fmtConv;



        public static D3D.Format ConvertEnum(PixelFormat format)
        {
            return fmtConv[(int)format];
            //switch (format)
            //{
            //    case PixelFormat.A8:
            //        return D3D.Format.A8;
            //    case PixelFormat.L8:
            //        return D3D.Format.L8;
            //    case PixelFormat.L16:
            //        return D3D.Format.L16;
            //    case PixelFormat.A4L4:
            //        return D3D.Format.A4L4;
            //    case PixelFormat.A8L8:
            //        return D3D.Format.A8L8;	// Assume little endian here
            //    case PixelFormat.R3G3B2:
            //        return D3D.Format.R3G3B2;
            //    case PixelFormat.A1R5G5B5:
            //        return D3D.Format.A1R5G5B5;
            //    case PixelFormat.A4R4G4B4:
            //        return D3D.Format.A4R4G4B4;
            //    case PixelFormat.R5G6B5:
            //        return D3D.Format.R5G6B5;
            //    case PixelFormat.R8G8B8:
            //        return D3D.Format.R8G8B8;
            //    case PixelFormat.X8R8G8B8:
            //        return D3D.Format.X8R8G8B8;
            //    case PixelFormat.A8R8G8B8:
            //        return D3D.Format.A8R8G8B8;
            //    case PixelFormat.X8B8G8R8:
            //        return D3D.Format.X8B8G8R8;
            //    case PixelFormat.A8B8G8R8:
            //        return D3D.Format.A8B8G8R8;
            //    case PixelFormat.A2R10G10B10:
            //        return D3D.Format.A2R10G10B10;
            //    case PixelFormat.A2B10G10R10:
            //        return D3D.Format.A2B10G10R10;
            //    case PixelFormat.R16F:
            //        return D3D.Format.R16F;
            //    case PixelFormat.A16B16G16R16F:
            //        return D3D.Format.A16B16G16R16F;
            //    case PixelFormat.R32F:
            //        return D3D.Format.R32F;
            //    case PixelFormat.A32B32G32R32F:
            //        return D3D.Format.A32B32G32R32F;
            //    case PixelFormat.A16B16G16R16:
            //        return D3D.Format.A16B16G16R16;
            //    case PixelFormat.DXT1:
            //        return D3D.Format.Dxt1;
            //    case PixelFormat.DXT2:
            //        return D3D.Format.Dxt2;
            //    case PixelFormat.DXT3:
            //        return D3D.Format.Dxt3;
            //    case PixelFormat.DXT4:
            //        return D3D.Format.Dxt4;
            //    case PixelFormat.DXT5:
            //        return D3D.Format.Dxt5;
            //    default:
            //        return D3D.Format.Unknown;
            //}
        }
        public static Usage ConvertEnum(TextureUsage usage)
        {
            Usage result = Usage.None;
            if ((usage & TextureUsage.AutoMipMap) == TextureUsage.AutoMipMap)
            {
                result |= Usage.AutoGenerateMipMap;
            }
            if ((usage & TextureUsage.Dynamic) == TextureUsage.Dynamic)
            {
                result |= Usage.Dynamic;
            }
            if ((usage & TextureUsage.WriteOnly) == TextureUsage.WriteOnly)
            {
                result |= Usage.WriteOnly;
            }
            if ((usage & TextureUsage.RenderTarget) == TextureUsage.RenderTarget)
            {
                result |= Usage.RenderTarget;
            }

            return result;
        }

        //static D3D.Format[] depConv = new D3D.Format[]
        //{
        //    D3D.Format.Unknown,
        //    D3D.Format.D15S1,
        //    D3D.Format.D16,
        //    D3D.Format.D16Lockable,
        //    D3D.Format.D24X8, 
        //    D3D.Format.D24S8,
        //    D3D.Format.D24X4S4,
        //    D3D.Format.D24SingleS8,
        //    D3D.Format.D32,
        //    D3D.Format.D32Lockable,
        //    D3D.Format.D32SingleLockable
        //};

        //public static D3D.Format ConvertEnum(DepthFormat format)
        //{
        //    return depConv[(int)format];
        //}


        public static PixelFormat ConvertEnum(D3D.Format format) 
        {
            switch (format)
            {
                case Format.D15S1:
                    return PixelFormat.Depth15Stencil1;
                case Format.D16:
                    return PixelFormat.Depth16;
                case Format.D16Lockable:
                    return PixelFormat.Depth16Lockable;
                case Format.D24S8:
                    return PixelFormat.Depth24Stencil8;
                case Format.D24SingleS8:
                    return PixelFormat.Depth24Stencil8Single;
                case Format.D24X4S4:
                    return PixelFormat.Depth24Stencil4;
                case Format.D24X8:
                    return PixelFormat.Depth24;
                case Format.D32:
                    return PixelFormat.Depth32;
                case Format.D32Lockable:
                    return PixelFormat.Depth32Lockable;
                case Format.D32SingleLockable:
                    return PixelFormat.Depth32Single;

                case Format.Dxt1:
                    return PixelFormat.DXT1;
                case Format.Dxt2:
                    return PixelFormat.DXT2;
                case Format.Dxt3:
                    return PixelFormat.DXT3;
                case Format.Dxt4:
                    return PixelFormat.DXT4;
                case Format.Dxt5:
                    return PixelFormat.DXT5;

                //case Format.A1:
                case Format.A16B16G16R16:
                    return PixelFormat.A16B16G16R16;
                case Format.A16B16G16R16F:
                    return PixelFormat.A16B16G16R16F;
                case Format.A1R5G5B5:
                    return PixelFormat.A1R5G5B5;
                case Format.A2B10G10R10 :
                    return PixelFormat.A2B10G10R10;
                case Format.A2R10G10B10 :
                    return PixelFormat.A2R10G10B10;
                //case Format.A2W10V10U10:

                case Format.A32B32G32R32F:
                    return PixelFormat.A32B32G32R32F;
                case Format.A4L4:
                    return PixelFormat.A4L4;
                case Format.A4R4G4B4:
                    return PixelFormat.A4R4G4B4;
                case Format.A8:
                    return PixelFormat.A8;
                case Format.A8B8G8R8:
                    return PixelFormat.A8B8G8R8;
                case Format.A8L8:
                    return PixelFormat.A8L8;
                //case Format.A8P8:
               // case Format.A8R3G3B2:

                case Format.A8R8G8B8:
                    return PixelFormat.A8R8G8B8;
                //case Format.ATI_R2VB:
                //case Format.BinaryBuffer:
                //case Format.CxV8U8:
                    
                case Format.G16R16:
                    return PixelFormat.G16R16;
                case Format.G16R16F:
                    return PixelFormat.G16R16F;
                case Format.G32R32F:
                    return PixelFormat.G32R32F;
                case Format.L16:
                    return PixelFormat.L16;
                case Format.L8:
                    return PixelFormat.L8;
                case Format.R16F:
                    return PixelFormat.R16F;
                case Format.R32F:
                    return PixelFormat.R32F;
                case Format.R3G3B2:
                    return PixelFormat.R3G3B2;
                case Format.R5G6B5:
                    return PixelFormat.R5G6B5;
                case Format.R8G8B8:
                    return PixelFormat.R8G8B8;
                case Format.X8B8G8R8:
                    return PixelFormat.X8B8G8R8;
                case Format.X8R8G8B8:
                    return PixelFormat.X8R8G8B8;
            }
            return PixelFormat.Unknown;
        }
        //public static DepthFormat ConvertEnum2(D3D.Format format)
        //{
        //    switch (format)
        //    {
        //        case Format.D15S1:
        //            return DepthFormat.Depth15Stencil1;
        //        case Format.D16:
        //            return DepthFormat.Depth16;
        //        case Format.D16Lockable:
        //            return DepthFormat.Depth16Lockable;
        //        case Format.D24S8:
        //            return DepthFormat.Depth24Stencil8;
        //        case Format.D24SingleS8:
        //            return DepthFormat.Depth24Stencil8Single;
        //        case Format.D24X4S4:
        //            return DepthFormat.Depth24Stencil4;
        //        case Format.D24X8:
        //            return DepthFormat.Depth24;
        //        case Format.D32:
        //            return DepthFormat.Depth32;
        //        case Format.D32Lockable:
        //            return DepthFormat.Depth32Lockable;
        //        case Format.D32SingleLockable:
        //            return DepthFormat.Depth32Single;
        //    }
        //    return DepthFormat.Unknown;
        //}

        //public static LockFlags LockMode2D3DLockFlag(LockMode mode)
        //{
        //    LockFlags result = LockFlags.None;
        //    if ((mode & LockMode.Discard) == LockMode.Discard)
        //    {
        //        result |= LockFlags.Discard;
        //    }
        //    if ((mode & LockMode.NoOverwrite) == LockMode.NoOverwrite)
        //    {
        //        result |= LockFlags.NoOverwrite;
        //    }
        //    if ((mode & LockMode.ReadOnly) == LockMode.ReadOnly)
        //    {
        //        result |= LockFlags.ReadOnly;
        //    }
        //    return result;
        //}

        //static D3D.SamplerState[] ssConv = new D3D.SamplerState[13]
        //{
        //    D3D.SamplerState.AddressU,
        //    D3D.SamplerState.AddressV,
        //    D3D.SamplerState.AddressW,
        //    D3D.SamplerState.BorderColor,
        //    D3D.SamplerState.MagFilter,
        //    D3D.SamplerState.MinFilter,
        //    D3D.SamplerState.MipFilter,
        //    D3D.SamplerState.MipMapLodBias,
        //    D3D.SamplerState.MaxMipLevel,        
        //    D3D.SamplerState.MaxAnisotropy,        
        //    D3D.SamplerState.SrgbTexture,
        //    D3D.SamplerState.ElementIndex,
        //    D3D.SamplerState.DisplacementMapOffset 
        //};

        //public static D3D.SamplerState ConvertEnum(SamplerState state)
        //{
        //    return ssConv[(int)state - 1];
        //}


        static D3D.TextureFilter[] tfConv = new D3D.TextureFilter[] 
        {
            D3D.TextureFilter.None, 
            D3D.TextureFilter.Point,
            D3D.TextureFilter.Linear,
            D3D.TextureFilter.Anisotropic,
            0, 0,
            D3D.TextureFilter.PyramidalQuad,
            D3D.TextureFilter.GaussianQuad, 
            D3D.TextureFilter.ConvolutionMono
        };

        public static D3D.TextureFilter ConvertEnum(TextureFilter filter)
        {
            return tfConv[(int)filter - 1];
        }

        public static TextureFilter ConvertEnum(D3D.TextureFilter ft)
        {
            switch (ft)
            {
                case D3D.TextureFilter.Anisotropic:
                    return TextureFilter.Anisotropic;
                case D3D.TextureFilter.ConvolutionMono:
                    return TextureFilter.GaussianQuad;
                case D3D.TextureFilter.GaussianQuad:
                    return TextureFilter.GaussianQuad;
                case D3D.TextureFilter.Linear:
                    return TextureFilter.Linear;
                case D3D.TextureFilter.None:
                    return TextureFilter.None;
                case D3D.TextureFilter.Point:
                    return TextureFilter.Point;
                case D3D.TextureFilter.PyramidalQuad:
                    return TextureFilter.PyramidalQuad;
            }
            return TextureFilter.None;
        }


        static D3D.TextureStage[] tsConv;

        public static D3D.TextureStage ConvertEnum(TextureStage stage)
        {
            return tsConv[(int)stage];
        }


        static D3D.IncludeType[] itConv = new D3D.IncludeType[2]
        {
            D3D.IncludeType.Local,
            D3D.IncludeType.System 
        };

        public static D3D.IncludeType ConvertEnum(IncludeType incType)
        {
            return itConv[(int)incType];
        }

        public static IncludeType ConvertEnum(D3D.IncludeType incType) 
        {
            if (incType == D3D.IncludeType.Local)
            {
                return IncludeType.Local;
            }
            return IncludeType.System;
        }
    }
}

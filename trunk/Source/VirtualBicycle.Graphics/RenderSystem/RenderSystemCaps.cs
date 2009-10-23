using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.RenderSystem
{
    [Flags]
    public enum CompareCaps
    {
        Always = 128,
        Equal = 4,
        Greater = 16,
        GreaterEqual = 64,
        Less = 2,
        LessEqual = 8,
        Never = 1,
        NotEqual = 32
    }
    [Flags]
    public enum StencilCaps
    {
        Decrement = 128,
        DecrementClamp = 16,
        Increment = 64,
        IncrementClamp = 8,
        Invert = 32,
        Keep = 1,
        Replace = 4,
        TwoSided = 256,
        Zero = 2
    }
    [Flags]
    public enum ShadeCaps
    {
        AlphaGouraudBlend = 16384,
        ColorGouraudRgb = 8,
        FogGouraud = 524288,
        SpecularGouraudRgb = 512
    }
    [Flags]
    public enum TextureCaps
    {
        Alpha = 4,
        AlphaPalette = 128,
        CubeMap = 2048,
        CubeMapPow2 = 131072,
        MipCubeMap = 65536,
        MipMap = 16384,
        MipVolumeMap = 32768,
        NonPow2Conditional = 256,
        NoProjectedBumpEnvironment = 2097152,
        Perspective = 1,
        Pow2 = 2,
        Projected = 1024,
        SquareOnly = 32,
        TextureRepeatNotScaledBySize = 64,
        VolumeMap = 8192,
        VolumeMapPow2 = 262144
    }
    [Flags]
    public enum TextureOperationCaps
    {
        Add = 64,
        AddSigned = 128,
        AddSigned2X = 256,
        AddSmooth = 1024,
        BlendCurrentAlpha = 32768,
        BlendDiffuseAlpha = 2048,
        BlendFactorAlpha = 8192,
        BlendTextureAlpha = 4096,
        BlendTextureAlphaPM = 16384,
        BumpEnvironmentMap = 2097152,
        BumpEnvironmentMapLuminance = 4194304,
        Disable = 1,
        DotProduct3 = 8388608,
        Lerp = 33554432,
        Modulate = 8,
        Modulate2X = 16,
        Modulate4X = 32,
        ModulateAlphaAddColor = 131072,
        ModulateColorAddAlpha = 262144,
        ModulateInvAlphaAddColor = 524288,
        ModulateInvColorAddAlpha = 1048576,
        MultiplyAdd = 16777216,
        Premodulate = 65536,
        SelectArg1 = 2,
        SelectArg2 = 4,
        Subtract = 512
    }
    [Flags]
    public enum TextureAddressCaps
    {
        Border = 8,
        Clamp = 4,
        IndependentUV = 16,
        Mirror = 2,
        MirrorOnce = 32,
        Wrap = 1
    }

    [Flags]
    public enum VertexProcessingCaps
    {
        DirectionalLights = 8,
        LocalViewer = 32,
        MaterialSource7 = 2,
        NoTexGenNonLocalViewer = 512,
        PositionalLights = 16,
        TexGenSphereMap = 256,
        TextureGen = 1,
        Tweening = 64
    }
    [Flags]
    public enum VertexShaderCaps
    {
        None,
        Predication
    }
    [Flags]
    public enum DeclarationTypeCaps
    {
        Dec3N = 128,
        HalfFour = 512,
        HalfTwo = 256,
        Short2N = 4,
        Short4N = 8,
        UByte4 = 1,
        UByte4N = 2,
        UDec3 = 64,
        UShort2N = 16,
        UShort4N = 32
    }
    [Flags]
    public enum BlendCaps
    {
        BlendFactor = 8192,
        BothInverseSourceAlpha = 4096,
        DestinationAlpha = 64,
        DestinationColor = 256,
        InverseDestinationAlpha = 128,
        InverseDestinationColor = 512,
        InverseSourceAlpha = 32,
        InverseSourceColor = 8,
        One = 2,
        SourceAlpha = 16,
        SourceAlphaSaturated = 1024,
        SourceColor = 4,
        Zero = 1
    }

    [Flags]
    public enum DeviceCaps
    {
        CanBlitSysToNonLocal = 131072,
        CanRenderAfterFlip = 2048,
        DrawPrimitives2 = 8192,
        DrawPrimitives2Extended = 32768,
        DrawPrimTLVertex = 1024,
        ExecuteSystemMemory = 16,
        ExecuteVideoMemory = 32,
        HWRasterization = 524288,
        HWTransformAndLight = 65536,
        NPatches = 16777216,
        PureDevice = 1048576,
        QuinticRTPatches = 2097152,
        RTPatches = 4194304,
        RTPatchHandleZero = 8388608,
        SeparateTextureMemory = 16384,
        TextureNonLocalVideoMemory = 4096,
        TextureSystemMemory = 256,
        TextureVideoMemory = 512,
        TLVertexSystemMemory = 64,
        TLVertexVideoMemory = 128
    }
    [Flags]
    public enum DevCaps2
    {
        AdaptiveTessNPatch = 8,
        AdaptiveTessRTPatch = 4,
        CanStretchRectFromTextures = 16,
        DMapNPatch = 2,
        PresampledMapNPatch = 32,
        StreamOffset = 1,
        VertexElementsCanShareStreamOffset = 64
    }
    [Flags]
    public enum FilterCaps
    {
        MagAnisotropic = 67108864,
        MagGaussianQuad = 268435456,
        MagLinear = 33554432,
        MagPoint = 16777216,
        MagPyramidalQuad = 134217728,
        MinAnisotropic = 1024,
        MinGaussianQuad = 4096,
        MinLinear = 512,
        MinPoint = 256,
        MinPyramidalQuad = 2048,
        MipLinear = 131072,
        MipPoint = 65536
    }
    [Flags]
    public enum LineCaps
    {
        AlphaCompare = 8,
        Antialias = 32,
        Blend = 4,
        DepthTest = 2,
        Fog = 16,
        Texture = 1
    }
    [Flags]
    public enum PixelShaderCaps
    {
        ArbitrarySwizzle = 1,
        GradientInstructions = 2,
        NoDependentReadLimit = 8,
        None = 0,
        NoTextureInstructionLimit = 16,
        Predication = 4
    }

    [Flags]
    public enum PrimitiveMiscCaps
    {
        BlendOperation = 2048,
        ClipPlanesScaledPoints = 256,
        ClipTLVertices = 512,
        ColorWriteEnable = 128,
        CullCCW = 64,
        CullCW = 32,
        CullNone = 16,
        FogAndSpecularAlpha = 65536,
        FogVertexClamped = 1048576,
        IndependentWriteMasks = 16384,
        MaskZ = 2,
        MrtIndependentBitDepths = 262144,
        MrtPostPixelShaderBlending = 524288,
        NullReference = 4096,
        PerStageConstant = 32768,
        PostBlendSrgbConvert = 2097152,
        SeparateAlphaBlend = 131072,
        TssArgTemp = 1024
    }

    [Flags]
    public enum RasterCaps
    {
        Anisotropy = 131072,
        ColorPerspective = 4194304,
        DepthBias = 67108864,
        DepthTest = 16,
        Dither = 1,
        FogRange = 65536,
        FogTable = 256,
        FogVertex = 128,
        MipMapLodBias = 8192,
        MultisampleToggle = 134217728,
        ScissorTest = 16777216,
        SlopeScaleDepthBias = 33554432,
        WBuffer = 262144,
        WFog = 1048576,
        ZBufferLessHsr = 32768,
        ZFog = 2097152
    }
   
    [Flags]
    public enum VertexFormatCaps
    {
        DoNotStripElements = 524288,
        PointSize = 1048576,
        TextureCoordCountMask = 65535
    }


    public struct PixelShader20Caps
    {
        public PixelShaderCaps Caps { get; set; }
        public int DynamicFlowControlDepth { get; set; }
        public int TempCount { get; set; }
        public int StaticFlowControlDepth { get; set; }
        public int InstructionSlotCount { get; set; }

    }
    public struct VertexShader20Caps
    {
        public VertexShaderCaps Caps { get; set; }
        public int DynamicFlowControlDepth { get; set; }
        public int TempCount { get; set; }
        public int StaticFlowControlDepth { get; set; }

    }
    /// <summary>
    /// Represents the capabilities of the hardware exposed through the Direct3D object.
    /// </summary>
    public class Capabilities
    {
        [Flags]
        enum Caps
        {
            None = 0,
            ReadScanline = 131072
        }
        [Flags]
        enum Caps2
        {
            CanAutoGenerateMipMap = 1073741824,
            CanCalibrateGamma = 1048576,
            CanManageResource = 268435456,
            DynamicTextures = 536870912,
            FullScreenGamma = 131072,
            None = 0
        }
        [Flags]
        enum Caps3
        {
            AlphaFullScreenFlipOrDiscard = 32,
            CopyToSystemMemory = 512,
            CopyToVideoMemory = 256,
            LinearToSrgbPresentation = 128,
            None = 0
        }


        Caps caps;
        Caps2 caps2;
        Caps3 caps3;

        public bool ReadScanline
        {
            get { return (caps & Caps.ReadScanline) != 0; }
            set
            {
                if (value)
                {
                    caps |= Caps.ReadScanline;
                }
                else
                {
                    caps ^= Caps.ReadScanline;
                }
            }
        }

        public bool CanAutoGenerateMipmap
        {
            get { return (caps2 & Caps2.CanAutoGenerateMipMap) != 0; }
            set
            {
                if (value)
                {
                    caps2 |= Caps2.CanAutoGenerateMipMap;
                }
                else
                {
                    caps2 ^= Caps2.CanAutoGenerateMipMap;
                }
            }
        }
        public bool CanCalibrateGamma
        {
            get { return (caps2 & Caps2.CanCalibrateGamma) != 0; }
            set
            {
                if (value)
                {
                    caps2 |= Caps2.CanCalibrateGamma;
                }
                else
                {
                    caps2 ^= Caps2.CanCalibrateGamma;
                }
            }
        }
        public bool CanManageResource
        {
            get { return (caps2 & Caps2.CanManageResource) != 0; }
            set
            {
                if (value)
                {
                    caps2 |= Caps2.CanManageResource;
                }
                else
                {
                    caps2 ^= Caps2.CanManageResource;
                }
            }
        }
        public bool DynamicTextures
        {
            get { return (caps2 & Caps2.DynamicTextures) != 0; }
            set
            {
                if (value)
                {
                    caps2 |= Caps2.DynamicTextures;
                }
                else
                {
                    caps2 ^= Caps2.DynamicTextures;
                }
            }
        }
        public bool FullScreenGamma
        {
            get { return (caps2 & Caps2.FullScreenGamma) != 0; }
            set
            {
                if (value)
                {
                    caps2 |= Caps2.FullScreenGamma;
                }
                else
                {
                    caps2 ^= Caps2.FullScreenGamma;
                }
            }
        }

        public bool AlphaFullScreenFlipOrDiscard
        {
            get { return (caps3 & Caps3.AlphaFullScreenFlipOrDiscard) != 0; }
            set
            {
                if (value)
                {
                    caps3 |= Caps3.AlphaFullScreenFlipOrDiscard;
                }
                else
                {
                    caps3 ^= Caps3.AlphaFullScreenFlipOrDiscard;
                }
            }
        }
        public bool CopyToSystemMemory
        {
            get { return (caps3 & Caps3.CopyToSystemMemory) != 0; }
            set
            {
                if (value)
                {
                    caps3 |= Caps3.CopyToSystemMemory;
                }
                else
                {
                    caps3 ^= Caps3.CopyToSystemMemory;
                }
            }
        }
        public bool CopyToVideoMemory
        {
            get { return (caps3 & Caps3.CopyToVideoMemory) != 0; }
            set
            {
                if (value)
                {
                    caps3 |= Caps3.CopyToVideoMemory;
                }
                else
                {
                    caps3 ^= Caps3.CopyToVideoMemory;
                }
            }
        }
        public bool LinearToSrgbPresentation
        {
            get { return (caps3 & Caps3.LinearToSrgbPresentation) != 0; }
            set
            {
                if (value)
                {
                    caps3 |= Caps3.LinearToSrgbPresentation;
                }
                else
                {
                    caps3 ^= Caps3.LinearToSrgbPresentation;
                }
            }
        }

        //public int AdapterOrdinal { get; set; }
        //public int AdapterOrdinalInGroup { get; set; }
        public CompareCaps AlphaCompareCaps { get; set; }


        public FilterCaps CubeTextureFilterCaps { get; set; }
        //public CursorCaps CursorCaps { get; set; }
        public DeclarationTypeCaps DeclarationTypes { get; set; }
        public CompareCaps DepthCompareCaps { get; set; }
        public BlendCaps DestinationBlendCaps { get; set; }
        public DeviceCaps DeviceCaps { get; set; }
        public DevCaps2 DeviceCaps2 { get; set; }
        //public DeviceType DeviceType { get; set; }
        public float ExtentsAdjust { get; set; }
        public VertexFormatCaps FVFCaps { get; set; }
        public float GuardBandBottom { get; set; }
        public float GuardBandLeft { get; set; }
        public float GuardBandRight { get; set; }
        public float GuardBandTop { get; set; }
        public LineCaps LineCaps { get; set; }
        public int MasterAdapterOrdinal { get; set; }
        public int MaxActiveLights { get; set; }
        public int MaxAnisotropy { get; set; }
        public float MaxNPatchTessellationLevel { get; set; }
        public int MaxPixelShader30InstructionSlots { get; set; }
        public float MaxPointSize { get; set; }
        public int MaxPrimitiveCount { get; set; }
        public int MaxPShaderInstructionsExecuted { get; set; }
        public int MaxSimultaneousTextures { get; set; }
        public int MaxStreams { get; set; }
        public int MaxStreamStride { get; set; }
        public int MaxTextureAspectRatio { get; set; }
        public int MaxTextureBlendStages { get; set; }
        public int MaxTextureHeight { get; set; }
        public int MaxTextureRepeat { get; set; }
        public int MaxTextureWidth { get; set; }
        public int MaxUserClipPlanes { get; set; }
        public int MaxVertexBlendMatrices { get; set; }
        public int MaxVertexBlendMatrixIndex { get; set; }
        public int MaxVertexIndex { get; set; }
        public int MaxVertexShader30InstructionSlots { get; set; }
        public int MaxVertexShaderConstants { get; set; }
        public float MaxVertexW { get; set; }
        public int MaxVolumeExtent { get; set; }
        public int MaxVShaderInstructionsExecuted { get; set; }
        public int NumberOfAdaptersInGroup { get; set; }
        public float PixelShader1xMaxValue { get; set; }
        public Version PixelShaderVersion { get; set; }
        public PresentInterval PresentationIntervals { get; set; }
        public PrimitiveMiscCaps PrimitiveMiscCaps { get; set; }
        public PixelShader20Caps PS20Caps { get; set; }
        public RasterCaps RasterCaps { get; set; }
        public ShadeCaps ShadeCaps { get; set; }
        public int SimultaneousRTCount { get; set; }
        public BlendCaps SourceBlendCaps { get; set; }
        public StencilCaps StencilCaps { get; set; }
        public FilterCaps StretchRectFilterCaps { get; set; }
        public TextureAddressCaps TextureAddressCaps { get; set; }
        public TextureCaps TextureCaps { get; set; }
        public FilterCaps TextureFilterCaps { get; set; }
        public TextureOperationCaps TextureOperationCaps { get; set; }
        public VertexProcessingCaps VertexProcessingCaps { get; set; }
        public Version VertexShaderVersion { get; set; }
        public FilterCaps VertexTextureFilterCaps { get; set; }
        public TextureAddressCaps VolumeTextureAddressCaps { get; set; }
        public FilterCaps VolumeTextureFilterCaps { get; set; }
        public VertexShader20Caps VS20Caps { get; set; }


        //public void Log()
        //{
        //    R3DConsole.Instance.Write("RenderSystem capabilities");
        //    R3DConsole.Instance.Write("-------------------------");
        //    R3DConsole.Instance.Write(
        //        " * Hardware generation of mipmaps: "
        //        + CanAutoGenerateMipmap.ToString());
        //    cons.WriteLine(
        //        " * Texture blending: "
        //        + this.TextureOperationCaps & TextureOperationCaps.Ble HasCapability(CapabilitiesItem.Blending).ToString());
        //    cons.WriteLine(
        //        " * Anisotropic texture filtering: "
        //        + HasCapability(CapabilitiesItem.Anisotropy).ToString());
        //    cons.WriteLine(
        //        " * Dot product texture operation: "
        //        + HasCapability(CapabilitiesItem.LayerBlendingDot3).ToString());
        //    cons.WriteLine(
        //        " * Cube mapping: "
        //        + HasCapability(CapabilitiesItem.CubeMapping).ToString());
        //    cons.WriteLine(
        //        " * Hardware stencil buffer: "
        //        + HasCapability(CapabilitiesItem.HardwareStencil).ToString());
        //    if (HasCapability(CapabilitiesItem.HardwareStencil))
        //    {
        //        cons.WriteLine(
        //            "   - Stencil depth: "
        //            + StencilBufferBitDepth.ToString());
        //        cons.WriteLine(
        //            "   - Two sided stencil support: "
        //            + HasCapability(CapabilitiesItem.TwoSidedStencil).ToString());
        //        cons.WriteLine(
        //            "   - Wrap stencil values: "
        //            + HasCapability(CapabilitiesItem.StencilWrap).ToString());
        //    }
        //    cons.WriteLine(
        //        " * Hardware vertex / index buffers: "
        //        + HasCapability(CapabilitiesItem.VertexIndexBuffer).ToString());
        //    cons.WriteLine(
        //        " * Vertex programs: "
        //        + HasCapability(CapabilitiesItem.VertexShader).ToString());
        //    if (HasCapability(CapabilitiesItem.VertexShader))
        //    {
        //        cons.WriteLine(
        //            "   - Max vertex program version: "
        //            + MaxVertexProgramVersion);
        //    }
        //    cons.WriteLine(
        //        " * Fragment programs: "
        //        + HasCapability(CapabilitiesItem.PixelShader).ToString());
        //    if (HasCapability(CapabilitiesItem.PixelShader))
        //    {
        //        cons.WriteLine(
        //            "   - Max fragment program version: "
        //            + MaxFragmentProgramVersion);
        //    }

        //    cons.WriteLine(
        //        " * Texture Compression: "
        //        + HasCapability(CapabilitiesItem.TextureCompression).ToString());
        //    if (HasCapability(CapabilitiesItem.TextureCompression))
        //    {
        //        cons.WriteLine(
        //            "   - DXT: "
        //            + HasCapability(CapabilitiesItem.TextureCompressionDXT).ToString());
        //        cons.WriteLine(
        //            "   - VTC: "
        //            + HasCapability(CapabilitiesItem.TextureCompressionVTC).ToString());
        //    }

        //    cons.WriteLine(
        //        " * Scissor Rectangle: "
        //        + HasCapability(CapabilitiesItem.ScissorTest).ToString());
        //    cons.WriteLine(
        //        " * Hardware Occlusion Query: "
        //        + HasCapability(CapabilitiesItem.HardwareOcclusion).ToString());
        //    cons.WriteLine(
        //        " * User clip planes: "
        //        + HasCapability(CapabilitiesItem.UserClipPlanes).ToString());
        //    cons.WriteLine(
        //        " * VET_UBYTE4 vertex element type: "
        //        + HasCapability(CapabilitiesItem.RSC_VERTEX_FORMAT_UBYTE4).ToString());
        //    cons.WriteLine(
        //        " * Infinite far plane projection: "
        //        + HasCapability(CapabilitiesItem.RSC_INFINITE_FAR_PLANE).ToString());
        //    cons.WriteLine(
        //        " * Hardware render-to-texture: "
        //        + HasCapability(CapabilitiesItem.RSC_HWRENDER_TO_TEXTURE).ToString());
        //    cons.WriteLine(
        //        " * Floating point textures: "
        //        + HasCapability(CapabilitiesItem.RSC_TEXTURE_FLOAT).ToString());
        //    cons.WriteLine(
        //        " * Non-power-of-two textures: "
        //        + HasCapability(CapabilitiesItem.RSC_NON_POWER_OF_2_TEXTURES).ToString());
        //    //+ (mNonPOW2TexturesLimited ? " (limited)" : ""));
        //    cons.WriteLine(
        //        " * Volume textures: "
        //        + HasCapability(CapabilitiesItem.RSC_TEXTURE_3D).ToString());
        //    cons.WriteLine(
        //        " * Multiple Render Targets: "
        //        + usNumMultiRenderTargets.ToString());
        //    cons.WriteLine(
        //        " * Point Sprites: "
        //        + HasCapability(CapabilitiesItem.RSC_POINT_SPRITES).ToString());
        //    cons.WriteLine(
        //        " * Extended point parameters: "
        //        + HasCapability(CapabilitiesItem.RSC_POINT_EXTENDED_PARAMETERS).ToString());
        //    cons.WriteLine(
        //        " * Max Point Size: "
        //        + dMaxPointSize.ToString());
        //    cons.WriteLine(
        //        " * Vertex texture fetch: "
        //        + HasCapability(CapabilitiesItem.RSC_VERTEX_TEXTURE_FETCH).ToString());
        //    if (HasCapability(CapabilitiesItem.RSC_VERTEX_TEXTURE_FETCH))
        //    {
        //        cons.WriteLine(
        //            "   - Max vertex textures: "
        //                + usNumVertexTextureUnits.ToString());
        //        cons.WriteLine(
        //            "   - Vertex textures shared: "
        //            + bVertexTextureUnitsShared.ToString());

        //    }

            
        //}
    }
}

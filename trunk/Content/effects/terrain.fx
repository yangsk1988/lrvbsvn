#include "ShadowMapping.fxh"
#include "post.fxh"

float4 I_a;
float4 I_d;
float4 I_s;

// materials 
float4 k_a;
float4 k_d;
float4 k_s;
float power;

float fogDensity;
float4 fogColor;


float3 lightDir;

texture normalMap;
texture colorMap;

texture indexMap;

texture detailMap1;
texture detailMap2;
texture detailMap3;
texture detailMap4;

texture detailNrmMap1;
texture detailNrmMap2;
texture detailNrmMap3;
texture detailNrmMap4;
texture shadowMap;

sampler ShadowMapSampler = sampler_state
{
    Texture = (shadowMap);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU = BORDER;
    AddressV = BORDER;
    BORDERCOLOR = 0xffffffff;
};

sampler	NormalMapSampler = sampler_state
{
    Texture = (normalMap);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler	IndexMapSampler = sampler_state
{
    Texture = (indexMap);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler	ColorMapSampler = sampler_state
{
    Texture = (colorMap);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler	DetailMap1Sampler = sampler_state
{
    Texture = (detailMap1);

    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
    MaxAnisotropy = 8;
    AddressU = Wrap;
    AddressV = Wrap;
    MIPMAPLODBIAS = -1;
};
sampler	DetailMap2Sampler = sampler_state
{
    Texture = (detailMap2);

    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
    MaxAnisotropy = 8;
    AddressU = Wrap;
    AddressV = Wrap;
    MIPMAPLODBIAS = -1;
};
sampler	DetailMap3Sampler = sampler_state
{
    Texture = (detailMap3);

    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
    MaxAnisotropy = 8;
    AddressU = Wrap;
    AddressV = Wrap;
    MIPMAPLODBIAS = -1;
};
sampler	DetailMap4Sampler = sampler_state
{
    Texture = (detailMap4);

    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
    MaxAnisotropy = 8;
    AddressU = Wrap;
    AddressV = Wrap;
    MIPMAPLODBIAS = -1;
};


sampler	DetailNrmMap1Sampler = sampler_state
{
    Texture = (detailNrmMap1);

    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
    MaxAnisotropy = 8;
    AddressU = Wrap;
    AddressV = Wrap;
    MIPMAPLODBIAS = -1;
};
sampler	DetailNrmMap2Sampler = sampler_state
{
    Texture = (detailNrmMap2);

    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
    MaxAnisotropy = 8;
    AddressU = Wrap;
    AddressV = Wrap;
    MIPMAPLODBIAS = -1;
};
sampler	DetailNrmMap3Sampler = sampler_state
{
    Texture = (detailNrmMap3);

    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
    MaxAnisotropy = 8;
    AddressU = Wrap;
    AddressV = Wrap;
    MIPMAPLODBIAS = -1;
};
sampler	DetailNrmMap4Sampler = sampler_state
{
    Texture = (detailNrmMap4);

    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
    MaxAnisotropy = 8;
    AddressU = Wrap;
    AddressV = Wrap;
    MIPMAPLODBIAS = -1;
};

float4 PSMain (
    float2 tc0 : TEXCOORD0,
    float3 viewDir : TEXCOORD4,
    float4 smLgtPos : TEXCOORD5,
    float2 depth : TEXCOORD6
) : COLOR
{
    float4 idxVal = tex2D(IndexMapSampler, tc0);
    float2 dmtc = tc0 * 16 * 2;


    float3 N = 2 * ((float3)tex2D(NormalMapSampler, tc0) - float3(0.5,0.5,0.5));
    float4 n1 = float4(0,0,0,0);
    float4 n2 = float4(0,0,0,0);
    float4 n3 = float4(0,0,0,0);
    float4 n4 = float4(0,0,0,0);  

    float4 clr1 = float4(0,0,0,0);
    float4 clr2 = float4(0,0,0,0);
    float4 clr3 = float4(0,0,0,0);
    float4 clr4 = float4(0,0,0,0);

    if (idxVal.a>0.1)
    {
        clr1 = tex2D(DetailMap1Sampler, dmtc);
        n1 = tex2D(DetailNrmMap1Sampler, dmtc);
    }
    if (idxVal.r>0.1)
    {
        clr2 = tex2D(DetailMap2Sampler, dmtc);
        n2 = tex2D(DetailNrmMap2Sampler, dmtc);
    }
    if (idxVal.g>0.1)
    {
        clr3 = tex2D(DetailMap3Sampler, dmtc);
        n3 = tex2D(DetailNrmMap3Sampler, dmtc);
    }
    if (idxVal.b>0.1)
    {
        clr4 = tex2D(DetailMap4Sampler, dmtc); 
        n4 = tex2D(DetailNrmMap4Sampler, dmtc);
    }


    N = normalize(N * (n1 * idxVal.a + n2 * idxVal.r + n3 * idxVal.g + n4 * idxVal.b));
   

    float ndl = -dot(N, lightDir);

    float3 R = normalize(2 * ndl * N + lightDir);

    float4 Amb = k_a * I_a;

    float4 Spec = k_s * I_s * pow(max(0, dot(R, viewDir)), power);



    float2 ShadowTexC = (smLgtPos.xy / smLgtPos.w) * 0.5 + float2( 0.5, 0.5 );
    ShadowTexC.y = 1.0f - ShadowTexC.y;

    float shd = 0;
    float pixDepth = smLgtPos.z / smLgtPos.w;
     
    shd += (tex2D( ShadowMapSampler, ShadowTexC + float2(-1.5/SMAP_SIZE,  0.5/SMAP_SIZE) ) + SHADOW_EPSILON < pixDepth)? 0.0f: 1.0f;  
    shd += (tex2D( ShadowMapSampler, ShadowTexC + float2( 0.5/SMAP_SIZE,  0.5/SMAP_SIZE) ) + SHADOW_EPSILON < pixDepth)? 0.0f: 1.0f;  
    shd += (tex2D( ShadowMapSampler, ShadowTexC + float2(-1.5/SMAP_SIZE, -1.5/SMAP_SIZE) ) + SHADOW_EPSILON < pixDepth)? 0.0f: 1.0f;  
    shd += (tex2D( ShadowMapSampler, ShadowTexC + float2( 0.5/SMAP_SIZE, -1.5/SMAP_SIZE) ) + SHADOW_EPSILON < pixDepth)? 0.0f: 1.0f;  
    shd *= 0.25f;


    //if (depth.x > 0.9955)
    //{
    //    shd = shd + (depth.x - 0.9955) * 750;
    //    if (shd > 1) shd = 1;
    //}

    float4 Diff = ( max( 0, ndl ) * shd * ( 1 - I_a ) + I_a ) * k_d;

    //return max( 0, ndl ) * k_d * I_d;
      

    float4 resultColor =
       tex2D(ColorMapSampler, tc0) * 
           (clr1 * idxVal.a + clr2 * idxVal.r + clr3 * idxVal.g + clr4 * idxVal.b) *
           (Amb + Diff) + Spec;
    float f = 1.0 / exp(depth.y * fogDensity);
    resultColor = lerp(fogColor, resultColor,  f);
    return Exposure(resultColor);
}

//-----------------------------------------------------------------------------

float3 cameraPos;
float4x4 mvp;
float4x4 worldT;
float4x4 smTrans;

texture disMap;



float heightScale;

sampler	DisMapSampler = sampler_state
{
    Texture = (disMap);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};



void VSMain(
    float4 iPos : POSITION,
    out float4 oPos : POSITION,
    out float2 oTex1 : TEXCOORD0,
    out float3 viewDir : TEXCOORD4,
    out float4 smLgtPos : TEXCOORD5,
    out float2 depth : TEXCOORD6
)
{
    oTex1 = float2(iPos.x/513, iPos.z/513);

    float height = tex2Dlod(DisMapSampler, float4(oTex1, 0,0)).r;

    iPos.y = height * heightScale;

    smLgtPos = mul(iPos , smTrans);

    oPos = mul(iPos, mvp);

    depth.x = oPos.z / oPos.w;
    depth.y = oPos.z;

    viewDir = normalize(iPos - cameraPos);
}

technique Terrain
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSMain();
        PixelShader = compile ps_3_0 PSMain();
    }
}
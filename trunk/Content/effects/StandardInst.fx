#include "ShadowMapping.fxh"
#include "post.fxh"

float4 I_a;
float4 I_d;
float4 I_s;

float4 k_a;
float4 k_d;
float4 k_s;
float4 k_e;
float power;

float fogDensity;
float4 fogColor;

float3 lightDir;

texture clrMap;
texture shadowMap;


sampler	ColorMapSampler = sampler_state
{
    Texture = (clrMap);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
    MaxAnisotropy = 4;
    
    AddressU = Wrap;
    AddressV = Wrap;

    MIPMAPLODBIAS = -3;
};
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



float4 PSMain (
    float2 tex1 : TEXCOORD0,
    float3 N : TEXCOORD3,
    float3 viewDir : TEXCOORD4,
    float4 smLgtPos : TEXCOORD5,
    float2 depth : TEXCOORD6
) : COLOR
{
    N = normalize(N);

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

    float4 Diff = ( max( 0, ndl ) * shd * ( 1 - I_a ) + I_a ) * k_d;  //max( 0, ndl ) * I_d * k_d;
    
    float4 resultColor = tex2D(ColorMapSampler, tex1) * (Amb + Diff + k_e);

    if (resultColor.a > 0.2)
    {
        float f = 1.0 / exp(depth.y * fogDensity);
        resultColor = lerp(fogColor, resultColor,  f);
        return Exposure(resultColor);
    }
    return float4(0,0,0,0);
}

//-----------------------------------------------------------------------------

float3 cameraPos;
float4x4 vp;
//float4x4 worldTrans;
float4x4 smTrans;

void VSMain(
    float4 iPos : POSITION,
    float3 iN : NORMAL,
    float2 iTex1 : TEXCOORD0,
    float3 row1 : TEXCOORD8,
    float3 row2 : TEXCOORD9,
    float3 row3 : TEXCOORD10,
    float3 row4 : TEXCOORD11,

    out float4 oPos : POSITION,
    out float3 oN : TEXCOORD3,
    out float2 oTex1 : TEXCOORD0,
    out float3 viewDir : TEXCOORD4,
    out float4 smLgtPos : TEXCOORD5,
    out float2 depth : TEXCOORD6
)
{
    float4x4 worldTrans;
    worldTrans[0] = float4(row1, 0);
    worldTrans[1] = float4(row2, 0);
    worldTrans[2] = float4(row3, 0);
    worldTrans[3] = float4(row4, 1);

    float4x4 mvp = mul(worldTrans, vp);
    float4x4 smTrans2 = mul(worldTrans, smTrans);

    oN = mul(float4(iN,0), worldTrans);
    oPos = mul(iPos, mvp);

    depth.x = oPos.z / oPos.w;
    depth.y = oPos.z;

    oTex1 = iTex1;
    smLgtPos = mul(iPos , smTrans2);
    viewDir = normalize(iPos.xyz - cameraPos);
}

technique Standard
{
    pass P0
    {
        ALPHATESTENABLE = TRUE;
        ALPHAREF = 0;

        VertexShader = compile vs_3_0 VSMain();
        PixelShader = compile ps_3_0 PSMain();
    }
}

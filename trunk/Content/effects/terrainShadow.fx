
float4 PSMain (float2 depth : TEXCOORD0) : COLOR
{
    return depth.x / depth.y;
}

//-----------------------------------------------------------------------------

float4x4 mvp;

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
    out float2 depth : TEXCOORD0
)
{
    float2 oTex1 = float2(iPos.x/513, iPos.z/513);
    float height = tex2Dlod(DisMapSampler, float4(oTex1, 0,0)).r;
    iPos.y = height * heightScale;

    oPos = mul(iPos, mvp);
    depth.xy = oPos.zw;
}

technique TerrainShadow
{
    pass P0
    {
        ALPHABLENDENABLE = FALSE;
        VertexShader = compile vs_3_0 VSMain();
        PixelShader = compile ps_3_0 PSMain();
    }
}
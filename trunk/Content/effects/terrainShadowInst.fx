
float4 PSMain (float2 depth : TEXCOORD0) : COLOR
{
    return depth.x / depth.y;
}

//-----------------------------------------------------------------------------

float4x4 vp;

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

    float3 row1 : TEXCOORD8,
    float3 row2 : TEXCOORD9,
    float3 row3 : TEXCOORD10,
    float3 row4 : TEXCOORD11,

    out float4 oPos : POSITION,
    out float2 depth : TEXCOORD0
)
{
    float2 oTex1 = float2(iPos.x/513, iPos.z/513);
    float height = tex2Dlod(DisMapSampler, float4(oTex1, 0,0)).r;
    iPos.y = height * heightScale;

    float4x4 worldT;
    worldT[0] = float4(row1, 0);
    worldT[1] = float4(row2, 0);
    worldT[2] = float4(row3, 0);
    worldT[3] = float4(row4, 1);

    float4x4 mvp = mul(worldT, vp);

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
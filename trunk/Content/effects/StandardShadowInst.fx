texture clrMap;

sampler	ColorMapSampler = sampler_state
{
    Texture = (clrMap);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};

float4 PSMain (
    float2 tex1 : TEXCOORD0,
    float2 depth : TEXCOORD1
) : COLOR
{
    float4 resultColor = depth.x / depth.y;

    resultColor.a = tex2D(ColorMapSampler, tex1).a;
    return resultColor;
}

//-----------------------------------------------------------------------------

float4x4 vp;

void VSMain(
    float4 iPos : POSITION,
    float2 iTex1 : TEXCOORD0,

    float3 row1 : TEXCOORD8,
    float3 row2 : TEXCOORD9,
    float3 row3 : TEXCOORD10,
    float3 row4 : TEXCOORD11,

    out float4 oPos : POSITION,
    out float2 oTex1 : TEXCOORD0,
    out float2 depth : TEXCOORD1
)
{
    oTex1 = iTex1;
    
    float4x4 worldT;
    worldT[0] = float4(row1, 0);
    worldT[1] = float4(row2, 0);
    worldT[2] = float4(row3, 0);
    worldT[3] = float4(row4, 1);

    float4x4 mvp = mul(worldT, vp);
    oPos = mul(iPos, mvp);
    depth.xy = oPos.zw;
}

technique StandardShadow
{
    pass P0
    {
        ALPHABLENDENABLE = FALSE;
        ALPHATESTENABLE = TRUE;
        ALPHAREF = 0;

        VertexShader = compile vs_3_0 VSMain();
        PixelShader = compile ps_3_0 PSMain();
    }
}

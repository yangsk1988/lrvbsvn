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

float4x4 mvp;

void VSMain(
    float4 iPos : POSITION,
    float2 iTex1 : TEXCOORD0,
    out float4 oPos : POSITION,
    out float2 oTex1 : TEXCOORD0,
    out float2 depth : TEXCOORD1
)
{
    oTex1 = iTex1;
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

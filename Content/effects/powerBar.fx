
texture barTex;

sampler	TextureSampler = sampler_state
{
    Texture = (barTex);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
};


float4 PSMain (
    float2 tc0 : TEXCOORD0
) : COLOR
{
    return tex2D(TextureSampler, tc0);
}

//-----------------------------------------------------------------------------


float2 barPosition;

float4x4 proj;

void VSMain(
    float4 iPos : POSITION,
    float2 iTex1 : TEXCOORD0,
    float dist : TEXCOORD1,
    float height : TEXCOORD2,
    out float4 oPos : POSITION,
    out float2 oTex1 : TEXCOORD0
)
{
    //oPos.y *= barHeights[(int)iPos.z];

    //oPos.x = (iPos.x )/1024 - 1;
    //oPos.y = (iPos.y )/768 - 1;
    //oPos.z = 0;
    //oPos.w = 1;
    iPos.x += dist;
    iPos.y = 125 - iPos.y * height;


    oTex1.y = iPos.y / 125;

    iPos.xy += barPosition;
    

    oPos = mul(iPos, proj);
    
    oTex1 = iTex1;    
}

technique PowerBar
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSMain();
        PixelShader = compile ps_3_0 PSMain();
    }
}
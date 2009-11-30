
float4x4 mvp;

void SMGenVS(
   float4 pos : POSITION,
   out float4 oPos : POSITION, 
   out float2 depth:TEXCOORD0)
{
    oPos = mul(pos, mvp);
    depth.xy = oPos.zw;    
}

float4 SMGenPS( float2 depth : TEXCOORD0) : COLOR 
{
    return depth.x / depth.y;
}
//-----------------------------------------------------------------------------

technique GenerateShadowMap
{
    pass P0
    {
        VertexShader = compile vs_3_0 SMGenVS();
        PixelShader = compile ps_3_0 SMGenPS();
    }
}


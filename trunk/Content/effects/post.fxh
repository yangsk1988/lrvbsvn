float4 Exposure(float4 color)
{
    float e = color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
    return (exp(0.7) - exp(0.7 - e)) * color;
}
void PostScreenQuadVS(float4 pos : POSITION, 
            float2 tex1 : TEXCOORD0,
            out float4 oPos : POSITION,
            out float2 oTex : TEXCOORD0)
{
    oPos = pos;
    tex1 = oTex;    
}

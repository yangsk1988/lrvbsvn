#include "post.fxh"

texture clrRt;
texture blmRt;

sampler ClrRTSampler = sampler_state
{
   Texture = (clrRt);
   MinFilter = Point;
   MagFilter = Point;
   AddressU = Clamp;
   AddressV = Clamp;
};
sampler BlmRTSampler = sampler_state
{
   Texture = (blmRt);
   MinFilter = Linear;
   MagFilter = Linear;
   AddressU = Clamp;
   AddressV = Clamp;
};

float4 PSMain(float2 tex1 : TEXCOORD0 ) : COLOR0
{
   return tex2D(BlmRTSampler, tex1).r;
}

technique Composite
{
    pass P0
    {
        VertexShader = NULL;//compile vs_3_0 PostScreenQuadVS();
        PixelShader = compile ps_3_0 PSMain();
    }
}
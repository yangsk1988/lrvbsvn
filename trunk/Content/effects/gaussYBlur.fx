#include "post.fxh"

texture rt;

sampler RTSampler = sampler_state
{
   Texture = (rt);
   MinFilter = Point;
   MagFilter = Point;
   MipFilter = None;
};

float4 PSMain(float2 tex1 : TEXCOORD0 ) : COLOR
{
   float d = 1.0 / 512.0;

   float4 color = 0;

   color += tex2D(RTSampler, tex1 + float2(0.0,-5.0 * d)) * 0.1;
   color += tex2D(RTSampler, tex1 + float2(0.0,-4.0 * d)) * 0.22;
   color += tex2D(RTSampler, tex1 + float2(0.0,-3.0 * d)) * 0.358;
   color += tex2D(RTSampler, tex1 + float2(0.0,-2.0 * d)) * 0.563;
   color += tex2D(RTSampler, tex1 + float2(0.0,-1.0 * d)) * 0.873;
   color += tex2D(RTSampler, tex1 ) * 1.0;
   color += tex2D(RTSampler, tex1 + float2(0.0, 1.0 * d)) * 0.873;
   color += tex2D(RTSampler, tex1 + float2(0.0, 2.0 * d)) * 0.563;
   color += tex2D(RTSampler, tex1 + float2(0.0, 3.0 * d)) * 0.358;
   color += tex2D(RTSampler, tex1 + float2(0.0, 4.0 * d)) * 0.22;
   color += tex2D(RTSampler, tex1 + float2(0.0, 5.0 * d)) * 0.1;
   return color / 5.0;
}

technique GaussY
{
    pass P0
    {
        VertexShader = NULL;//compile vs_3_0 PostScreenQuadVS();
        PixelShader = compile ps_3_0 PSMain();
    }
}
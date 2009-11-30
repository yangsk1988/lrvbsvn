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
   //float dx = 1.0 / 512.0;
   //float dy = 1.0 / 512.0;

   float4 color = tex2D(RTSampler, tex1);
   //color += tex2D(RTSampler, tex1 + float2(dx*2, 0));
   //color += tex2D(RTSampler, tex1 + float2(dx*4, 0));
   //color += tex2D(RTSampler, tex1 + float2(dx*6, 0));

   //color += tex2D(RTSampler, tex1 + float2(0,    dy*2.0));
   //color += tex2D(RTSampler, tex1 + float2(dx*2, dy*2.0));
   //color += tex2D(RTSampler, tex1 + float2(dx*4, dy*2.0));
   //color += tex2D(RTSampler, tex1 + float2(dx*6, dy*2.0));

   //color += tex2D(RTSampler, tex1 + float2(0,    dy*4.0));
   //color += tex2D(RTSampler, tex1 + float2(dx*2, dy*4.0));
   //color += tex2D(RTSampler, tex1 + float2(dx*4, dy*4.0));
   //color += tex2D(RTSampler, tex1 + float2(dx*6, dy*4.0));

   //color += tex2D(RTSampler, tex1 + float2(0,    dy*6));
   //color += tex2D(RTSampler, tex1 + float2(dx*2, dy*6));
   //color += tex2D(RTSampler, tex1 + float2(dx*4, dy*6));
   //color += tex2D(RTSampler, tex1 + float2(dx*6, dy*6));

   //color /= 16.0;

   float e = log2(color.r * 0.3 + color.g * 0.59 + color.b * 0.11 + .75);
   if (e >= 0)
   {
       return e * e;
   }
   return 0;
}

technique Bloom
{
    pass P0
    {
        VertexShader = NULL;//compile vs_3_0 PostScreenQuadVS();
        PixelShader = compile ps_3_0 PSMain();
    }
}
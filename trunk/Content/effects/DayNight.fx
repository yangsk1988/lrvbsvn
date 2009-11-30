#include "post.fxh"

textureCUBE Day;
textureCUBE Night;

float nightAlpha;

samplerCUBE NightTex = sampler_state
{
    Texture = (Night);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MaxAnisotropy = 4;
};

samplerCUBE DayTex = sampler_state
{
    Texture = (Day);

    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MaxAnisotropy = 4;
};

float4 PSMain (float3 tc0 : TEXCOORD0) : COLOR
{
    float4 night = texCUBE(NightTex, tc0);
    float4 day = texCUBE(DayTex, tc0);
    return Exposure(day * (1-nightAlpha) + night * nightAlpha);
}


//-----------------------------------------------------------------------------



technique DayNight
{
    pass P0
    {
        VertexShader = NULL;
        PixelShader = compile ps_2_0 PSMain();
    }
}
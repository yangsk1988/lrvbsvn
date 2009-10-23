using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Graphics;
using VirtualBicycle.Media;
using D3D = SlimDX.Direct3D9;

namespace VirtualBicycle.Graphics.D3D9
{
    internal class D3D9SamplerState : SamplerState
    {
        D3D.Device device;

        public D3D9SamplerState(D3D9RenderSystem d3drs, int index)
            : base(index)
        {
            device = d3drs.D3DDevice;
        }



        public override TextureAddressMode AddressU
        {
            get
            {
                D3D.TextureAddress dv = device.GetSamplerState<D3D.TextureAddress>(Index, D3D.SamplerState.AddressU);

                return D3D9Utils.ConvertEnum(dv);
            }
            set
            {
                device.SetSamplerState(Index, D3D.SamplerState.AddressU, D3D9Utils.ConvertEnum(value));

            }
        }

        public override TextureAddressMode AddressV
        {
            get
            {
                D3D.TextureAddress dv = device.GetSamplerState<D3D.TextureAddress>(Index, D3D.SamplerState.AddressV);

                return D3D9Utils.ConvertEnum(dv);
            }
            set
            {
                device.SetSamplerState(Index, D3D.SamplerState.AddressV, D3D9Utils.ConvertEnum(value));
            }
        }

        public override TextureAddressMode AddressW
        {
            get
            {
                D3D.TextureAddress dv = device.GetSamplerState<D3D.TextureAddress>(Index, D3D.SamplerState.AddressW);

                return D3D9Utils.ConvertEnum(dv);
            }
            set
            {
                device.SetSamplerState(Index, D3D.SamplerState.AddressW, D3D9Utils.ConvertEnum(value));
            }
        }

        public override ColorValue BorderColor
        {
            get
            {
                return new ColorValue(device.GetSamplerState(Index, D3D.SamplerState.BorderColor));
            }
            set
            {
                device.SetSamplerState(Index, D3D.SamplerState.BorderColor, (int)value.PackedValue);
            }
        }

        public override TextureFilter MagFilter
        {
            get
            {
                D3D.TextureFilter dtf = device.GetSamplerState<D3D.TextureFilter>(Index, D3D.SamplerState.MagFilter);
                return D3D9Utils.ConvertEnum(dtf);
            }
            set
            {
                device.SetSamplerState(Index, D3D.SamplerState.MagFilter, D3D9Utils.ConvertEnum(value));
            }
        }

        public override int MaxAnisotropy
        {
            get
            {
                return device.GetSamplerState(Index, D3D.SamplerState.MaxAnisotropy);
            }
            set
            {
                device.SetSamplerState(Index, D3D.SamplerState.MaxAnisotropy, value);
            }
        }

        public override int MaxMipLevel
        {
            get
            {
                return device.GetSamplerState(Index, D3D.SamplerState.MaxMipLevel);
            }
            set
            {
                device.SetSamplerState(Index, D3D.SamplerState.MaxMipLevel, value);
            }
        }

        public override TextureFilter MinFilter
        {
            get
            {
                D3D.TextureFilter dtf = device.GetSamplerState<D3D.TextureFilter>(Index, D3D.SamplerState.MinFilter);
                return D3D9Utils.ConvertEnum(dtf);
            }
            set
            {
                device.SetSamplerState(Index, D3D.SamplerState.MinFilter, D3D9Utils.ConvertEnum(value));
            }
        }

        public override TextureFilter MipFilter
        {
            get
            {
                D3D.TextureFilter dtf = device.GetSamplerState<D3D.TextureFilter>(Index, D3D.SamplerState.MipFilter);
                return D3D9Utils.ConvertEnum(dtf);
            }
            set
            {
                device.SetSamplerState(Index, D3D.SamplerState.MipFilter, D3D9Utils.ConvertEnum(value));
            }
        }

        public override float MipMapLevelOfDetailBias
        {
            get
            {
                return device.GetSamplerState(Index, D3D.SamplerState.MipMapLodBias);
            }
            set
            {
                device.SetSamplerState(Index, D3D.SamplerState.MipMapLodBias, value);
            }
        }
    }
}

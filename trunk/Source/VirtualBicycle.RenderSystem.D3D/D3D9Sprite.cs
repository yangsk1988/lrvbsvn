using System;
using System.Collections.Generic;
using System.Text;

using D3D = SlimDX.Direct3D9;

namespace VirtualBicycle.RenderSystem.D3D9
{
    internal sealed class D3D9Sprite : Sprite 
    {
        D3D.Sprite sprite;

        internal D3D.Sprite D3DSprite
        {
            get { return sprite; }
        }

        public D3D9Sprite(D3D9RenderSystem d3drs)
            : base(d3drs)
        {
            sprite = new D3D.Sprite(d3drs.D3DDevice);
        }

        public void Begin() 
        {
            sprite.Begin(D3D.SpriteFlags.DoNotSaveState | D3D.SpriteFlags.AlphaBlend);
        }
        
        public void End()
        {
            sprite.End();
        }


        //public void Draw(Texture texture)
        //{

        //}
    }
}

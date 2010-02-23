using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.MathLib;
using D3D = SlimDX.Direct3D9;

namespace VirtualBicycle.Graphics.D3D9
{
    internal sealed class D3D9Font : Font
    {
        D3D.Font font;

        public D3D9Font(D3D9RenderSystem rs, System.Drawing.Font font)
            : base(rs)
        {
            this.font = new D3D.Font(rs.D3DDevice, font);
            
        }


        public override void DrawString(Sprite sprite, string text, int x, int y, int color)
        {
            D3D9Sprite dsprite = (D3D9Sprite)sprite;

            font.DrawString(dsprite.D3DSprite, text, x, y, color);
        }

        public override void DrawString(Sprite sprite, string text, Rectangle rectangle, DrawTextFormat format, int color)
        {
            D3D9Sprite dsprite = (D3D9Sprite)sprite;
            font.DrawString(dsprite.D3DSprite, text, (System.Drawing.Rectangle)rectangle, (D3D.DrawTextFormat)format, color);
        }

        public override Rectangle MeasureString(Sprite sprite, string text, DrawTextFormat format)
        {
            D3D9Sprite dsprite = (D3D9Sprite)sprite;
            return (Rectangle)font.MeasureString(dsprite.D3DSprite, text, (D3D.DrawTextFormat)format);
        }
    }
}

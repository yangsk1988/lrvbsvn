using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace VirtualBicycle.UI
{
    public class DrawTextBox
    {
        #region Fields
        SlimDX.Direct3D9.Font font;
        DrawTextFormat format;
        Color color;
        int fontSize;
        Game game;
        string text;

        private System.Drawing.Rectangle rect;
        public System.Drawing.Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }
        #endregion

        #region Constructor
        public DrawTextBox(Game game, Device device, String text, string fontName, int fontSize, 
            Color drawColor, Rectangle rect)
        {
            font = new SlimDX.Direct3D9.Font(device, FontManager.GetInstance().CreateFont(fontName, fontSize));
            this.color = drawColor;
            this.rect = rect;
            this.game = game;
            this.text = text;

            //默认使用居中
            this.format = DrawTextFormat.Center | DrawTextFormat.VerticalCenter | DrawTextFormat.SingleLine;
        }

        public DrawTextBox(Game game, Device device, String text, string fontName, int fontSize, 
            Color drawColor, Rectangle rect, DrawTextFormat format)
        {
            font = new SlimDX.Direct3D9.Font(device, FontManager.GetInstance().CreateFont(fontName, fontSize));
            this.color = drawColor;
            this.rect = rect;
            this.game = game;
            this.text = text;

            //默认使用居中
            this.format = format;
        }

        public DrawTextBox(DrawTextBox box)
        {
            this.font = box.font;
            this.color = box.color;
            this.fontSize = box.fontSize;
            this.format = box.format;
            this.game = box.game;
            this.rect = box.rect;
            this.text = box.text;
        }

        public DrawTextBox(Game game, Device device, String text, string fontName, int fontSize, Color drawColor)
        {
            font = new SlimDX.Direct3D9.Font(device, FontManager.GetInstance().CreateFont(fontName, fontSize));
            this.color = drawColor;
            this.game = game;
            this.text = text;

            //默认使用居中
            this.format = DrawTextFormat.Center | DrawTextFormat.VerticalCenter | DrawTextFormat.SingleLine;
        }
        #endregion

        #region Methods
        public void Render(Sprite sprite,bool isShadowed)
        {
            Render(sprite, this.text, this.color, isShadowed);
        }

        public void Render(Sprite sprite, string text, bool isShadowed)
        {
            Render(sprite, text, this.color, isShadowed);
        }

        public void Render(Sprite sprite, string text, Color color, bool isShadowed)
        {
            if ((this.rect.Width == 0) || (this.rect.Height == 0))
            {
                throw new Exception("绘制Rect没有初始化");
            }

            Rectangle _rect = this.rect;
            _rect.X = (int)GetPositionWidth(rect.X);
            _rect.Y = (int)GetPositionHeight(rect.Y);
            _rect.Width = (int)GetPositionWidth(rect.Width);
            _rect.Height = (int)GetPositionHeight(rect.Height);

            if ((isShadowed) && (color != Color.Black))
            {
                _rect.X += 3;
                _rect.Y += 3;
                font.DrawString(sprite, text, _rect, format, Color.Black);
            }
            font.DrawString(sprite, text, _rect, format, color);

        }

        public void Unload()
        {
        }

        public float GetPositionWidth(float x)
        {
            const float defaultWidth = 1024f;
            float newx = x;
            if (game != null)
            {
                newx = x / defaultWidth * game.Window.ClientSize.Width;
            }

            return newx;
        }

        public float GetPositionHeight(float x)
        {
            const float defaultHeight = 768f;
            float newx = x;
            if (game != null)
            {
                newx = x / defaultHeight * game.Window.ClientSize.Height;
            }

            return newx;
        }

        public Vector2 GetPosition(Vector2 pos)
        {
            const float defaultWidth = 1024f;
            const float defaultHeight = 768f;
            float x = pos.X / defaultWidth * game.Window.ClientSize.Width;
            float y = pos.Y / defaultHeight * game.Window.ClientSize.Height;
            return new Vector2(x, y);
        }
        #endregion
    }
}

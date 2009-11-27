using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Graphics
{
    public class FontManager : Singleton
    {
        static volatile FontManager singleton;
        static volatile object syncHelper = new object();

        public static FontManager Instance 
        {
            get
            {
                if (singleton == null)
                {
                    lock (syncHelper)
                    {
                        if (singleton == null)
                        {
                            singleton = new FontManager();
                        }
                    }
                }
                return singleton;
            }
        }
        
        Dictionary<string, Font> loadedFonts;

        private FontManager()
        {
            loadedFonts = new Dictionary<string, Font>();
        }



        //public Font CreateInstance(RenderSystem rs, System.Drawing.Font font)
        //{
        //    Font gfont;
        //    FontDescription tmp = new FontDescription(font.Name, font.Size, font.Style);
        //    if (!buffered.TryGetValue(tmp, out gfont))
        //    {
        //        gfont = new Font(rs, font);
        //    }

        //    return gfont;
        //}

        //public Font CreateInstance(RenderSystem rs, string name, float fontSize, System.Drawing.FontStyle style)
        //{
        //    Font font;
        //    FontDescription tmp = new FontDescription(name, fontSize, style);
        //    if (!buffered.TryGetValue(tmp, out font))
        //    {
        //        font = new Font(rs, new System.Drawing.Font(name, fontSize, style));
        //    }

        //    return font;
        //}

        protected override void dispose()
        {
            Dictionary<string, Font>.ValueCollection vals = loadedFonts.Values;
            foreach (Font e in vals)
            {
                e.Dispose();
            }
            loadedFonts.Clear();
        }
    }
}

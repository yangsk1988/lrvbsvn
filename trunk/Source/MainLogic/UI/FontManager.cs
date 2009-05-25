using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace VirtualBicycle.UI
{
    /// <summary>
    /// 管理字体,动态创建
    /// 采用单件模式
    /// </summary>
    public class FontManager
    {
        #region Fields
        private static FontManager instance;
        List<Font> fonts;
        #endregion

        #region Constructor
        private FontManager()
        {
            fonts = new List<Font>();
        }
        #endregion

        #region Methods
        public static FontManager GetInstance()
        {
            if (instance == null)
            {
                instance = new FontManager();
            }
            return instance;
        }


        public Font CreateFont(string familyName, float size)
        {
            for (int i = 0; i < fonts.Count; i++)
            {
                if ((fonts[i].Name == familyName) && (fonts[i].Size == size))
                {
                    return fonts[i];
                }
            }

            Font font = new Font(familyName, size);
            fonts.Add(font);
            return font;
        }
        #endregion
    }
}

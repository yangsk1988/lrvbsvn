using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using VirtualBicycle.Collections;

namespace VirtualBicycle.UI
{
    /// <summary>
    /// 管理字体,动态创建
    /// 采用单件模式
    /// </summary>
    public class FontManager
    {
        struct Entry 
        {
            public string FamilyName;
            public float Size;

            public Entry(string familyName, float size) 
            {
                this.FamilyName = familyName;
                this.Size = size;
            }

            public override int GetHashCode()
            {
                return FamilyName.GetHashCode() ^ Size.GetHashCode();
            }
        }

        #region Fields
        private static FontManager instance;
        Dictionary<Entry, Font> fonts;
        #endregion

        #region Constructor
        private FontManager()
        {
            fonts = new Dictionary<Entry, Font>();
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
            Font font;
            Entry entry = new Entry(familyName, size);

            if (!fonts.TryGetValue(entry, out font))
            {
                font = new Font(familyName, size);
                fonts.Add(entry, font);
            }

            return font;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using VirtualBicycle;

namespace VirtualBicycle.UI
{
    public static class LanguageParser
    {
        static StringTable strTable;

        public static void ParseLanguage(StringTable data, Control ctl)
        {
            strTable = data;
            ParseLanguage(ctl);
            strTable = null;
        }

        public static void ParseLanguage(StringTable data, ToolStrip ctl)
        {
            strTable = data;
            for (int i = 0; i < ctl.Items.Count; i++)
                ParseLanguage(ctl.Items[i]);
            strTable = null;
        }

        public static void ParseLanguage(StringTable data, ListView ctl)
        {
            strTable = data;
            ParseLanguage(ctl);
            strTable = null;
        }

        static void ParseLanguage(Control ctl)
        {
            if (!string.IsNullOrEmpty(ctl.Text))
            {
                ctl.Text = strTable[ctl.Text];
            }

            Control.ControlCollection col = ctl.Controls;
            for (int i = 0; i < col.Count; i++)
                ParseLanguage(col[i]);
        }

        static void ParseLanguage(ToolStripItem ctl)
        {
            if (!string.IsNullOrEmpty(ctl.Text))
            {
                ctl.Text = strTable[ctl.Text];
            }

            ToolStripDropDownItem itm = ctl as ToolStripDropDownItem;
            if (itm != null)
                for (int i = 0; i < itm.DropDownItems.Count; i++)
                    ParseLanguage(itm.DropDownItems[i]);

        }
        static void ParseLanguage(ListView lv)
        {
            for (int i = 0; i < lv.Columns.Count; i++)
            {
                lv.Columns[i].Text = strTable[lv.Columns[i].Text];
            }
        }
    }
}
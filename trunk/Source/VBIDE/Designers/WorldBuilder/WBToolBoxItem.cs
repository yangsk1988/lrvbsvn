using System;
using System.Collections.Generic;
using System.Text;
using VBIDE.Tools;

namespace VBIDE.Designers.WorldBuilder
{
    public abstract class WBToolBoxItem : ToolBoxItem
    {
        public abstract void Render();   
    }
}

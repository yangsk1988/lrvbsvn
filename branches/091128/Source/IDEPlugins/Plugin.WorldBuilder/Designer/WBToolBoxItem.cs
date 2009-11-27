using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Ide.Tools;

namespace Plugin.WorldBuilder
{
    public abstract class WBToolBoxItem : ToolBoxItem
    {
        public abstract void Render();   
    }
}

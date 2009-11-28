using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Apoc3D.Ide.Tools
{
    public abstract class ToolBoxCategory
    {
        public abstract Image Icon
        {
            get;
        }

        public abstract string Name
        {
            get;
        }
    }
}

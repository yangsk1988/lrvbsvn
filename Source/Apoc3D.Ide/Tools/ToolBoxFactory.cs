using System;
using System.Collections.Generic;
using System.Text;

namespace Apoc3D.Ide.Tools
{
    public class ToolBoxFactory : IToolAbstractFactory
    {
        #region IToolAbstractFactory 成员

        public ITool CreateInstance()
        {
            return new ToolBox();
        }

        public Type CreationType
        {
            get { return typeof(ToolBox); }
        }

        #endregion
    }
}

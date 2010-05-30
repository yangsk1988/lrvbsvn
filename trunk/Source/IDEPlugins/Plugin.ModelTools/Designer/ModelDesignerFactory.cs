using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Ide;
using Apoc3D.Ide.Designers;
using Apoc3D.Vfs;

namespace Plugin.DXBased
{
    public class ModelDesignerFactory : DesignerAbstractFactory
    {
        public override DocumentBase CreateInstance(ResourceLocation res)
        {
            return new ModelDesigner(this, res);
        }

        public override Type CreationType
        {
            get { return typeof(ModelDesigner); }
        }

        public override string Description
        {
            get { return DevStringTable.Instance["DOCS:ModelDesc"]; }
        }

        public override string[] Filters
        {
            get { return new string[] { ModelDesigner.Extension }; }
        }
    }
}

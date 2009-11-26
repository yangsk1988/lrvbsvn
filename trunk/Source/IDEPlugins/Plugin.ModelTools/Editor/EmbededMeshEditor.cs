using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using VirtualBicycle.Ide;
using VirtualBicycle.Ide.Designers;

namespace Plugin.ModelTools
{
    public class EmbededMeshEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            EmbeddedMeshDocument doc = new EmbeddedMeshDocument((EditableMesh)value);
            Program.MainForm.AddDocumentTab(doc);
            return value;            
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Text;
using System.Windows.Forms.Design;
using Apoc3D.MathLib;

namespace Apoc3D.Design
{
    public class MatrixEditor : UITypeEditor
    {
#if !XBOX
        MatrixEditControl ui;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (ui == null)
            {
                Assembly asm = Assembly.GetEntryAssembly();
                Type tpe = asm.GetType("VirtualBicycle.Ide.DevStringTable", true);
                PropertyInfo stProp = tpe.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);

                ui = new MatrixEditControl((StringTable)stProp.GetValue(null, null));
            }

            //m_ui.SetStates((DockAreas)value);
            ui.Value = (Matrix)value;
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            ui.Service = edSvc;
            edSvc.DropDownControl(ui);

            //ui.Value = null;
            Matrix mat = ui.Value;
            ui.Reset();
            return mat;
            //return m_ui.DockAreas;
        }
        public override bool IsDropDownResizable
        {
            get { return true; }
        }
#endif
    }
}

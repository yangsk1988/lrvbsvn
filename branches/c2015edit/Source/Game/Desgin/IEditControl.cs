using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;

namespace VirtualBicycle.Design
{
    public interface IEditControl<T>
    {
        T Value { get; set; }
        IWindowsFormsEditorService Service { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Apoc3D.Design
{
#if !XBOX
    using System.Windows.Forms.Design;

    public interface IEditControl<T>
    {
        T Value { get; set; }
        IWindowsFormsEditorService Service { get; set; }
    }
#endif
}

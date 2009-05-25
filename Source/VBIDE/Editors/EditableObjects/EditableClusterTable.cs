using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Scene;

namespace VBIDE.Editors.EditableObjects
{
    public class EditableClusterTable : ClusterTableBase<EditableCluster>
    {
        public EditableClusterTable(EditableCluster[] clusters)
            : base(clusters)
        {
        }
    }
}

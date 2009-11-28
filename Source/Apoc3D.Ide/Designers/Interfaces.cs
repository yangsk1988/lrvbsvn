using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Apoc3D.IO;
using WeifenLuo.WinFormsUI.Docking;

namespace Apoc3D.Ide.Designers
{
    
    public interface IDocument
    {
        //int GetHashCode();
        //Icon GetIcon();
        //string ToString();
        void DocActivate();
        void DocDeactivate();
        bool IsActivated { get; }
    }
    public abstract class DesignerAbstractFactory
    {
        public abstract DocumentBase CreateInstance(ResourceLocation res);

        public abstract Type CreationType { get; }

        public abstract string Description { get; }

        public virtual string Filter
        {
            get { return DevUtils.GetFilter(Description, Filters); }
        }

        public abstract string[] Filters { get; }
    }
}

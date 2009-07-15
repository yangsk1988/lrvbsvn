using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VirtualBicycle.Ide.Converters
{
    public partial class DemConvDlg : Form
    {
        DemConverter converter;
        DemParameters conParams;

        public DemConvDlg(DemConverter converter)
        {
            InitializeComponent();

            this.converter = converter;
            this.conParams = new DemParameters();
            this.propertyGrid1.SelectedObject = conParams;
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using VirtualBicycle.IO;

namespace VirtualBicycle.Ide.Converters
{
    class DemParameters
    {
        public DemParameters()
        {
            CellSize = 90f;
            HeightScale = 5000f;

            GenereateNoise = true;
        }

        [DefaultValue(90f)]
        public float CellSize
        {
            get;
            set;
        }

        [DefaultValue(5000f)]
        public float HeightScale
        {
            get;
            set;
        }

        [DefaultValue(true)]
        public bool GenereateNoise
        {
            get;
            set;
        }
    }

    public class DemConverter : ConverterBase
    {

        public override void ShowDialog(object sender, EventArgs e)
        {
            DemConvDlg dlg = new DemConvDlg(this);
            dlg.ShowDialog();
        }

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return DevStringTable.Instance["GUI:DemConverter"]; ; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".asc", ".tif" }; }
        }

        public override string[] DestExt
        {
            get { return new string[] { ".dmp" }; }
        }

        public override string SourceDesc
        {
            get { return "Elevation Data"; }
        }

        public override string DestDesc
        {
            get { return "Displacement Map"; }
        }
    }
}

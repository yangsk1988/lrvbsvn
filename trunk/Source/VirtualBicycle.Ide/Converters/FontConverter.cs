using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Ide.Converters
{
    public class FontConverter : ConverterBase
    {
        public override void ShowDialog(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void Convert(VirtualBicycle.IO.ResourceLocation source, VirtualBicycle.IO.ResourceLocation dest)
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return "Font Converter"; }
        }

        public override string[] SourceExt
        {
            get { return new string[0]; }
        }

        public override string[] DestExt
        {
            get { return new string[] { ".fnt" }; }
        }

        public override string SourceDesc
        {
            get { return "System font"; }
        }

        public override string DestDesc
        {
            get { throw new NotImplementedException(); }
        }
    }
}

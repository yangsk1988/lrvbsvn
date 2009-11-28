using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Apoc3D;

namespace Apoc3D.Ide
{
    public class DevStringTable : Singleton
    {
        static Apoc3D.StringTable singleton;

        public static Apoc3D.StringTable Instance
        {
            get { return singleton; }
        }

        public static void Initialize()
        {
            singleton = (new StringTableCsfFormat()).Load(
                new DevFileLocation(
                    Path.Combine(Application.StartupPath, "VBIDE.csf")
                    ));
        }

        protected override void dispose()
        {
            singleton = null;
        }
    }

    public static class Program
    {
        static Icon defaultIcon;
        //static StringTable strTable;
        static MainForm form;

        public static MainForm MainForm
        {
            get { return form; }
        }
        public static Icon DefaultIcon
        {
            get { return defaultIcon; }
        }


        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            DevStringTable.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            defaultIcon = new Icon(typeof(Form), "wfc.ico");
            form = new MainForm();
            Application.Run(form);
        }


    }
}

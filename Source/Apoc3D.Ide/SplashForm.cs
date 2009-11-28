using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Apoc3D.Ide
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
        }

        public void PluginProgressCallBack(IPlugin plugin, int index, int count)
        {
            label1.Visible = true;
            label2.Visible = true;
            progressBar1.Visible = true;
            pictureBox1.Visible = true;

            progressBar1.Maximum = count;
            progressBar1.Value = index;

            if (plugin != null)
            {
                label2.Text = plugin.Name;

                if (plugin.PluginIcon != null)
                {
                    pictureBox1.Image = plugin.PluginIcon.ToBitmap();
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
            Application.DoEvents();
        }

    }
}

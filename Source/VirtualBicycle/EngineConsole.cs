using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VirtualBicycle
{
    public partial class EngineConsole : Form
    {
        static Color[] colorTable =
         new Color[] { Color.DarkGray, Color.Black, Color.Black, Color.Orange, Color.Red };

        string[] msgPrefixTable = new string[] 
        {
            string.Empty,
            string.Empty,
            "注意：",
            "警告：",
            "错误："
        };


        delegate void WriteHelper(string msg, ConsoleMessageType type);
        delegate string GetTextHelper();

        static EngineConsole singleton;

        public static EngineConsole Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new EngineConsole();
                return singleton;
            }
        }
        
        ConsoleMessageType msgLvl;
        bool isShown;
        StringBuilder context;
        private EngineConsole()
        {
            InitializeComponent();
            context = new StringBuilder();

            this.Text = "控制台";
            //this.Text = ResourceAssembly.Instance.Console;// "控制台";
            //button1.Text = ResourceAssembly.Instance.Console_Submit;// "提交";
            
        }
        public ConsoleMessageType MessageLevel        
        {
            get { return msgLvl; }
            set { msgLvl = value; }
        }


        void write(string msg, ConsoleMessageType msgType)
        {
            if (msgType >= msgLvl)
            {
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;

                DateTime time = DateTime.Now;
                msg = time.Hour.ToString() + ":" + time.Minute.ToString() + ":" + time.Second.ToString() + " >" + msgPrefixTable[(int)msgType] + msg + "\r\n";

                richTextBox1.SelectionColor = colorTable[(int)msgType];
                richTextBox1.SelectedText = msg;

                richTextBox1.ScrollToCaret();
            }
        }
        string getText() { return richTextBox1.Text; }

        public void Write(string msg)
        {
            Write(msg, ConsoleMessageType.Normal);
        }
        public void Write(string msg, ConsoleMessageType msgType)
        {
            if (isShown)
            {
                if (richTextBox1.InvokeRequired)
                {
                    this.Invoke((WriteHelper)this.write, msg, (object)msgType);
                }
                else
                {
                    if (msgType >= msgLvl)
                    {
                        richTextBox1.SelectionStart = richTextBox1.TextLength;
                        richTextBox1.SelectionLength = 0;

                        DateTime time = DateTime.Now;
                        msg = time.Hour.ToString() + ":" + time.Minute.ToString() + ":" + time.Second.ToString() + " >" + msgPrefixTable[(int)msgType] + msg + "\r\n";

                        richTextBox1.SelectionColor = colorTable[(int)msgType];
                        richTextBox1.SelectedText = msg;

                        richTextBox1.ScrollToCaret();
                    }
                }
            }
            else
            {
                if (msgType >= msgLvl)
                {
                    context.AppendLine(DateTime.UtcNow.TimeOfDay.ToString() + msgPrefixTable[(int)msgType] + msg);
                }
            }
            //
        }

        public void Write(ConsoleMessageType msgType, string msg, params string[] param)
        {
            Write(string.Format(msg, param), msgType);
        }
        public void Write(string msg, params string[] param)
        {
            Write(string.Format(msg, param));
        }
        public string ConsoleText
        {
            get
            {
                if (!richTextBox1.InvokeRequired)
                {
                    return isShown ? richTextBox1.Text : context.ToString();
                }
                else
                {
                    return isShown ? (string)richTextBox1.Invoke((GetTextHelper)getText) : context.ToString();
                }
            }
        }


        private void EngineConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
        if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }     
        }

        private void EngineConsole_Load(object sender, EventArgs e)
        {
            isShown = true;
        }
    }

    public enum ConsoleMessageType : uint
    {
        Normal = 0,
        Information,
        Exclamation,
        Warning,
        Error
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace MultiControl
{
    public partial class FormEditCfg : Form
    {
        public enum CfgType
        {
            CT_MONIPOWER = 0,
            CT_SYSINFO = 1,
            CT_WIFI = 2,
            CT_AUDIOLOOPBACK = 3,
            CT_GPS = 4,
            CT_NONE
        }

        public enum InputType
        {
            IT_DIGITAL = 0,
            IT_TEXT = 1
        }

        public class EditorTag
        {
            public string KEY;
            public InputType TYPE;

            public EditorTag(string key, InputType type)
            {
                KEY = key;
                TYPE = type;
            }
        }

        private string _caption = string.Empty;
        private string _configFolder = string.Empty;
        private Dictionary<string, string> Options = new Dictionary<string, string>();
        private ArrayList mCommentList = new ArrayList();
        private string mCurrentCfgFile = string.Empty;
        public CfgType mCFGTYPE = CfgType.CT_NONE;

        public string Title
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public string ConfigFolder
        {
            get { return _configFolder; }
            set { _configFolder = value; }
        }

        public FormEditCfg()
        {
            InitializeComponent();
        }

        private void FormEditCfg_Load(object sender, EventArgs e)
        {
            this.label1.Text = "Config File Editor\r\n    * Enter - Save modification \r\n";
            switch (mCFGTYPE)
            {
                case CfgType.CT_MONIPOWER:
                    this.Text = Title + " MoniPower Editor";
                    mCurrentCfgFile = ConfigFolder + "\\monipower.cfg";
                    break;
                case CfgType.CT_SYSINFO:
                    this.Text = Title + " SysInfo Editor";
                    mCurrentCfgFile = ConfigFolder + "\\sysinfo.cfg";
                    break;
                case CfgType.CT_WIFI:
                    this.Text = Title + " WIFI Editor";
                    mCurrentCfgFile = ConfigFolder + "\\wifi.cfg";
                    break;
                case CfgType.CT_AUDIOLOOPBACK:
                    this.Text = Title + " Audio Loopback Editor";
                    mCurrentCfgFile = ConfigFolder + "\\audio_loopback.cfg";
                    break;
                case CfgType.CT_GPS:
                    this.Text = Title + " GPS Editor";
                    mCurrentCfgFile = ConfigFolder + "\\gps.cfg";
                    break;
                case CfgType.CT_NONE:
                    this.Text = Title + " None CFG Editor";
                    mCurrentCfgFile = ConfigFolder + "\\monipower.cfg";
                    break;
            }
            LoadCfgFromFile(mCurrentCfgFile);
            this.panel1.Controls.Clear();
            int x = 4, y = 4;
            foreach (string key in Options.Keys)
            {
                Label label = new Label();
                label.Location = new Point(x, y);
                label.Size = new System.Drawing.Size(this.panel1.Width, 24);
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Text = key;
                label.Font = new System.Drawing.Font("Cambria", 12.00F);
                TextBox text = new TextBox();
                text.Location = new Point(x, y + 26);
                text.Size = new System.Drawing.Size(this.panel1.Width - 8, 16);
                text.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_Press);
                text.Name = key;
                text.Tag = new EditorTag(key, InputType.IT_DIGITAL);
                text.Text = Options[key];
                text.Font = new System.Drawing.Font("Arial", 10.00F);
                this.panel1.Controls.Add(label);
                this.panel1.Controls.Add(text);
                y += 44;
            }
        }

        private void TextBox_Press(object sender, KeyPressEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (e.KeyChar == 13)
            {
                if (!string.IsNullOrEmpty(box.Text))
                {
                    EditorTag tag = box.Tag as EditorTag;
                    Options[tag.KEY] = box.Text;
                }
            }
        }

        private int LoadCfgFromFile(string file)
        {
            if (File.Exists(file))
            {
                char [] trimChars = {' ','#'};
                FileStream fs = new FileStream(file, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string result = sr.ReadLine();
                while (!string.IsNullOrEmpty(result))
                {
                    if (!result.StartsWith("#"))
                    {
                        string[] values = result.Split('=');
                        if (values.Length == 2) //options = [option = 12]
                        {
                            string key = values[0].Trim(trimChars);
                            string value = values[1].Trim(trimChars);
                            Options.Add(key, value);
                        }
                    }
                    else
                    {
                        mCommentList.Add(result);
                    }
                    result = sr.ReadLine();
                }
                sr.Close();
                fs.Close();
            }

            return 0;
        }

        private int SaveCfgFile(string file)
        {
            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                FileStream fs = new FileStream(file, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                foreach (string comment in mCommentList)
                {
                    sw.WriteLine(comment);
                }
                foreach (string key in Options.Keys)
                {
                    sw.WriteLine(key + "=" + Options[key]);
                }
                sw.Close();
                fs.Close();
            }
            return 0;
        }

        private void FormEditCfg_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCfgFile(mCurrentCfgFile);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RegisterMachine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //string uid = this.textBoxId.Text;
            //string key = this.textBoxKey.Text;
            //if (Validate(uid, key) == 0)
            //{
            Close();
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string uid = this.textBoxId.Text;
            if (string.IsNullOrEmpty(uid))
            {
                MessageBox.Show("Please input valid user id");
            }
            else
            {
                StringBuilder sbr = new StringBuilder(35);
                int result = Generate(uid, sbr);
                this.textBoxKey.Text = sbr.ToString();
            }
        }

        [DllImport("KeyGenerate.dll", EntryPoint = "fnGenerateKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Generate(string user, StringBuilder sbr);

        [DllImport("KeyGenerate.dll", EntryPoint = "fnValidateKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Validate(string user, string key);
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //创建MenuStrip对象
            MenuStrip MS = new MenuStrip();
            //创建一个ToolStripMenuItem菜单，可以文本和图片一并添加
            ToolStripMenuItem tsmi = new ToolStripMenuItem("测试按钮");
            //绑定菜单的点击事件
            tsmi.Click += DemoClick;
            //创建子菜单 可以文本和图片一并添加
            ToolStripMenuItem tsmi2 = new ToolStripMenuItem("测试子按钮");
            //绑定子菜单点击事件
            tsmi2.Click += DemoClick;
            //添加子菜单
            tsmi.DropDownItems.Add(tsmi2);
            //添加主菜单
            MS.Items.Add(tsmi);
            //界面显示
            this.Controls.Add(MS);
        }


        //自己定义个点击事件需要执行的动作
        private void DemoClick(object sender, EventArgs e)
        {
            ToolStripMenuItem but = sender as ToolStripMenuItem;
            MessageBox.Show(but.Text);
        }
    }
    }

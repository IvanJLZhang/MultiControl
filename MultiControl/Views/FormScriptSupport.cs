using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml.Linq;
using System.Xml;
using MultiControl.Common;

namespace MultiControl
{
    public partial class FormScriptSupport : Form
    {
        private XmlDocument xml = new XmlDocument();

        public FormScriptSupport()
        {
            InitializeComponent();
            LoadTestItemData();
        }

        private int LoadTestItemData()
        {
            xml.Load("ScriptData.xml");
            RecursionTreeControl(xml.DocumentElement, tvTerminal.Nodes);
            return 0;
        }

        private void RecursionTreeControl(XmlNode xmlNode, TreeNodeCollection nodes)
        {
            foreach (XmlNode node in xmlNode.ChildNodes)//循环遍历当前元素的子元素集合
            {
                TreeNode child = new TreeNode();//定义一个TreeNode节点对象
                //new_child.Name = node.Attributes["name"].Value;
                string name = string.Empty; 
                string script = string.Empty;
                bool movable = false;
                string context = string.Empty;
                if (node.Attributes["name"] != null)
                {
                    name = node.Attributes["name"].Value;
                }
                if (node.Attributes["script"] != null)
                {
                    script = node.Attributes["script"].Value;
                }
                if (node.Attributes["movable"] != null)
                {
                    movable = int.Parse(node.Attributes["movable"].Value) == 1 ? true : false;
                }
                if (node.Attributes["context"] != null)
                {
                    context = node.Attributes["context"].Value;
                }
                child.Text = name;
                child.Tag = new TestItem(name, script, movable, context);
                nodes.Add(child);//向当前TreeNodeCollection集合中添加当前节点
                RecursionTreeControl(node, child.Nodes);//调用本方法进行递归
            }
        }

        private void tvTerminal_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode CurrentNode = tvTerminal.GetNodeAt(ClickPoint);
                if (CurrentNode != null)//判断你点的是不是一个节点
                {
                    if (CurrentNode.Tag != null)
                    {
                        TestItem item = CurrentNode.Tag as TestItem;
                        if (!string.IsNullOrEmpty(item.Script))
                        {
                            this.statusStrip1.Items[0].Text = item.Script;
                        }
                    }
                }
            }
        }

        private void tvTerminal_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode CurrentNode = tvTerminal.GetNodeAt(ClickPoint);
                if (CurrentNode != null)//判断你点的是不是一个节点
                {
                    tvTerminal.SelectedNode = CurrentNode;//选中这个节点
                    tvTerminal.LabelEdit = true;
                    tvTerminal.SelectedNode.BeginEdit();
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            TreeNode CurrentNode = this.tvTerminal.SelectedNode;
            if (CurrentNode != null)
            {
                if (CurrentNode.Tag != null)
                {
                    TestItem item = CurrentNode.Tag as TestItem;
                    //this.contextMenuStrip1.Items[0].Text = "Add " + item.Context;
                    this.contextMenuStrip1.Items[1].Text = "Delete " + item.Context;
                    this.contextMenuStrip1.Items[2].Enabled = item.Movable;
                    this.contextMenuStrip1.Items[3].Enabled = item.Movable;
                    switch (CurrentNode.Level)
                    {
                        case 0:
                            this.productToolStripMenuItem.Enabled = true;
                            this.modelToolStripMenuItem.Enabled = true;
                            this.stageToolStripMenuItem.Enabled = false;
                            this.procedureToolStripMenuItem.Enabled = false;
                            break;
                        case 1:
                            this.productToolStripMenuItem.Enabled = false;
                            this.modelToolStripMenuItem.Enabled = true;
                            this.stageToolStripMenuItem.Enabled = true;
                            this.procedureToolStripMenuItem.Enabled = false;
                            break;
                        case 2:
                            this.productToolStripMenuItem.Enabled = false;
                            this.modelToolStripMenuItem.Enabled = false;
                            this.stageToolStripMenuItem.Enabled = true;
                            this.procedureToolStripMenuItem.Enabled = true;
                            break;
                        case 3:
                            this.productToolStripMenuItem.Enabled = false;
                            this.modelToolStripMenuItem.Enabled = false;
                            this.stageToolStripMenuItem.Enabled = false;
                            this.procedureToolStripMenuItem.Enabled = true;
                            break;
                        default:
                            this.productToolStripMenuItem.Enabled = false;
                            this.modelToolStripMenuItem.Enabled = false;
                            this.stageToolStripMenuItem.Enabled = false;
                            this.procedureToolStripMenuItem.Enabled = false;
                            break;
                    }
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode CurrentNode = this.tvTerminal.SelectedNode;
            if (CurrentNode != null)
            {
                try
                {
                    CurrentNode.Remove();
                }
                catch (Exception ex)
                {
                    common.m_log.Add(ex.Message, LogHelper.MessageType.ERROR);
                }
            }
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void productToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int index = this.tvTerminal.Nodes.Count;
                TreeNode node = new TreeNode();
                node.Text = "Tablet " + index;
                node.Tag = new TestItem(node.Text, string.Empty, false, "Product");
                this.tvTerminal.Nodes.Add(node);
            }
            catch (Exception ex)
            {
                common.m_log.Add(ex.Message, LogHelper.MessageType.ERROR);
            }
        }

        private void modelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode CurrentNode = this.tvTerminal.SelectedNode;
            if (CurrentNode != null)
            {
                try
                {
                    if (CurrentNode.Level == 0)//root
                    {
                        int index = CurrentNode.Nodes.Count;
                        TreeNode node = new TreeNode();
                        node.Text = "Model " + index;
                        node.Tag = new TestItem(node.Text, string.Empty, false, "Model");
                        CurrentNode.Nodes.Add(node);
                    }
                    else//model node
                    {
                        int index = CurrentNode.Parent.Nodes.Count;
                        TreeNode node = new TreeNode();
                        node.Text = "Model " + index;
                        node.Tag = new TestItem(node.Text, string.Empty, false, "Model");
                        CurrentNode.Parent.Nodes.Add(node);
                    }
                }
                catch (Exception ex)
                {
                    common.m_log.Add(ex.Message, LogHelper.MessageType.ERROR);
                }
            }
        }

        private void stageToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void procedureToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        //
    }

    public class TestItem
    {
        private string _name;
        private string _script;
        private bool _movable;
        private string _context;

        public TestItem(string name, string script, bool movable, string context)
        {
            this._name = name;
            this._script = script;
            this._movable = movable;
            this._context = context;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Script
        {
            get { return _script; }
            set { _script = value; }
        }

        public string Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public bool Movable
        {
            get { return _movable; }
            set { _movable = value; }
        }

    }

}

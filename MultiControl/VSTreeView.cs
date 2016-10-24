using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace MultiControl
{
    /// <summary>
    /// 基于树视图的任务控件
    /// </summary>
    public class VSTreeView:TreeView
    {
        private BufferedGraphicsContext mContext = null;

        #region //construckor
        /// <summary>
        /// 
        /// </summary>
        public VSTreeView()
        {
            FullRowSelect = true;
            ShowLines = false;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer
            , true);
            this.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            this.ItemHeight = 20;
            this.ShowLines = true;

            mContext = BufferedGraphicsManager.Current;
            mContext.MaximumBuffer = new Size(Width + 1, Height + 1);
        }
        #endregion
        #region //field & property
        /// <summary>
        /// 展开按扭的大小
        /// </summary>
        private Size ExpandButtonSize = new Size(16, 16);
        private Color _ExpandButtonColor = Color.FromArgb(255,255,255);
        /// <summary>
        /// 展开按扭颜色
        /// </summary>
        [Description("展开按扭颜色"), Category("XOProperty")]
        public Color ExpandButtonColor
        {
            get
            {
                return _ExpandButtonColor;
            }
            set
            {
                if (_ExpandButtonColor != value)
                {
                    _ExpandButtonColor = value;
                    this.Invalidate();
                }
            }
        }
        private Color _GroupBgColor = Color.FromArgb(95,162,211);
        /// <summary>
        /// 分组栏背景色
        /// </summary>
        [Description("分组栏背景色"), Category("XOProperty")]
        public Color GroupBgColor
        {
            get
            {
                return _GroupBgColor;
            }
            set
            {
                if (_GroupBgColor != value)
                {
                    _GroupBgColor = value;
                    this.Invalidate();
                    
                }
            }
        }
        private Color _GroupTitleColor = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// 分组栏标题色
        /// </summary>
        [Description("分组栏标题色"), Category("XOProperty")]
        public Color GroupTitleColor
        {
            get
            {
                return _GroupTitleColor;
            }
            set
            {
                if (_GroupTitleColor != value)
                {
                    _GroupTitleColor = value;
                    this.Invalidate();
                }
            }
        }
        private Color _OverForeColor = Color.LightBlue;
        /// <summary>
        /// 鼠标悬停色
        /// </summary>
        [Description("鼠标悬停色"), Category("XOProperty")]
        public Color OverForeColor
        {
            get
            {
                return _OverForeColor;
            }
            set
            {
                if (_OverForeColor != value)
                {
                    _OverForeColor = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// 节点高度
        /// </summary>
        public new int ItemHeight
        {
            get
            {
                return base.ItemHeight;
            }
            set
            {
                if (base.ItemHeight != value && value>=20)
                {
                    base.ItemHeight = value;
                    this.Invalidate();
                }
            }
        }
        
        #endregion
        #region //override
        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            //base.OnDrawNode(e);
            DrawNodeItem(e);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0201)//单击
            {
                int wparam = m.LParam.ToInt32();
                Point point = new Point(
                    LOWORD(wparam),
                    HIWORD(wparam));
                //point = PointToClient(point);
                TreeNode tn = this.GetNodeAt(point);
                if (tn == null)
                {
                    base.WndProc(ref m);
                    return;
                }
                base.WndProc(ref m);
                //tn.IsSelected = true;
                this.SelectedNode = tn;
            }
            else if (m.Msg == 0x0203)//双击
            {
                int wparam = m.LParam.ToInt32();
                Point point = new Point(
                    LOWORD(wparam),
                    HIWORD(wparam));
                //point = PointToClient(point);
                TreeNode tn = this.GetNodeAt(point);
                if (tn == null)
                {
                    base.WndProc(ref m);
                    //return;
                }
                if (tn.Level == 0)
                {
                    base.WndProc(ref m);
                    //m.Result = IntPtr.Zero;
					//return;
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
            else if (m.Msg == 0x0200)//鼠标移动
            {
                try
                {
                    int wparam = m.LParam.ToInt32();
                    Point point = new Point(
                        LOWORD(wparam),
                        HIWORD(wparam));
                    //point = PointToClient(point);
                    TreeNode tn = this.GetNodeAt(point);
                    if (tn == null)
                    {
                        //this.SelectedNode = null;
                        base.WndProc(ref m);
                    }
                    //this.SelectedNode = tn;
                }
                catch { }
            }
            else if (m.Msg == 0x02A3)//鼠标移出 WM_MOUSELEAVE = $02A3;
            {
                //this.SelectedNode = null;
                base.WndProc(ref m);
            }
            else
            {
                base.WndProc(ref m);
            }
            //WM_LBUTTONDOWN = $0201
            //WM_LBUTTONDBLCLK = $0203;
        }
        #endregion

        #region //private method
        /// <summary>
        /// 自定义绘制节点
        /// </summary>
        /// <param name="e"></param>
        private void DrawNodeItem(DrawTreeNodeEventArgs e)
        {
            TreeNode tn = e.Node;
            if (tn.IsVisible)
            {
                //BufferedGraphics bg = mContext.Allocate(e.Graphics, e.Bounds);
                using (Graphics g = e.Graphics)
                {
                    if (tn.IsSelected)
                    {
                        g.FillRectangle(Brushes.Gray, e.Bounds);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.White, e.Bounds);
                    }
                    Rectangle collapse = ExpandButtonBounds(e.Bounds);
                    int x = collapse.Left + 2 + tn.Level * 20;
                    int y = collapse.Top + 3;

                    if (tn.IsExpanded)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            g.DrawLine(Pens.Blue, x, y, x, y + 4);
                            x++; y++;
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            g.DrawLine(Pens.Blue, x, y, x, y + 4);
                            x++; y--;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            g.DrawLine(Pens.Blue, x, y, x + 4, y);
                            x++; y++;
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            g.DrawLine(Pens.Blue, x, y, x + 4, y);
                            x--; y++;
                        }
                    }

                    //绘制分组的文本
                    TextRenderer.DrawText(g, e.Node.Text, this.Font, GroupTitleBounds(e.Bounds, tn.Level), this.GroupTitleColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }

                //bg.Render(e.Graphics);
                //bg.Dispose();
            }
            //END
        }
        /// <summary>
        /// 展开按扭区域
        /// </summary>
        /// <param name="childRect"></param>
        /// <returns></returns>
        private Rectangle ExpandButtonBounds(Rectangle childRect)
        {
            Rectangle lrect = new Rectangle(new Point(2, (childRect.Height - ExpandButtonSize.Height) / 2 + childRect.Top), ExpandButtonSize);
            return lrect;
        }
        /// <summary>
        /// 取得分组标题绘制空间
        /// </summary>
        /// <param name="childRect"></param>
        /// <returns></returns>
        private Rectangle GroupTitleBounds(Rectangle childRect, int level)
        {
            Rectangle lrect = childRect;
            lrect.Offset(20 + level * 20, 0);
            return lrect;
        }
        #endregion
        
        public static int LOWORD(int value)
        {
            return value & 0xFFFF;
        }
        public static int HIWORD(int value)
        {
            return value >> 16;
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace MultiControl
{
    public partial class UserButton : UserControl
    {
        private bool mMouseHover = false;
        private bool mMouseDown = false;
        public ButtonFormat mFormat;

        private BufferedGraphicsContext mContext;

        //Event System
        public event ItemClickedEventHandler ButtonClick;
        public delegate void ItemClickedEventHandler(System.Object sender, System.EventArgs e);

        private Color mColorTop;
        private Color mColorBottom;
        private Color mClickedColorTop;
        private Color mClickedColorBottom;

        private Color mFocusColor;

        public enum ButtonFormat
        {
            Normal = 0,
            Mininum = 1,
            Maxim = 2,
            Close = 3,
            About = 4
        }

        public UserButton()
        {
            InitializeComponent();

            mContext = BufferedGraphicsManager.Current;
            mContext.MaximumBuffer = new Size(Width + 1, Height + 1);

            mClickedColorTop = Color.Gray;//.FromArgb(216, 235, 198);
            mClickedColorBottom = Color.Gray;
            mColorTop = Color.Gray;
            mColorBottom = Color.Gray;
            mFocusColor = Color.Orange;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            BufferedGraphics bg = mContext.Allocate(this.CreateGraphics(), this.ClientRectangle);

            DrawBackground(bg);

            DrawButton(bg);

            bg.Render(e.Graphics);
            bg.Dispose();
        }

        private void DrawBackground(BufferedGraphics bg)
        {
            Graphics g = bg.Graphics;
            Rectangle r = new Rectangle(0, 0, Width, Height);
            LinearGradientBrush lgb = null;
            if (mMouseDown)
            {
                lgb = new LinearGradientBrush(r, mColorTop, mColorBottom, LinearGradientMode.Vertical);
            }
            else
            {
                if (mMouseHover)
                {
                    lgb = new LinearGradientBrush(r, mFocusColor, mClickedColorBottom, LinearGradientMode.Vertical);
                }
                else
                {
                    lgb = new LinearGradientBrush(r, mClickedColorTop, mClickedColorBottom, LinearGradientMode.Vertical);
                }
            }
            g.FillRectangle(lgb, r);
        }

        private void DrawButton(BufferedGraphics bg)
        {
            Graphics g = bg.Graphics;
            Pen pen = new Pen(Color.White);
            pen.Width = 3;
            if (mFormat == ButtonFormat.Mininum)
            {
                g.DrawLine(pen, ClientRectangle.Left + 5, ClientRectangle.Bottom - 6, ClientRectangle.Right - 5, ClientRectangle.Bottom - 6);
            }
            else if (mFormat == ButtonFormat.Close)
            {
                pen.Width = 1;
                //g.DrawLine(pen, ClientRectangle.Left + 10, ClientRectangle.Bottom - 6, ClientRectangle.Right - 10, ClientRectangle.Top + 6);
                //g.DrawLine(pen, ClientRectangle.Left + 10, ClientRectangle.Top + 6, ClientRectangle.Right - 10, ClientRectangle.Bottom - 6);
                int size = ClientRectangle.Height - 10;
                int x = (ClientRectangle.Width - size) / 2;
                int y = (ClientRectangle.Height - size) / 2;
                int endx = x + size;
                int endy = y + size;
                g.DrawLine(pen, x + 1, y + 1, endx - 1, endy - 1);
                g.DrawLine(pen, x, y + 1, endx - 1, endy);
                g.DrawLine(pen, x + 1, y, endx, endy - 1);
                g.DrawLine(pen, x + 1, endy - 1, endx - 1, y + 1);
                g.DrawLine(pen, x, endy - 1, endx - 1, y);
                g.DrawLine(pen, x + 1, endy, endx, y + 1);
            }
            else if (mFormat == ButtonFormat.About)
            {
                int top = ClientRectangle.Top + 6;
               // g.DrawLine(pen, ClientRectangle.Left + 4, top, ClientRectangle.Right - 4, top);
                pen.Width = 1;
                int height = ClientRectangle.Width - 8;
                top += 2;
                int start = ClientRectangle.Left + 4;
                int end = ClientRectangle.Right - 4;
                for (int i = 0; i < height; i++)
                {
                    g.DrawLine(pen, start, top, end, top);
                    top++; start++; end--;
                    if (start == end)
                    {
                        g.DrawLine(pen, start, top, start, top - 2);
                        break;
                    }
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (ButtonClick != null)
            {
                ButtonClick(this, e);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mMouseDown = true;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (ClientRectangle.Contains(e.Location))
            {
                mMouseHover = true;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mMouseHover = false;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mMouseDown = false;
            Invalidate();
        }
    }
}

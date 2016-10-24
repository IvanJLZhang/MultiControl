using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DutClassLibrary
{
    public partial class DutControl : UserControl
    {
        public delegate void ResetControlHandle(object sender, ResetEventArgs e);
        public event ResetControlHandle ContentReset;
        //private bool bChecked = false;
        //private Color mOldColor;
        private BufferedGraphicsContext mContext = null;

        public const int SweepAngle = 90;

        private int _id = -1;

        private bool _focus = false;
        private Color _colorFrame = Color.Black;
        private float _frameBorder = 1.0f;

        private bool _checked = false;
        private Pen _checkedPen = null;

        private bool _active = false;

        public Pen CheckedPen
        {
            get { return _checkedPen; }
            set { _checkedPen = value; }
        }

        public bool IsActivedDut
        {
            get { return _active; }
            set { _active = value; }
        }

        public bool ManualChecked
        {
            get { return _checked; }
            set { 
                    _checked = value;
                    if (_checked)
                    {
                        this.checkBox1.Checked = true;
                    }
                    else
                    {
                        this.checkBox1.Checked = false;
                    }
                    Invalidate();
                }
        }

        public bool ManualFocus
        {
            get { return _focus; }
            set
            {
                if (_focus != value)
                {
                    _focus = value;
                    SetFocus(_focus);
                }
            }
        }

        public DutControl()
        {
            InitializeControlStyle();

            InitializeComponent();

            this.Resize += new System.EventHandler(this.LayoutSubControls);
            //mOldColor = this.BackColor;

            InitializeControlProperty();
            //SetFocus(false);
            this.checkBox1.Visible = false;

            _checkedPen = new Pen(new SolidBrush(Color.Green), this.BorderThickness);
        }

        private void InitializeControlStyle()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            mContext = BufferedGraphicsManager.Current;
            mContext.MaximumBuffer = new Size(Width + 1, Height + 1);
        }

        private void LayoutSubControls(object sender, EventArgs e)
        {
            //this.progressBar1.Location = new Point(this.labelID.Width + 2, this.progressBar1.Location.Y);
            this.labelPercentage.Location = new Point(this.Width - 4 - this.labelPercentage.Width - 4,this.labelPercentage.Location.Y);
            this.progressBar1.Size = new Size(this.Width - this.labelPercentage.Width - 20, this.progressBar1.Height);
            this.checkBox1.Location = new Point(this.Width - this.checkBox1.Width - 8, this.checkBox1.Location.Y);
            this.pictureBoxIMEI.Location = new Point(this.Width - this.pictureBoxIMEI.Width - 10, this.pictureBoxIMEI.Location.Y);
        }

        public void RegisterSubControlEvent()
        {
            
        }

        public void Reset()
        {
            this.labelSN.Text = string.Empty;
            this.labelIMEI.Text = string.Empty;
            this.labelModel.Text = string.Empty;
            this.labelCPU.Text = string.Empty;
            this.labelPercentage.Text = string.Empty;
            this.pictureBoxIMEI.Image = null;
            this.progressBar1.Value = 0;
            this.progressBar1.UpdateBarContent(string.Empty);

            if (ContentReset != null)
            {
                int id = int.Parse(this.labelID.Text);
                ContentReset(this, new ResetEventArgs(id,string.Empty));
            }
        }

        public void SetQRCode_SN(Image image)
        {
            this.pictureBoxSN.Image = image;
        }

        public void SetQRCode_IMEI(Image image)
        {
            this.pictureBoxIMEI.Image = image;
        }

        public void SetDutStatus(string status)
        {
            this.labelStatus.Text = status;
        }

        public void SetDutItem(string item)
        {
            this.labelItem.Text = item;
            this.progressBar1.UpdateBarContent(item);
        }

        public void SetDutPercentage(string percentage)
        {
            this.labelPercentage.Text = percentage;
        }

        public string GetDutSN()
        {
            return this.labelSN.Text;
        }

        public void SetDutSN(string sn)
        {
            this.labelSN.Text = sn;
            this.progressBar1.Visible = true;
        }

        public void SetDutCPU(string model)
        {
            this.labelCPU.Text = model;
        }

        public void SetDutModel(string model)
        {
            this.labelModel.Text = model;
        }

        public void SetDutIMEI(string imei)
        {
            this.labelIMEI.Text = imei ;
        }

        public void SetDutProgress(int current, int total)
        {
            this.progressBar1.Maximum = total;
            //this.progressBar1.Step = 1;
            this.progressBar1.Value = current;
        }

        public void SetDutGridID(int id)
        {
            this.labelID.Text = id.ToString();
            _id = id;
        }

        public int ID
        {
            get { return _id; }
        }

        public int GetDutGridID()
        {
            return int.Parse(labelID.Text);
        }

        private void DutControl_Click(object sender, EventArgs e)
        {
            
        }

        public bool IsDutSelected()
        {
            return this.checkBox1.Checked;
        }
        /*
        private void DutControl_MouseMove(object sender, MouseEventArgs e)
        {
            Console.WriteLine("{0} - {1}", e.X.ToString(), e.Y.ToString());
        }
        */

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            //BufferedGraphics bg = mContext.Allocate(this.CreateGraphics(), this.ClientRectangle);
            //DrawBackground(bg);
            //bg.Render(e.Graphics);
            //bg.Dispose();
            DrawBackground(e.Graphics);
        }

        private void SetFocus(bool focus)
        {
            if (focus)
            {
                _colorFrame = Color.OrangeRed;
                _frameBorder = 2.0f;
            }
            else
            {
                _colorFrame = this.BorderColor;
                _frameBorder = this.BorderThickness;
            }
            Invalidate();
        }


        private void DrawBackground(System.Drawing.Graphics g)
        {
            //Graphics g = bg.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int ArcWidth = this.C_RoundCorners * 2;
            int ArcHeight = this.C_RoundCorners * 2;
            int ArcX1 = 0;
            int ArcX2 = (this.ShadowControl) ? (this.Width - (ArcWidth + 1)) - this.ShadowThickness : this.Width - (ArcWidth + 1);
            int ArcY1 = 0;
            int ArcY2 = (this.ShadowControl) ? (this.Height - (ArcHeight + 1)) - this.ShadowThickness : this.Height - (ArcHeight + 1);

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            System.Drawing.Brush BorderBrush = new SolidBrush(_colorFrame);//this.BorderColor
            System.Drawing.Pen BorderPen = new Pen(BorderBrush, _frameBorder);//this.BorderThickness
            System.Drawing.Drawing2D.LinearGradientBrush BackgroundGradientBrush = null;
            System.Drawing.Brush BackgroundBrush = new SolidBrush(this.BackgroundColor);
            System.Drawing.SolidBrush ShadowBrush = null;
            System.Drawing.Drawing2D.GraphicsPath ShadowPath = null;

            //画出阴影效果
            if (this.ShadowControl)
            {
                ShadowBrush = new SolidBrush(this.ShadowColor);
                ShadowPath = new System.Drawing.Drawing2D.GraphicsPath();
                ShadowPath.AddArc(ArcX1 + this.ShadowThickness, ArcY1 + this.ShadowThickness, ArcWidth, ArcHeight, 180, SweepAngle);//顶端的左角
                ShadowPath.AddArc(ArcX2 + this.ShadowThickness, ArcY1 + this.ShadowThickness, ArcWidth, ArcHeight, 270, SweepAngle);//顶端右角
                ShadowPath.AddArc(ArcX2 + this.ShadowThickness, ArcY2 + this.ShadowThickness, ArcWidth, ArcHeight, 360, SweepAngle);//底部右角
                ShadowPath.AddArc(ArcX1 + this.ShadowThickness, ArcY2 + this.ShadowThickness, ArcWidth, ArcHeight, 90, SweepAngle);//底部左角 
                ShadowPath.CloseAllFigures();

                g.FillPath(ShadowBrush, ShadowPath);
            }

            //画出图形
            path.AddArc(ArcX1, ArcY1, ArcWidth, ArcHeight, 180, SweepAngle);
            path.AddArc(ArcX2, ArcY1, ArcWidth, ArcHeight, 270, SweepAngle);
            path.AddArc(ArcX2, ArcY2, ArcWidth, ArcHeight, 360, SweepAngle);
            path.AddArc(ArcX1, ArcY2, ArcWidth, ArcHeight, 90, SweepAngle);
            path.CloseAllFigures();

            if (this.C_BackgroundGradientMode == DutBoxGradientMode.None)
            {
                g.FillPath(BackgroundBrush, path);
            }
            else
            {
                BackgroundGradientBrush = new LinearGradientBrush(new Rectangle(0, 0, this.Width, this.Height), this.BackgroundColor, this.BackgroundGradientColor, (LinearGradientMode)this.BackgroundGradientMode);

                g.FillPath(BackgroundGradientBrush, path);
            }
            //画对号
            if (IsActivedDut && ManualChecked)
            {
                //g.DrawLine(BorderPen, 10, 10, 20, 20);
                int x = Width - 30;
                int y = 12;
                /*
                x--; y++;
                g.DrawLine(pen, x, y, x, y + 4);
                x--; y++;
                g.DrawLine(pen, x, y, x, y + 2);
                x--; y++;
                x = Width - 30;
                y = 12;
                 * */
                for (int i = 0; i < 6; i++)
                {
                    g.DrawLine(CheckedPen, x, y, x, y+6);
                    x++; y++;
                }
                for (int i = 0; i < 12; i++)
                {
                    g.DrawLine(CheckedPen, x, y, x, y + 6);
                    x++; y--;
                }
                /*
                g.DrawLine(pen, x, y + 2, x, y + 6);
                x++; y++;
                g.DrawLine(pen, x, y + 2, x, y + 4);
                x++; y++;
                 * */
            }
            //画边框
            if (IsActivedDut)
            {
                g.DrawPath(BorderPen, path);
            }
            else
            {
                System.Drawing.Brush defaultBrush = new SolidBrush(this.BorderColor);
                System.Drawing.Pen defaultPen = new Pen(defaultBrush, this.BorderThickness);
                g.DrawPath(defaultPen, path);
            }
            //销毁Graphic 对象
            if (path != null)
            {
                path.Dispose();
            }
            if (BorderBrush != null)
            {
                BorderPen.Dispose();
            }
            if (BorderPen != null)
            {
                BorderPen.Dispose();
            }

            if (BackgroundGradientBrush != null)
            {
                BackgroundGradientBrush.Dispose();
            }
            if (BackgroundBrush != null)
            {
                BackgroundBrush.Dispose();
            }
            if (ShadowBrush != null)
            {
                ShadowBrush.Dispose();
            }
            if (ShadowPath != null)
            {
                ShadowPath.Dispose();
            }
        }

        [Category("Appearance"), Description("")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                this.Refresh();
            }
        }
        [Category("Appearance"), Description("")]
        public int ShadowThickness
        {
            get
            {
                return C_ShadowThickness;
            }
            set
            {
                if (value > 10)
                {
                    C_ShadowThickness = 10;
                }
                else
                {
                    if (value < 1)
                    {
                        C_ShadowThickness = 1;
                    }
                    else
                    {
                        C_ShadowThickness = value;
                    }
                }
                this.Refresh();
            }
        }
        [Category("Appearance"), Description("")]
        public System.Drawing.Color BackgroundColor
        {
            get
            {
                return C_BackgroundColor;
            }
            set
            {
                C_BackgroundColor = value;
                this.Refresh();
            }
        }
        [Category("Appearance"), Description("")]
        public bool ShadowControl
        {
            get
            {
                return C_ShadowControl;
            }
            set
            {
                C_ShadowControl = value;
                this.Refresh();
            }

        }
        [Category("Appearance"), Description("")]
        public float BorderThickness
        {
            get
            {
                return C_BorderThickness;
            }
            set
            {
                if (value > 3)
                {
                    C_BorderThickness = 3;
                }
                else
                {
                    if (value < 1)
                    {
                        C_BorderThickness = 1;
                    }
                    else
                    {
                        C_BorderThickness = value;
                    }
                }
                this.Refresh();
            }

        }
        [Category("Appearance"), Description("")]
        public System.Drawing.Color BorderColor
        {
            get
            {
                return C_BorderColor;
            }
            set
            {
                C_BorderColor = value;
                this.Refresh();
            }
        }
        [Category("Appearance"), Description("")]
        public System.Drawing.Color ShadowColor
        {
            get
            {
                return C_ShadowColor;
            }
            set
            {
                C_ShadowColor = value;
                this.Refresh();
            }
        }
        [Category("Appearance"), Description("")]
        public DutBoxGradientMode BackgroundGradientMode
        {
            get
            {
                return C_BackgroundGradientMode;
            }
            set
            {
                C_BackgroundGradientMode = value;
                this.Refresh();
            }
        }
        [Category("Appearance"), Description("")]
        public int RoundCorners
        {
            get
            {
                return C_RoundCorners;
            }
            set
            {
                if (value > 25)
                {
                    C_RoundCorners = 25;
                }
                else
                {
                    if (value < 1)
                    {
                        C_RoundCorners = 1;
                    }
                    else
                    {
                        C_RoundCorners = value;
                    }
                }
                this.Refresh();
            }
        }
        [Category("Appearance"), Description("")]
        public System.Drawing.Color BackgroundGradientColor
        {
            get
            {
                return C_BackgroundGradientColor;
            }
            set
            {
                C_BackgroundGradientColor = value;
                this.Refresh();
            }
        }

        public enum DutBoxGradientMode
        {
            None = 4,

            BackwardDiagonal = 3,

            ForwardDiagonal = 2,

            Horuzontal = 0,

            Vertical = 1

        }
        private int C_RoundCorners = 10;
        private bool C_ShadowControl = false;
        private System.Drawing.Color C_BackgroundColor = Color.White;
        private System.Drawing.Color C_ShadowColor = Color.DarkGray;
        private float C_BorderThickness = 1;
        private int C_ShadowThickness = 3;
        private System.Drawing.Color C_BorderColor = Color.Black;
        private System.Drawing.Color C_BackgroundGradientColor = Color.White;
        private DutBoxGradientMode C_BackgroundGradientMode = DutBoxGradientMode.None;

        private void InitializeControlProperty()
        {
            ShadowControl = true;
            BackgroundGradientMode = DutBoxGradientMode.Vertical;
            BackgroundGradientColor = Color.Gray;
            this.labelItem.Visible = false;
            this.progressBar1.Visible = false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                //cp.Style |= WS_MINIMIZEBOX;
                //cp.ClassStyle |= CS_DBLCLKS;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
    }

    public class MyProgressBar : ProgressBar
    {
        private string mContent = string.Empty;

        public MyProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;
            Graphics g = e.Graphics;

            if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(g, rect);

            Pen pen = new Pen(Color.DeepSkyBlue, 1);
            g.DrawRectangle(pen, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
            rect.Inflate(-1, -1);
            g.FillRectangle(new SolidBrush(Color.FromArgb(196, 255, 255)), rect);

            rect.Inflate(-1, -1);
            if (Value > 0)
            {
                var clip = new Rectangle(rect.X, rect.Y, (int)((float)Value / Maximum * rect.Width), rect.Height);
                //ProgressBarRenderer.DrawHorizontalChunks(g, clip);
                g.FillRectangle(new SolidBrush(Color.DarkOrange), clip);
            }

            if (!string.IsNullOrEmpty(mContent))
            {
                using (var font = new Font("Arial", 8.0f))
                {
                    SizeF sz = g.MeasureString(mContent, font);
                    var location = new PointF(rect.Width / 2 - sz.Width / 2, rect.Height / 2 - sz.Height / 2 + 2);
                    g.DrawString(mContent, font, Brushes.Black, location);
                }
            }
        }

        public void UpdateBarContent(string content)
        {
            mContent = content;
            this.Invalidate();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MyProgressBar
            //
            this.ResumeLayout(false);

        }

    }

    public class ResetEventArgs : EventArgs
    {
        private string message;
        private int id;
        public ResetEventArgs(int id, string message)
        {
            this.id = id;
            this.message = message;
        }

        public string Message
        {
            get { return message; }
        }

        public int ID
        {
            get { return id; }
        }
    }
}

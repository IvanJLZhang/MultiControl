using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Printing;

namespace UserGridClassLibrary
{
    public class GridProgressBar
    {
        private int _min;
        private int _max;
        private int _value;
        private string _caption;
        private Rectangle _rect;
        private Rectangle _printIocn;    //20161223 bonnie  
        public GridProgressBar()
        {
           
        }

        

        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public Rectangle Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }

        public Rectangle PrintIocn//20161323 bonnie
        {
            get { return _printIocn; }
            set { _printIocn = value; }
        }

        public void Draw(Graphics g)
        {
            StringFormat alignFormat = new StringFormat();
            alignFormat.LineAlignment = StringAlignment.Center;
            Font font = null;// new Font("Arial", 8.75F);
            //BAR
            Rectangle bar = new Rectangle(Rect.Left, Rect.Top, Rect.Width - 48, Rect.Height);
            if (Value >= 0 && Max > 0)
            {
                g.DrawRectangle(Pens.Green, bar);
                int tWidth = bar.Width - 3;
                float pValue = (float)Value / (float)(Max);
                float pWidth = pValue * tWidth;
                RectangleF progress = new RectangleF(bar.Left + 2, bar.Top + 2, pWidth, bar.Height - 3);
                g.FillRectangle(Brushes.DarkOrange, progress);
            }
            //ITEM CONTENT
            if (!string.IsNullOrEmpty(Caption))
            {
                g.DrawRectangle(Pens.Green, bar);
                font = new Font("Arial", 8.75F);
                alignFormat.Alignment = StringAlignment.Center;
                g.DrawString(Caption, font, Brushes.Black, bar, alignFormat);
            }
            //PROGRESS
            if (Max > 0)
            {
                bar = new Rectangle(bar.Right, Rect.Top, 46, Rect.Height);
                font = new Font("Arial", 10.25F);
                alignFormat.Alignment = StringAlignment.Far;
                int Total = Max; // +1 = PQAA INSTALL
                g.DrawString(Value + "/" + Total, font, Brushes.Black, bar, alignFormat);
            }
        }

        public void Reset()
        {
            Min = 0;
            Max = 0;
            Value = 0;
            Caption = string.Empty;
        }
    }

    public enum TestWay
    {
        TW_NONE,
        TW_AUTO,
        TW_MANUAL
    }

    public enum ItemResult
    {
        IR_NONE,
        IR_TESTING,
        IR_PASS,
        IR_FAIL
    }

    public class GridItem
    {
        private int _id;
        private string _name;
        private Rectangle _rect;
        private Rectangle _printIocn;//bonnie
        private bool _connected;
        private bool _active;
        private bool _checked;
        private bool _isPrint;//bonnie
        private bool _focus;
        private string _status;
        private string _serial;
        private string _imei;
        private string _model;
        private Image _qrCode;
        private Image _PrintIocn;
        private GridProgressBar _progressBar = new GridProgressBar();
        //Additional Request
        private TestWay _testWay;
        private float _estimateTime;
        private float _elapsedTime;

        private float _installTime;
        private ItemResult _result;

        //private Color _colorFrame = Color.Black;
        //private float _focusBorder = 1.0f;
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
        public delegate void ResetControlHandle(object sender, ResetEventArgs e);
        public event ResetControlHandle OnContentReset;

        public class GridItemInvalidateEventArgs : EventArgs
        {
            private string message;
            private int id;
            private Rectangle _rect;
            public GridItemInvalidateEventArgs(int id, string message)
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

            public Rectangle InvalidRect
            {
                get { return _rect; }
                set { _rect = value; }
            }
        }
        public delegate void GridItemInvalidateHandle(object sender, GridItemInvalidateEventArgs e);
        public event GridItemInvalidateHandle OnGridItemInvalidate;

        public enum DutBoxGradientMode
        {
            None = 4,

            BackwardDiagonal = 3,

            ForwardDiagonal = 2,

            Horuzontal = 0,

            Vertical = 1

        }
        private int C_RoundCorners = 10;
        private bool C_ShadowControl = true;
        private System.Drawing.Color C_BackgroundColor = Color.White;
        private System.Drawing.Color C_ShadowColor = Color.DarkGray;
        private float C_BorderThickness = 1;
        private int C_ShadowThickness = 3;
        private System.Drawing.Color C_BorderColor = Color.Black;
        private System.Drawing.Color C_BackgroundGradientColor = Color.FromArgb(192, Color.Gray);
        private DutBoxGradientMode C_BackgroundGradientMode = DutBoxGradientMode.Vertical;
        public const int SweepAngle = 90;

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
                C_BackgroundGradientColor = Color.FromArgb(192, value);
            }
        }

        public GridItem()
        {
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
        public bool Print
        {
            get { return _isPrint; }
            set { _isPrint = value; }
        }
        public bool Checked
        {
            get { return _checked; }
            set { _checked = value; }
        }

        public ItemResult Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public bool Focus
        {
            get { return _focus; }
            set { _focus = value; }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string SerialNumber
        {
            get { return _serial; }
            set { _serial = value; }
        }

        public string IMEI
        {
            get { return _imei; }
            set { _imei = value; }
        }

        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public Image QRCode
        {
            get { return _qrCode; }
            set { _qrCode = value; }
        }
        
        public Image PrintIocnImage
        {
            get { return _PrintIocn; }
            set { _PrintIocn = value; }
        }
        public Rectangle PrintIocn//20161223 bonnie
        {
            get { return _printIocn; }
            set { _printIocn = value; }
        }

        public Rectangle Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }

        public GridProgressBar ProgressBar
        {
            get { return _progressBar; }
            set { _progressBar = value; }
        }

        public TestWay Way
        {
            get { return _testWay; }
            set { _testWay = value; }
        }

        public float EstimateTime
        {
            get { return _estimateTime; }
            set { _estimateTime = value; }
        }

        public float ElapsedTime
        {
            get { return _elapsedTime; }
            set { _elapsedTime = value; }
        }

        public float InstallTime
        {
            get { return _installTime; }
            set { _installTime = value; }
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int ArcWidth = this.C_RoundCorners * 2;
            int ArcHeight = this.C_RoundCorners * 2;
            int ArcX1 = Rect.Left;
            int ArcX2 = (this.ShadowControl) ? (Rect.Left + Rect.Width - (ArcWidth + 1)) - this.ShadowThickness : Rect.Left + Rect.Width - (ArcWidth + 1);
            int ArcY1 = Rect.Top;
            int ArcY2 = (this.ShadowControl) ? (Rect.Top + Rect.Height - (ArcHeight + 1)) - this.ShadowThickness : Rect.Top + Rect.Height - (ArcHeight + 1);

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            System.Drawing.Brush BorderBrush = new SolidBrush(Color.Orange);
            System.Drawing.Pen BorderPen = new Pen(BorderBrush, 2.0f);
            System.Drawing.Drawing2D.LinearGradientBrush BackgroundGradientBrush = null;
            System.Drawing.Brush BackgroundBrush = new SolidBrush(Color.White);
            System.Drawing.SolidBrush ShadowBrush = null;
            System.Drawing.Drawing2D.GraphicsPath ShadowPath = null;

            //画出阴影效果
            if (this.ShadowControl)
            {
                Color shadowColor = Color.FromArgb(192, this.ShadowColor);
                //说明：1-（128/255）=1-0.5=0.5 透明度为0.5，即50% 
                ShadowBrush = new SolidBrush(shadowColor);
                //ShadowBrush = new SolidBrush(this.ShadowColor);
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
                BackgroundBrush = new SolidBrush(this.BackgroundGradientColor);
                g.FillPath(BackgroundBrush, path);
            }
            else
            {
                Color startColor = Color.FromArgb(192, Color.White);
                BackgroundGradientBrush = new LinearGradientBrush(new Rectangle(Rect.Left, Rect.Top, Rect.Width, Rect.Height), startColor, this.BackgroundGradientColor, (LinearGradientMode)this.BackgroundGradientMode);

                g.FillPath(BackgroundGradientBrush, path);
            }
            //画边框
            if (Focus)
            {
                g.DrawPath(BorderPen, path);
            }
            else
            {
                System.Drawing.Brush defaultBrush = new SolidBrush(this.BorderColor);
                System.Drawing.Pen defaultPen = new Pen(defaultBrush, this.BorderThickness);
                g.DrawPath(defaultPen, path);

                if (defaultBrush != null)
                {
                    defaultBrush.Dispose();
                }
                if (defaultPen != null)
                {
                    defaultPen.Dispose();
                }
            }
            //画内容 ID
            Font font = new Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            g.DrawString((ID + 1).ToString(), font, Brushes.Black, Rect.Left + 6, Rect.Top + 6);
            if (Connected)
            {
                //画状态
                if (!string.IsNullOrEmpty(Status))
                {
                    font = new Font("Arial", 11.25F, FontStyle.Bold);
                    g.DrawString(Status, font, Brushes.Black, Rect.Left + 6 + 56, Rect.Top + 6 + 2);
                }
                //画进度条

                Rectangle bar = new Rectangle(Rect.Left + 6, Rect.Top + 28, Rect.Width - 10, 14);
                ProgressBar.Rect = bar;
                g.SmoothingMode = SmoothingMode.Default;
                ProgressBar.Draw(g);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                /* Button but = new Button( );
                 but.Text = "打印";
                 but.Location = new Point(Rect.Width - 5,14);*/

                //画打印图标 20161223 bonnie
                string haarXmlPath = @"../../打印图标.png";

                FileInfo file = new FileInfo(haarXmlPath);
                string fullName = file.FullName;
             
                //画SN
                if (!string.IsNullOrEmpty(SerialNumber))
                {
                    font = new Font("Arial", 10.25F);
                    g.DrawString("SN:" + SerialNumber, font, Brushes.Black, Rect.Left + 6, Rect.Top + 6 + 40);
                }
                //画IMEI
                if (!string.IsNullOrEmpty(IMEI))
                {
                    font = new Font("Arial", 10.25F);
                    g.DrawString("IMEI:" + IMEI, font, Brushes.Black, Rect.Left + 6, Rect.Top + 6 + 60);
                }
                font = new Font("Arial", 10.25F, FontStyle.Bold);
                //画time
                if (ElapsedTime > 0.0f)
                {
                    g.DrawString((int)ElapsedTime + "/" + (int)EstimateTime, font, Brushes.Black, Rect.Left + 6, Rect.Top + 6 + 80);
                }
                //画测试方式
                switch (Way)
                {
                    case TestWay.TW_AUTO:
                        g.DrawString("Auto", font, Brushes.DarkGreen, Rect.Left + 6 + 90, Rect.Top + 6 + 80);
                        break;
                    case TestWay.TW_MANUAL:
                        g.DrawString("Manual", font, Brushes.Red, Rect.Left + 6 + 80, Rect.Top + 6 + 80);
                        break;
                    case TestWay.TW_NONE:
                        break;
                }
                //画Model
                if (!string.IsNullOrEmpty(Model))
                {
                    font = new Font("Arial", 10.25F);
                    g.DrawString(Model, font, Brushes.Black, Rect.Left + 6, Rect.Top + 6 + 100);
                }
                //画QRCode 48 x 48
                if (QRCode != null)
                {
                    bar = new Rectangle(Rect.Right - 58, Rect.Top + 84, 48, 48);
                    //g.FillRectangle(Brushes.Red, bar);
                    g.SmoothingMode = SmoothingMode.None;
                    g.DrawImageUnscaled(QRCode, bar);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }

                if(QRCode != null)
                {
                    PrintIocnImage=Image.FromFile(Application.StartupPath + "\\打印图标.png");
                    g.SmoothingMode = SmoothingMode.None;
                    g.DrawImage(PrintIocnImage, Rect.Right - 98, Rect.Top + 104);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }
                
                
            }
            else // disconnected - TESTING & FAIL
            {
                if (Result == ItemResult.IR_TESTING)
                {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.LineAlignment = StringAlignment.Center;
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.FormatFlags = StringFormatFlags.LineLimit;
                    stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                    font = new Font("Arial", 16.25F, FontStyle.Bold);

                    g.DrawString("OFFLINE", font, Brushes.White, Rect, stringFormat);
                }
            }
            //画对号
            if (Checked)
            {
                //g.DrawLine(BorderPen, 10, 10, 20, 20);
                int x = Rect.Left + Rect.Width - 30;
                int y = Rect.Top + 8;
                Pen pen = new Pen(new SolidBrush(Color.DarkOrange), this.BorderThickness);
                for (int i = 0; i < 6; i++)
                {
                    g.DrawLine(pen, x, y, x, y + 6);
                    x++; y++;
                }
                for (int i = 0; i < 11; i++)
                {
                    g.DrawLine(pen, x, y, x, y + 6);
                    x++; y--;
                }
                if (pen != null)
                {
                    pen.Dispose();
                }
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

        //Function to update
        public void Reset()
        {
            Status = string.Empty;
            SerialNumber = string.Empty;
            IMEI = string.Empty;
            Model = string.Empty;
            QRCode = null;
            ProgressBar.Reset();
            Way = TestWay.TW_NONE;
            Result = ItemResult.IR_NONE;
            ElapsedTime = 0.0f;
            BackgroundGradientMode = DutBoxGradientMode.Vertical;
            BackgroundGradientColor = Color.FromArgb(192, Color.Gray);
            Active = false;
            if (OnContentReset != null)
            {
                OnContentReset(this, new ResetEventArgs(ID, string.Empty));
            }
        }

        public void SetQRCode_IMEI(Image image)
        {
            QRCode = image;
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public void SetDutStatus(string status)
        {
            Status = status;
            if (Status.Equals("Online"))
            {
                Connected = true;
            }
            else
            {
                Connected = false;
            }
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public void SetItemTestWay(TestWay way)
        {
            this.Way = way;
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public void SetDutItem(string item, bool auto = true)
        {
            ProgressBar.Caption = item;
            if (auto)
            {
                this.Way = TestWay.TW_AUTO;
            }
            else
            {
                this.Way = TestWay.TW_MANUAL;
            }
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public void UpdateElapseTime(float time)
        {
            ElapsedTime = time;
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public void SetDutProgress(string item, int value, int max)
        {
            ProgressBar.Caption = item;
            ProgressBar.Value = value;
            ProgressBar.Max = max;
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public void SetDutProgress(int value, int max)
        {
            ProgressBar.Value = value;
            ProgressBar.Max = max;
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public void SetDutProgress(int max)
        {
            ProgressBar.Max = max;
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public string GetDutSN()
        {
            return SerialNumber;
        }

        public void SetDutSN(string sn)
        {
            SerialNumber = sn;
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public void SetDutModel(string model)
        {
            Model = model;
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public void SetDutIMEI(string imei)
        {
            IMEI = imei;
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public void SetDutGridID(int id)
        {
            ID = id;
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }

        public int GetDutGridID()
        {
            return ID;
        }

        public bool IsDutSelected()
        {
            return Checked;
        }

        public void SetFocus(bool focus)
        {
            Focus = focus;
            if (OnGridItemInvalidate != null)
            {
                GridItemInvalidateEventArgs args = new GridItemInvalidateEventArgs(ID, string.Empty);
                args.InvalidRect = Rect;
                OnGridItemInvalidate(this, args);
            }
        }
    }

    public partial class UserGrid : UserControl, IEnumerable
    {
        private int _row;
        private int _column;

        private int _sidePadding = 6;
        private int _gridPadding = 4;

        private BufferedGraphicsContext mContext = null;

        private List<GridItem> _gridItems = new List<GridItem>();

        private int _itemWidth;
        private int _itemHeight;

        private int mLastHover = -1;

        public Bitmap mImageBackground = null;

        private List<string> mInfo = new List<string>();

        public UserGrid()
        {
            InitializeComponent();

            mContext = BufferedGraphicsManager.Current;
            mContext.MaximumBuffer = new Size(Width + 1, Height + 1);

            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
            ControlStyles.Opaque, true);
            this.BackColor = Color.Transparent;

            mInfo.Add("Realtime");
            mInfo.Add("Intelligent");
            mInfo.Add("Visualization");
            mInfo.Add("Multiple terminals");
            mInfo.Add("Script Supported");
            mInfo.Add("High Performance");
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = 0x20;
                return cp;
            }
        }

        public List<GridItem> GridItems
        {
            get { return _gridItems; }
        }

        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }

        public int SidePadding
        {
            get { return _sidePadding; }
            set { _sidePadding = value; }
        }

        public int GridPadding
        {
            get { return _gridPadding; }
            set { _gridPadding = value; }
        }

        public int ItemWidth
        {
            get { return _itemWidth; }
        }

        public int ItemHeight
        {
            get { return _itemHeight; }
        }

        public void InitializeGridItems()
        {
            _itemWidth = (Width - SidePadding * 2 - (Column - 1) * GridPadding) / Column;
            _itemHeight = (Height - SidePadding * 2 - (Row - 1) * GridPadding) / Row;

            int id = 0;
            for (int index = 0; index < Row; index++)
            {
                for (int indey = 0; indey < Column; indey++)
                {
                    //GridItem item = new GridItem();
                    //item.ID = id;
                    //GridItems.Add(item);
                    AddGridItem();
                    id++;
                }
            }

            //mContext.MaximumBuffer = new Size(Width + 1, Height + 1);
        }

        public void AddGridItem()
        {
            GridItem item = new GridItem();
            item.ID = GridItems.Count;
            item.OnGridItemInvalidate += new GridItem.GridItemInvalidateHandle(this.OnGridItemInvalidateHandle);
            item.OnContentReset += new GridItem.ResetControlHandle(this.OnResetControlHandle);
            GridItems.Add(item);
        }

        private void OnGridItemInvalidateHandle(object sender, GridItem.GridItemInvalidateEventArgs args)
        {
            this.Invalidate(args.InvalidRect);
        }

        private void OnResetControlHandle(object sender, GridItem.ResetEventArgs args)
        {
            this.Invalidate(GridItems[args.ID].Rect);
            if (OnGridItemReset != null)
            {
                OnGridItemReset(this, args);
            }
        }

        private void UserGrid_Paint(object sender, PaintEventArgs e)
        {
            BufferedGraphics bg = mContext.Allocate(this.CreateGraphics(), this.ClientRectangle);
            DrawBackground(bg);
            bg.Render(e.Graphics);
            bg.Dispose();
        }

        private void DrawBackground(BufferedGraphics bg)
        {
            Graphics g = bg.Graphics;
            /*
            if (mImageBackground != null)
            {
                //g.DrawImageUnscaled(mImageBackground, 0, 0);
                Point pt = this.Parent.PointToScreen(new Point(this.Left, this.Top));
                g.DrawImage(mImageBackground, this.ClientRectangle, new Rectangle(pt.X, pt.Y, Bounds.Width, Bounds.Height), GraphicsUnit.Pixel);
            }
            */
            g.FillRectangle(Brushes.White, this.ClientRectangle);
            g.DrawRectangle(Pens.Green, this.ClientRectangle);
            g.DrawLine(Pens.Green, this.Width - 1, 0, this.Width - 1, this.Height - 1);
            g.DrawLine(Pens.Green, 0, this.Height - 1, this.Width - 1, this.Height - 1);
            //StringFormat format = new StringFormat();
            //format.LineAlignment = StringAlignment.Center;
            //format.Alignment = StringAlignment.Center;
            //g.DrawString("Intelligent/Realtime/Visualization/Multiple terminals/Script Supported", new Font("Arial", 24.0F, FontStyle.Bold), Brushes.Blue, 100, 100);
            Font infoFont = new Font("Arial", 24.0F, FontStyle.Bold);
            float y = 176;
            foreach (string info in mInfo)
            {
                SizeF sz = g.MeasureString(info, infoFont);
                var location = new PointF(Width / 2 - sz.Width / 2, y);
                //g.DrawString(info, infoFont, Brushes.Black, location);
                y += 32;
            }
            //Grid Items
            //int counts = GridItems.Count;
            int row = 0;
            int column = 0;
            foreach (GridItem item in GridItems)
            {
                int left = SidePadding + column * (ItemWidth + GridPadding);
                int top = SidePadding + row * (ItemHeight + GridPadding);
                Rectangle rect = new Rectangle(left, top, ItemWidth, ItemHeight);
                item.Rect = rect;
                item.PrintIocn = new Rectangle(item.Rect.Right  - 98,item.Rect.Top + 104,30,25);//20161223 bonnie
                item.Draw(g);
                column++;
                if (column >= this.Column)
                {
                    column = 0;
                    row++;
                }
                if (row >= this.Row)
                {
                    break;
                }
            }
            //////////////////////////////////////////////////
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            OnHandleMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                OnHandleItemClicked(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                OnHandleItemRightClicked(e.X, e.Y);
            }
        }

        private bool OnHandleItemRightClicked(int x, int y)
        {
            int index = 0;
            foreach (GridItem item in GridItems)
            {
                if (item.Active && item.Rect.Contains(x, y))
                {
                    if (OnGridItemRightClicked != null)
                    {
                        RightClickedEventArgs args = new RightClickedEventArgs(index, string.Empty);
                        args.X = x; args.Y = y;
                        OnGridItemRightClicked(this, args);
                    }
                }
                index++;
            }

            return false;
        }
        BarcodeLib.Barcode b = new BarcodeLib.Barcode();
        private bool OnHandleItemClicked(int x, int y)
        {
            int index = 0;
            foreach (GridItem item in GridItems)
            {
                if(item.Active && item.PrintIocn.Contains (x,y))
                {
                    item.Print = true;
                    
                }
                if (item.Active && item.Rect.Contains(x, y))
                {
                    
                    item.Checked = !item.Checked;
                    
                    
                    Invalidate(item.Rect);
                    if (OnGridItemChecked != null)
                    {
                        OnGridItemChecked(this, new CheckedEventArgs(index, item.Checked, string.Empty));
                    }
                }
                index++;
            }

            return false;
        }

        private void OnHandleMouseMove(MouseEventArgs e)
        {
            int hover = -1;
            int index = 0;
            foreach (GridItem item in GridItems)
            {
                if (item.Active && item.Rect.Contains(e.Location))
                {
                    item.Focus = true;
                    hover = index;
                    if (OnGridItemFocus != null)
                    {
                        FocusEventArgs args = new FocusEventArgs(hover, string.Empty);
                        args.X = e.X; args.Y = e.Y;
                        OnGridItemFocus(this, args);
                    }
                }
                else
                {
                    item.Focus = false;
                }
                index++;
            }
            if (hover != -1 && hover != mLastHover)
            {
                if (mLastHover != -1)
                {
                    Invalidate(GridItems[mLastHover].Rect);
                }
                mLastHover = hover;
                Invalidate(GridItems[mLastHover].Rect);
            }
            else
            {
                if (mLastHover != -1)
                {
                    Invalidate(GridItems[mLastHover].Rect);
                    mLastHover = hover;
                }

                if (OnGridItemFocus != null)
                {
                    FocusEventArgs args = new FocusEventArgs(hover, string.Empty);
                    args.X = e.X; args.Y = e.Y;
                    OnGridItemFocus(this, args);
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (mLastHover != -1)
            {
                GridItems[mLastHover].Focus = false;
                Invalidate(GridItems[mLastHover].Rect);
                mLastHover = -1;
            }
        }

        public GridItem this[int index]
        {
            get
            {
                if (index < 0 || index >= GridItems.Count)
                {
                    return null;
                }
                else
                {
                    return GridItems[index];
                }
            }

            set
            {
                if (index >= 0 || index < GridItems.Count)
                {
                    GridItems[index] = value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public GridItemEnum GetEnumerator()
        {
            return new GridItemEnum(_gridItems);
        }

        //ARGS
        public class FocusEventArgs : EventArgs
        {
            private string message;
            private int index;
            public int X;
            public int Y;
            public FocusEventArgs(int id, string message)
            {
                this.index = id;
                this.message = message;
            }

            public string Message
            {
                get { return message; }
            }

            public int Index
            {
                get { return index; }
            }
        }

        public class CheckedEventArgs : EventArgs
        {
            private string message;
            private int index;
            private bool _checked;
            public CheckedEventArgs(int id, bool _checked, string message)
            {
                this.index = id;
                this._checked = _checked;
                this.message = message;
            }

            public string Message
            {
                get { return message; }
            }

            public int Index
            {
                get { return index; }
            }

            public bool Checked
            {
                get { return _checked; }
            }
        }

        public class RightClickedEventArgs : EventArgs
        {
            private string message;
            private int index;
            private int _x;
            private int _y;
            public RightClickedEventArgs(int id, string message)
            {
                this.index = id;
                this.message = message;
            }

            public string Message
            {
                get { return message; }
            }

            public int Index
            {
                get { return index; }
            }

            public int X
            {
                get { return _x; }
                set { _x = value; }
            }

            public int Y
            {
                get { return _y; }
                set { _y = value; }
            }
        }

        //EVENT
        public delegate void GridItemFocusHandle(object sender, FocusEventArgs e);
        public event GridItemFocusHandle OnGridItemFocus;

        public delegate void GridItemCheckedHandle(object sender, CheckedEventArgs e);
        public event GridItemCheckedHandle OnGridItemChecked;
      //  public event PrintEventHandler p;
        public delegate void GridItemResetHandle(object sender, UserGridClassLibrary.GridItem.ResetEventArgs e);
        public event GridItemResetHandle OnGridItemReset;

        public delegate void GridItemRightClickHandle(object sender, RightClickedEventArgs e);
        public event GridItemRightClickHandle OnGridItemRightClicked;
    }

    public class GridItemEnum : IEnumerator
    {
        public List<GridItem> _item;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public GridItemEnum(List<GridItem> list)
        {
            _item = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < _item.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public GridItem Current
        {
            get
            {
                try
                {
                    return _item[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}

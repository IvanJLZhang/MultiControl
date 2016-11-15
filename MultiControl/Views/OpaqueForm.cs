using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace MultiControl
{
    public partial class OpaqueForm : Form
    {
        public class ChangedEventArgs : EventArgs
        {
            private string message;
            private int id;
            private float elapsed;
            public ChangedEventArgs(int id, string message)
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

            public float Elapsed
            {
                get { return elapsed; }
                set { elapsed = value; }
            }
        }
        //EVENT
        public delegate void ItemChangedHandle(object sender, ChangedEventArgs e);
        public event ItemChangedHandle OnItemChanged;

        private Font mFont = null;
        private Pen labelBorderPen = null;
        private SolidBrush labelBackColorBrush = null;
        private SolidBrush workingBrush = null;

        private int mCurrentKey = -1;
        private Dictionary<int, ArrayList> mDictResult = new Dictionary<int, ArrayList>();

        private Dictionary<int, int> mDictCycle = new Dictionary<int, int>();
        private Dictionary<int, string> mDictTag = new Dictionary<int, string>();
        private Dictionary<int, int> mDictHeight = new Dictionary<int, int>();
        private Dictionary<int, float> mDictTotalCost = new Dictionary<int, float>();
        private Dictionary<int, string> mDictModel = new Dictionary<int, string>();
        private Dictionary<int, ResultItem> mDictWorkingItem = new Dictionary<int, ResultItem>();
        private Dictionary<int, MyResult> mDictTestStatus = new Dictionary<int, MyResult>();

        public Dictionary<int, MyResult> DictionaryWorkingStatus
        {
            get { return mDictTestStatus; }
        }
        
        //private int mCycle = 0;
        //private string mTag = string.Empty;
        private BufferedGraphicsContext mContext = null;
        //private int mCurrentWorkingIndex = -1;
        private int mHeaderHeight = 30;
        private int mFooterHeight = 30;
        private int mLineHeight = 18;

        private bool mExitWorkerThread = false;

        public bool ExitWorkerThread
        {
            get { return mExitWorkerThread; }
            set { mExitWorkerThread = value; }
        }

        public int HeadHeight
        {
            get { return mHeaderHeight; }
            set { mHeaderHeight = value; }
        }

        public int FootHeight
        {
            get { return mFooterHeight; }
            set { mFooterHeight = value; }
        }

        public int LineHeight
        {
            get { return mLineHeight; }
            set { mLineHeight = value; }
        }

        public enum MyResult
        {
            WORKING,
            PASS,
            FAIL
        }

        public class ResultItem
        {
            private string _name;
            
            private MyResult _result;

            private int _index;

            private float _cost;

            private string _additional;

            public ResultItem(string name, MyResult result, int index, float cost)
            {
                Name = name;
                Result = result;
                Index = index;
                Cost = cost;
            }

            public float Cost
            {
                get { return _cost; }
                set { _cost = value; }
            }

            public int Index
            {
                get { return _index; }
                set { _index = value; }
            }

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public MyResult Result
            {
                get { return _result; }
                set { _result = value; }
            }

            public String Additional
            {
                get { return _additional; }
                set { _additional = value; }
            }
        }

        private EventWaitHandle[] _EventWaitHandle = null;

        public EventWaitHandle[] EventWaitHandle
        {
            get { return _EventWaitHandle; }
        }

        public OpaqueForm()
        {
            InitializeComponent();

            SetStyle(System.Windows.Forms.ControlStyles.Opaque, true);
            mFont = new Font("Arial", 10.0f, FontStyle.Bold);
            labelBorderPen = new Pen(Color.DarkGreen, 1);
            labelBackColorBrush = new SolidBrush(Color.LightGreen);
            workingBrush = new SolidBrush(Color.FromArgb(192, 255, 255));
            //AddResult(1, "AA", MyResult.WORKING);
            mContext = BufferedGraphicsManager.Current;
            mContext.MaximumBuffer = new Size(Width + 1, Height + 1);

            CreateWorkerThread();
        }

        private void CreateWorkerThread()
        {
            _EventWaitHandle = new EventWaitHandle[2];
            for (int i = 0; i < 2; ++i)
            {
                _EventWaitHandle[i] = new EventWaitHandle(false, EventResetMode.ManualReset);
            }

            Thread thread = new Thread(new ThreadStart(BackgroundWorkerThread));
            thread.IsBackground = true;
            thread.Start();
        }
        
        public void UpdateWorkingItem(object sender, EventArgs e)
        {
            if (CurrentKey != -1 && Visible)
            {
                if (mDictWorkingItem[CurrentKey] != null)
                {
                    Invalidate(new Rectangle(10, 30 + mDictWorkingItem[CurrentKey].Index * 18, Width, 18));
                }
            }
        }

        private void BackgroundWorkerThread()
        {
            //while (!mExitWorkerThread)
            while(true)
            {
                int index = WaitHandle.WaitAny(_EventWaitHandle);
                if (index == 0)
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new EventHandler(UpdateWorkingItem), null);
                    }
                    else
                    {
                        UpdateWorkingItem(this, null);
                    }
                }
                else
                {
                    break;
                }
                Thread.Sleep(500);
            }
        }

        public int ClearResultList(int key)
        {
            ArrayList result = FindResultByKey(key);
            if (result != null)
            {
                result.Clear();
                mDictCycle[key] = 0;
                mDictTag[key] = string.Empty;
                mDictTotalCost[key] = 0.0f;
                mDictHeight[key] = HeadHeight + LineHeight + FootHeight;
                //this.Height = mDictHeight[key];
                if (key == CurrentKey)
                {
                    this.Height = mDictHeight[key];
                    Invalidate();
                }
                return 1;
            }
            return -1;
        }

        private ArrayList FindResultByKey(int key)
        {
            if (key >=0 )
            {
                foreach (var result in mDictResult)
                {
                    if (result.Key == key)
                    {
                        return result.Value;
                    }
                }
            }

            return null;
        }

        private ResultItem FindItemByName(ArrayList list, string name)
        {
            foreach (ResultItem item in list)
            {
                if (item.Name.Equals(name))
                {
                    return item;
                }
            }
            return null;
        }

        public int CurrentKey
        {
            get { return mCurrentKey; }
            set { 
                    mCurrentKey = value; 
                    UpdateCurrentMapKey(mCurrentKey); 
                }
        }

        private void UpdateCurrentMapKey(int key)
        {
            mCurrentKey = key;
            ArrayList resultItem = FindResultByKey(mCurrentKey);
            if (resultItem != null)
            {
                if (resultItem.Count > 0)
                {
                    Visible = true;
                    Invalidate();
                    if (DictionaryWorkingStatus[mCurrentKey] == OpaqueForm.MyResult.WORKING)
                    {
                        EventWaitHandle[0].Set();
                    }
                }
                else
                {
                    Visible = false;
                }
                this.Height = mDictHeight[mCurrentKey];
            }
            else
            {
                Visible = false;
            }
        }
        /*
        public int UpdateResult(int key, string name, MyResult result)
        {
            ArrayList resultList = FindResultByKey(key);
            if (resultList != null)
            {
                ResultItem item = FindItemByName(resultList, name);
                if (item != null)
                {
                    if (item.Result != result)
                    {
                        item.Result = result;
                        Invalidate();
                    }
                }
            }
            
            return 0;
        }
        */

        public int AddResult(int key, string model, string nameOrg, MyResult result, float cost)
        {
            string name = nameOrg;
            string additional = string.Empty;
            //Filter SUB ITEMS
            string[] subItemsResult = nameOrg.Split(':');
            if (subItemsResult.Count() > 1)
            {
                int count = subItemsResult.Count();
                name = subItemsResult[0].ToString();
                for (int i = 1; i < count - 1; i++)
                {
                    additional += subItemsResult[i].ToString();
                    additional += "|";
                }
                int length = additional.Length;
                additional = additional.Substring(0, length - 1);
            }

            ArrayList resultList = FindResultByKey(key);
            if (resultList == null)
            {
                mDictResult.Add(key, new ArrayList());
                resultList = mDictResult[key];

                mDictCycle.Add(key, 0);
                mDictTag.Add(key, string.Empty);
                mDictTotalCost.Add(key, 0.0f);
                mDictModel.Add(key, model);
            }
            //
            ResultItem item = FindItemByName(resultList, name);
            if (item != null)
            {
                item.Additional = additional;
                if (result == MyResult.WORKING)
                {
                    item.Result = result;
                    //Invalidate(new Rectangle(10, 30 + item.Index * 18, Width, 18));
                    //mCurrentWorkingIndex = item.Index;
                    mDictWorkingItem[key] = item;
                    if (key == CurrentKey)
                    {
                        Invalidate();
                    }
                    EventWaitHandle[0].Set();
                }
                else
                {
                    if (item.Result != result)
                    {
                        item.Result = result;
                        item.Cost = cost;
                        if (key == CurrentKey)
                        {
                            Invalidate();
                        }
                        //Invalidate(new Rectangle(0, 30 + item.Index * 18, Width, 18));
                    }
                }
            }
            else
            {
                int index = resultList.Count;
                item = new ResultItem(name, result, index, cost);
                item.Additional = additional;
                resultList.Add(item);
                if (result == MyResult.WORKING)
                {
                    //mCurrentWorkingIndex = index;
                    //mCycle = 0;
                    //mTag = string.Empty;
                    mDictCycle[key] = 0;
                    mDictTag[key] = string.Empty;
                    mDictWorkingItem[key] = item;
                    EventWaitHandle[0].Set();
                }
                else
                {
                    ;
                }
                mDictHeight[key] = HeadHeight + resultList.Count * LineHeight + FootHeight;
                if (key == CurrentKey)
                {
                    this.Height = mDictHeight[key];
                    Invalidate();
                }
            }

            if (OnItemChanged != null && key != -1)
            {
                ChangedEventArgs args = new ChangedEventArgs(key, string.Empty);
                args.Elapsed = GetTotalElapsedTestTime(key);// mDictTotalCost[key] + item.Cost;
                OnItemChanged(this, args);
            }

            return resultList.Count;
        }

        private float GetTotalElapsedTestTime(int key)
        {
            float result = 0.0f;
            ArrayList resultItem = FindResultByKey(key);
            if (resultItem != null)
            {
                foreach (ResultItem item in resultItem)
                {
                    if (item.Result != MyResult.WORKING)
                    {
                        result += item.Cost;
                    }
                }
            }
            return result;
        }

        public int GetTotalPassItemCount(int key)
        {
            int result = 0;
            ArrayList resultItem = FindResultByKey(key);
            if (resultItem != null)
            {
                foreach (ResultItem item in resultItem)
                {
                    if (item.Result == MyResult.PASS)
                    {
                        result ++;
                    }
                }
            }
            return result;
        }

        private void DrawBackground(BufferedGraphics bg)
        {
            Graphics g = bg.Graphics;
            //g.Clear(System.Drawing.Color.White);
            g.DrawRectangle(labelBorderPen, 0, 0, this.Width - 1, this.Height - 1);
            g.FillRectangle(labelBackColorBrush, 1, 1, this.Width - 2, this.Height - 2);

            if (mCurrentKey != -1)
            {
                SizeF sz = g.MeasureString(mDictModel[mCurrentKey], mFont);
                var location = new PointF(Width / 2 - sz.Width / 2, 11 - sz.Height / 2);
                g.DrawString(mDictModel[mCurrentKey], mFont, Brushes.Black, location);
            }

            g.DrawLine(labelBorderPen, 0, 22, Width, 22);
            ////////////////////////////////////////////////////////////////////////////////
            ArrayList resultItem = FindResultByKey(mCurrentKey);
            if (resultItem != null)
            {
                int y = HeadHeight;
                StringFormat costFormat = new StringFormat();
                costFormat.Alignment = StringAlignment.Far;
                costFormat.LineAlignment = StringAlignment.Near;
                //float _totalCost = 0.0f;
                mDictTotalCost[mCurrentKey] = 0.0f;
                foreach (ResultItem item in resultItem)
                {
                    switch (item.Result)
                    {
                        case MyResult.WORKING:
                            //DRAW IT
                            //mCycle++;
                            mDictCycle[mCurrentKey]++;
                            g.DrawString(item.Name + mDictTag[mCurrentKey], mFont, Brushes.DarkOrange, 10, y);
                            mDictTag[mCurrentKey] += ".";
                            //SizeF offSize = e.Graphics.MeasureString(item.Name, mFont);
                            //SizeF offSizeAfter = e.Graphics.MeasureString(item.Name + mTag, mFont);
                            //mOffset = offSize.Width;
                            if (mDictCycle[mCurrentKey] > 6)
                            {
                                mDictCycle[mCurrentKey] = 0;
                                mDictTag[mCurrentKey] = string.Empty;
                            }
                            //Console.WriteLine("{0} - {1}", item.Name, mDictCycle[mCurrentKey].ToString());
                            break;
                        case MyResult.PASS:
                            g.DrawString(item.Name, mFont, Brushes.Green, 10, y);
                            g.DrawString(item.Cost.ToString("0.00"), mFont, Brushes.Black, new Rectangle(0, y, Width, 18), costFormat);
                            mDictTotalCost[mCurrentKey] += item.Cost;
                            break;
                        case MyResult.FAIL:
                            g.DrawString(item.Name + ":" + item.Additional, mFont, Brushes.Red, 10, y);
                            g.DrawString(item.Cost.ToString("0.00"), mFont, Brushes.Black, new Rectangle(0, y, Width, 18), costFormat);
                            mDictTotalCost[mCurrentKey] += item.Cost;
                            break;
                    }
                    y += 18;
                    //g.DrawLine(labelBorderPen, 10, y -2, Width - 20, y -2);
                }
                //Draw footer
                g.DrawLine(labelBorderPen, 0, y + 8, Width, y + 8);
                Rectangle rectFooter = new Rectangle(0, y + 8, Width, 22);
                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Far;
                g.DrawString(mDictTotalCost[mCurrentKey].ToString("0.00"), mFont, Brushes.Black, rectFooter, format);
            }
            /////////////////////////////////////////////////////////////////////////////////
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            BufferedGraphics bg = mContext.Allocate(this.CreateGraphics(), this.ClientRectangle);
            DrawBackground(bg);
            bg.Render(e.Graphics);
            bg.Dispose();
        }

        protected override CreateParams CreateParams//v1.10 
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //0x20;  // 开启 WS_EX_TRANSPARENT,使控件支持透明
                return cp;
            }
        }
    }
}

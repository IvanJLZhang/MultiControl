using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CustomListViewEx
{
    public partial class AlphabetListView : UserControl
    {
        public class ListItem
        {
            public ListItem()
            {
            }

            private int _index;
            private bool _checked;
            private Rectangle _bound;
            private Rectangle _checkedBound;
            private string _name;
            private string _description;
            private string _pinyin;
            private bool _mouseHover;
            private bool _focus;

            public int Index
            {
                get { return _index; }
                set { _index = value; }
            }

            public string LabelText
            {
                get { return _name; }
                set { _name = value; }
            }

            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }

            public bool Checked
            {
                get { return _checked; }
                set { _checked = value; }
            }

            public bool MouseHover
            {
                get { return _mouseHover; }
                set { _mouseHover = value; }
            }

            public Rectangle Bound
            {
                get { return _bound; }
                set { _bound = value; }
            }

            public Rectangle CheckedBound
            {
                get { return _checkedBound; }
                set { _checkedBound = value; }
            }

            public bool Focus
            {
                get { return _focus; }
                set { _focus = value; }
            }
        }

        public class AlphabetCharItem
        {
            public string Value;
            public Rectangle Bounds;
            public bool Focus;

            public AlphabetCharItem(string value)
            {
                Value = value;
            }
        }

        public enum CheckedStatus
        {
            CS_NONE = 0,
            CS_PART = 1,
            CS_ALL = 2,
        }

        private class AlphaBarEventArgs : EventArgs
        {
            public int Time;
            public string Alpha;

            public AlphaBarEventArgs(int time, string alpha)
            {
                Time = time;
                Alpha = alpha;
            }
        }

        //FONT
        private Font mFont = null;
        private Font mFontBold = null;
        private Font mFontChar = null;
        private Font mFontCharBold = null;
        //PROP
        private Color mBackgroundClr;
        //HEAD TEXT
        private string mHeaderCaption = "ITEMS";
        //SIZE
        private int mHeaderHeight = 32;

        //DOUBLE BUFFER
        private BufferedGraphicsContext mContext;
        private int mItemHeight;
        private int mAlphaWidth;
        private int mOffsetY;
        private bool mDragging = false;
        private int mDraggingY;

        private ListItem mCurrentItem = null;
        private ListItem mLastItem = null;

        private List<ListItem> _listItems = new List<ListItem>();

        public List<ListItem> mCtrlSelect = new List<ListItem>();
        public bool mCtrlItem = false;

        private List<AlphabetCharItem> AlphaCharItems = new List<AlphabetCharItem>();

        private Rectangle mHeaderRect;
        private Rectangle mListRect;
        private Rectangle mAlphaBar;

        private int mLastHover = -1;
        private int mLastHoverChar = -1;

        private int mSelectedIndex = -1;

        public List<ListItem> Items
        {
            get { return _listItems; }
        }

        public AlphabetListView()
        {
            InitializeComponent();

            mFont = new Font("Arial", 12.25f);
            mFontBold = new Font("Arial", 12.25f, FontStyle.Bold);
            mFontChar = new Font("Arial", 8.0f);
            mFontCharBold = new Font("Arial", 11.0f, FontStyle.Bold);
            mContext = BufferedGraphicsManager.Current;
            mContext.MaximumBuffer = new Size(Width + 1, Height + 1);
            mHeaderRect = new Rectangle(0, 0, Width, mHeaderHeight);
            mAlphaWidth = 22; mItemHeight = 32;
            mListRect = new Rectangle(0, mHeaderHeight, Width - mAlphaWidth, Height - mHeaderHeight);
            mAlphaBar = new Rectangle(mListRect.Right, mHeaderHeight + 1, mAlphaWidth, mListRect.Height);
            mBackgroundClr = Color.Gray;
            mOffsetY = 0;

            string alpha = "#ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            foreach (char c in alpha)
            {
                AlphaCharItems.Add(new AlphabetCharItem(c.ToString()));
            }

            AlphaBarChanged += new AlphaBarChangedEventHandler(AlphaBarChangedHandler);
        }

        public delegate void ItemClickedEventHandler(object sender, ListItem e);
        public event ItemClickedEventHandler ItemClicked;

        public delegate void ItemCheckedEventHandler(object sender, ItemCheckEventArgs e);
        public event ItemCheckedEventHandler ItemChecked;

        private delegate void AlphaBarChangedEventHandler(AlphaBarEventArgs e);
        private event AlphaBarChangedEventHandler AlphaBarChanged;
        private int AlphaClickedTimes = 1;

        public int GetItemCount()
        {
            return Items.Count;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            BufferedGraphics bg = mContext.Allocate(this.CreateGraphics(), this.ClientRectangle);
            DrawBackground(bg);
            DrawHeader(bg);
            DrawItemList(bg);
            DrawAlphaBar(bg);
            bg.Render(e.Graphics);
            bg.Dispose();
        }

        private void DrawAlphaBar(BufferedGraphics bg)
        {
            Graphics g = bg.Graphics;
            if (mLastHoverChar != -1)
            {
                g.FillRectangle(Brushes.DarkOrange, mAlphaBar);
            }
            else
            {
                g.FillRectangle(Brushes.White, mAlphaBar);
            }
            
            StringFormat stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.FormatFlags = StringFormatFlags.LineLimit;
            stringFormat.Trimming = StringTrimming.EllipsisCharacter;

            int height = mAlphaBar.Height / 27;
            int offset = mAlphaBar.Height % 27;
            Rectangle rectLabel = new Rectangle(mAlphaBar.Left, mAlphaBar.Top, mAlphaWidth, height + offset);
            foreach (AlphabetCharItem item in AlphaCharItems)
            {
                item.Bounds = rectLabel;
                if (item.Focus)
                {
                    g.DrawString(item.Value, mFontCharBold, Brushes.White, item.Bounds, stringFormat);
                }
                else
                {
                    g.DrawString(item.Value, mFontChar, Brushes.Black, item.Bounds, stringFormat);
                }
                rectLabel.Y += item.Bounds.Height;
                rectLabel.Height = height;
            }
        }

        private void DrawBackground(BufferedGraphics bg)
        {
            Graphics g = bg.Graphics;
            g.Clear(mBackgroundClr);
        }

        private void DrawItemList(BufferedGraphics bg)
        {
            Graphics g = bg.Graphics;
            g.FillRectangle(Brushes.White, mListRect);

            int counts = Items.Count;
            int top = mHeaderHeight;
            foreach (ListItem item in Items)
            {
                Rectangle rect = new Rectangle(0, top + mOffsetY, Width - mAlphaWidth, mItemHeight);
                item.Bound = rect;
                item.CheckedBound = new Rectangle(rect.Left + 5, rect.Top + 5, mItemHeight - 10, rect.Height - 10);
                DrawItem(g, item);
                top += mHeaderHeight;
            }
            Rectangle rectBoder = mListRect;
            rectBoder.Width -= 1;
            rectBoder.Height -= 1;
            g.DrawRectangle(Pens.DimGray, rectBoder);
        }

        private void DrawItem(Graphics g, ListItem item, bool ctrlPress, bool keypress)
        {
            if (ctrlPress)
            {
                if (keypress)
                {
                    g.FillRectangle(Brushes.SlateBlue, item.Bound);
                }
                else
                {
                    g.FillRectangle(Brushes.White, item.Bound);
                }
            }
            else
            {
                if (item.Focus)
                {
                    g.FillRectangle(Brushes.SlateBlue, item.Bound);
                }
                else
                {
                    g.FillRectangle(Brushes.White, item.Bound);
                }
            }

            if (item.MouseHover)
            {
                g.DrawRectangle(Pens.DimGray, item.Bound);
                Rectangle hoverEdge = new Rectangle(item.Bound.Left + 2, item.Bound.Top + 2, item.Bound.Width - 5, item.Bound.Height - 4);
                g.DrawRectangle(Pens.Orange, hoverEdge);
            }
            else
            {
                g.DrawRectangle(Pens.DimGray, item.Bound);
            }

            string caption = item.LabelText;

            StringFormat stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.FormatFlags = StringFormatFlags.LineLimit;
            stringFormat.Trimming = StringTrimming.EllipsisCharacter;
            Rectangle rectLabel = new Rectangle(item.Bound.Left + mItemHeight, item.Bound.Top, item.Bound.Width - mItemHeight, item.Bound.Height);
            if (item.Checked)
            {
                if (item.Focus)
                {
                    g.DrawString(caption, mFontBold, Brushes.White, rectLabel, stringFormat);
                }
                else
                {
                    g.DrawString(caption, mFontBold, Brushes.Black, rectLabel, stringFormat);
                }
            }
            else
            {
                if (item.Focus)
                {
                    g.DrawString(caption, mFont, Brushes.White, rectLabel, stringFormat);
                }
                else
                {
                    g.DrawString(caption, mFont, Brushes.Black, rectLabel, stringFormat);
                }
            }

            //DRAW CHECKBOX
            g.DrawRectangle(Pens.DarkOrange, item.CheckedBound);
            if (item.Checked)
            {
                int x = item.CheckedBound.Left + 3;
                int y = item.CheckedBound.Top + 7;
                Pen pen = new Pen(new SolidBrush(Color.DarkOrange), 1.0f);
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
        }

        private void DrawItem(Graphics g, ListItem item)
        {
            if (mCtrlSelect.Count != 0 && mCtrlItem)
            {
                foreach (ListItem i in mCtrlSelect)
                {
                    if (item == i)
                    {
                        g.FillRectangle(Brushes.SlateBlue, item.Bound);
                    }
                }
            }
            else
            {
                if (item.Focus)
                {
                    g.FillRectangle(Brushes.SlateBlue, item.Bound);
                }
                else
                {
                    g.FillRectangle(Brushes.White, item.Bound);
                }
            }
            
            if (item.MouseHover)
            {
                g.DrawRectangle(Pens.DimGray, item.Bound);
                Rectangle hoverEdge = new Rectangle(item.Bound.Left + 2, item.Bound.Top + 2, item.Bound.Width - 5, item.Bound.Height - 4);
                g.DrawRectangle(Pens.Orange, hoverEdge);
            }
            else
            {
                g.DrawRectangle(Pens.DimGray, item.Bound);
            }

            string caption = item.LabelText;
            StringFormat stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.FormatFlags = StringFormatFlags.LineLimit;
            stringFormat.Trimming = StringTrimming.EllipsisCharacter;
            Rectangle rectLabel = new Rectangle(item.Bound.Left + mItemHeight, item.Bound.Top, item.Bound.Width - mItemHeight, item.Bound.Height);
            
            if (item.Checked)
            {
                if (item.Focus)
                {
                    g.DrawString(caption, mFontBold, Brushes.White, rectLabel, stringFormat);
                }
                else
                {
                    g.DrawString(caption, mFontBold, Brushes.Black, rectLabel, stringFormat);
                }
            }
            else
            {
                if (item.Focus)
                {
                    g.DrawString(caption, mFont, Brushes.White, rectLabel, stringFormat);
                }
                else
                {
                    g.DrawString(caption, mFont, Brushes.Black, rectLabel, stringFormat);
                }
            }

            //DRAW CHECKBOX
            g.DrawRectangle(Pens.DarkOrange, item.CheckedBound);
            if (item.Checked)
            {
                int x = item.CheckedBound.Left + 3;
                int y = item.CheckedBound.Top + 7;
                Pen pen = new Pen(new SolidBrush(Color.DarkOrange), 1.0f);
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
            //
        }

        private void DrawHeader(BufferedGraphics bg)
        {
            Graphics g = bg.Graphics;

            g.FillRectangle(Brushes.White, mHeaderRect);

            StringFormat stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.FormatFlags = StringFormatFlags.LineLimit;
            stringFormat.Trimming = StringTrimming.EllipsisCharacter;
            g.DrawString(mHeaderCaption, mFontBold, Brushes.Black, mHeaderRect, stringFormat);

            Rectangle rectBorder = mHeaderRect;
            rectBorder.Width -= 1;
            g.DrawRectangle(Pens.DimGray, rectBorder);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                if (mListRect.Contains(e.X, e.Y))
                {
                    foreach (ListItem item in Items)
                    {
                        if (item.CheckedBound.Contains(e.Location))
                        {
                            CheckState currentValue = item.Checked ? CheckState.Checked : CheckState.Unchecked;
                            item.Checked = !item.Checked;
                            CheckState newValue = item.Checked ? CheckState.Checked : CheckState.Unchecked;
                            if (ItemChecked != null)
                            {
                                ItemChecked(this, new ItemCheckEventArgs(item.Index, newValue, currentValue));
                            }
                            Invalidate(item.Bound);
                            return;
                        }
                    }
                    bool trigger = OnHandleListClicked(e.X, e.Y);
                    if (ItemClicked != null && trigger)
                    {
                        ItemClicked(this, mCurrentItem);
                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)//当ctrl按下时产生
                        {
                            mCtrlItem = true;
                            if (!mCtrlSelect.Contains(mCurrentItem))
                                mCtrlSelect.Add(mCurrentItem);
                            else
                                mCtrlSelect.Remove(mCurrentItem);
                        }
                        else
                        {
                            mCtrlSelect.Clear();
                            mCtrlItem = false;
                        }
                    }
                }
                else if(mAlphaBar.Contains(e.X,e.Y))
                {
                    if (AlphaBarChanged != null && mLastHoverChar != -1)
                    {
                        AlphaClickedTimes++;
                        AlphaBarChanged(new AlphaBarEventArgs(AlphaClickedTimes, AlphaCharItems[mLastHoverChar].Value));
                    }
                }
            }
        }

        private ListItem GetFocusedItem()
        {
            foreach (ListItem item in Items)
            {
                if (item.Focus)
                {
                    return item;
                }
            }
            return null;
        }

        private bool OnHandleListClicked(int x, int y)
        {
            if (mLastItem != mCurrentItem)
            {
                mLastItem = mCurrentItem;
            }
            mCurrentItem = null;
            foreach (ListItem item in Items)
            {
                if (item.Bound.Contains(x, y))
                {
                    ListItem focusItem = GetFocusedItem();
                    if (item != focusItem)
                    {
                        item.Focus = true;
                        Rectangle update = item.Bound;
                        if (focusItem != null)
                        {
                            focusItem.Focus = false;
                            update = Rectangle.Union(update, focusItem.Bound);
                        }
                        Invalidate(mListRect);
                    }
                    mCurrentItem = item;
                }
                else
                {
                    item.Focus = false;
                }
            }

            if (mCurrentItem != mLastItem)
            {
                Invalidate(mListRect);
                return true;
            }

            return false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                if (mListRect.Contains(e.X, e.Y))
                {
                    mDragging = true;
                    mDraggingY = e.Y;
                }
            }
        }

        private void OnHandleListDrag(int y)
        {
            mOffsetY += y - mDraggingY;
            mDraggingY = y;

            if (mOffsetY >= 0)
            {
                mOffsetY = 0;
            }

            Invalidate(mListRect);
        }

        private void OnHandleMouseEnter(MouseEventArgs e)
        {
            int hover = -1;
            int index = 0;
            if (mAlphaBar.Contains(e.Location))
            {
                ResetListItem();
                foreach (AlphabetCharItem item in AlphaCharItems)
                {
                    if (item.Bounds.Contains(e.Location))
                    {
                        item.Focus = true;
                        hover = index;
                    }
                    else
                    {
                        item.Focus = false;
                    }
                    index++;
                }
                if (hover != mLastHoverChar)
                {
                    mLastHoverChar = hover;
                    Invalidate(mAlphaBar);
                    AlphaClickedTimes = 1;
                    if (AlphaBarChanged != null && mLastHoverChar != -1)
                    {
                        AlphaBarChanged(new AlphaBarEventArgs(AlphaClickedTimes, AlphaCharItems[mLastHoverChar].Value));
                    }
                }
            }
            else if (mListRect.Contains(e.Location))
            {
                ResetAlphaBar();
                foreach (ListItem item in Items)
                {
                    if (item.Bound.Contains(e.Location))
                    {
                        item.MouseHover = true;
                        hover = index;
                    }
                    else
                    {
                        item.MouseHover = false;
                    }
                    index++;
                }
                if (hover != mLastHover)
                {
                    mLastHover = hover;
                    Invalidate(mListRect);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (mDragging)
            {
                OnHandleListDrag(e.Y);
            }
            else
            {
                OnHandleMouseEnter(e);
            }
        }

        private int GetItemsAlphaCount(string alpha)
        {
            int result = 0;
            foreach (ListItem item in Items)
            {
                string name = item.LabelText;
                if (name[0].ToString().ToUpper().Equals(alpha))
                {
                    result++;
                }
            }
            return result;
        }

        private void AlphaBarChangedHandler(AlphaBarEventArgs e)
        {
            int result = GetItemsAlphaCount(e.Alpha);
            if (result > 0)
            {
                if (e.Time >= result)
                {
                    AlphaClickedTimes = 0;
                }
                int AlphaIndex = -1;
                int count = 0;
                foreach (ListItem item in Items)
                {
                    string name = item.LabelText;
                    if (name[0].ToString().ToUpper().Equals(e.Alpha))
                    {
                        count++;
                        if (count == e.Time)
                        {
                            AlphaIndex = item.Index;
                            break;
                        }
                    }
                }
                if (AlphaIndex != -1)
                {
                    mOffsetY = -AlphaIndex * mItemHeight;
                    Invalidate(mListRect);
                }
            }
        }

        private void ResetListItem()
        {
            foreach (ListItem item in Items)
            {
                item.MouseHover = false;
                mLastHover = -1;
            }
            Invalidate(mListRect);
        }

        private void ResetAlphaBar()
        {
            foreach (AlphabetCharItem alpha in AlphaCharItems)
            {
                alpha.Focus = false;
                mLastHoverChar = -1;
            }
            Invalidate(mAlphaBar);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            ResetListItem();
            ResetAlphaBar();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (mDragging)
            {
                mDragging = false;
            }
        }

        public void Delete(ListItem item)
        {
            if (item != null)
            {
                Items.Remove(item);
                Invalidate();
            }
        }

        public void SetHeaderCaption(string caption)
        {
            mHeaderCaption = caption;
            Invalidate(mHeaderRect);
        }

        public void SetHeaderHeight(int height)
        {
            mHeaderHeight = height;
        }

        public ListItem GetSelectedItem()
        {
            return mCurrentItem;
        }

        public void Add(string name, bool isChecked)
        {
            ListItem item = new ListItem() { LabelText = name, Checked = isChecked };
            Add(item);
            Invalidate(mListRect);
        }

        public void RenameItem(string oldName, string newName)
        {
            ListItem updateItem = null;
            foreach (ListItem item in Items)
            {
                if (item.LabelText.Equals(oldName))
                {
                    updateItem = item;
                    break;
                }
            }

            if (updateItem != null)
            {
                updateItem.LabelText = newName;
                Invalidate(mListRect);
            }
        }

        public int ClearItems()
        {
            Items.Clear();
            Invalidate();
            return 0;
        }

        public void Add(ListItem value)
        {
            if (value != null)
            {
                int lastIdx = Items.Count;
                value.Index = lastIdx;
                Items.Add(value);
            }
        }

        public ListItem GetItem(int index)
        {
            ListItem item = null;

            if (index >= 0 && index < Items.Count)
            {
                item = Items[index];
            }

            return item;
        }

        public bool ItemExist(string name)
        {
            foreach (ListItem item in Items)
            {
                if (item.LabelText == name)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (mContext != null)
            {
                mContext.MaximumBuffer = new Size(Width + 1, Height + 1);
                mHeaderRect = new Rectangle(0, 0, Width, mHeaderHeight);
                mListRect = new Rectangle(0, mHeaderHeight, Width - mAlphaWidth, Height - mHeaderHeight);
                mAlphaBar = new Rectangle(mListRect.Right, mHeaderHeight + 1, mAlphaWidth, mListRect.Height);
            }
        }
        //ALL
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {//处理方向键
                case Keys.Up:
                    OnHandleKeyUp();
                    return true;
                case Keys.Down:
                    OnHandleKeyDown();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnHandleKeyDown()
        {
            if (Items.Count == 0)
                return;
            
            ListItem item = GetSelectedItem();
            if (item != null)
            {
                mSelectedIndex = item.Index;
                item.Focus = false;
            }
            mSelectedIndex++;
            item = GetItem(mSelectedIndex);
            if (item == null)
            {
                mSelectedIndex = Items.Count - 1;
                item = GetItem(mSelectedIndex);
            }
            item.Focus = true;
            mCurrentItem = item;
            ForceItemVisible(mCurrentItem,1);
            //EVENT
            if (mLastItem != mCurrentItem)
            {
                mLastItem = mCurrentItem;
                ItemSelectedChangeEvent();
            }
            //CHECK OFFSET
            Invalidate(mListRect);
        }

        private bool ForceItemVisible(ListItem item, int dir)
        {
            if (item != null)
            {
                Rectangle willShowBounds = item.Bound;
                if (!mListRect.Contains(willShowBounds))
                {
                    switch(dir)
                    {
                        case 0:
                            //while (!mListRect.Contains(willShowBounds))
                            {
                                //willShowBounds.Y -= mItemHeight;
                                mOffsetY += mItemHeight;
                            }
                            if (mOffsetY > 0)
                            {
                                mOffsetY = 0;
                            }
                            break;
                        case 1:
                            //while (!mListRect.Contains(willShowBounds))
                            {
                                //willShowBounds.Y += mItemHeight;
                                mOffsetY -= mItemHeight;
                                if (mOffsetY < -(Items.Count-1) * mItemHeight)
                                {
                                    mOffsetY = -(Items.Count-1) * mItemHeight;
                                }
                            }
                            break;
                    }
                }
            }

            return true;
        }

        private void OnHandleKeyUp()
        {
            if (Items.Count == 0)
                return;

            ListItem item = GetSelectedItem();
            if (item != null)
            {
                mSelectedIndex = item.Index;
                item.Focus = false;
            }
            mSelectedIndex--;
            item = GetItem(mSelectedIndex);
            if (item == null)
            {
                mSelectedIndex = 0;
                item = GetItem(mSelectedIndex);
            }
            item.Focus = true;
            mCurrentItem = item;
            ForceItemVisible(mCurrentItem, 0);
            //EVENT
            if (mLastItem != mCurrentItem)
            {
                mLastItem = mCurrentItem;
                ItemSelectedChangeEvent();
            }
            //CHECK OFFSET
            Invalidate(mListRect);
        }

        private void ItemSelectedChangeEvent()
        {
            if (ItemClicked != null && mCurrentItem != null)
            {
                ItemClicked(this, mCurrentItem);
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)//当ctrl按下时产生
                {
                    mCtrlItem = true;
                    if (!mCtrlSelect.Contains(mCurrentItem))
                        mCtrlSelect.Add(mCurrentItem);
                    else
                        mCtrlSelect.Remove(mCurrentItem);
                }
                else
                {
                    mCtrlSelect.Clear();
                    mCtrlItem = false;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.W)
            {
                OnHandleKeyUp();
            }
            else if (e.KeyCode == Keys.S)
            {
                OnHandleKeyDown();
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar == 13)
            {
                ListItem item = GetSelectedItem();
                if (item != null)
                {
                    CheckState currentValue = item.Checked ? CheckState.Checked : CheckState.Unchecked;
                    item.Checked = !item.Checked;
                    CheckState newValue = item.Checked ? CheckState.Checked : CheckState.Unchecked;
                    if (ItemChecked != null)
                    {
                        ItemChecked(this, new ItemCheckEventArgs(item.Index, newValue, currentValue));
                    }
                    Invalidate(item.Bound);
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
    }
}

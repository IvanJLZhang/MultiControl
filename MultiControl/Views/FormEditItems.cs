using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using CustomListViewEx;

namespace MultiControl
{
    public partial class FormEditItems : Form
    {
        public class TestItem
        {
            public string Name;
            public Dictionary<string, string> Options = new Dictionary<string, string>();
            public bool Checked;

            public TestItem(string name, bool check)
            {
                Name = name;
                Checked = check;
            }
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

        public AlphabetListView mAlphabetList = null;

        private string _caption = string.Empty;
        private string _configFolder = string.Empty;
        private ArrayList _items = new ArrayList();

        private ArrayList mCommentList = new ArrayList();

        private Font mFont;
        private Font mFontBold;
        private float mFontSize = 14.25F;

        private int _selectedItemIndex = -1;

        public int SelectedItemIndex
        {
            get { return _selectedItemIndex; }
            set { _selectedItemIndex = value; }
        }

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

        public ArrayList TestItems
        {
            get { return _items; }
        }

        public FormEditItems()
        {
            InitializeComponent();
        }

        private void FormEditItems_Load(object sender, EventArgs e)
        {
            this.Text = Title + " Test Items Editor";
            this.label1.Text = "Test Items Editor\r\n    * Click the checkbox to select/unselect test item\r\n    * Click test item to show out test parameters on the right\r\n";
            LoadTestItemsFromFile(ConfigFolder + "\\pqaa.cfg");
            //ALPHABET LIST CTRL
            mAlphabetList = new AlphabetListView();
            mAlphabetList.SetHeaderCaption(Title + " Item Selection");
            mAlphabetList.Location = this.checkedListBox1.Location;
            mAlphabetList.Size = this.checkedListBox1.Size;
            mAlphabetList.ItemClicked += AlphabetListClicked;
            mAlphabetList.ItemChecked += AlphabetListItemChecked;
            this.Controls.Add(mAlphabetList);

            foreach(TestItem item in TestItems)
            {
                //this.checkedListBox1.Items.Add(item.Name, item.Checked);
                //mAlphabetList.Add(i, i.ToString(), item.Name);
                mAlphabetList.Add(item.Name,item.Checked);
            }

            FontStyle style = new FontStyle();
            mFont = new Font(this.checkedListBox1.Font.FontFamily, mFontSize, style);
            mFontBold = new Font(this.checkedListBox1.Font.FontFamily, mFontSize, style|FontStyle.Bold);
        }

        private void AlphabetListClicked(object sender, AlphabetListView.ListItem selectedItem)
        {
            if (selectedItem != null)
            {
                SelectedItemIndex = selectedItem.Index;

                TestItem item = TestItems[SelectedItemIndex] as TestItem;
                this.panel1.Controls.Clear();
                int x = 4, y = 4;
                foreach (string key in item.Options.Keys)
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
                    text.Text = item.Options[key];
                    text.Font = new System.Drawing.Font("Arial", 10.00F);
                    this.panel1.Controls.Add(label);
                    this.panel1.Controls.Add(text);
                    y += 44;
                }
            }
        }

        private bool IsValidLine(string line)
        {
            char [] trimChars = {' ','#'};
            string data = line.Trim(trimChars);
            if (!string.IsNullOrEmpty(data) && data.StartsWith("item"))
            {
                return true;
            }
            return false;
        }

        private int LoadTestItemsFromFile(string file)
        {
            if (File.Exists(file))
            {
                char [] trimChars = {' ','#'};
                FileStream fs = new FileStream(file, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string result = sr.ReadLine();
                bool saveToComment = true;
                while (!string.IsNullOrEmpty(result))
                {
                    //if (result.StartsWith("#item") || result.StartsWith("item"))//COMMENT
                    if(IsValidLine(result))
                    {
                        saveToComment = false;
                        //ITEM
                        string[] values = result.Split('=');
                        string name = values[1].Trim(trimChars);
                        TestItem item = null;
                        if (values[0].StartsWith("#"))
                        {
                            item = new TestItem(name, false);
                        }
                        else
                        {
                            item = new TestItem(name, true);
                        }
                        if (item != null)
                        {
                            //OPTIONS
                            string options = sr.ReadLine();
                            while (!string.IsNullOrEmpty(options))
                            {
                                values = options.Split('=');
                                if (values.Length == 2) //options = [#option = 12]
                                {
                                    string option = values[0].Trim(trimChars);
                                    string value = values[1].Trim(trimChars);
                                    item.Options.Add(option, value);
                                    options = sr.ReadLine();
                                }
                                else //options = [#########]
                                {
                                    break;
                                }
                            }
                            TestItems.Add(item);
                        }
                    }
                    if (saveToComment)
                    {
                        mCommentList.Add(result);
                    }
                    result = sr.ReadLine();
                }
                sr.Close();
                fs.Close();
            }
            return TestItems.Count;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedIndex !=-1 && checkedListBox1.SelectedIndex != SelectedItemIndex)
            {
                SelectedItemIndex = checkedListBox1.SelectedIndex;

                TestItem item = TestItems[SelectedItemIndex] as TestItem;
                this.panel1.Controls.Clear();
                int x = 4, y = 4;
                foreach (string key in item.Options.Keys)
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
                    text.Text = item.Options[key];
                    text.Font = new System.Drawing.Font("Arial", 10.00F);
                    this.panel1.Controls.Add(label);
                    this.panel1.Controls.Add(text);
                    y += 44;
                }
            }
        }
        //
        private void TextBox_Press(object sender, KeyPressEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (e.KeyChar == 13)
            {
                if (!string.IsNullOrEmpty(box.Text))
                {
                    if (SelectedItemIndex != -1)
                    {
                        TestItem item = TestItems[SelectedItemIndex] as TestItem;
                        EditorTag tag = box.Tag as EditorTag;
                        item.Options[tag.KEY] = box.Text;
                    }
                }
            }
        }

        private void FormEditItems_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveTestItemsToFile(ConfigFolder + "\\pqaa.cfg");
        }

        private int SaveTestItemsToFile(string file)
        {
            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                FileStream fs = new FileStream(file, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                foreach (string comment in mCommentList)
                {
                    sw.WriteLine(comment);
                }
                foreach (TestItem item in TestItems)
                {
                    if (item.Checked)
                    {
                        sw.WriteLine("item=" + item.Name);
                        foreach (string key in item.Options.Keys)
                        {
                            sw.WriteLine(key + "=" + item.Options[key]);
                        }
                    }
                    else
                    {
                        sw.WriteLine("#item=" + item.Name);
                        foreach (string key in item.Options.Keys)
                        {
                            sw.WriteLine("#" + key + "=" + item.Options[key]);
                        }
                    }
                    sw.WriteLine("######################################################");
                }
                sw.Close();
                fs.Close();
            }
            return TestItems.Count;
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index != -1)
            {
                TestItem item = TestItems[e.Index] as TestItem;
                item.Checked = (e.NewValue == CheckState.Checked) ? true : false;
            }
        }

        private void AlphabetListItemChecked(object sender, ItemCheckEventArgs e)
        {
            if (e.Index != -1)
            {
                TestItem item = TestItems[e.Index] as TestItem;
                item.Checked = (e.NewValue == CheckState.Checked) ? true : false;
            }
        }

        private Font checkedListBox1_GetFont(CustomCheckedListBox listbox, DrawItemEventArgs e)
        {
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                return mFontBold;
            }
            return mFont;
        }
        //

    }
}

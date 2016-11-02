using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading.Tasks;

using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ThoughtWorks.QRCode.Codec.Util;

using System.Management;
using MultiControl.Common;
using MultiControl.Lib;

namespace MultiControl
{
    public partial class Form1 : Form, INotifyPropertyChanged
    {
        #region 全局变量
        StringBuilder TestLog = new StringBuilder();

        private int mCount = 0;
        private DutDevice[] mConnectedDut;
        private int mRows, mCols;

        SynchronizationContext _syncContext = null;

        public string mCfgFolder = string.Empty;
        public string mLogFolder = string.Empty;
        private DataSet mConfigPath;
        private DataSet mConfigSDCard;
        private bool bMultiModelSupport = true;
        //private bool bTerminateThread = false;

        private UserButton mButtonClose = null;
        private UserButton mButtonAbout = null;
        private UserButton mButtonMin = null;
        private string mGlobalmd5Code = string.Empty;

        private string mLicenseKey = string.Empty;
        private bool mLicensed = false;

        private USB mUSB = new USB();

        private int mExcelStartRow = 2;

        private string[] mFileHeader = {
                                           "PQAA SW", "S/N", "Brand", "Model Name", "Android Version",
                                           "IMEI", "Log Time", "Test Time(s)"
                                       };

        private string[] mFileTestItem = {
                                            "AudioLoopback","BlueTooth","Camera","ConfigChk","Display",
                                            "MoniPower","SDCard","TouchPanel","HeadsetLoopback","Vibration",
                                            "Wifi","RAM","OTG","GPS","NFC","SIM","Button","ReceiverLoopback",
                                            "LED","Audio","Brightness","ECompass","GSensor","GyroSensor",
                                            "LightSensor","ProximitySensor","Headset","MultiTouch","HallSensor",
                                            "HDMI","BarometerSensor","ReceiverLoopback","LTE","IrDA","WirelessCharging"
                                         };

        private string[] mFileFooter = {
                                           "Result"
                                       };
        //private int mTickCount = 5;
        //private OpaqueCommand mTipsLayer = new OpaqueCommand();
        //private MyOpaqueLayer mOpaqueLayer = null;
        private OpaqueForm mOpaqueLayer = new OpaqueForm();

        private int mPseudoTotal = 0;

        private string SPECIFIC_TAG_PATH = "/Android/data/com.wistron.generic.pqaa/files/path.verify.pass";
        //private string CFG_FILE_PACKAGE = "/mnt/sdcard/Android/data/com.wistron.generic.pqaa/";
        private string CFG_FILE_ROOT = "/Android/data/com.wistron.generic.pqaa/files/";
        private string CFG_FILE_PQAA = "/Android/data/com.wistron.generic.pqaa/files/pqaa_config";

        //Event
        public event OnStartUpdateHandle StartUpdate;
        public event OnResultUpdateHandle ResultUpdate;
        public event OnProgressUpdateHandle ProgressUpdate;
        public event OnFinishUpdateHandle FinishUpdate;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(object sender, string e)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(e));
        }
        private UserGridClassLibrary.UserGrid mDuts = new UserGridClassLibrary.UserGrid();
        private Bitmap mImageBackground = null;

        /// <summary>
        /// 处理CMD命令
        /// </summary>
        CMDHelper cmd;
        #endregion

        #region Initilize/Navigation methods
        public Form1()
        {
            InitializeComponent();
            // cmd操作类初始化
            cmd = new CMDHelper();
            //cmd.OutputDataReceived += Cmd_OutputDataReceived;
            //cmd.Exited += Cmd_Exited;

            _syncContext = SynchronizationContext.Current;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width - 32, Screen.PrimaryScreen.WorkingArea.Height - 25);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 加载cfg参数
            LoadCfg();

            //SaveResultToFile("I:\\Wistron\\MultiControl\\MultiControl\\bin\\Debug\\log\\Tiger\\20160427\\32dcc06f_result.txt",0);

            OnDutInitialize();

            this.Resize += new System.EventHandler(this.LayoutDutControls);
            this.StartUpdate += new OnStartUpdateHandle(this.OnStartUpdate);
            this.ResultUpdate += new OnResultUpdateHandle(this.OnResultUpdate);
            this.ProgressUpdate += new OnProgressUpdateHandle(this.OnProgressUpdate);
            this.FinishUpdate += new OnFinishUpdateHandle(this.OnFinishUpdate);

            mContext = BufferedGraphicsManager.Current;
            mContext.MaximumBuffer = new Size(Width + 1, Height + 1);
            mFontCaption = new Font("Microsoft YaHei", 11.0f, FontStyle.Bold);
            mFontBold = new Font("Microsoft YaHei", 12.0f);
            mTitleColor = System.Drawing.Color.Gray;
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width - 32, Screen.PrimaryScreen.WorkingArea.Height - 26);
            //this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void LoadCfg()
        {
            IniFile mIni = new IniFile(Application.StartupPath + "\\cfg.ini");
            mCfgFolder = mIni.IniReadValue("config", "Folder");
            this.label1.Text = "Load CFG from <" + mCfgFolder + ">";
            mLogFolder = mIni.IniReadValue("config", "Logger");

            mRows = int.Parse(mIni.IniReadValue("layout", "Row").ToString());
            mCols = int.Parse(mIni.IniReadValue("layout", "Column").ToString());

            //LOAD CFG TABLE
            mConfigPath = new DataSet("ConfigData");
            mConfigPath.ReadXml("ConfigData.xml");

            mConfigSDCard = new DataSet("ConfigPath");
            mConfigSDCard.ReadXml("ConfigPath.xml");

            int multiSupport = int.Parse(mIni.IniReadValue("mode", "MultiSupport").ToString());
            if (multiSupport == 1)
            {
                bMultiModelSupport = true;
            }
            else
            {
                bMultiModelSupport = false;
            }

            if (bMultiModelSupport)
            {
                //this.label1.Visible = false;
                this.label1.Text = "Auto download CFG by DUT model";
                this.configurationToolStripMenuItem.Enabled = false;
                this.modelMappingConfigurationToolStripMenuItem.Enabled = true;
                this.checkBoxModel.Checked = true;
            }
            else
            {
                //this.label1.Visible = true;
                this.label1.Text = "Load CFG from <" + mCfgFolder + ">";
                this.configurationToolStripMenuItem.Enabled = true;
                this.modelMappingConfigurationToolStripMenuItem.Enabled = false;
                this.checkBoxModel.Checked = false;
            }
        }
        #region .net framework 小于4.0时使用此方法
        private void Cmd_Exited(object sender, ProcessExitAgs e)
        {
            Debug.WriteLine($"{e.Command} exited");
        }

        List<string> _deviceList = new List<string>();
        List<string> DeviceList
        {
            get
            {
                return _deviceList;
            }
            set
            {
                _deviceList = value;
                if (_deviceList != null && _deviceList.Count > 0)
                {
                    OnPropertyChanged(this, nameof(DeviceList));
                }
            }
        }
        private void Cmd_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Debug.WriteLine(e.Data);
            if (e.Data != null && e.Data.Contains("\tdevice"))
            {
                Debug.WriteLine(e.Data);
                string device = Regex.Split(e.Data, "\t", RegexOptions.IgnoreCase)[0];
                if (!_deviceList.Contains(device))
                {
                    List<string> dvicelist = new List<string>(_deviceList.ToArray());
                    dvicelist.Add(device);
                    DeviceList = dvicelist;
                }
                else
                {
                    OnPropertyChanged(this, nameof(DeviceList));// 属于是更新状态
                }
            }
        }

        /// <summary>
        /// 检测到device列表发生了变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeviceList))
            {
                foreach (string device_sn in DeviceList)
                {
                    bool Found = false;
                    foreach (DutDevice uut in mConnectedDut)
                    {
                        if (!string.IsNullOrEmpty(uut.SerialNumber) && uut.SerialNumber.Equals(device_sn))
                        {
                            uut.Connected = true;
                            Found = true;
                            break;
                        }
                    }
                    if (!Found)
                    {
                        //NEW UUT WAS INSERT
                        TestSpecifiedUUT(device_sn);
                    }
                }
            }
        }
        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
            PropertyChanged += OnDeviceChanged;
        }
        /// <summary>
        /// 监听USB插拔事件
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == USB.WM_DEVICECHANGE)
                {
                    var value = m.WParam.ToInt32();
                    switch (m.WParam.ToInt32())
                    {
                        // USB插上  
                        case USB.DBT_DEVICEARRIVAL:
                            Debug.WriteLine("Device arrival");
                            //await Task.Delay(1000);
                            Thread.Sleep(1000);
                            OnHandleUUTInsertion();
                            break;
                        // USB移除  
                        case USB.DBT_DEVICEREMOVECOMPLETE:
                            Debug.WriteLine("Device removed");
                            OnHandleUUTRemoval();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            base.WndProc(ref m);
        }
        /// <summary>
        /// 拔除设备响应
        /// </summary>
        private async void OnHandleUUTRemoval()
        {
            string result = await Execute(AdbCommand.Adb_devices);
            var deviceList = common.GetDeviceList(result);
            for (int index = 0; index < mConnectedDut.Length; index++)
            {
                var device = mConnectedDut[index];
                if (deviceList.Contains(device.SerialNumber))
                {
                    device.Connected = true;
                }
                else
                {
                    switch (device.TestResult)
                    {
                        case DutDevice.DutResult.DR_PASS:
                            device.Reset();
                            mDuts[index].Reset();
                            break;
                        case DutDevice.DutResult.DR_FAIL:
                            device.ExitRunningThread = true;
                            device.Reset();
                            mDuts[index].Reset();
                            break;
                        case DutDevice.DutResult.DR_NONE:
                        case DutDevice.DutResult.DR_TESTING:
                        default:
                            device.Connected = false;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 插入设备时响应
        /// </summary>
        private async void OnHandleUUTInsertion()
        {
            string result = await Execute(AdbCommand.Adb_devices);
            var deviceList = common.GetDeviceList(result);
            int count = 0;
            while (count <= 10)
            {
                result = await Execute(AdbCommand.Adb_devices);
                var deviceList1 = common.GetDeviceList(result);
                if (deviceList1.Count > deviceList.Count)
                {
                    deviceList = deviceList1;
                    break;
                }
                await Task.Delay(500);
                count++;
            }
            for (int index = 0; index < deviceList.Count; index++)
            {
                var device = deviceList[index];
                if (mConnectedDut.Contains(new DutDevice() { SerialNumber = device.ToString() }, DutDevice.Default))
                {
                    int indey = common.IndexDevice(mConnectedDut, device.ToString());
                    mConnectedDut[indey].Connected = true;
                }
                else
                {
                    TestSpecifiedUUT(device.ToString());
                }
            }
        }
        /// <summary>
        /// 重新开始测试
        /// </summary>
        private async void DoControlTest()
        {
            //RESET TIME
            foreach (DutDevice terminal in mConnectedDut)
            {
                terminal.BenginTime = DateTime.Now;
            }
            //CHECK DEVICES 
            foreach (UserGridClassLibrary.GridItem dut in mDuts)
            {
                if (!string.IsNullOrEmpty(dut.GetDutSN()))
                {
                    int id = dut.GetDutGridID();
                    mConnectedDut[id].ExitRunningThread = true;
                    dut.Reset();
                }
            }
            //END
            string computer = common.GetComputerName();
            string md5Code = string.Empty;
            //md5Code = common.GetMD5Code("wortsin");
            //MessageBox.Show(computer);
            if (mLicensed)
            {
                md5Code = common.GetMD5Code("wortsin");
            }
            else
            {
                md5Code = common.GetMD5Code(computer);
            }
            mGlobalmd5Code = md5Code;
            //MD5.txt
            common.CreateFile("md5.txt", md5Code);
            //Computer.txt
            common.CreateFile("computer.txt", computer);

            string result = await Execute(AdbCommand.Adb_devices);
            var deviceList = common.GetDeviceList(result);
            //int count = 0;
            //while (count <= 10)
            //{
            //    result = await Execute(AdbCommand.Adb_devices);
            //    var deviceList1 = common.GetDeviceList(result);
            //    if (deviceList1.Count > deviceList.Count)
            //    {
            //        deviceList = deviceList1;
            //        break;
            //    }
            //    await Task.Delay(500);
            //    count++;
            //}
            for (int index = 0; index < deviceList.Count; index++)
            {
                var device = deviceList[index];
                if (mConnectedDut.Contains(new DutDevice() { SerialNumber = device.ToString() }, DutDevice.Default))
                {
                    int indey = common.IndexDevice(mConnectedDut, device.ToString());
                    mConnectedDut[indey].Connected = true;
                }
                else
                {
                    TestSpecifiedUUT(device.ToString());
                }
            }

        }


        #endregion

        public void UpdateLicenseInformation(string key, bool licensed)
        {
            mLicenseKey = key;
            mLicensed = licensed;
        }

        private void OnDutInitialize()
        {
            mButtonClose = new UserButton();
            mButtonClose.mFormat = UserButton.ButtonFormat.Close;
            mButtonClose.Text = "x_x";
            mButtonClose.Location = new Point(Width - 30, 4);
            mButtonClose.Size = new Size(20, 20);
            mButtonClose.BackColor = System.Drawing.Color.White;

            mButtonClose.ButtonClick += this.OnCloseWindow;
            this.Controls.Add(mButtonClose);

            mButtonAbout = new UserButton();
            mButtonAbout.mFormat = UserButton.ButtonFormat.About;
            mButtonAbout.Text = "x_x";
            mButtonAbout.Location = new Point(Width - 52, 4);
            mButtonAbout.Size = new Size(20, 20);
            mButtonAbout.BackColor = System.Drawing.Color.White;

            mButtonAbout.ButtonClick += this.helpToolStripMenuItem_Click;
            this.Controls.Add(mButtonAbout);

            mButtonMin = new UserButton();
            mButtonMin.mFormat = UserButton.ButtonFormat.Mininum;
            mButtonMin.Text = "x_x";
            mButtonMin.Location = new Point(Width - 74, 4);
            mButtonMin.Size = new Size(20, 20);
            mButtonMin.BackColor = System.Drawing.Color.White;

            mButtonMin.ButtonClick += this.OnMinimumWindow;
            this.Controls.Add(mButtonMin);

            int size = mRows * mCols;
            mConnectedDut = new DutDevice[mRows * mCols];

            for (int i = 0; i < size; i++)
            {
                mConnectedDut[i] = new DutDevice();
            }

            mOpaqueLayer.Show(this);
            mOpaqueLayer.Visible = false;

            mOpaqueLayer.OnItemChanged += new OpaqueForm.ItemChangedHandle(this.OnTestItemChanged);
        }

        private void OnTestItemChanged(object sender, OpaqueForm.ChangedEventArgs arg)
        {
            int index = arg.ID;
            if (index != -1)
            {
                mDuts[index].UpdateElapseTime(arg.Elapsed);
            }
        }

        private void LayoutDutControls(object sender, EventArgs e)
        {
            int width = this.Width;
            int height = this.Height;

            mImageBackground = new Bitmap(Width, Height);
            this.button1.Location = new Point(32, this.Height - this.statusStrip1.Height - this.button1.Height - 48);
            this.checkBoxModel.Location = new Point(this.Width - this.checkBoxModel.Width - 32, this.checkBoxModel.Location.Y);

            mButtonClose.Location = new Point(Width - 30, 4);
            mButtonAbout.Location = new Point(Width - 52, 4);

            //UserGrid
            mDuts.Location = new Point(32, 92);
            mDuts.Width = this.Width - 64;
            mDuts.Height = this.Height - 120;
            mDuts.Row = mRows;
            mDuts.Column = mCols;
            mDuts.InitializeGridItems();
            //mUserGrid.Invalidate();
            this.Controls.Add(mDuts);

            mDuts.OnGridItemChecked += new UserGridClassLibrary.UserGrid.GridItemCheckedHandle(this.OnGridItemCheckedHandle);
            mDuts.OnGridItemFocus += new UserGridClassLibrary.UserGrid.GridItemFocusHandle(this.OnGridItemFocusHandle);
            mDuts.OnGridItemReset += new UserGridClassLibrary.UserGrid.GridItemResetHandle(this.OnGridItemResetHandle);
            mDuts.OnGridItemRightClicked += new UserGridClassLibrary.UserGrid.GridItemRightClickHandle(this.OnGridItemRightClicked);
        }


        private async void TestSpecifiedUUT(string matchedUUT)
        {
            if (!string.IsNullOrEmpty(matchedUUT))
            {
                //CHECK DEVICES
                int index = FindAvaiableDUTIndex();
                if (index != -1)
                {
                    mConnectedDut[index].BenginTime = DateTime.Now;
                    //END
                    string computer = common.GetComputerName();
                    string md5Code = string.Empty;
                    if (mLicensed)
                    {
                        md5Code = common.GetMD5Code("wortsin");
                    }
                    else
                    {
                        md5Code = common.GetMD5Code(computer);
                    }
                    mGlobalmd5Code = md5Code;
                    //MD5.txt
                    common.CreateFile("md5.txt", md5Code);
                    //Computer.txt
                    common.CreateFile("computer.txt", computer);
                    //CHECK DUTS
                    mDuts[index].Reset();
                    mConnectedDut[index].SerialNumber = matchedUUT;
                    //get product mode ro.product.model
                    string modeCmd = "adb -s " + mConnectedDut[index].SerialNumber + " shell getprop ro.product.model";

                    string model = await Execute(modeCmd);
                    //string model = common.FilterModelName(cmd.CMD_RunEx(modeCmd));


                    model = model.Replace("\r\n", "").Replace("\r", "");
                    if (common.IsNumeric(model))
                    {
                        modeCmd = "adb -s " + mConnectedDut[index].SerialNumber + " shell getprop ro.product.brand";
                        //model = cmd.CMD_RunEx(modeCmd);
                        model = await Execute(modeCmd);

                        model = model.Replace("\r\n", "").Replace("\r", "");
                    }
                    mConnectedDut[index].Model = model;
                    mConnectedDut[index].ConfigPath = QueryModelConfigPathFromDataSet(model);// Application.StartupPath + "\\" + model;
                    mConnectedDut[index].Estimate = QueryModelEstimateTimeFromDataSet(model);
                    mConnectedDut[index].Brand = QueryModelBrandFromDataSet(model);
                    mConnectedDut[index].Connected = true;
                    //mConnectedDut[index].ExitRunningThread = false;
                    string androidCmd = "adb -s " + mConnectedDut[index].SerialNumber + " shell getprop ro.build.version.release";
                    //string androidVersion = cmd.CMD_RunEx(androidCmd);
                    string androidVersion = await Execute(androidCmd);


                    androidVersion = androidVersion.Replace("\r\n", "").Replace("\r", "");
                    mConnectedDut[index].AndroidVersion = androidVersion;
                    //ro.build.id
                    string buildCmd = "adb -s " + mConnectedDut[index].SerialNumber + " shell getprop ro.build.id";
                    //string buildId = cmd.CMD_RunEx(buildCmd);
                    string buildId = await Execute(buildCmd);

                    buildId = buildId.Replace("\r\n", "").Replace("\r", "");
                    mConnectedDut[index].BuildId = buildId;

                    Thread th = new Thread(new ParameterizedThreadStart(ThreadMethod));
                    th.Start(index); //启动线程
                }
            }
        }

        private int FindAvaiableDUTIndex()
        {
            int index = -1;
            bool Found = false;
            //FIND BLANK UUT
            foreach (DutDevice uut in mConnectedDut)
            {
                index++;
                if (string.IsNullOrEmpty(uut.SerialNumber))
                {
                    Found = true;
                    break;
                }
            }
            if (!Found)
            {
                index = -1;
                foreach (DutDevice uut in mConnectedDut)
                {
                    index++;
                    if (!uut.Connected)
                    {
                        Found = true;
                        uut.ExitRunningThread = true;
                        break;
                    }
                }
            }
            if (!Found)
            {
                index = -1;
            }
            return index;
        }

        private async Task<string> QueryModelConfigSDCardFromDataSet(int i)
        {
            string result = string.Empty;
            mConnectedDut[i].SDCard = string.Empty;

            if (mConfigSDCard.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in mConfigSDCard.Tables[0].Rows)
                {
                    //SD chance path @20160518
                    string verify = row[1].ToString();
                    string pullCmd = "adb -s " + mConnectedDut[i].SerialNumber + " pull " + verify + SPECIFIC_TAG_PATH;
                    string console = await Execute(pullCmd);


                    if (console.Contains("remote object"))
                    {
                        continue;
                    }
                    else
                    {
                        mConnectedDut[i].SDCard = verify;
                        result = mConnectedDut[i].SDCard;
                        //delete path.verify.pass file before break
                        if (File.Exists("path.verify.pass"))
                        {
                            File.Delete("path.verify.pass");
                        }
                        break;
                    }
                }
            }

            return result;
        }

        private string QueryModelConfigPathFromDataSet(string model)
        {
            string result = Application.StartupPath + "\\default";

            if (mConfigPath.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in mConfigPath.Tables[0].Rows)
                {
                    //if (row[0].ToString().CompareTo(model) == 0)//FIND
                    if (row[0].ToString().Equals(model))
                    {
                        result = row[2].ToString();
                        break;
                    }
                }
            }

            return result;
        }

        private string QueryModelBrandFromDataSet(string model)
        {
            string result = string.Empty;
            if (mConfigPath.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in mConfigPath.Tables[0].Rows)
                {
                    //if (row[0].ToString().CompareTo(model) == 0)//FIND
                    if (row[0].ToString().Equals(model))
                    {
                        if (row[1] != null)
                        {
                            result = row[1].ToString();
                        }
                        break;
                    }
                }
            }

            return result;
        }

        private float QueryModelEstimateTimeFromDataSet(string model)
        {
            float result = 0.0f;
            if (mConfigPath.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in mConfigPath.Tables[0].Rows)
                {
                    //if (row[0].ToString().CompareTo(model) == 0)//FIND
                    if (row[0].ToString().Equals(model))
                    {
                        if (row[3] != null)
                        {
                            if (!string.IsNullOrEmpty(row[3].ToString()))
                            {
                                result = float.Parse(row[3].ToString());
                            }
                            else
                            {
                                result = 255.0f;
                            }
                        }
                        break;
                    }
                }
            }

            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DoControlTest();
        }


        #region Update Devic Stattus
        delegate void UpdateUIStatusDelegate(object index);

        void SetDutStatusInvoke(object index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
                {
                    SetDutStatus(_index);
                }), new object[] { index });
            }
            else
            {
                SetDutStatus(index);
            }
        }

        void SetDutIMEIInvoke(object index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
                {
                    SetDutIMEI(_index);
                }), new object[] { index });
            }
            else
            {
                SetDutIMEI(index);
            }
        }

        void SetDutMD5Invoke(object index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
                {
                    SetDutMD5(_index);
                }), new object[] { index });
            }
            else
            {
                SetDutMD5(index);
            }
        }

        void SetDutPQAAFolderInvoke(object index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
                {
                    SetDutPQAAFolder(_index);
                }), new object[] { index });
            }
            else
            {
                SetDutPQAAFolder(index);
            }
        }

        void SetDutInstallPQAAInvoke(object index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
                {
                    SetDutInstallPQAA(_index);
                }), new object[] { index });
            }
            else
            {
                SetDutInstallPQAA(index);
            }
        }

        void SetDutStartPQAAInvoke(object index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
                {
                    SetDutStartPQAA(_index);
                }), new object[] { index });
            }
            else
            {
                SetDutStartPQAA(index);
            }
        }

        void SetDutTestProgressInvoke(object index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
                {
                    SetDutTestProgress(_index);
                }), new object[] { index });
            }
            else
            {
                SetDutTestProgress(index);
            }
        }

        void SetDutTestFinishInvoke(object index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
                {
                    SetDutTestFinish(_index);
                }), new object[] { index });
            }
            else
            {
                SetDutTestFinish(index);
            }
        }


        private void SetDutDisconnected(object index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
                {
                    int i = (int)index;
                    mDuts[i].BackgroundGradientColor = System.Drawing.Color.FromArgb(192, 64, 64, 64);//System.Drawing.Color.Blue;
                    mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.None;
                    mDuts[i].SetDutStatus("Offline");
                }), new object[] { index });
            }
            else
            {
                int i = (int)index;
                mDuts[i].BackgroundGradientColor = System.Drawing.Color.FromArgb(192, 64, 64, 64);//System.Drawing.Color.Blue;
                mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.None;
                mDuts[i].SetDutStatus("Offline");
            }
        }

        private void SetDutConnected(object index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
                {
                    int i = (int)index;
                    mDuts[i].SetDutStatus("Online");
                    switch (mConnectedDut[i].TestResult)
                    {
                        case DutDevice.DutResult.DR_TESTING:
                            break;
                        case DutDevice.DutResult.DR_PASS:
                            mDuts[i].BackgroundGradientColor = System.Drawing.Color.Green;
                            mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
                            break;
                        case DutDevice.DutResult.DR_FAIL:
                            mDuts[i].BackgroundGradientColor = System.Drawing.Color.Red;
                            mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
                            break;
                    }
                }), new object[] { index });
            }
            else
            {
                int i = (int)index;
                mDuts[i].SetDutStatus("Online");
                switch (mConnectedDut[i].TestResult)
                {
                    case DutDevice.DutResult.DR_TESTING:
                        break;
                    case DutDevice.DutResult.DR_PASS:
                        mDuts[i].BackgroundGradientColor = System.Drawing.Color.Green;
                        mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
                        break;
                    case DutDevice.DutResult.DR_FAIL:
                        mDuts[i].BackgroundGradientColor = System.Drawing.Color.Red;
                        mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
                        break;
                }
            }

        }

        private void SetDutStatus(object index)
        {
            int i = (int)index;
            //START UPDATE NEW INFO.
            mConnectedDut[i].ID = i;
            //mDuts[i].SetDutSN("SN - " + mDevice[i].ToUpper());
            mDuts[i].SetDutSN(mConnectedDut[i].SerialNumber.ToUpper());
            mDuts[i].SetDutStatus("Online");
            mDuts[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
            mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            mDuts[i].Active = true;
        }

        private void SetDutModel(object index)
        {
            //this.Dispacher
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
                {
                    int i = (int)index;
                    mDuts[i].SetDutModel(mConnectedDut[i].Model);
                    mDuts[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
                    mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
                }), new object[] { index });
            }
            else
            {
                int i = (int)index;
                mDuts[i].SetDutModel(mConnectedDut[i].Model);
                mDuts[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
                mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            }
        }

        private void SetDutIMEI(object index)
        {
            int i = (int)index;
            mDuts[i].SetDutIMEI(mConnectedDut[i].IMEI);
            mDuts[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
            mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            //SET QRCODE
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 1;
            qrCodeEncoder.QRCodeVersion = 9;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            Image image;
            string BN = mConnectedDut[i].BuildNumber;
            string data = "Model:" + mConnectedDut[i].Model + System.Environment.NewLine +
                "OS Version:" + mConnectedDut[i].BuildId + System.Environment.NewLine +
                "SN:" + mConnectedDut[i].SerialNumber.ToUpper() + System.Environment.NewLine +
                "IMEI:" + mConnectedDut[i].IMEI.ToUpper() + System.Environment.NewLine +
                "Memory:" + mConnectedDut[i].RAM + System.Environment.NewLine +
                "Flash:" + mConnectedDut[i].FLASH + System.Environment.NewLine + "BuildNumber:";//bonnie20160805

            image = qrCodeEncoder.Encode(data + BN);
            mDuts[i].SetQRCode_IMEI(image);
        }

        private void SetDutMD5(object index)
        {
            int i = (int)index;
            mDuts[i].SetDutItem("md5.txt");
            mDuts[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
            mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
        }

        private void SetDutPQAAFolder(object index)
        {
            int i = (int)index;
            mDuts[i].SetDutItem("config files");
            mDuts[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
            mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
        }

        private void SetDutInstallPQAA(object index)
        {
            int i = (int)index;
            mDuts[i].SetDutItem("Install PQAA");
            mDuts[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
            mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            if (this.StartUpdate != null)
            {
                ResultEventArgs args = new ResultEventArgs(i, string.Empty);
                args.Current = 0;
                args.Total = -2;
                args.Time = 0.0f;
                args.Item = "PQAA Install";
                args.Model = mConnectedDut[i].Model;
                StartUpdate(this, args);
            }
        }

        private void SetDutStartPQAA(object index)
        {
            int i = (int)index;
            mDuts[i].SetDutItem("Start Testing...");
            //mDuts[i].SetDutPercentage("");
            mDuts[i].BackgroundGradientColor = System.Drawing.Color.Orange;
            mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            if (this.StartUpdate != null)
            {
                ResultEventArgs args = new ResultEventArgs(i, string.Empty);
                args.Current = 0;
                args.Total = -1;
                args.Time = mDuts[i].InstallTime;
                args.Item = "PQAA Install";
                args.Model = mConnectedDut[i].Model;
                StartUpdate(this, args);
            }
        }

        private void SetDutTestProgress(object progress)
        {
            string value = progress.ToString();
            string[] sArray = value.Split('/');
            int i = int.Parse(sArray[0].ToString());
            int current = int.Parse(sArray[1].ToString());
            int total = int.Parse(sArray[2].ToString());
            string item = sArray[3].ToString();
            if (this.ProgressUpdate != null)
            {
                TestProgressEventArgs args = new TestProgressEventArgs();
                args.ResultIndex = i;
                args.Current = current;
                args.Total = total;
                args.Item = item;
                ProgressUpdate(this, args);
            }
            //UPDATE RESULT ITEM
            if (this.ResultUpdate != null)
            {
                string resultName = mConnectedDut[i].SerialNumber + "_result.txt";
                int index = i;
                ResultEventArgs args = new ResultEventArgs(index, resultName);
                args.Current = current;
                args.Total = total;
                args.Item = item;
                args.Model = mConnectedDut[i].Model;
                ResultUpdate(this, args);
            }
        }

        private void OnFinishUpdate(object sender, TestProgressEventArgs e)
        {
            int i = e.ResultIndex;
            mDuts[i].SetDutStatus("Online");
            mPseudoTotal = e.Total;
            //mDuts[i].SetDutProgress(e.Current, e.Total);
            mDuts[i].SetDutItem(e.Item);
            //mDuts[i].BackgroundGradientColor = System.Drawing.Color.Orange;
            if (e.Item.Equals("PASS"))
            {
                mDuts[i].BackgroundGradientColor = System.Drawing.Color.Green;
                mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
                mConnectedDut[i].TestResult = DutDevice.DutResult.DR_PASS;
                mDuts[i].Result = UserGridClassLibrary.ItemResult.IR_PASS;
                mOpaqueLayer.DictionaryWorkingStatus[i] = OpaqueForm.MyResult.PASS;
            }
            else
            {
                mDuts[i].BackgroundGradientColor = System.Drawing.Color.Red;
                mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
                mConnectedDut[i].TestResult = DutDevice.DutResult.DR_FAIL;
                mDuts[i].Result = UserGridClassLibrary.ItemResult.IR_FAIL;
                mOpaqueLayer.DictionaryWorkingStatus[i] = OpaqueForm.MyResult.FAIL;
            }
            if (mOpaqueLayer != null)
            {
                mOpaqueLayer.EventWaitHandle[0].Reset();
            }
        }

        private void OnProgressUpdate(object sender, TestProgressEventArgs e)
        {
            int i = e.ResultIndex;
            mDuts[i].SetDutStatus("Online");
            mPseudoTotal = e.Total;
            //mDuts[i].SetDutProgress(e.Current, e.Total);
            //mDuts[i].ElapsedTime += 
            string name = e.Item;
            if (
                name.Contains("ECompass") || name.Contains("Barometer") || name.Contains("OTG") ||
                name.Contains("Brightness") || name.Contains("Display") ||
                name.Contains("GSensor") || name.Contains("GyroSensor") || name.Contains("LightSensor") ||
                name.Contains("TouchPanel") || name.Contains("ProximitySensor") || name.Contains("Headset") ||
                name.Contains("MultiTouch") || name.Contains("HallSensor") || name.Contains("Button") ||
                name.Contains("HDMI") || name.Contains("NFC")
              )
            {
                mDuts[i].SetDutItem(e.Item, false);
            }
            else
            {
                mDuts[i].SetDutItem(e.Item);
            }
            mDuts[i].BackgroundGradientColor = System.Drawing.Color.Orange;
            mDuts[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            mDuts[i].Result = UserGridClassLibrary.ItemResult.IR_TESTING;
            mConnectedDut[i].TestResult = DutDevice.DutResult.DR_TESTING;
        }

        private void OnStartUpdate(object sender, ResultEventArgs e)
        {
            int key = e.ResultIndex;
            if (mOpaqueLayer != null)
            {
                mOpaqueLayer.DictionaryWorkingStatus[key] = OpaqueForm.MyResult.WORKING;
            }
            switch (e.Total)
            {
                case -2:
                    mOpaqueLayer.AddResult(key, e.Model, e.Item, OpaqueForm.MyResult.WORKING, 0.0f);
                    break;
                case -1:
                    mOpaqueLayer.AddResult(key, e.Model, e.Item, OpaqueForm.MyResult.PASS, e.Time);
                    break;
            }
        }

        private void OnResultUpdate(object sender, ResultEventArgs e)
        {
            string resultName = e.ResultFile;
            int key = e.ResultIndex;
            if (mOpaqueLayer != null)
            {
                mOpaqueLayer.DictionaryWorkingStatus[key] = OpaqueForm.MyResult.WORKING;
            }
            if (File.Exists(resultName))
            {
                FileStream fs = new FileStream(resultName, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string result = sr.ReadLine();
                while (!string.IsNullOrEmpty(result))
                {
                    string[] combinedResult = result.Split('=');
                    string _name = combinedResult[0].ToString();

                    int _result = int.Parse(combinedResult[1].ToString());
                    float _cost = float.Parse(combinedResult[2].ToString());
                    if (_result == 1)
                    {
                        mOpaqueLayer.AddResult(key, e.Model, _name, OpaqueForm.MyResult.PASS, _cost);
                    }
                    else if (_result == 2)
                    {
                        mOpaqueLayer.AddResult(key, e.Model, _name, OpaqueForm.MyResult.FAIL, _cost);
                    }
                    result = sr.ReadLine();
                }
                sr.Close();
                fs.Close();
            }
            if (e.Current < e.Total)
            {
                mOpaqueLayer.AddResult(key, e.Model, e.Item, OpaqueForm.MyResult.WORKING, 0.0f);
            }
            //UPDATE PROGRESS BAR NUMBER
            int passCount = mOpaqueLayer.GetTotalPassItemCount(key);
            mDuts[key].SetDutProgress(passCount, mPseudoTotal);
        }

        private void SetDutTestFinish(object progress)
        {
            string value = progress.ToString();
            string[] sArray = value.Split('/');
            int i = int.Parse(sArray[0].ToString());
            int current = int.Parse(sArray[1].ToString());
            int total = int.Parse(sArray[2].ToString());
            string result = sArray[3].ToString();
            if (this.FinishUpdate != null)
            {
                TestProgressEventArgs args = new TestProgressEventArgs();
                args.ResultIndex = i;
                args.Current = current;
                args.Total = total;
                args.Item = result;
                FinishUpdate(this, args);
            }
        }
        #endregion
        /// <summary>
        /// device 测试线程
        /// </summary>
        /// <param name="index"></param>
        private async void ThreadMethod(object index)
        {
            int i = (int)index;
            mDuts[i].Result = UserGridClassLibrary.ItemResult.IR_TESTING;
            //_syncContext.Post(SetDutStatus, i);
            SetDutStatusInvoke(i);
            /*
            //get product mode ro.product.model
            string modeCmd = "adb -s " + mConnectedDut[i].SerialNumber + " shell getprop ro.product.model";
            string model = Execute(modeCmd);
            model = model.Replace("\r\n", "").Replace("\r", "");
            mConnectedDut[i].Model = model;
            mConnectedDut[i].ConfigPath = Application.StartupPath + "\\" + model;
            * */
            //_syncContext.Post(SetDutModel, i);
            SetDutModel(i);
            mDuts[i].EstimateTime = mConnectedDut[i].Estimate;
            //DateTime dtStart = DateTime.Now;
            //push md5 file
            string pushCmd = string.Empty;

            //pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " uninstall com.wistron.generic.pqaa";
            //Execute(pushCmd);
            //_syncContext.Post(SetDutInstallPQAA, i);
            //ADD PQAA INSTALL TIME---------------------------------------------
            //mOpaqueLayer.AddResult(i, mConnectedDut[i].Model, "Install PQAA", OpaqueForm.MyResult.WORKING, 0.0f);
            //DateTime dtStart = DateTime.Now;
            pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " install -r Generic_PQAA.apk";
            await Execute(pushCmd, true);



            //adb shell am startservice -a com.wistron.generic.get.sdcard.path
            // /storage/sdcard1/Android/data/com.wistron.generic.pqaa/files/path.verify.pass
            //_syncContext.Post(SetDutInstallPQAA, i);
            SetDutInstallPQAAInvoke(i);

            pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " shell am startservice --user 0 -a com.wistron.generic.get.sdcard.path";
            await Execute(pushCmd, true);



            //adb shell am startservice -a com.wistron.generic.get.sdcard.path
            // /storage/sdcard1/Android/data/com.wistron.generic.pqaa/files/path.verify.pass
            string sdcard = await QueryModelConfigSDCardFromDataSet(i);
            //_syncContext.Post(SetDutInstallPQAA, i);

            if (string.IsNullOrEmpty(sdcard))
            {
                MessageBox.Show("No avaiable SD card for testing...");
                return;
            }
            /*
            pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " shell mkdir " + CFG_FILE_PACKAGE; // /mnt/sdcard/pqaa_config";
            Execute(pushCmd);
            _syncContext.Post(SetDutMD5, i);

            pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " shell mkdir " + CFG_FILE_ROOT; // /mnt/sdcard/pqaa_config";
            Execute(pushCmd);
            _syncContext.Post(SetDutMD5, i);
            */

            pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push md5.txt " + mConnectedDut[i].SDCard + CFG_FILE_ROOT; // /mnt/sdcard/";
            await Execute(pushCmd, true);


            //_syncContext.Post(SetDutMD5, i);
            SetDutMD5Invoke(i);
            /*
            pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " shell mkdir " + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            Execute(pushCmd);
            _syncContext.Post(SetDutPQAAFolder, i);
            */
            //------START-------------------------------------------
            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mConnectedDut[i].ConfigPath +
                    "\\" + "audio_loopback.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                //pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push audio_loopback.cfg /mnt/sdcard/pqaa_config";
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mCfgFolder +
                "\\" + "audio_loopback.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);
            //cmd.CMD_Run(pushCmd);

            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mConnectedDut[i].ConfigPath +
                    "\\" + "monipower.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "monipower.cfg \"" + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);



            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mConnectedDut[i].ConfigPath +
                    "\\" + "sysinfo.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "sysinfo.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);


            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mConnectedDut[i].ConfigPath +
                    "\\" + "wifi.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "wifi.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);



            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mConnectedDut[i].ConfigPath +
                    "\\" + "gps.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "gps.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);




            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mConnectedDut[i].ConfigPath +
                    "\\" + "headsetloopback.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "headsetloopback.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);




            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mConnectedDut[i].ConfigPath +
                    "\\" + "pqaa.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "pqaa.cfg\" " + mConnectedDut[i].SDCard + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);



            //--------------END--------------------------------------

            //pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " uninstall com.wistron.generic.pqaa";
            //Execute(pushCmd);
            //_syncContext.Post(SetDutInstallPQAA, i);
            ////ADD PQAA INSTALL TIME---------------------------------------------
            ////mOpaqueLayer.AddResult(i, mConnectedDut[i].Model, "Install PQAA", OpaqueForm.MyResult.WORKING, 0.0f);
            ////DateTime dtStart = DateTime.Now;
            //pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " install Generic_PQAA.apk";
            //Execute(pushCmd);

            DateTime dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - mConnectedDut[i].BenginTime;// dtStart;
            float _cost = 0.0f;
            _cost = (float)ts.TotalMilliseconds / 1000;
            //mConnectedDut[i].InstallTime = _cost;
            mDuts[i].InstallTime = _cost;
            //ADD END---------------------------------------------------------
            string md5String = mGlobalmd5Code;
            pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " shell am start -n com.wistron.generic.pqaa/.TestItemsList --ei block " + (i + 1).ToString() + " --ei autostart 1" + " --es md5code " + md5String;
            await Execute(pushCmd, true);

            //_syncContext.Post(SetDutStartPQAA, i);
            SetDutStartPQAAInvoke(i);
            //IMEI
            string wInfo = string.Empty;
            string dutInfo = mConnectedDut[i].SerialNumber + "_wInfo.txt";
            wInfo = "adb -s " + mConnectedDut[i].SerialNumber + " pull " + mConnectedDut[i].SDCard + CFG_FILE_ROOT + "wInfo.txt " + dutInfo;
            while (!File.Exists(dutInfo))
            {
                await Execute(wInfo);
            }
            string imei = string.Empty;
            string ram = string.Empty;
            string flash = string.Empty;
            string buildNO = string.Empty;//bonnie
            if (File.Exists(dutInfo))
            {
                FileStream fs = new FileStream(dutInfo, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                imei = sr.ReadLine();
                ram = sr.ReadLine();
                flash = sr.ReadLine();
                buildNO = sr.ReadLine();//bonnie20160805
                buildNO = sr.ReadLine();//bonnie20160805
                sr.Close();
                fs.Close();

                mConnectedDut[i].IMEI = imei;
                mConnectedDut[i].RAM = ram;
                mConnectedDut[i].FLASH = flash;
                mConnectedDut[i].BuildNumber = buildNO;//bonnie20160805

                //_syncContext.Post(SetDutIMEI, i);
                SetDutIMEIInvoke(i);

                File.Delete(dutInfo);
            }
            //END
            string pullCmd = string.Empty;
            string progName = mConnectedDut[i].SerialNumber + "_progress.txt";
            //while (true)
            mConnectedDut[i].ExitRunningThread = false;

            //while(!bTerminateThread)
            int walkedIndex = -1;
            while (!mConnectedDut[i].ExitRunningThread)
            {
                //check this DUT connect or disconnect to quit!
                if (!mConnectedDut[i].Connected)
                {
                    Thread.Sleep(300);
                    //_syncContext.Post(SetDutDisconnected, i);
                    SetDutDisconnected(i);
                    walkedIndex = -1;
                    continue;
                }
                else
                {
                    _syncContext.Post(SetDutConnected, i);
                }
                pullCmd = "adb -s " + mConnectedDut[i].SerialNumber + " pull " + mConnectedDut[i].SDCard + CFG_FILE_ROOT + "progress.txt " + progName;
                await Execute(pullCmd);

                string prog = string.Empty;
                string item = string.Empty;
                if (File.Exists(progName))
                {
                    //1. FILE read exception BUG found @20151204 ->delay to fix
                    //2. ADB server kill BUG found @20151204
                    //3. Exception BUG while remove device during install PQAA foud @20151204
                    Thread.Sleep(100);
                    try
                    {
                        FileStream fs = new FileStream(progName, FileMode.Open);
                        StreamReader sr = new StreamReader(fs);
                        prog = sr.ReadLine();
                        item = sr.ReadLine();
                        sr.Close();
                        fs.Close();
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine("Exception = " + ex.Message);
                        //MessageBox.Show(ex.Message);
                    }
                }
                if (!string.IsNullOrEmpty(prog))// && !string.IsNullOrEmpty(item))
                {
                    //PULL test result
                    string resultName = mConnectedDut[i].SerialNumber + "_result.txt";
                    pullCmd = "adb -s " + mConnectedDut[i].SerialNumber + " pull " + mConnectedDut[i].SDCard + CFG_FILE_ROOT + "result.txt " + resultName;
                    await Execute(pullCmd);



                    //UPDATE TEST PROGRESS
                    string value = i + "/" + prog + "/" + item;
                    //_syncContext.Post(SetDutTestProgress, value);
                    //break this loop
                    string[] sArray = prog.Split('/');
                    try
                    {
                        int cur = int.Parse(sArray[0].ToString());
                        int total = int.Parse(sArray[1].ToString());

                        if (walkedIndex != cur)
                        {
                            //_syncContext.Post(SetDutTestProgress, value);
                            SetDutTestProgressInvoke(value);
                            walkedIndex = cur;
                        }

                        if (cur == total)
                        {
                            //_syncContext.Post(SetDutTestFinish, value);
                            SetDutTestFinishInvoke(value);
                            string delCmd = "adb -s " + mConnectedDut[i].SerialNumber + " shell rm " + mConnectedDut[i].SDCard + CFG_FILE_ROOT + "progress.txt";
                            await Execute(delCmd);


                            if (File.Exists(progName))
                            {
                                File.Delete(progName);
                            }
                            delCmd = "adb -s " + mConnectedDut[i].SerialNumber + " shell rm " + mConnectedDut[i].SDCard + CFG_FILE_ROOT + "result.txt";
                            await Execute(delCmd);



                            if (File.Exists(resultName))
                            {
                                //Create MODEL FOLDER and DATE
                                string date = DateTime.Now.ToString("yyyyMMdd");
                                string modelFolder = mLogFolder + "\\" + mConnectedDut[i].Model;
                                string logFolder = modelFolder + "\\" + date;
                                try
                                {
                                    if (!Directory.Exists(modelFolder))
                                    {
                                        Directory.CreateDirectory(modelFolder);
                                    }
                                    if (!Directory.Exists(logFolder))
                                    {
                                        Directory.CreateDirectory(logFolder);
                                    }
                                    File.Copy(resultName, logFolder + "\\" + resultName, true);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }

                                if (File.Exists(resultName))
                                {
                                    try
                                    {

                                        File.Delete(resultName);
                                        //
                                        string source = logFolder + "\\" + resultName;
                                        SaveResultToFile(source, 0);
                                    }
                                    catch (Exception ex)
                                    {
                                        //EXCEPTION MESSAGE
                                        MessageBox.Show(ex.Message);
                                    }
                                }
                            }
                            if (item.Equals("PASS"))
                            {
                                UninstallPQAA(i);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ;
                    }
                }
                else
                {
                    //disconnect
                    //_syncContext.Post(SetDutStartPQAA, i);
                }
                Thread.Sleep(100);
            }
            //UNINSTALL GENERIC PQAA
            //UninstallPQAA(i);
            //pushCmd = "adb -s " + mConnectedDut[i].SerialNumber + " uninstall com.wistron.generic.pqaa";
            //Execute(pushCmd);
        }

        private int SaveResultToFile(string source, int index)
        {
            int result = 0;
            //HARD VALUE
            string Version = "V1.4.0";
            string SN = mConnectedDut[index].SerialNumber;
            string Brand = mConnectedDut[index].Brand;
            string Model = mConnectedDut[index].Model;
            string AndroidVersion = mConnectedDut[index].AndroidVersion;
            string IMEI = mConnectedDut[index].IMEI;
            string logDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            // string BuildNumber = mConnectedDut[index].BuildNumber;//bonnie20160805
            //MAKE FILE
            string date = DateTime.Now.ToString("yyyyMMdd");
            //Tiger\\20160428\\
            string subFolder = mConnectedDut[index].Model + "\\" + date + "\\";
            string csvFile = mLogFolder + "\\" + subFolder + date + "_" + SN + ".xlsx";
            ExcelOperaNamespace.MyExcel LocalExcel = new ExcelOperaNamespace.MyExcel();
            bool existed = false;
            if (File.Exists(csvFile))
            {
                //DATA
                LocalExcel.Open(csvFile);
                LocalExcel.OpenSheet("LOG");
                LocalExcel.AddRow(2);
                LocalExcel.AutoRange();
                existed = true;
                //Set TO N/A
                int column = mFileHeader.Length + 1;
                foreach (string item in mFileTestItem)
                {
                    LocalExcel.SetItemText(2, column, "N/A");
                    column++;
                }
            }
            else
            {
                LocalExcel.NewExcel();
                LocalExcel.AddSheet("LOG");
                LocalExcel.AutoRange();
                //ADD
                //mGlobalExcel.SetItemText(1, 1, "PQAA SW");
                int i = 1;
                foreach (string header in mFileHeader)
                {
                    LocalExcel.SetItemText(1, i, header);
                    i++;
                }
                foreach (string item in mFileTestItem)
                {
                    LocalExcel.SetItemText(1, i, item);
                    LocalExcel.SetItemText(2, i, "N/A");
                    i++;
                }
                foreach (string footer in mFileFooter)
                {
                    LocalExcel.SetItemText(1, i, footer);
                    i++;
                }
            }
            //"PQAA SW", "S/N", "Brand", "Model Name", "Android Version",
            //"IMEI", "Date/Time", "Test Time(s)", "AudioLoopback",
            //"BlueTooth","Camera","ConfigChk","Display","MoniPower",
            //"SDCard","TouchPanel","HeadsetLoopback","Vibration","Wifi",
            //"RAM","OTG","GPS","NFC","SIM","Button","ReceiverLoopback",
            //"LED","SensorTest","Result"
            LocalExcel.SetItemText(mExcelStartRow + index, 1, Version);
            LocalExcel.SetItemText(mExcelStartRow + index, 2, "'" + SN);
            LocalExcel.SetItemText(mExcelStartRow + index, 3, Brand);
            LocalExcel.SetItemText(mExcelStartRow + index, 4, Model);
            LocalExcel.SetItemText(mExcelStartRow + index, 5, AndroidVersion);
            LocalExcel.SetItemText(mExcelStartRow + index, 6, "'" + IMEI);
            LocalExcel.SetItemText(mExcelStartRow + index, 7, "'" + logDateTime);
            float totalTimes = 0.0f;
            int failCount = 0;
            //BEGIN TO PARSE LOG FILE
            try
            {
                FileStream fs = new FileStream(source, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string line = string.Empty;
                while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                {
                    string[] itemResult = line.Split('=');
                    string name = itemResult[0].ToString();
                    int res = int.Parse(itemResult[1].ToString());
                    float time = float.Parse(itemResult[2].ToString());
                    totalTimes += time;
                    if (name.Contains("SensorTest"))
                    {
                        string[] sensorResult = name.Split(':');
                        LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("LightSensor"), "PASS");
                        LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("GSensor"), "PASS");
                        LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("ProximitySensor"), "PASS");
                        LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("ECompass"), "PASS");
                        LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("GyroSensor"), "PASS");
                        //LS,GS,PS,EC,GyS
                        foreach (string sensor in sensorResult)
                        {
                            if (sensor.Equals("LS"))
                            {
                                LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("LightSensor"), "FAIL");
                                failCount++;
                            }
                            else if (sensor.Equals("GS"))
                            {
                                LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("GSensor"), "FAIL");
                                failCount++;
                            }
                            else if (sensor.Equals("PS"))
                            {
                                LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("ProximitySensor"), "FAIL");
                                failCount++;
                            }
                            else if (sensor.Equals("EC"))
                            {
                                LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("ECompass"), "FAIL");
                                failCount++;
                            }
                            else if (sensor.Equals("GyS"))
                            {
                                LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("GyroSensor"), "FAIL");
                                failCount++;
                            }
                        }
                    }
                    else if (name.Contains("MultiTest"))
                    {
                        string[] sensorResult = name.Split(':');
                        LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("BlueTooth"), "PASS");
                        LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("Wifi"), "PASS");
                        LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("GPS"), "PASS");
                        //LS,GS,PS,EC,GyS
                        foreach (string sensor in sensorResult)
                        {
                            if (sensor.Equals("BT"))
                            {
                                LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("BlueTooth"), "FAIL");
                                failCount++;
                            }
                            else if (sensor.Equals("WF"))
                            {
                                LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("Wifi"), "FAIL");
                                failCount++;
                            }
                            else if (sensor.Equals("GPS"))
                            {
                                LocalExcel.SetItemText(mExcelStartRow + index, GetColumnMapping("GPS"), "FAIL");
                                failCount++;
                            }
                        }
                    }
                    else
                    {
                        int column = GetColumnMapping(name);
                        switch (res)
                        {
                            case 1:
                                LocalExcel.SetItemText(mExcelStartRow + index, column, "PASS");
                                break;
                            case 2:
                                LocalExcel.SetItemText(mExcelStartRow + index, column, "FAIL");
                                failCount++;
                                break;
                        }
                    }
                }
                sr.Close();
                fs.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine("Exception = " + ex.Message);
            }
            int integerTimes = Convert.ToInt32(totalTimes);
            LocalExcel.SetItemText(mExcelStartRow + index, 8, integerTimes.ToString());
            int lastColumn = mFileHeader.Length + mFileTestItem.Length + mFileFooter.Length;
            if (failCount > 0)
            {
                LocalExcel.SetItemText(mExcelStartRow + index, lastColumn, "FAIL");
            }
            else
            {
                LocalExcel.SetItemText(mExcelStartRow + index, lastColumn, "PASS");
            }
            if (existed)
            {
                LocalExcel.Save();//.SaveAs(csvFile);
            }
            else
            {
                LocalExcel.SaveAs(csvFile);
            }
            LocalExcel.Exit();
            ExcelOperaNamespace.MyExcel.DeleteExcelExe();

            return result;
        }

        private int GetColumnMapping(string item)
        {
            int result = mFileHeader.Length + 1;
            foreach (string testItem in mFileTestItem)
            {
                if (testItem.Equals(item))
                {
                    break;
                }
                result++;
            }
            return result;
        }

        private void UninstallPQAA(int id)
        {
            if (id >= 0)
            {
                string pushCmd = "adb -s " + mConnectedDut[id].SerialNumber + " uninstall com.wistron.generic.pqaa";
                Execute(pushCmd, true);



                //since move folder to package, no need remove config folder @20160222 BillGe
                //pushCmd = "adb -s " + mConnectedDut[id].SerialNumber + " shell rm -rf " + CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
                //Execute(pushCmd);
            }
        }

        public async Task<string> Execute(string dosCommand, bool NeedResponseResult = true)
        {
            return await cmd.CMD_RunAsync(dosCommand, 0, NeedResponseResult);
        }
        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfiguration cfg = new FormConfiguration();
            cfg.ShowDialog();
            this.label1.Text = "Load CFG from <" + cfg.mCfgFolder + ">";
            mCfgFolder = cfg.mCfgFolder;
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help about = new Help();
            if (mLicensed)
            {
                about.setLicensedKey(mLicenseKey);
            }
            about.ShowDialog();
        }

        private void OnMinimumWindow(System.Object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void startTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mCount = 0;
            DoControlTest();
        }

        private void startSelectedAndroidDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //mCount = 0;
            //DoControlTest(true);
            TestSelectedDUT();
        }

        private int GetCheckedDUT()
        {
            int count = 0, i = 0;
            foreach (UserGridClassLibrary.GridItem ctrl in mDuts)
            {
                if (ctrl.IsDutSelected() &&
                    !string.IsNullOrEmpty(mConnectedDut[i].SerialNumber) &&
                    mConnectedDut[i].Connected)
                {
                    count++;
                }
                i++;
            }
            return count;
        }

        private void TestSelectedDUT()
        {
            if (GetCheckedDUT() > 0)
            {
                //RESET TIME
                foreach (DutDevice terminal in mConnectedDut)
                {
                    terminal.BenginTime = DateTime.Now;
                }
                //CHECK DEVICES
                foreach (UserGridClassLibrary.GridItem dut in mDuts)
                {
                    if (dut.IsDutSelected())
                    {
                        int idx = dut.GetDutGridID();
                        if (!string.IsNullOrEmpty(mConnectedDut[idx].SerialNumber) && mConnectedDut[idx].Connected)
                        {
                            mConnectedDut[idx].ExitRunningThread = true;
                            dut.Reset();
                        }
                    }
                }
                //CHECK DEVICES 
                //END
                string computer = common.GetComputerName();
                string md5Code = string.Empty;
                //md5Code = common.GetMD5Code("wortsin");
                //MessageBox.Show(computer);
                if (mLicensed)
                {
                    md5Code = common.GetMD5Code("wortsin");
                }
                else
                {
                    md5Code = common.GetMD5Code(computer);
                }
                mGlobalmd5Code = md5Code;
                //MD5.txt
                common.CreateFile("md5.txt", md5Code);
                //Computer.txt
                common.CreateFile("computer.txt", computer);

                //BEGIN LOOP
                int i = 0;
                foreach (UserGridClassLibrary.GridItem ctrl in mDuts)
                {
                    if (ctrl.IsDutSelected())
                    {
                        i = ctrl.GetDutGridID();
                        if (!string.IsNullOrEmpty(mConnectedDut[i].SerialNumber) &&
                            mConnectedDut[i].Connected)
                        {
                            //START TEST
                            Thread th = new Thread(new ParameterizedThreadStart(ThreadMethod));
                            th.Start(i); //启动线程
                        }
                    }
                    //i++;
                }
            }
            else
            {
                MessageBox.Show("No Selected or Valid DUT Client");
            }
        }

        private void modelMappingConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMapping mapping = new FormMapping();
            mapping.ShowDialog();
            mConfigPath = new DataSet("ConfigData");
            mConfigPath.ReadXml("ConfigData.xml");
        }

        private void checkBoxModel_CheckedChanged(object sender, EventArgs e)
        {
            bMultiModelSupport = this.checkBoxModel.Checked;
            if (bMultiModelSupport)
            {
                //this.label1.Visible = false;
                this.label1.Text = "Auto download CFG by DUT model";
                this.configurationToolStripMenuItem.Enabled = false;
                this.modelMappingConfigurationToolStripMenuItem.Enabled = true;
            }
            else
            {
                //this.label1.Visible = true;
                this.label1.Text = "Load CFG from <" + mCfgFolder + ">";
                this.configurationToolStripMenuItem.Enabled = true;
                this.modelMappingConfigurationToolStripMenuItem.Enabled = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mUSB.RemoveUSBEventWatcher();
            IniFile mIni = new IniFile(Application.StartupPath + "\\cfg.ini");
            if (bMultiModelSupport)
            {
                mIni.IniWriteValue("mode", "MultiSupport", "1");
            }
            else
            {
                mIni.IniWriteValue("mode", "MultiSupport", "0");
            }
            //bTerminateThread = true;
            foreach (DutDevice dut in mConnectedDut)
            {
                dut.ExitRunningThread = true;
            }
            if (mOpaqueLayer != null)
            {
                mOpaqueLayer.EventWaitHandle[0].Reset();
                mOpaqueLayer.EventWaitHandle[1].Set();
            }
        }

        //Round corner win form
        public void SetWindowRegion()
        {
            System.Drawing.Drawing2D.GraphicsPath FormPath;

            FormPath = new System.Drawing.Drawing2D.GraphicsPath();

            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);//this.Left-10,this.Top-10,this.Width-10,this.Height-10);                 

            FormPath = GetRoundedRectPath(rect, 20);

            this.Region = new Region(FormPath);

        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {

            int diameter = radius;

            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            GraphicsPath path = new GraphicsPath();

            //   左上角   
            path.AddArc(arcRect, 180, 90);

            //   右上角   
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            //   右下角   
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);

            //   左下角   
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();

            return path;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetWindowRegion();
        }

        //C++ PART
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private const int WS_MINIMIZEBOX = 0x20000;
        private const int CS_DBLCLKS = 0x8;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                //cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                //cp.ExStyle |= 0x02000000; //用双缓冲绘制窗口的所有子控件 
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            BufferedGraphics bg = mContext.Allocate(this.CreateGraphics(), this.ClientRectangle);
            DrawBackground(bg);
            /*
            if (mImageBackground != null)
            {
                using (Graphics g = Graphics.FromImage(mImageBackground))
                {
                    g.Clear(System.Drawing.Color.White);
                    bg.Render(g);
                    mDuts.mImageBackground = mImageBackground;
                }
            }
             * */
            DrawTitleBar(bg);
            bg.Render(e.Graphics);
            bg.Dispose();
        }

        private void DrawBackground(BufferedGraphics bg)
        {
            Graphics g = bg.Graphics;
            g.Clear(System.Drawing.Color.White);
            Rectangle rectBackground = new Rectangle(0, 0, Width, Height);
            // Creating the lineargradient
            LinearGradientBrush bBackground = new LinearGradientBrush(rectBackground, System.Drawing.Color.Gainsboro, System.Drawing.Color.White, 90.0f);
            g.FillRectangle(bBackground, rectBackground);
            bBackground.Dispose();
            StringFormat stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.FormatFlags = StringFormatFlags.LineLimit;
            stringFormat.Trimming = StringTrimming.EllipsisCharacter;
            Rectangle rect = ClientRectangle;
            rect.Offset(0, -60);
            //g.DrawString(mTickCount.ToString(), mFontBold, Brushes.Black, rect, stringFormat);
            g.DrawLine(Pens.DarkGray, 0, 0, Width, 0);
            g.DrawLine(Pens.DarkGray, 0, 0, 0, Height);
            g.DrawLine(Pens.DarkGray, Width - 1, 0, Width - 1, Height);
            g.DrawLine(Pens.DarkGray, 0, Height - 1, Width - 1, Height - 1);
        }

        private void DrawTitleBar(BufferedGraphics bg)
        {
            Graphics g = bg.Graphics;
            Rectangle rect = new Rectangle(1, 1, Width - 2, mTitleBarHeight);
            g.FillRectangle(new SolidBrush(mTitleColor), rect);
            g.DrawLine(Pens.DarkGray, 0, mTitleBarHeight, Width, mTitleBarHeight);
            StringFormat stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.FormatFlags = StringFormatFlags.LineLimit;
            stringFormat.Trimming = StringTrimming.EllipsisCharacter;

            g.DrawString("Wistron Multi Android Control Test", mFontCaption, Brushes.White, rect, stringFormat);
        }

        private BufferedGraphicsContext mContext = null;
        private int mTitleBarHeight = 28;

        private Font mFontCaption;
        private Font mFontBold;
        private System.Drawing.Color mTitleColor;

        [DllImport("user32.dll", SetLastError = true)]
        private extern static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        //窗体关闭消息
        private const uint WM_CLOSE = 0x0010;
        private void OnCloseWindow(System.Object sender, System.EventArgs e)
        {
            //System.Environment.Exit(System.Environment.ExitCode);
            SendMessage(this.Handle, WM_CLOSE, 0, 0);
            //this.Dispose();
            //this.Close();
        }


        private void buttonUserControlLoad_Click(object sender, EventArgs e)
        {

        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            if (mOpaqueLayer != null)
            {
                //mOpaqueLayer.Visible = false;
                mOpaqueLayer.CurrentKey = -1;
            }
        }

        private void OnGridItemCheckedHandle(object sender, UserGridClassLibrary.UserGrid.CheckedEventArgs args)
        {
        }

        private void OnGridItemRightClicked(object sender, UserGridClassLibrary.UserGrid.RightClickedEventArgs args)
        {
        }

        private void OnGridItemResetHandle(object sender, UserGridClassLibrary.GridItem.ResetEventArgs args)
        {
            if (mOpaqueLayer != null)
            {
                mOpaqueLayer.ClearResultList(args.ID);
            }
        }

        private void OnGridItemFocusHandle(object sender, UserGridClassLibrary.UserGrid.FocusEventArgs args)
        {
            Point location = new Point();
            if (sender is UserGridClassLibrary.UserGrid)
            {
                location = ((UserGridClassLibrary.UserGrid)sender).PointToScreen(new Point(args.X, args.Y));
            }
            //Cal width and height;
            int currentKey = args.Index;
            Point point = this.PointToClient(location);
            //UPDATE OPAQUELAYER POSITION
            if (location.Y + mOpaqueLayer.Height > Screen.PrimaryScreen.WorkingArea.Height)
            {
                int offset = location.Y + mOpaqueLayer.Height - Screen.PrimaryScreen.WorkingArea.Height;
                location.Y -= offset;
                mOpaqueLayer.Location = location;
            }
            else
            {
                mOpaqueLayer.Location = location;
            }
            //Console.WriteLine("{0} - {1}", currentKey.ToString(), mOpaqueLayer.CurrentKey.ToString());
            if (currentKey != mOpaqueLayer.CurrentKey)
            {
                mOpaqueLayer.CurrentKey = currentKey;
            }
        }

        private void logOutputConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger logger = new Logger();
            logger.ShowDialog();
            mLogFolder = logger.mLogFolder;
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormScriptSupport script = new FormScriptSupport();
            script.ShowDialog();
        }

        private void phoneSDCardPathConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSDCard sdcard = new FormSDCard();
            sdcard.ShowDialog();
            mConfigSDCard = new DataSet("ConfigPath");
            mConfigSDCard.ReadXml("ConfigPath.xml");
        }

        private async void configuratToSysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = 0;
            string result = await Execute("adb devices");

            if (result == "List of devices attached \r\n\r\n")
            { }
            string[] devices = Regex.Split(result, "\r\n", RegexOptions.IgnoreCase);
            //MessageBox.Show(result);
            foreach (string device in devices)
            {
                if (device.Contains("\tdevice"))
                {
                    string[] serials = Regex.Split(device, "\t", RegexOptions.IgnoreCase);
                    //mDevice[mCount] = serials[0];
                    mConnectedDut[0].SerialNumber = serials[0];
                    saveMSD.mNumber = serials[0];
                    count = 1;
                    break;
                }

            }
            if (count == 0)
            {
                MessageBox.Show("No Connected DUT ");
            }
            else
            {
                if (mConfigSDCard.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in mConfigSDCard.Tables[0].Rows)
                    {
                        //SD chance path @20160518
                        string verify = row[1].ToString();
                        string pullCmd = "adb -s " + mConnectedDut[0].SerialNumber + " pull " + verify + SPECIFIC_TAG_PATH;
                        string console = await Execute(pullCmd);


                        if (console.Contains("remote object"))
                        {
                            continue;
                        }
                        else
                        {
                            saveMSD.SD = verify;
                            // mConnectedDut[i].SDCard = verify;
                            // result = mConnectedDut[i].SDCard;
                            //delete path.verify.pass file before break
                            if (File.Exists("path.verify.pass"))
                            {
                                File.Delete("path.verify.pass");
                            }
                            break;
                        }
                    }
                }
                string pushCmd = "adb -s " + mConnectedDut[0].SerialNumber + " install -r configcheck.apk";
                string x2 = await Execute(pushCmd);
                string startCmd = "adb -s " + mConnectedDut[0].SerialNumber + " shell am start -n com.wistron.get.config.information/.MainActivity";
                string x = await Execute(startCmd);




                FormConfigurateSysinfoLocation cfg = new FormConfigurateSysinfoLocation();
                cfg.ShowDialog();
                mLogFolder = cfg.mLogFolder;
            }

            // MessageBox.Show("No Connected DUT");          

        }
    }
}

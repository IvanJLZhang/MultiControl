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
using ExcelOperaNamespace;
using LogHelper;
using LibUsbDotNet.DeviceNotify;
using MultiControl.Functions;
using LibUsbDotNet;

namespace MultiControl
{
    public partial class Form1 : Form, INotifyPropertyChanged
    {
        #region 全局变量

        StringBuilder TestLog = new StringBuilder();
        private DutDevice[] mConnectedDut;
        private int mRows, mCols;


        public string mCfgFolder = string.Empty;
        public string mLogFolder = string.Empty;
        private DataSet mConfigPath;
        private DataSet mConfigSDCard;
        private bool bMultiModelSupport = true;

        private UserButton mButtonClose = null;
        private UserButton mButtonAbout = null;
        private UserButton mButtonMin = null;
        private string mGlobalmd5Code = string.Empty;

        private string mLicenseKey = string.Empty;
        private bool mLicensed = false;

        private OpaqueForm mOpaqueLayer = new OpaqueForm();
        private int mPseudoTotal = 0;
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
        /// <summary>
        /// USB插拔事件通知接口
        /// </summary>
        IDeviceNotifier UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
        /// <summary>
        /// 是否允许进行端口注册
        /// </summary>
        bool IsEnabledIndexRegister
        {
            get
            {
                return PortToIndexFactory.IsEnabled;
            }
            set
            {
                PortToIndexFactory.IsEnabled = value;
            }
        }
        /// <summary>
        /// 端口注册表操作对象
        /// </summary>
        //PortToIndexFactory m_PortToIndexFactory;
        #endregion

        #region Initilize/Navigation methods
        public Form1()
        {
            InitializeComponent();

            MyExcel.DeleteExcelExe();
            common.DeleteConhostExe();
            // cmd操作类初始化
            cmd = new CMDHelper();
            common.m_log = new LogMsg(Application.StartupPath);
            Boolean.TryParse(ConfigurationHelper.ReadConfig("DebugLogEnabled"), out common.m_log.IsEnable);

            // 注册USB设备插入/拔出事件
            UsbDeviceNotifier.OnDeviceNotify += UsbDeviceNotifier_OnDeviceNotify;

            //m_PortToIndexFactory = new PortToIndexFactory();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width - 32, Screen.PrimaryScreen.WorkingArea.Height - 25);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 加载cfg参数
            if (!LoadCfg())
            {
                this.Close();
            }
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
            this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width - 32, Screen.PrimaryScreen.WorkingArea.Height - 26);
        }

        private async void UsbDeviceNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            switch (e.EventType)
            {
                case EventType.CustomEvent:
                    break;
                case EventType.DeviceArrival:
                    if (e.Device != null)
                    {
                        UsbDeviceInfoEx device = new UsbDeviceInfoEx();
                        device.SerialNumber = e.Device.SerialNumber;

                        if (mConnectedDut.Contains(new DutDevice() { SerialNumber = device.SerialNumber }, DutDevice.Default))
                        {
                            int index = common.IndexDevice(mConnectedDut, device.SerialNumber);
                            mConnectedDut[index].Connected = true;
                            break;
                        }
                        UsbDeviceFactory usb = new UsbDeviceFactory();
                        string usb_port = await usb.FindArrivaledDevice(e.Device.SerialNumber);
                        device.Port_Path = usb_port;
                        //if (IsEnabledIndexRegister)
                        //{
                        //    if (device.Port_Path == String.Empty)
                        //    {
                        //        common.m_log.Add("can not read port number.");
                        //        break;
                        //    }
                        //    // 检查端口配置情况
                        //    int index = m_PortToIndexFactory.GetIndex(device.Port_Path);
                        //    if (index <= -1)
                        //    {
                        //        PortToIndexForm portForm = new PortToIndexForm(device.Port_Path);
                        //        if (portForm.ShowDialog() == DialogResult.OK)
                        //            index = portForm.Index;
                        //    }
                        //    device.Index = index - 1;
                        //}
                        DeviceArrival(device);
                    }
                    break;
                case EventType.DeviceQueryRemove:
                    break;
                case EventType.DeviceQueryRemoveFailed:
                    break;
                case EventType.DeviceRemoveComplete:
                    {
                        if (e.Device != null)
                        {
                            UsbDeviceInfoEx device = new UsbDeviceInfoEx();
                            device.SerialNumber = e.Device.SerialNumber;
                            DeviceRemoved(device);
                        }
                    }
                    break;
                case EventType.DeviceRemovePending:
                    break;
                case EventType.DeviceTypeSpecific:
                    break;
                default:
                    break;
            }
        }
        private bool LoadCfg()
        {
            string iniCfgFile = Application.StartupPath + @"\cfg.ini";
            if (!File.Exists(iniCfgFile))
            {
                MessageBox.Show("Error: can not find cfg.ini file.");
                common.m_log.Add("Error: can not find cfg.ini file.");
                return false;
            }
            IniFile mIni = new IniFile(iniCfgFile);
            mCfgFolder = mIni.IniReadValue("config", "Folder");
            mLogFolder = mIni.IniReadValue("config", "Logger");
            if (!Directory.Exists(mLogFolder))
            {
                MessageBox.Show("Error: can not find log folder.");
                logOutputConfigurationToolStripMenuItem_Click(null, null);
            }
            mRows = int.Parse(mIni.IniReadValue("layout", "Row").ToString());
            mCols = int.Parse(mIni.IniReadValue("layout", "Column").ToString());

            //LOAD CFG TABLE
            mConfigPath = new DataSet("ConfigData");
            mConfigPath.ReadXml("ConfigData.xml");

            mConfigSDCard = new DataSet("ConfigPath");

            if (!File.Exists("ConfigPath.xml"))
            {
                MessageBox.Show("Error: can not find ConfigPath.xml file.");
                common.m_log.Add("Error: can not find ConfigPath.xml file.");
                return false;
            }
            mConfigSDCard.ReadXml("ConfigPath.xml");

            int multiSupport = int.Parse(mIni.IniReadValue("mode", "MultiSupport").ToString());
            if (multiSupport == 1)
            {
                bMultiModelSupport = true;
            }
            else
            {
                bMultiModelSupport = false;
                if (!Directory.Exists(mCfgFolder))
                {
                    MessageBox.Show("Error: can not find cfg folder.");
                    common.m_log.Add("Error: can not find ConfigPath.xml file.");
                    configurationToolStripMenuItem_Click(null, null);
                }
            }
            if (bMultiModelSupport)
            {
                this.label1.Text = "Auto download CFG by DUT model";
                this.configurationToolStripMenuItem.Enabled = false;
                this.modelMappingConfigurationToolStripMenuItem.Enabled = true;
                this.checkBoxModel.Checked = true;
            }
            else
            {
                this.label1.Text = "Load CFG from <" + mCfgFolder + ">";
                this.configurationToolStripMenuItem.Enabled = true;
                this.modelMappingConfigurationToolStripMenuItem.Enabled = false;
                this.checkBoxModel.Checked = false;
            }
            return true;
        }
        #region .net framework 小于4.0时使用此方法
        private void Cmd_Exited(object sender, ProcessExitAgs e)
        {
            common.m_log.Add_Debug($"{e.Command} exited");
        }

        #endregion
        private void DeviceArrival(UsbDeviceInfoEx device)
        {
            if (IsEnabledIndexRegister)
            {
                if (device.Index <= -1)
                    return;
                var indexDevice = mConnectedDut[device.Index];
                if (String.IsNullOrEmpty(indexDevice.SerialNumber))
                {
                    TestNewDevice_RegisterIndex(device);
                }
                else
                {
                    indexDevice.Connected = true;
                }
            }
            else
            {
                if (mConnectedDut.Contains(new DutDevice() { SerialNumber = device.SerialNumber }, DutDevice.Default))
                {
                    int index = common.IndexDevice(mConnectedDut, device.SerialNumber);
                    mConnectedDut[index].Connected = true;
                }
                else
                {
                    TestNewDevice_UnRegisterIndex(device);
                }
            }
        }
        private void DeviceRemoved(UsbDeviceInfoEx device)
        {
            for (int index = 0; index < mConnectedDut.Length; index++)
            {
                var connected_device = mConnectedDut[index];
                if (device.SerialNumber == connected_device.SerialNumber)
                {
                    switch (connected_device.TestResult)
                    {
                        case DutDevice.DutResult.DR_PASS:
                            connected_device.Reset();
                            mDuts[index].Reset();
                            break;
                        case DutDevice.DutResult.DR_FAIL:
                            connected_device.ExitRunningThread = true;
                            connected_device.Reset();
                            mDuts[index].Reset();
                            break;
                        case DutDevice.DutResult.DR_NONE:
                        case DutDevice.DutResult.DR_TESTING:
                        default:
                            connected_device.Connected = false;
                            break;
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// 不需要注册端口的测试
        /// </summary>
        /// <param name="device"></param>
        async void TestNewDevice_UnRegisterIndex(UsbDeviceInfoEx device)
        {
            //CHECK DEVICES
            int index = FindAvaiableDUTIndex();
            if (index == -1)
            {
                common.m_log.Add("No extra room for a new test.", MessageType.ERROR);
                return;
            }
            bool adb_avaiable = await cmd.CheckDeviceConnection(device);
            if (!adb_avaiable)
            {
                common.m_log.Add($"adb: can not connect to this device:SN: {device.SerialNumber}; Port: {device.Port_Path}", MessageType.ERROR);
                return;
            }
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
            mConnectedDut[index].SerialNumber = device.SerialNumber;
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

        async void TestNewDevice_RegisterIndex(UsbDeviceInfoEx device)
        {
            if (device.Index <= -1)
            {
                common.m_log.Add("No extra room for a new test.", MessageType.ERROR);
                return;
            }
            bool adb_avaiable = await cmd.CheckDeviceConnection(device);
            if (!adb_avaiable)
            {
                common.m_log.Add($"adb: can not connect to this device:SN: {device.SerialNumber}; Port: {device.Port_Path}", MessageType.ERROR);
                return;
            }
            int index = device.Index;
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
            mConnectedDut[index].SerialNumber = device.SerialNumber;
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

        async void btn_StartTest()
        {
            //RESET TIME
            foreach (DutDevice terminal in mConnectedDut)
            {
                terminal.BenginTime = DateTime.Now;
                terminal.SerialNumber = String.Empty;
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
            await CMDHelper.Adb_KillServer();
            await Task.Delay(config_inc.CMD_REPEAT_WAIT_TIME);
            // 开始测试
            //m_PortToIndexFactory = new PortToIndexFactory();
            //UsbDeviceFactory device_factory = new UsbDeviceFactory();
            //var device_list = await device_factory.GetAllDevices();
            //foreach (var device in device_list)
            //{
            //    if (IsEnabledIndexRegister)
            //    {
            //        if (device.Port_Path == String.Empty)
            //        {
            //            common.m_log.Add("can not read port number.");
            //            continue;
            //        }
            //        // 检查端口配置情况
            //        int index = m_PortToIndexFactory.GetIndex(device.Port_Path);
            //        if (index <= -1)
            //        {
            //            PortToIndexForm portForm = new PortToIndexForm(device.Port_Path);
            //            if (portForm.ShowDialog() == DialogResult.OK)
            //                index = portForm.Index;
            //        }
            //        device.Index = index - 1;
            //    }
            //    DeviceArrival(device);
            //}
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

        private async Task<string> QueryModelConfigSDCardFromDataSet(int index, string destPath)
        {
            string result = string.Empty;
            mConnectedDut[index].SDCard = string.Empty;

            string dstfile = $@"{destPath}\{config_inc.PATH_VERIFY_PATH}";
            foreach (DataRow sdPath in mConfigSDCard.Tables[0].Rows)
            {
                //SD chance path @20160518
                string verify = sdPath[1].ToString();
                string pullCmd = "adb -s " + mConnectedDut[index].SerialNumber + " pull " + verify + config_inc.SPECIFIC_TAG_PATH + " \"" + dstfile + "\"";
                string ret = await Execute(pullCmd);
                await Task.Delay(50);
                if (File.Exists(dstfile))
                {
                    result = mConnectedDut[index].SDCard = verify;
                    File.Delete(dstfile);
                    break;
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
            //2/1/2/SensorTest
            string value = progress.ToString();
            string[] sArray = value.Split('/');
            int index = int.Parse(sArray[0].ToString());
            int current = int.Parse(sArray[1].ToString());
            int total = int.Parse(sArray[2].ToString());
            string item = sArray[3].ToString();
            if (this.ProgressUpdate != null)
            {
                TestProgressEventArgs args = new TestProgressEventArgs();
                args.ResultIndex = index;
                args.Current = current;
                args.Total = total;
                args.Item = item;
                ProgressUpdate(this, args);
            }
            //UPDATE RESULT ITEM
            if (this.ResultUpdate != null)
            {
                string log_date = DateTime.Now.ToString("yyyyMMdd");
                string result_file = $@"{mLogFolder}\{mConnectedDut[index].Model}\{log_date}\{mConnectedDut[index].SerialNumber}_result.txt";
                ResultEventArgs args = new ResultEventArgs(index, result_file);
                args.Current = current;
                args.Total = total;
                args.Item = item;
                args.Model = mConnectedDut[index].Model;
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
                File.Delete(resultName);
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
            int ThreadIndex = (int)index;

            #region 检测相关路径
            string log_date = DateTime.Now.ToString("yyyyMMdd");
            string logDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            DirectoryInfo log_folder = new DirectoryInfo(mLogFolder);
            if (!Directory.Exists(mLogFolder))
            {
                common.m_log.Add($"log folder can not be found:{mLogFolder}", MessageType.ERROR);

                return;
            }
            DirectoryInfo log_model_path = new DirectoryInfo(log_folder.FullName + $@"\{mConnectedDut[ThreadIndex].Model}");
            if (!log_model_path.Exists)
                log_model_path.Create();

            DirectoryInfo log_model_date_path = new DirectoryInfo(log_model_path.FullName + $@"\{log_date}");
            if (!log_model_date_path.Exists)
            {
                log_model_date_path.Create();
            }
            #endregion

            string response = String.Empty;
            mDuts[ThreadIndex].Result = UserGridClassLibrary.ItemResult.IR_TESTING;
            SetDutStatusInvoke(ThreadIndex);
            SetDutModel(ThreadIndex);
            mDuts[ThreadIndex].EstimateTime = mConnectedDut[ThreadIndex].Estimate;

            string pushCmd = string.Empty;
            pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " install -r Generic_PQAA.apk";
            response = await Execute(pushCmd, true);



            SetDutInstallPQAAInvoke(ThreadIndex);

            pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " shell am startservice --user 0 -a com.wistron.generic.get.sdcard.path";
            var result = await Execute(pushCmd, true);

            int count = 0;
            string sdcard = await QueryModelConfigSDCardFromDataSet(ThreadIndex, log_model_date_path.FullName);
            while (String.IsNullOrEmpty(sdcard) && count <= config_inc.CMD_REPEAT_MAX_TIME)
            {
                sdcard = await QueryModelConfigSDCardFromDataSet(ThreadIndex, log_model_date_path.FullName);
                count++;
                common.m_log.Add_Debug($"{mConnectedDut[ThreadIndex].SerialNumber}: can not find sd card path, try again. {count++}", MessageType.ERROR);

                await Task.Delay(300);
            }
            if (string.IsNullOrEmpty(sdcard))
            {
                common.m_log.Add($"{mConnectedDut[ThreadIndex].SerialNumber}: Test fail: No available SD card for testing...");
                SetDutTestFinishInvoke($@"{ThreadIndex}/0/0/FAIL");
                return;
            }

            #region push测试项config文件
            pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push md5.txt " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_ROOT; // /mnt/sdcard/";
            await Execute(pushCmd, true);

            SetDutMD5Invoke(ThreadIndex);
            //------START-------------------------------------------
            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mConnectedDut[ThreadIndex].ConfigPath +
                    "\\" + "audio_loopback.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mCfgFolder +
                "\\" + "audio_loopback.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);
            //cmd.CMD_Run(pushCmd);

            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mConnectedDut[ThreadIndex].ConfigPath +
                    "\\" + "monipower.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "monipower.cfg \"" + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);



            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mConnectedDut[ThreadIndex].ConfigPath +
                    "\\" + "sysinfo.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "sysinfo.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);


            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mConnectedDut[ThreadIndex].ConfigPath +
                    "\\" + "wifi.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "wifi.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);
            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mConnectedDut[ThreadIndex].ConfigPath +
                    "\\" + "gps.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "gps.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);
            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mConnectedDut[ThreadIndex].ConfigPath +
                    "\\" + "headsetloopback.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "headsetloopback.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);
            if (bMultiModelSupport)
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mConnectedDut[ThreadIndex].ConfigPath +
                    "\\" + "pqaa.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            else
            {
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " push \"" + mCfgFolder +
                    "\\" + "pqaa.cfg\" " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_PQAA; // /mnt/sdcard/pqaa_config";
            }
            await Execute(pushCmd, true);
            //--------------END--------------------------------------

            DateTime dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - mConnectedDut[ThreadIndex].BenginTime;// dtStart;
            float _cost = 0.0f;
            _cost = (float)ts.TotalMilliseconds / 1000;
            //mConnectedDut[i].InstallTime = _cost;
            mDuts[ThreadIndex].InstallTime = _cost;
            //ADD END---------------------------------------------------------
            string md5String = mGlobalmd5Code;
            pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " shell am start -n com.wistron.generic.pqaa/.TestItemsList --ei block " + (ThreadIndex + 1).ToString() + " --ei autostart 1" + " --es md5code " + md5String;
            response = await Execute(pushCmd, true);

            SetDutStartPQAAInvoke(ThreadIndex);
            #endregion

            #region pull wInfo文件并更新界面设备相关信息
            //IMEI
            string wInfo = string.Empty;
            string dutInfo = mConnectedDut[ThreadIndex].SerialNumber + "_wInfo.txt";
            dutInfo = log_model_date_path.FullName + @"\" + dutInfo;
            wInfo = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " pull " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_ROOT + "wInfo.txt \"" + dutInfo + "\"";

            count = 0;
            while (!File.Exists(dutInfo) && count <= config_inc.CMD_REPEAT_MAX_TIME)
            {
                string str = await Execute(wInfo);
                await Task.Delay(300);
                count++;
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
                mConnectedDut[ThreadIndex].IMEI = imei;
                mConnectedDut[ThreadIndex].RAM = ram;
                mConnectedDut[ThreadIndex].FLASH = flash;
                mConnectedDut[ThreadIndex].BuildNumber = buildNO;//bonnie20160805

                SetDutIMEIInvoke(ThreadIndex);
                File.Delete(dutInfo);
            }
            #endregion

            #region 追踪测试进度信息
            string pullCmd = string.Empty;
            string progress_file = mConnectedDut[ThreadIndex].SerialNumber + "_progress.txt";
            progress_file = log_model_date_path.FullName + @"\" + progress_file;

            mConnectedDut[ThreadIndex].ExitRunningThread = false;
            int walkedIndex = -1;
            while (!mConnectedDut[ThreadIndex].ExitRunningThread)
            {
                #region 实时追踪测试进度信息
                //check this DUT connect or disconnect to quit!
                // 追踪device连接状态
                if (!mConnectedDut[ThreadIndex].Connected)
                {
                    Thread.Sleep(300);
                    SetDutDisconnected(ThreadIndex);
                    walkedIndex = -1;
                    continue;
                }
                SetDutConnected(ThreadIndex);
                // pull progress文件
                pullCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " pull " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_ROOT + "progress.txt \"" + progress_file + "\"";
                await Execute(pullCmd);

                string progressStr = string.Empty;
                string testItem = string.Empty;
                if (File.Exists(progress_file))
                {
                    //1. FILE read exception BUG found @20151204 ->delay to fix
                    //2. ADB server kill BUG found @20151204
                    //3. Exception BUG while remove device during install PQAA foud @20151204
                    Thread.Sleep(100);
                    try
                    {
                        string[] progress_context = File.ReadAllLines(progress_file);

                        await Task.Delay(300);
                        File.Delete(progress_file);
                        if (progress_context.Length >= 1)
                            progressStr = progress_context[0].Trim();
                        if (progress_context.Length >= 2)
                            testItem = progress_context[1].Trim();
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"read progress file error: {ex.Message}");
                    }
                }
                if (String.IsNullOrEmpty(progressStr))
                {
                    Thread.Sleep(500);
                    continue;
                }

                //PULL test result
                string result_file = mConnectedDut[ThreadIndex].SerialNumber + "_result.txt";
                result_file = log_model_date_path + @"\" + result_file;
                pullCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " pull " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_ROOT + "result.txt \"" + result_file + "\"";
                await Execute(pullCmd);

                //UPDATE TEST PROGRESS
                string value = ThreadIndex + "/" + progressStr + "/" + testItem;
                common.m_log.Add_Debug(value);
                string[] progress_arr = progressStr.Split('/');
                int progress_current = int.Parse(progress_arr[0].ToString());
                int progress_total = int.Parse(progress_arr[1].ToString());

                if (walkedIndex != progress_current)
                {
                    SetDutTestProgressInvoke(value);// 更新界面进度
                    walkedIndex = progress_current;
                }
                if (progress_current < progress_total)
                {
                    Thread.Sleep(300);
                    continue;
                }
                #endregion
                // 测试结束, 处理测试结果
                SetDutTestFinishInvoke(value);

                //下达删除progress.txt命令， 同时删除本地文件
                string delCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " shell rm " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_ROOT + "progress.txt";
                await Execute(delCmd);
                if (File.Exists(progress_file))
                {
                    File.Delete(progress_file);
                }

                //PULL test result
                result_file = mConnectedDut[ThreadIndex].SerialNumber + "_result.txt";
                result_file = log_model_date_path + @"\" + result_file;
                pullCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " pull " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_ROOT + "result.txt \"" + result_file + "\"";
                await Execute(pullCmd);

                count = 0;// 循环执行CMD_REPEAT_MAX_TIME次， 如果还是抓不到result文件， 则路过处理result文件步骤
                while (!File.Exists(result_file) && count <= config_inc.CMD_REPEAT_MAX_TIME)
                {
                    Thread.Sleep(300);
                    await Execute(pullCmd);
                    count++;
                }

                //下达删除result.txt命令， 本地保存log
                delCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " shell rm " + mConnectedDut[ThreadIndex].SDCard + config_inc.CFG_FILE_ROOT + "result.txt";
                await Execute(delCmd);

                //if (testItem.Equals("PASS"))
                //{// 测试通过以后push卸载pqaa指令， 并退出测试
                pushCmd = "adb -s " + mConnectedDut[ThreadIndex].SerialNumber + " uninstall com.wistron.generic.pqaa";
                await Execute(pushCmd, true);
                //break;
                //}
                mConnectedDut[ThreadIndex].ExitRunningThread = true;
                Thread.Sleep(100);
                SaveResultToFile(result_file, ThreadIndex);
            }
            #endregion
        }

        private int SaveResultToFile(string source, int index)
        {
            string log_date = DateTime.Now.ToString("yyyyMMdd");
            string logDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            DirectoryInfo log_folder = new DirectoryInfo(mLogFolder);
            if (!Directory.Exists(mLogFolder))
            {
                common.m_log.Add($"log folder can not be found:{mLogFolder}");
                return -1;
            }
            DirectoryInfo log_model_path = new DirectoryInfo(log_folder.FullName + $@"\{mConnectedDut[index].Model}");
            if (!log_model_path.Exists)
                log_model_path.Create();

            DirectoryInfo log_model_date_path = new DirectoryInfo(log_model_path.FullName + $@"\{log_date}");
            if (!log_model_date_path.Exists)
            {
                log_model_date_path.Create();
            }
            if (!File.Exists(source))
                return -1;

            string result_file = source;//;log_model_date_path.FullName + $@"\{source}";
            //File.Copy(source, result_file, true);
            //File.Delete(source);
            FileInfo result_xls_file = new FileInfo(log_model_date_path.FullName + "\\" + mConnectedDut[index].SerialNumber + ".xlsx");

            DataTable tbl_result = new DataTable();
            foreach (string header in config_inc.mFileHeader)
            {
                if (!tbl_result.Columns.Contains(header))
                    tbl_result.Columns.Add(header);
            }
            foreach (string item in config_inc.mFileTestItem)
            {
                if (!tbl_result.Columns.Contains(item))
                    tbl_result.Columns.Add(item);
            }
            foreach (string footer in config_inc.mFileFooter)
            {
                if (!tbl_result.Columns.Contains(footer))
                    tbl_result.Columns.Add(footer);
            }
            DataRow newrow = tbl_result.NewRow();

            foreach (DataColumn col in tbl_result.Columns)
            {
                newrow[col.Caption] = "N/A";// 默认设为N/A
            }
            // 纪录设备信息
            newrow[0] = config_inc.PQAA_SW_VERSION;
            newrow[1] = mConnectedDut[index].SerialNumber;
            newrow[2] = mConnectedDut[index].Brand;
            newrow[3] = mConnectedDut[index].Model;
            newrow[4] = mConnectedDut[index].AndroidVersion;
            newrow[5] = mConnectedDut[index].IMEI;
            newrow[6] = logDateTime;

            string[] result_array = File.ReadAllLines(result_file);
            double total_testtime = 0;
            TEST_RESULT total_test_result = TEST_RESULT.PASS;
            foreach (var item in result_array)
            {
                if (String.IsNullOrEmpty(item))
                    continue;
                #region 整理测试结果
                string[] item_result_array = item.Split('=');
                string test_item = item_result_array[0].Trim();
                TEST_RESULT test_item_result = (TEST_RESULT)Enum.Parse(typeof(TEST_RESULT), item_result_array[1].Trim());
                if (test_item_result == TEST_RESULT.FAIL)
                    total_test_result = test_item_result;
                double test_item_testtime = double.Parse(item_result_array[2].Trim());
                total_testtime += test_item_testtime;
                if (test_item.Contains("SensorTest"))
                {
                    string[] sensor_items = test_item.Split(':');
                    foreach (var sensor in sensor_items)
                    {
                        switch (sensor.Trim())
                        {
                            case "LS":
                                newrow["LightSensor"] = test_item_result.ToString();
                                break;
                            case "GS":
                                newrow["GSensor"] = test_item_result.ToString();
                                break;
                            case "PS":
                                newrow["ProximitySensor"] = test_item_result.ToString();
                                break;
                            case "EC":
                                newrow["ECompass"] = test_item_result.ToString();
                                break;
                            case "GyS":
                                newrow["GyroSensor"] = test_item_result.ToString();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (test_item.Contains("MultiTest"))
                {
                    string[] audio_items = test_item.Split(':');
                    foreach (var audio in audio_items)
                    {
                        switch (audio.Trim())
                        {
                            case "BT":
                                newrow["BlueTooth"] = test_item_result.ToString();
                                break;
                            case "WF":
                                newrow["Wifi"] = test_item_result.ToString();
                                break;
                            case "GPS":
                                newrow["GPS"] = test_item_result.ToString();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    newrow[test_item] = test_item_result.ToString();
                }
                #endregion
            }
            newrow["Test Time(s)"] = (int)total_testtime;
            newrow["Result"] = total_test_result.ToString();
            output_resultToXls(newrow, result_xls_file);

            File.Delete(result_file);
            return 0;
        }

        private void output_resultToXls(DataRow newrow, FileInfo xlsFile)
        {
            MyExcel xlsApp = null;
            try
            {
                xlsApp = new MyExcel();
            }
            catch (Exception ex)
            {
                common.m_log.Add(ex.Message);
                return;
            }
            if (!xlsFile.Exists)
            {
                xlsApp.NewExcel();
                xlsApp.SaveAs(xlsFile.FullName);
            }
            else
            {
                xlsApp.Open(xlsFile.FullName);
            }
            xlsApp.OpenSheetByIndex(1);
            xlsApp.WorkSheetName = "LOG";
            xlsApp.AutoRange();
            int nStartRow = 1;
            int nStartCol = 1;
            string checkStr = xlsApp.GetItemText(nStartRow, 1);
            xlsApp.SetRowHeight(nStartRow, 20);
            if (String.IsNullOrEmpty(checkStr))
            {// 第一行第一列为空， 则表示是第一次产生结果， 需要写入title
                #region write title
                foreach (DataColumn column in newrow.Table.Columns)
                {
                    xlsApp.SetItemText(nStartRow, nStartCol, column.Caption);
                    // 标题样式
                    xlsApp.SetCellbackgroundStyle(nStartRow, nStartCol, (ColorIndex)(4));//背景
                    var font = xlsApp.GetCellFontStyle(nStartRow, nStartCol);
                    font.Bold = true;
                    font.Size = 10;// 字体
                    font.Name = "Microsoft JhengHei UI Light";
                    xlsApp.HorAligment(nStartRow, nStartCol, Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter);//对齐
                    xlsApp.SetBordersLineStyle(nStartRow, nStartCol, LineStyle.连续直线);// 边框

                    nStartCol++;
                }
                xlsApp.FreezePanes(2, 1);
                xlsApp.AutoFitAll();
                #endregion
            }
            nStartRow++;
            checkStr = xlsApp.GetItemText(nStartRow, 1);
            while (!String.IsNullOrEmpty(checkStr))
            {// 定位到最后一行
                nStartRow++;
                checkStr = xlsApp.GetItemText(nStartRow, 1);
            }
            nStartCol = 1;
            foreach (DataColumn column in newrow.Table.Columns)
            {
                if (column.Caption == "S/N" || column.Caption == "IMEI")
                {
                    xlsApp.SetItemText(nStartRow, nStartCol, "'" + newrow[column.Caption].ToString());
                }
                else
                {
                    xlsApp.SetItemText(nStartRow, nStartCol, newrow[column.Caption].ToString());
                    string resultStr = newrow[column.Caption].ToString();
                    if (resultStr == "PASS")
                    {
                        xlsApp.SetCellbackgroundStyle(nStartRow, nStartCol, ColorIndex.绿色);//背景
                    }
                    else if (resultStr == "FAIL")
                    {
                        xlsApp.SetCellbackgroundStyle(nStartRow, nStartCol, ColorIndex.红色);//背景
                    }
                }
                nStartCol++;
            }
            xlsApp.AutoFitAll();
            xlsApp.Save();
            xlsApp.Exit();
            common.m_log.Add($"Save Result to file:{xlsFile.Name}");
        }
        private async void UninstallPQAA(int id)
        {
            if (id >= 0)
            {
                string pushCmd = "adb -s " + mConnectedDut[id].SerialNumber + " uninstall com.wistron.generic.pqaa";
                await Execute(pushCmd, true);
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
            //if (mLicensed)
            //{
            //    about.setLicensedKey(mLicenseKey);
            //}
            about.ShowDialog();
        }

        private void OnMinimumWindow(System.Object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private async void startTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyExcel.DeleteExcelExe();
            common.DeleteConhostExe();
            await CMDHelper.Adb_KillServer();
            btn_StartTest();
        }
        private void startSelectedAndroidDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //mCount = 0;
            //DoControlTest(true);
            //TestSelectedDUT();
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
                int index = 0;
                foreach (UserGridClassLibrary.GridItem ctrl in mDuts)
                {
                    if (ctrl.IsDutSelected())
                    {
                        index = ctrl.GetDutGridID();
                        if (!string.IsNullOrEmpty(mConnectedDut[index].SerialNumber) &&
                            mConnectedDut[index].Connected)
                        {
                            //START TEST
                            Thread th = new Thread(new ParameterizedThreadStart(ThreadMethod));
                            th.Start(index); //启动线程
                        }
                    }
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
            //mUSB.RemoveUSBEventWatcher();
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


        private void viewPortIndexTableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //m_PortToIndexFactory = new PortToIndexFactory();
            //IndexToPortTable table_form = new IndexToPortTable(m_PortToIndexFactory.Node_Table);
            //table_form.ShowDialog();
        }

        private void initializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("the registered port to index data will be wiped out, are you sure to continue?", "Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                //m_PortToIndexFactory = new PortToIndexFactory();
                //m_PortToIndexFactory.InittializeTable();
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //RESET TIME
            foreach (DutDevice terminal in mConnectedDut)
            {
                terminal.BenginTime = DateTime.Now;
                terminal.Reset();
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
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("are you sure to exit?", "Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.UsbDeviceNotifier.Enabled = false;

                this.UsbDeviceNotifier.OnDeviceNotify -= UsbDeviceNotifier_OnDeviceNotify;
                this.Close();
            }
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
                        string pullCmd = "adb -s " + mConnectedDut[0].SerialNumber + " pull " + verify + config_inc.SPECIFIC_TAG_PATH;
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelOperaNamespace;
using LibUsbDotNet.DeviceNotify;
using LogHelper;
using MultiControl.Common;
using MultiControl.Functions;
using MultiControl.Lib;
using ThoughtWorks.QRCode.Codec;
using UserGridClassLibrary;
using System.Drawing.Printing;

namespace MultiControl
{
    public partial class MainWindow : Form
    {
        #region 全局变量
        /// <summary>
        /// 设备列表
        /// </summary>
        private List<DutDevice> m_DeviceList;
        /// <summary>
        /// 显示设备信息的界面控件
        /// </summary>
        UserGrid m_DeviceList_UI;
        /// <summary>
        /// 界面显示测试设备列表的行数/列数
        /// </summary>
        public static int m_Rows = 4, m_Cols = 6;
        /// <summary>
        /// 生成的机器md5 code
        /// </summary>
        private string m_md5_code = String.Empty;
        /// <summary>
        /// 蒙板图层(用于实时显示机器测试进度信息)
        /// </summary>
        private OpaqueForm m_OpaqueLayer = new OpaqueForm();
        /// <summary>
        /// adb命令操作类
        /// </summary>
        CMDHelper m_adb;
        /// <summary>
        /// USB device插拔通知接口
        /// </summary>
        IDeviceNotifier UsbDeviceNotifier;

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

        DirectoryInfo m_default_config_folder = null;
        DirectoryInfo m_result_folder = null;

        /// <summary>
        /// 是否多机型自行定制项目测试
        /// </summary>
        bool IsMultiModelTest = true;

        bool save_as_excel = true;
        bool save_as_xml = true;
        Int32 m_PseudoTotal = 0;
        //Event
        public event OnStartUpdateHandle StartUpdate;
        public event OnResultUpdateHandle ResultUpdate;
        public event OnProgressUpdateHandle ProgressUpdate;
        public event OnFinishUpdateHandle FinishUpdate;
        public event PropertyChangedEventHandler PropertyChanged;

        public object lock_obj = new object();
        #endregion

        #region 全局静态变量
        /// <summary>
        /// 机器是否注册
        /// </summary>
        public static bool m_bLicensed = false;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            this.Load += MainWindow_Load;
        }
        BarcodeLib.Barcode b = new BarcodeLib.Barcode();
        private void MainWindow_Load(object sender, EventArgs e)
        {
            // 初始化日志记录
            printDocument1 = new PrintDocument();//添加打印事件
            printDocument1.PrintPage += new PrintPageEventHandler(this.printDocument1_PrintPage);
            common.m_log = new LogHelper.LogMsg(Application.StartupPath);
            Boolean.TryParse(ConfigurationHelper.ReadConfig("DebugLogEnabled"), out common.m_log.IsEnable);

            if (!CheckRegister())
            {
                common.m_log.Add("Can not register this machine.", LogHelper.MessageType.ERROR);
                this.Close();
                return;
            }
            InitializeVariable();

            if (!InitializeConfigData())
            {
                this.Close();
                return;
            }
            InitializeUI();
        }

        #region Initialize UI/data
        /// <summary>
        /// 检查注册信息
        /// </summary>
        /// <returns></returns>
        bool CheckRegister()
        {
            // 检测是否已经注册
            //this.Hide();
            while (!m_bLicensed)
            {
                RegisterForm register_form = new RegisterForm();
                var result = register_form.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    break;
                }
            }
            // 生成pqaa启动验证码
            if (m_bLicensed)
            {
                this.m_md5_code = common.GetMD5Code("wortsin");
            }
            else
            {
                string computer_name = common.GetComputerName();
                this.m_md5_code = common.GetMD5Code(computer_name);
            }
            //this.Show();
            return m_bLicensed;
        }
        bool autoPrint = true ;
        bool InitializeConfigData()
        {
            FolderBrowserDialog folder_dialog = new FolderBrowserDialog();
            folder_dialog.SelectedPath = Application.StartupPath;
            // 验证config存放路径
            string config_path = ConfigurationHelper.ReadConfig("Test_Config_Folder");
            FileInfo dir_config_path = new FileInfo(config_path + @"\cfg.ini");
            while (!dir_config_path.Exists)
            {
                folder_dialog.Description = "can not find cfg.ini file, please specify cfg.ini folder.";
                if (folder_dialog.ShowDialog() == DialogResult.OK)
                {
                    dir_config_path = new FileInfo(folder_dialog.SelectedPath + @"\cfg.ini");
                    config_path = folder_dialog.SelectedPath;
                    ConfigurationHelper.WriteConfig("Test_Config_Folder", folder_dialog.SelectedPath);
                }
            }
            #region 读取并验证cfg.ini文件中相关配置
            FileInfo cfg_ini_file = new FileInfo(dir_config_path.FullName);

            IniFile iniFile = new IniFile(cfg_ini_file.FullName);
            this.m_default_config_folder = new DirectoryInfo(iniFile.IniReadValue("config", "Folder"));
            this.m_result_folder = new DirectoryInfo(iniFile.IniReadValue("config", "Logger"));
            if (!this.m_result_folder.Exists)
            {
                folder_dialog.Description = "please specify result log saving folder.";
                while (true)
                {
                    if (folder_dialog.ShowDialog() == DialogResult.OK)
                    {
                        this.m_result_folder = new DirectoryInfo(folder_dialog.SelectedPath);
                        iniFile.IniWriteValue("config", "Logger", this.m_result_folder.FullName);
                        break;
                    }
                }
            }

            Int32.TryParse(iniFile.IniReadValue("layout", "Row"), out m_Rows);
            Int32.TryParse(iniFile.IniReadValue("layout", "Column"), out m_Cols);
            Boolean.TryParse(iniFile.IniReadValue("mode", "MultiSupport"), out this.IsMultiModelTest);
            Boolean.TryParse(iniFile.IniReadValue("result", "save_as_excel"), out this.save_as_excel);
            Boolean.TryParse(iniFile.IniReadValue("result", "save_as_xml"), out this.save_as_xml);
            autoPrint = false;
            Boolean.TryParse(iniFile.IniReadValue("autoprint", "AutoPrint"), out this.autoPrint );
            Int32.TryParse(iniFile.IniReadValue("PrintFont", "FontSize"), out fontSize );
            if (!this.m_default_config_folder.Exists)
            {
                folder_dialog.Description = "please specify default PQAA test config folder for all models.";
                while (true)
                {
                    if (folder_dialog.ShowDialog() == DialogResult.OK)
                    {
                        this.m_default_config_folder = new DirectoryInfo(folder_dialog.SelectedPath);
                        iniFile.IniWriteValue("config", "Folder", this.m_default_config_folder.FullName);
                        break;
                    }
                }
            }
            #endregion

            // 加载/初始化 相关config data
            try
            {
                FileInfo ConfigPathData_file = new FileInfo(config_path + @"\ConfigData.xml");
                SpecifiedConfigPathFactory specified = new SpecifiedConfigPathFactory(ConfigPathData_file.FullName);

                FileInfo SDCardPathData_file = new FileInfo(config_path + @"\ConfigPath.xml");
                SDCardPathFactory sd_card_path_factory = new SDCardPathFactory(SDCardPathData_file.FullName);

                FileInfo PortToIndex_file = new FileInfo(config_path + @"\PortToIndex.xml");
                PortToIndexFactory port_to_index_factory = new PortToIndexFactory(PortToIndex_file.FullName);
            }
            catch (Exception ex)
            {
                common.m_log.Add(ex.Message, LogHelper.MessageType.ERROR);
                return false;
            }
            return true;
        }

        void InitializeVariable()
        {
            //清除残留的无用后台进程
            MyExcel.DeleteExcelExe();
            common.DeleteConhostExe();
            CMDHelper.Adb_StartServer();
            m_adb = new CMDHelper();
            UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
            UsbDeviceNotifier.OnDeviceNotify += UsbDeviceNotifier_OnDeviceNotify;
            UsbDeviceNotifier.Enabled = true;

            m_DeviceList = new List<DutDevice>(m_Rows * m_Cols);
            for (int index = 0; index < m_Rows * m_Cols; index++)
            {
                DutDevice device = new DutDevice();
                device.Reset();
                m_DeviceList.Add(device);
            }

            StartUpdate += MainWindow_StartUpdate;
            ResultUpdate += MainWindow_ResultUpdate;
            ProgressUpdate += MainWindow_ProgressUpdate;
            FinishUpdate += MainWindow_FinishUpdate;
        }

        void InitializeUI()
        {
            m_OpaqueLayer.Show(this);
            m_OpaqueLayer.Visible = false;
            m_OpaqueLayer.OnItemChanged += new OpaqueForm.ItemChangedHandle(this.OnTestItemChanged);

            this.viewGlobalLogToolStripMenuItem.Enabled = common.m_log.IsEnable;

            //this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width - 32, Screen.PrimaryScreen.WorkingArea.Height - 25);
            this.Size = new Size(1320, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = $"Multi-Control Test Tool——{config_inc.MULTICONTROL_VERSION}";
            this.Resize += MainWindow_Resize;

            m_DeviceList_UI = new UserGrid();
            m_DeviceList_UI.Location = new Point(20, 30);
            m_DeviceList_UI.Width = this.Width - 64;
            m_DeviceList_UI.Height = this.Height - 100;
            m_DeviceList_UI.Row = m_Rows;
            m_DeviceList_UI.Column = m_Cols;
            m_DeviceList_UI.InitializeGridItems();
            this.Controls.Add(m_DeviceList_UI);

            m_DeviceList_UI.OnGridItemFocus += new UserGridClassLibrary.UserGrid.GridItemFocusHandle(this.OnGridItemFocusHandle);
            m_DeviceList_UI.OnGridItemReset += new UserGridClassLibrary.UserGrid.GridItemResetHandle(this.OnGridItemResetHandle);

        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            m_DeviceList_UI.OnGridItemFocus -= new UserGridClassLibrary.UserGrid.GridItemFocusHandle(this.OnGridItemFocusHandle);
            m_DeviceList_UI.OnGridItemReset -= new UserGridClassLibrary.UserGrid.GridItemResetHandle(this.OnGridItemResetHandle);
            this.Controls.Remove(m_DeviceList_UI);

            m_DeviceList_UI = new UserGrid();
            m_DeviceList_UI.Location = new Point(20, 30);
            m_DeviceList_UI.Width = this.Width - 64;
            m_DeviceList_UI.Height = this.Height - 100;
            m_DeviceList_UI.Row = m_Rows;
            m_DeviceList_UI.Column = m_Cols;
            m_DeviceList_UI.InitializeGridItems();
            this.Controls.Add(m_DeviceList_UI);

            m_DeviceList_UI.OnGridItemFocus += new UserGridClassLibrary.UserGrid.GridItemFocusHandle(this.OnGridItemFocusHandle);
            m_DeviceList_UI.OnGridItemReset += new UserGridClassLibrary.UserGrid.GridItemResetHandle(this.OnGridItemResetHandle);

            StartBtnTest();
        }
        #endregion
        private delegate void DeviceChanged(DeviceNotifyEventArgs e);
        void OnDeviceChanged(DeviceNotifyEventArgs e)
        {
            switch (e.EventType)
            {
                case EventType.CustomEvent:
                    break;
                case EventType.DeviceArrival:
                    if (e.Device != null)
                    {
                        common.m_log.Add($"Device arrival.{e.Device.SerialNumber}");
                        #region 插入设备
                        UsbDeviceInfoEx device = new UsbDeviceInfoEx();
                        device.SerialNumber = e.Device.SerialNumber;

                        if (this.m_DeviceList.Contains(new DutDevice() { SerialNumber = device.SerialNumber }, DutDevice.Default))
                        {// 如果是正在测试中的设备， 则设置上线
                            int iindex = common.IndexDevice(m_DeviceList.ToArray(), device.SerialNumber);
                            m_DeviceList[iindex].Connected = true;
                            break;
                        }
                        // 对新加入的设备， 获取该设备的端口号等相关信息
                        UsbPortTree.RefreshUsbPort();
                        foreach (UsbDeviceInfoEx item in UsbPortTree.ConnectedAndroidDevices)
                        {
                            if (item.SerialNumber == e.Device.SerialNumber)
                            {
                                device = item;
                                break;
                            }
                        }
                        if (device.Port_Path == String.Empty)
                        {
                            common.m_log.Add($"can not read device:{device.SerialNumber}'s port number.");
                            break;
                        }
                        int index = PortToIndexFactory.GetIndex(device.Port);
                        while (index <= -1)
                        {// 配置端口和Multicontrol的Index对应
                            PortToIndexForm portForm = new PortToIndexForm(device.Port);
                            portForm.ShowDialog();
                            index = PortToIndexFactory.GetIndex(device.Port);
                        }
                        device.Index = index - 1;
                        var newTestDevice = m_DeviceList[device.Index];
                        newTestDevice.Reset();
                        newTestDevice.test_thread = new Thread(new ParameterizedThreadStart(TestThread));

                        m_DeviceList_UI[device.Index].Reset();// 界面重置
                        // 开户测试线程
                        newTestDevice.test_thread.Start(device);
                        #endregion
                    }
                    break;
                case EventType.DeviceQueryRemove:
                    break;
                case EventType.DeviceQueryRemoveFailed:
                    break;
                case EventType.DeviceRemoveComplete:
                    if (e.Device != null)
                    {
                        common.m_log.Add($"Device removed.{e.Device.SerialNumber}");
                        #region 设备拔出
                        UsbDeviceInfoEx device = new UsbDeviceInfoEx();
                        device.SerialNumber = e.Device.SerialNumber;
                        for (int index = 0; index < m_DeviceList.Count; index++)
                        {
                            var connected_device = m_DeviceList[index];
                            if (device.SerialNumber == connected_device.SerialNumber)
                            {
                                switch (connected_device.TestResult)
                                {
                                    case DutDevice.DutResult.DR_PASS:
                                        connected_device.Reset();
                                        m_DeviceList_UI[index].Reset();
                                        break;
                                    case DutDevice.DutResult.DR_FAIL:
                                        connected_device.ExitRunningThread = true;
                                        connected_device.Reset();
                                        m_DeviceList_UI[index].Reset();
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
                        #endregion
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
        void StartBtnTest()
        {
            common.m_log.Add("Reset All test events by user.");
            //RESET TIME
            foreach (DutDevice terminal in m_DeviceList)
            {
                terminal.BenginTime = DateTime.Now;
                terminal.Reset();
            }
            //CHECK DEVICES 
            foreach (UserGridClassLibrary.GridItem dut in m_DeviceList_UI)
            {
                if (!string.IsNullOrEmpty(dut.GetDutSN()))
                {
                    int id = dut.GetDutGridID();
                    dut.Reset();
                }
            }
            common.m_log.Add("Detect All Usb Android devices.");
            // 对新加入的设备， 获取该设备的端口号等相关信息
            UsbPortTree.RefreshUsbPort();
            for (int index = 0; index < UsbPortTree.ConnectedAndroidDevices.Count; index++)
            {
                var device = UsbPortTree.ConnectedAndroidDevices[index];
                int iindex = PortToIndexFactory.GetIndex(device.Port);
                while (iindex <= -1)
                {// 配置端口和Multicontrol的Index对应
                    PortToIndexForm portForm = new PortToIndexForm(device.Port);
                    portForm.ShowDialog();
                    iindex = PortToIndexFactory.GetIndex(device.Port);
                }
                device.Index = iindex - 1;
                var newTestDevice = m_DeviceList[device.Index];
                newTestDevice.Reset();
                newTestDevice.test_thread = new Thread(new ParameterizedThreadStart(TestThread));

                m_DeviceList_UI[device.Index].Reset();// 界面重置
                                                      // 开启测试线程
                newTestDevice.test_thread.Start(device);
            }
        }
        void StartCheckedBtnTest()
        {
            if (GetCheckedDUT() > 0)
            {
                common.m_log.Add("Test selected device/s.");
                //CHECK DEVICES
                foreach (UserGridClassLibrary.GridItem dut in m_DeviceList_UI)
                {
                    if (dut.IsDutSelected())
                    {
                        int idx = dut.GetDutGridID();
                        if (!string.IsNullOrEmpty(m_DeviceList[idx].SerialNumber) && m_DeviceList[idx].Connected)
                        {
                            UsbDeviceInfoEx device = new UsbDeviceInfoEx();
                            device.SerialNumber = m_DeviceList[idx].SerialNumber;
                            device.Index = idx;
                            device.Port_Path = m_DeviceList[idx].Port_Path;
                            device.Port_Index = m_DeviceList[idx].Port_Index;

                            dut.Reset();
                            m_DeviceList[idx].Reset();
                            m_DeviceList[idx].test_thread = new Thread(new ParameterizedThreadStart(TestThread));
                            m_DeviceList[idx].test_thread.Start(device);
                        }
                    }
                }
            }
        }
        private int GetCheckedDUT()
        {
            int count = 0, index = 0;
            foreach (UserGridClassLibrary.GridItem ctrl in m_DeviceList_UI)
            {
                if (ctrl.IsDutSelected() &&
                    !string.IsNullOrEmpty(m_DeviceList[index].SerialNumber) &&
                    m_DeviceList[index].Connected)
                {
                    count++;
                }
                index++;
            }
            return count;
        }

        async void SpecifySysInfocfgFile()
        {
            common.m_log.Add("Reset All test events by user.");
            common.m_log.Add("specify sysinfo.cfg file.");
            //RESET TIME
            foreach (DutDevice terminal in m_DeviceList)
            {
                terminal.BenginTime = DateTime.Now;
                terminal.Reset();
            }
            //CHECK DEVICES 
            foreach (UserGridClassLibrary.GridItem dut in m_DeviceList_UI)
            {
                if (!string.IsNullOrEmpty(dut.GetDutSN()))
                {
                    int id = dut.GetDutGridID();
                    dut.Reset();
                }
            }
            common.m_log.Add("Detect All Usb Android devices.");
            // 对新加入的设备， 获取该设备的端口号等相关信息
            UsbPortTree.RefreshUsbPort();
            if (UsbPortTree.ConnectedAndroidDevices.Count <= 0)
            {
                common.m_log.Add("none of android devices is detected, please retry!");
                MessageBox.Show("none of android devices is detected, please retry!", "Warning");
                return;
            }
            var device = UsbPortTree.ConnectedAndroidDevices[0];
            bool adb_avaiable = await m_adb.CheckDeviceConnection(device);
            if (!adb_avaiable)
            {
                common.m_log.Add($"adb: can not connect to this device:SN: {device.SerialNumber}; Port: {device.Port}", MessageType.ERROR);
                MessageBox.Show($"adb: can not connect to this device:SN: {device.SerialNumber}; Port: {device.Port}", "Warning");
                return;
            }
            string cmd_str = String.Empty;
            string response = String.Empty;
            #region get sd card path
            // uninstall pqaa
            cmd_str = $"adb -s {device.SerialNumber} uninstall com.wistron.generic.pqaa";
            response = await m_adb.CMD_RunAsync(cmd_str);
            common.m_log.Add(cmd_str);
            common.m_log.Add(response);

            cmd_str = $"adb -s {device.SerialNumber} install -r \"./apks/Generic_PQAA.apk\"";
            response = await m_adb.CMD_RunAsync(cmd_str);
            common.m_log.Add(cmd_str);
            common.m_log.Add(response);

            cmd_str = $"adb -s {device.SerialNumber} shell am startservice --user 0 -a com.wistron.generic.get.sdcard.path";
            response = await m_adb.CMD_RunAsync(cmd_str);
            common.m_log.Add(cmd_str);
            common.m_log.Add(response);


            string sd_card_path = String.Empty;
            DataTable model_table = SDCardPathFactory.SDCardPath_Table;
            int count = 0;
            while (String.IsNullOrEmpty(sd_card_path) && count < config_inc.CMD_REPEAT_MAX_TIME)
            {
                foreach (DataRow item in model_table.Rows)
                {
                    string remote_file = $"{item["InternalCard"].ToString()}{config_inc.SPECIFIC_TAG_PATH}";
                    string local_file = $".\\{config_inc.PATH_VERIFY_PATH}";

                    cmd_str = $"adb -s {device.SerialNumber} pull {remote_file} \"{local_file}\"";
                    response = await m_adb.CMD_RunAsync(cmd_str);
                    await Task.Delay(50);

                    if (File.Exists(local_file))
                    {
                        common.m_log.Add(cmd_str);
                        common.m_log.Add(response);
                        sd_card_path = item["InternalCard"].ToString();
                        File.Delete(local_file);
                        break;
                    }
                }
                count++;
            }
            if (sd_card_path == String.Empty)
            {
                common.m_log.Add($"{device.SerialNumber}: Test fail: No available SD card for testing...", MessageType.ERROR);
                MessageBox.Show($"{device.SerialNumber}: Test fail: No available SD card for testing...", "Warning");
                return;
            }
            #endregion

            #region get sysinfo.cfg
            cmd_str = $"adb -s {device.SerialNumber} install -r \"./apks/configcheck.apk\"";
            response = await m_adb.CMD_RunAsync(cmd_str);
            common.m_log.Add(cmd_str);
            common.m_log.Add(response);

            cmd_str = $"adb -s {device.SerialNumber} shell am start -n com.wistron.get.config.information/.MainActivity";
            response = await m_adb.CMD_RunAsync(cmd_str);
            common.m_log.Add(cmd_str);
            common.m_log.Add(response);

            FolderBrowserDialog folder_dialog = new FolderBrowserDialog();
            folder_dialog.SelectedPath = this.m_default_config_folder.FullName;
            folder_dialog.Description = "please select the folder that sysinfo.cfg should be saved.";
            folder_dialog.ShowDialog();

            this.m_default_config_folder = new DirectoryInfo(folder_dialog.SelectedPath);
            string remote_sysinfo_file = $"{sd_card_path}/Android/data/com.wistron.get.config.information/files/sysinfo.cfg";
            string local_sysinfo_file = $"{this.m_default_config_folder.FullName}\\sysinfo.cfg";

            if (File.Exists(local_sysinfo_file))
            {
                File.Delete(local_sysinfo_file);
            }
            cmd_str = $"adb -s {device.SerialNumber} pull {remote_sysinfo_file} \"{local_sysinfo_file}\"";

            count = 0;
            while (!File.Exists(local_sysinfo_file) && count < config_inc.CMD_REPEAT_MAX_TIME)
            {
                response = await m_adb.CMD_RunAsync(cmd_str);
                count++;
                common.m_log.Add(cmd_str);
                common.m_log.Add(response);
                await Task.Delay(config_inc.CMD_REPEAT_WAIT_TIME);
            }
            if (!File.Exists(local_sysinfo_file))
            {
                MessageBox.Show($"can not pull sysinfo.cfg file.error: {response.Trim()}");
                common.m_log.Add($"can not pull sysinfo.cfg file.error: {response.Trim()}", MessageType.ERROR);
            }
            #endregion

            // uninstall 
            cmd_str = $"adb -s {device.SerialNumber} uninstall com.wistron.get.config.information";
            response = await m_adb.CMD_RunAsync(cmd_str);
            common.m_log.Add(cmd_str);
            common.m_log.Add(response);

            common.m_log.Add("specify sysinfo.cfg file.--done!");
            MessageBox.Show("specify sysinfo.cfg file.--done!", "OK");
        }


        #region 打印
        int barcodeW = 0;
        int barcodeH = 0;
        int fontSize = 12;
        int printI=0;
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {                       
             Font font = new Font("黑体", fontSize,FontStyle.Bold );
            Font font1 = new Font("黑体", 10, FontStyle.Bold);
            Brush bru = Brushes.Black ;
            /* if (printI >= 24)
                 printI = 0;
                Font font2 = new Font("华文行楷", 10, FontStyle.Bold);
                Image x;
               x = Image.FromFile("E:\\捕获2.PNG");
                e.Graphics.DrawImage(x, 20, 20);*/


            barcodeH = 8 * fontSize;
            barcodeW = 4 * fontSize;
                e.Graphics.DrawString(m_DeviceList[printI].PringString, font, bru, 0, 0);
                image[printI] = b.Encode(BarcodeLib.TYPE.CODE128, m_DeviceList[printI].IMEI , ForeColor, BackColor, 140, 35);
                e.Graphics.DrawImage(image[printI], barcodeW, barcodeH);
               int  w = image[printI].Width - m_DeviceList[printI].IMEI.Length * 8;
                e.Graphics.DrawString(m_DeviceList[printI ].IMEI , font1, bru, w / 2+ barcodeW, barcodeH + image[printI].Height+4);   

            //  e.Graphics.DrawImage(image, 20, 20);

        }
        #endregion
        #region 测试线程

        async void TestThread(object obj)
        {
           
            UsbDeviceInfoEx device = obj as UsbDeviceInfoEx;
            int ThreadIndex = device.Index;
            var dut_device = m_DeviceList[ThreadIndex];
            dut_device.SerialNumber = device.SerialNumber;
            dut_device.Port_Path = device.Port_Path;
            dut_device.Port_Index = device.Port_Index;

            var dut_device_ui = m_DeviceList_UI[ThreadIndex];

            int count = 0;

            #region 测试准备 检查是否连接以及获取设备相关详细信息
            common.m_log.Add($"Thread:{ThreadIndex + 1}--start test device: {dut_device.SerialNumber}...", MessageType.NORMAL);
            bool adb_avaiable = await m_adb.CheckDeviceConnection(device);
            if (!adb_avaiable)
            {
                common.m_log.Add($"Thread:{ThreadIndex + 1}--adb: can not connect to this device:SN: {dut_device.SerialNumber}; Port: {dut_device.Port}", MessageType.ERROR);
                return;
            }

            dut_device.BenginTime = DateTime.Now;

            // 获取设备型号
            string cmd_str = $"adb -s {dut_device.SerialNumber} shell getprop ro.product.model";
            string response = await m_adb.CMD_RunAsync(cmd_str);
            response = response.Trim();
            if (common.IsNumeric(response))
            {
                cmd_str = $"adb -s {dut_device.SerialNumber} shell getprop ro.product.brand";
                response = await m_adb.CMD_RunAsync(cmd_str);
                response = response.Trim();
            }
            dut_device.Model = response;
            dut_device.Connected = true;

            cmd_str = $"adb -s {dut_device.SerialNumber} shell getprop ro.build.version.release";
            response = await m_adb.CMD_RunAsync(cmd_str);
            response = response.Trim();
            dut_device.AndroidVersion = response;

            cmd_str = $"adb -s {dut_device.SerialNumber} shell getprop ro.build.id";
            response = await m_adb.CMD_RunAsync(cmd_str);
            dut_device.BuildId = response.Trim();

            // 获取ConfigData 参数
            DataTable config_path_table = SpecifiedConfigPathFactory.Model_Table;
            if (config_path_table == null)
            {
                dut_device.ConfigPath = this.m_default_config_folder.FullName;
                dut_device.Estimate = 255.0f;
            }
            else
            {
                foreach (DataRow row in config_path_table.Rows)
                {
                    dut_device.ConfigPath = this.m_default_config_folder.FullName;
                    dut_device.Estimate = 255.0f;
                    if (row["Name"].ToString() == dut_device.Model)
                    {
                        dut_device.ConfigPath = row["Path"].ToString();
                        dut_device.Brand = row["Brand"].ToString();
                        dut_device.Estimate = float.Parse(row["Estimate"].ToString());
                        break;
                    }
                }
            }
            #endregion
            common.m_log.Add($"Thread:{ThreadIndex + 1}--Model: {dut_device.Model}, Android Version: {dut_device.AndroidVersion}, build id: {dut_device.BuildId}");

            #region 检测相关路径
            string log_date = DateTime.Now.ToString("yyyyMMdd");
            string logDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            DirectoryInfo log_folder = new DirectoryInfo(this.m_result_folder.FullName);
            DirectoryInfo log_model_path = new DirectoryInfo(log_folder.FullName + $@"\{dut_device.Model}");
            if (!log_model_path.Exists)
                log_model_path.Create();

            DirectoryInfo log_model_date_path = new DirectoryInfo(log_model_path.FullName + $@"\{log_date}");
            if (!log_model_date_path.Exists)
            {
                log_model_date_path.Create();
            }

            FileInfo log_file = new FileInfo(log_model_date_path.FullName + $@"\{dut_device.SerialNumber}_log.log");
            #endregion
            common.m_log.Add($"Thread:{ThreadIndex + 1}--Detail test information file: {log_file.FullName}");

            common.m_log.Add_File($"start test device: SN: {dut_device.SerialNumber}, Model: {dut_device.Model}, Android Version: {dut_device.AndroidVersion}, build id: {dut_device.BuildId}", log_file.FullName, MessageType.NORMAL, true);
            dut_device_ui.Result = ItemResult.IR_TESTING;
            dut_device_ui.EstimateTime = dut_device.Estimate;
            SetDutStatusInvoke(ThreadIndex);
            SetDutModel(ThreadIndex);

            #region push 安装pqaa, 获取mnt sd 卡路径，测试config档等
            // uninstall pqaa
            cmd_str = $"adb -s {dut_device.SerialNumber} uninstall com.wistron.generic.pqaa";
            response = await m_adb.CMD_RunAsync(cmd_str);
            common.m_log.Add_File(cmd_str, log_file.FullName);
            common.m_log.Add_File(response, log_file.FullName);

            cmd_str = $"adb -s {dut_device.SerialNumber} install -r \"./apks/Generic_PQAA.apk\"";
            response = await m_adb.CMD_RunAsync(cmd_str);
            common.m_log.Add_File(cmd_str, log_file.FullName);
            common.m_log.Add_File(response, log_file.FullName);
            SetDutInstallPQAAInvoke(ThreadIndex);

            cmd_str = $"adb -s {dut_device.SerialNumber} shell am startservice --user 0 -a com.wistron.generic.get.sdcard.path";
            response = await m_adb.CMD_RunAsync(cmd_str);
            common.m_log.Add_File(cmd_str, log_file.FullName);
            common.m_log.Add_File(response, log_file.FullName);

            string sd_card_path = String.Empty;
            DataTable model_table = SDCardPathFactory.SDCardPath_Table;
            count = 0;
            while (String.IsNullOrEmpty(sd_card_path) && count < config_inc.CMD_REPEAT_MAX_TIME)
            {
                foreach (DataRow item in model_table.Rows)
                {
                    string remote_file = $"{item["InternalCard"].ToString()}{config_inc.SPECIFIC_TAG_PATH}";
                    string local_file = $"{log_model_date_path}\\{config_inc.PATH_VERIFY_PATH}";

                    cmd_str = $"adb -s {dut_device.SerialNumber} pull {remote_file} \"{local_file}\"";
                    response = await m_adb.CMD_RunAsync(cmd_str);
                    await Task.Delay(50);

                    if (File.Exists(local_file))
                    {
                        common.m_log.Add_File(cmd_str, log_file.FullName);
                        common.m_log.Add_File(response, log_file.FullName);
                        sd_card_path = item["InternalCard"].ToString();
                        File.Delete(local_file);
                        break;
                    }
                }
                count++;
            }
            if (sd_card_path == String.Empty)
            {
                common.m_log.Add_File($"{dut_device.SerialNumber}: Test fail: No available SD card for testing...", log_file.FullName, MessageType.ERROR);
                return;
            }
            dut_device.SDCard = sd_card_path;
            string config_path = String.Empty;
            string remote_path = dut_device.SDCard + config_inc.CFG_FILE_ROOT;
            string remote_pqaa_path = dut_device.SDCard + config_inc.CFG_FILE_PQAA;

            if (IsMultiModelTest && Directory.Exists(dut_device.ConfigPath))
            {
                config_path = dut_device.ConfigPath;
            }
            else
            {
                config_path = this.m_default_config_folder.FullName;
            }

            DirectoryInfo config_folder = new DirectoryInfo(config_path);
            FileInfo[] config_files = config_folder.GetFiles("*.cfg", SearchOption.TopDirectoryOnly);
            foreach (var config_file in config_files)
            {
                cmd_str = $"adb -s {dut_device.SerialNumber} push \"{config_file.FullName}\" {remote_pqaa_path}";
                response = await m_adb.CMD_RunAsync(cmd_str);
                common.m_log.Add_File(cmd_str, log_file.FullName);
                common.m_log.Add_File(response, log_file.FullName);
            }
            DateTime dtEnd = DateTime.Now;
            var ts = dtEnd - dut_device.BenginTime;
            dut_device_ui.InstallTime = (float)ts.TotalMilliseconds / 1000;
            #endregion

            #region 开始测试， 并获取wInfo文件
            cmd_str = $"adb -s {dut_device.SerialNumber} shell am start -n com.wistron.generic.pqaa/.TestItemsList --ei block {ThreadIndex + 1} --ei autostart 1 --es md5code {this.m_md5_code}";
            response = await m_adb.CMD_RunAsync(cmd_str);
            common.m_log.Add_File(cmd_str, log_file.FullName);
            common.m_log.Add_File(response, log_file.FullName);
            SetDutStartPQAAInvoke(ThreadIndex);

            string remote_wInfo_file = remote_path + "wInfo.txt";
            string local_wInfo_file = log_model_date_path.FullName + $@"\{dut_device.SerialNumber}_wInfo.txt";
            cmd_str = $"adb -s {dut_device.SerialNumber} pull {remote_wInfo_file} \"{local_wInfo_file}\"";

            count = 0;
            while (!File.Exists(local_wInfo_file) && count < config_inc.CMD_REPEAT_MAX_TIME)
            {
                response = await m_adb.CMD_RunAsync(cmd_str);

                common.m_log.Add_File(cmd_str, log_file.FullName);
                common.m_log.Add_File(response, log_file.FullName);

                await Task.Delay(config_inc.CMD_REPEAT_WAIT_TIME);
                count++;
            }
            if (File.Exists(local_wInfo_file))
            {
                using (FileStream file = new FileStream(local_wInfo_file, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        dut_device.IMEI = await reader.ReadLineAsync();
                        dut_device.RAM = await reader.ReadLineAsync();
                        dut_device.FLASH = await reader.ReadLineAsync();
                        dut_device.BuildNumber = await reader.ReadLineAsync();
                        dut_device.BuildNumber = await reader.ReadLineAsync();
                       
                    }
                }
                File.Delete(local_wInfo_file);
            }
            else
            {
                common.m_log.Add_File("can not pull wInfo.txt file", log_file.FullName);
            }
            SetDutIMEIInvoke(ThreadIndex);
            #endregion
            dut_device.ExitRunningThread = false;
            int last_progress = -1;
            while (!dut_device.ExitRunningThread)
            {
                #region 追踪测试进度
                if (!dut_device.Connected)
                {// 断开连接
                    await Task.Delay(300);
                    SetDutDisconnected(ThreadIndex);
                    last_progress = -1;
                    continue;
                }
                // update connected Info
                SetDutConnected(ThreadIndex);

                // progress file
                string progressStr = string.Empty;
                string testItem = string.Empty;

                string remote_progress_file = dut_device.SDCard + config_inc.CFG_FILE_ROOT + "progress.txt";
                string local_pogress_file = log_model_date_path.FullName + $@"\{dut_device.SerialNumber}_progress.txt";
                cmd_str = $"adb -s {dut_device.SerialNumber} pull {remote_progress_file} \"{local_pogress_file}\"";
                response = await m_adb.CMD_RunAsync(cmd_str);
                if (File.Exists(local_pogress_file))
                {
                    Thread.Sleep(100);
                    try
                    {
                        string[] progress_context = File.ReadAllLines(local_pogress_file);

                        await Task.Delay(300);
                        if (progress_context.Length >= 1)
                            progressStr = progress_context[0].Trim();
                        if (progress_context.Length >= 2)
                            testItem = progress_context[1].Trim();
                    }
                    catch (IOException ex)
                    {
                        common.m_log.Add_File($"read progress file error: {ex.Message}", log_file.FullName);
                    }
                    File.Delete(local_pogress_file);
                }
                if (String.IsNullOrEmpty(progressStr))
                {
                    await Task.Delay(config_inc.CMD_REPEAT_WAIT_TIME);
                    continue;
                }

                // result file
                string remote_result_file = remote_path + "result.txt";
                string local_result_file = log_model_date_path.FullName + $@"\{dut_device.SerialNumber}_result.txt";
                cmd_str = $"adb -s {dut_device.SerialNumber} pull {remote_result_file} \"{local_result_file}\"";
                response = await m_adb.CMD_RunAsync(cmd_str);

                //UPDATE TEST PROGRESS
                string value = ThreadIndex + "/" + progressStr + "/" + testItem;
                common.m_log.Add_Debug(value);
                string[] progress_arr = progressStr.Split('/');
                int progress_current = int.Parse(progress_arr[0].ToString());
                int progress_total = int.Parse(progress_arr[1].ToString());

                // 更新界面进度
                if (last_progress != progress_current)
                {
                    SetDutTestProgressInvoke(value);
                    common.m_log.Add_File($"{value}", log_file.FullName);
                    last_progress = progress_current;
                }
                if (progress_current < progress_total)
                {
                    Thread.Sleep(300);
                    continue;
                }
                #endregion

                #region 测试结束
                // pull最后一次result文件
                cmd_str = $"adb -s {dut_device.SerialNumber} pull {remote_result_file} \"{local_result_file}\"";
                response = await m_adb.CMD_RunAsync(cmd_str);
                // uninstall pqaa
                cmd_str = $"adb -s {dut_device.SerialNumber} uninstall com.wistron.generic.pqaa";
                response = await m_adb.CMD_RunAsync(cmd_str);
                common.m_log.Add_File(cmd_str, log_file.FullName);
                common.m_log.Add_File(response, log_file.FullName);

                dut_device.ExitRunningThread = true;
                await Task.Delay(300);
                common.m_log.Add_File($"Write result to log.", log_file.FullName);
                // 保存测试结果
                SaveResultToFile(local_result_file, ThreadIndex, log_file);
                common.m_log.Add_File($"Test Over.", log_file.FullName);
                SetDutTestFinishInvoke(value);
                #endregion
            }
        }
        private int SaveResultToFile(string source, int index, FileInfo log_file)
        {
            var dut_device = this.m_DeviceList[index];
            string log_date = DateTime.Now.ToString("yyyyMMdd");
            string logDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            if (!m_result_folder.Exists)
            {
                common.m_log.Add($"log folder can not be found:{m_result_folder}");
                return -1;
            }
            DirectoryInfo log_model_path = new DirectoryInfo(m_result_folder.FullName + $@"\{dut_device.Model}");
            if (!log_model_path.Exists)
                log_model_path.Create();

            DirectoryInfo log_model_date_path = new DirectoryInfo(log_model_path.FullName + $@"\{log_date}");
            if (!log_model_date_path.Exists)
            {
                log_model_date_path.Create();
            }
            if (!File.Exists(source))
            {
                common.m_log.Add_File($"can not find result file: {source}", log_file.FullName);
                return -1;
            }

            string result_file = source;
            FileInfo result_xls_file = new FileInfo(log_model_date_path.FullName + "\\" + dut_device.Model + ".xlsx");
            FileInfo result_xml_file = new FileInfo(log_model_date_path.FullName + "\\" + dut_device.Model + ".xml");

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
            newrow[1] = dut_device.SerialNumber;
            newrow[2] = dut_device.Brand;
            newrow[3] = dut_device.Model;
            newrow[4] = dut_device.AndroidVersion;
            newrow[5] = dut_device.IMEI;
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
            newrow["TestTime"] = (int)total_testtime;
            newrow["Test_Result"] = total_test_result.ToString();

            lock (lock_obj)
            {// 由于按照Model为result文件名， 所以需要考虑相同机型同时测试 时候访问同一个result文件出现的错误
                if (save_as_excel)
                    output_resultToXls(newrow, result_xls_file, log_file);
                if (save_as_xml)
                    output_resultToXml(newrow, result_xml_file, log_file);
            }
            File.Delete(result_file);
            return 0;
        }
        private void output_resultToXls(DataRow newrow, FileInfo xlsFile, FileInfo log_file)
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
            common.m_log.Add_File($"Save Result to file: {xlsFile.FullName}", log_file.FullName);
        }

        private void output_resultToXml(DataRow newrow, FileInfo xmlFile, FileInfo log_file)
        {
            try
            {
                DataSet ds = new DataSet("PQAA_Test_Rsult");
                DataTable result_table = null;
                if (xmlFile.Exists)
                {
                    ds.ReadXml(xmlFile.FullName);
                    result_table = ds.Tables["Result"];

                    DataTable table = result_table.Clone();

                    for (int index = 0; index < table.Columns.Count; index++)
                    {
                        table.Columns[index].DataType = typeof(String);
                    }
                    foreach (DataRow row in result_table.Rows)
                    {
                        table.Rows.Add(row.ItemArray);
                    }
                    result_table = table;
                }
                else
                {
                    newrow.Table.TableName = "Result";
                    result_table = newrow.Table.Clone();
                }
                result_table.Rows.Add(newrow.ItemArray);

                ds = new DataSet("PQAA_Test_Rsult");
                ds.Tables.Add(result_table);
                ds.WriteXml(xmlFile.FullName);
            }
            catch (Exception ex)
            {
                common.m_log.Add_File($"Save Result to file: {xmlFile.FullName}, Error: {ex.Message}", log_file.FullName);
                return;
            }
            common.m_log.Add_File($"Save Result to file: {xmlFile.FullName}", log_file.FullName);
        }
        #endregion

        #region 线程中更新UI
        delegate void UpdateUIStatusDelegate(object index);

        void SetDutStatusInvoke(object index)
        {
            this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
            {
                SetDutStatus(_index);
            }), new object[] { index });
        }

        void SetDutIMEIInvoke(object index)
        {
            this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
            {
                SetDutIMEI(_index);
            }), new object[] { index });
        }

        void SetDutMD5Invoke(object index)
        {
            this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
            {
                SetDutMD5(_index);
            }), new object[] { index });
        }

        void SetDutPQAAFolderInvoke(object index)
        {
            this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
            {
                SetDutPQAAFolder(_index);
            }), new object[] { index });
        }

        void SetDutInstallPQAAInvoke(object index)
        {
            this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
            {
                SetDutInstallPQAA(_index);
            }), new object[] { index });
        }

        void SetDutStartPQAAInvoke(object index)
        {
            this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
            {
                SetDutStartPQAA(_index);
            }), new object[] { index });
        }

        void SetDutTestProgressInvoke(object index)
        {
            this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
            {
                SetDutTestProgress(_index);
            }), new object[] { index });
        }

        void SetDutTestFinishInvoke(object index)
        {
            this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
            {
                SetDutTestFinish(_index);
            }), new object[] { index });
        }


        private void SetDutDisconnected(object index)
        {
            this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
            {
                int i = (int)index;
                m_DeviceList_UI[i].BackgroundGradientColor = System.Drawing.Color.FromArgb(192, 64, 64, 64);//System.Drawing.Color.Blue;
                m_DeviceList_UI[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.None;
                m_DeviceList_UI[i].SetDutStatus("Offline");
            }), new object[] { index });
        }

        private void SetDutConnected(object index)
        {
            this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
            {
                int i = (int)index;
                m_DeviceList_UI[i].SetDutStatus("Online");
                switch (m_DeviceList[i].TestResult)
                {
                    case DutDevice.DutResult.DR_TESTING:
                        break;
                    case DutDevice.DutResult.DR_PASS:
                        m_DeviceList_UI[i].BackgroundGradientColor = System.Drawing.Color.Green;
                        m_DeviceList_UI[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
                        break;
                    case DutDevice.DutResult.DR_FAIL:
                        m_DeviceList_UI[i].BackgroundGradientColor = System.Drawing.Color.Red;
                        m_DeviceList_UI[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
                        break;
                }
            }), new object[] { index });

        }

        private void SetDutStatus(object index)
        {
            int i = (int)index;
            //START UPDATE NEW INFO.
            m_DeviceList[i].ID = i;
            //m_DeviceList_UI[i].SetDutSN("SN - " + mDevice[i].ToUpper());
            m_DeviceList_UI[i].SetDutSN(m_DeviceList[i].SerialNumber.ToUpper());
            m_DeviceList_UI[i].SetDutStatus("Online");
            m_DeviceList_UI[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
            m_DeviceList_UI[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            m_DeviceList_UI[i].Active = true;
        }

        private void SetDutModel(object index)
        {
            this.BeginInvoke(new UpdateUIStatusDelegate((_index) =>
            {
                int i = (int)index;
                m_DeviceList_UI[i].SetDutModel(m_DeviceList[i].Model);
                m_DeviceList_UI[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
                m_DeviceList_UI[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            }), new object[] { index });
        }
       // string[] PringString=new string[24];
      //  bool [] pringNO=new bool [24] ;
        Image[] image = new Image[24];
        private void SetDutIMEI(object index)
        {
            int i = (int)index;
            m_DeviceList_UI[i].SetDutIMEI(m_DeviceList[i].IMEI);
            m_DeviceList_UI[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
            m_DeviceList_UI[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            //SET QRCODE
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 1;
            qrCodeEncoder.QRCodeVersion = 9;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            Image image;
            string BN = m_DeviceList[i].BuildNumber;
            string data = "Model:" + m_DeviceList[i].Model + System.Environment.NewLine +
                "OS Version:" + m_DeviceList[i].BuildId + System.Environment.NewLine +
                "SN:" + m_DeviceList[i].SerialNumber.ToUpper() + System.Environment.NewLine +
                "IMEI:" + m_DeviceList[i].IMEI.ToUpper() + System.Environment.NewLine +
                "Memory:" + m_DeviceList[i].RAM + System.Environment.NewLine +
                "Flash:" + m_DeviceList[i].FLASH + System.Environment.NewLine + "BuildNumber:"+m_DeviceList[i].BuildNumber;//bonnie20160805

            // pringNO[i]= true ;
            m_DeviceList[i].PringString = "Model:" + m_DeviceList[i].Model + System.Environment.NewLine +
                "OS Version:" + m_DeviceList[i].BuildId + System.Environment.NewLine +
                "SN:" + m_DeviceList[i].SerialNumber.ToUpper() + System.Environment.NewLine+
                "Memory:" + m_DeviceList[i].RAM + System.Environment.NewLine +
                "Flash:" + m_DeviceList[i].FLASH + System.Environment.NewLine+"IMEI:";
            m_DeviceList[i].IsPrint = true;
            if (i<printI)
            {
                printI = i;

            }      
          //  this.printDocument1.Print();//开始打印
           
            
            image = qrCodeEncoder.Encode(data + BN);
            m_DeviceList_UI[i].SetQRCode_IMEI(image);
        }

        private void SetDutMD5(object index)
        {
            int i = (int)index;
            m_DeviceList_UI[i].SetDutItem("md5.txt");
            m_DeviceList_UI[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
            m_DeviceList_UI[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
        }

        private void SetDutPQAAFolder(object index)
        {
            int i = (int)index;
            m_DeviceList_UI[i].SetDutItem("config files");
            m_DeviceList_UI[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
            m_DeviceList_UI[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
        }

        private void SetDutInstallPQAA(object index)
        {
            int i = (int)index;
            m_DeviceList_UI[i].SetDutItem("Install PQAA");
            m_DeviceList_UI[i].BackgroundGradientColor = System.Drawing.Color.Yellow;
            m_DeviceList_UI[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            if (this.StartUpdate != null)
            {
                ResultEventArgs args = new ResultEventArgs(i, string.Empty);
                args.Current = 0;
                args.Total = -2;
                args.Time = 0.0f;
                args.Item = "PQAA Install";
                args.Model = m_DeviceList[i].Model;
                StartUpdate(this, args);
            }
        }

        private void SetDutStartPQAA(object index)
        {
            int i = (int)index;
            m_DeviceList_UI[i].SetDutItem("Start Testing...");
            m_DeviceList_UI[i].BackgroundGradientColor = System.Drawing.Color.Orange;
            m_DeviceList_UI[i].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            if (this.StartUpdate != null)
            {
                ResultEventArgs args = new ResultEventArgs(i, string.Empty);
                args.Current = 0;
                args.Total = -1;
                args.Time = m_DeviceList_UI[i].InstallTime;
                args.Item = "PQAA Install";
                args.Model = m_DeviceList[i].Model;
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
            // 界面进度更新
            m_DeviceList_UI[index].SetDutStatus("Online");
            m_DeviceList_UI[index].SetDutProgress(current, total);

            string name = item;
            if (
                name.Contains("ECompass") || name.Contains("Barometer") || name.Contains("OTG") ||
                name.Contains("Brightness") || name.Contains("Display") ||
                name.Contains("GSensor") || name.Contains("GyroSensor") || name.Contains("LightSensor") ||
                name.Contains("TouchPanel") || name.Contains("ProximitySensor") || name.Contains("Headset") ||
                name.Contains("MultiTouch") || name.Contains("HallSensor") || name.Contains("Button") ||
                name.Contains("HDMI") || name.Contains("NFC")
              )
            {
                m_DeviceList_UI[index].SetDutItem(name, false);
            }
            else
            {
                m_DeviceList_UI[index].SetDutItem(name);
            }
            m_DeviceList_UI[index].BackgroundGradientColor = System.Drawing.Color.Orange;
            m_DeviceList_UI[index].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            m_DeviceList_UI[index].Result = UserGridClassLibrary.ItemResult.IR_TESTING;
            m_DeviceList[index].TestResult = DutDevice.DutResult.DR_TESTING;

            //UPDATE RESULT ITEM
            if (this.ResultUpdate != null)
            {
                string log_date = DateTime.Now.ToString("yyyyMMdd");
                string result_file = $@"{this.m_result_folder}\{m_DeviceList[index].Model}\{log_date}\{m_DeviceList[index].SerialNumber}_result.txt";
                ResultEventArgs args = new ResultEventArgs(index, result_file);
                args.Current = current;
                args.Total = total;
                args.Item = item;
                args.Model = m_DeviceList[index].Model;
                ResultUpdate(this, args);
            }
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

        private void MainWindow_FinishUpdate(object sender, TestProgressEventArgs e)
        {
            int index = e.ResultIndex;
            m_DeviceList_UI[index].SetDutStatus("Online");
            m_PseudoTotal = e.Total;
            m_DeviceList_UI[index].SetDutProgress(e.Current, e.Total);
            m_DeviceList_UI[index].SetDutItem(e.Item);
            if (e.Item.Equals("PASS"))
            {
                m_DeviceList_UI[index].BackgroundGradientColor = System.Drawing.Color.Green;
                m_DeviceList_UI[index].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
                m_DeviceList[index].TestResult = DutDevice.DutResult.DR_PASS;
                m_DeviceList_UI[index].Result = UserGridClassLibrary.ItemResult.IR_PASS;
                m_OpaqueLayer.DictionaryWorkingStatus[index] = OpaqueForm.MyResult.PASS;
            }
            else
            {
                m_DeviceList_UI[index].BackgroundGradientColor = System.Drawing.Color.Red;
                m_DeviceList_UI[index].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
                m_DeviceList[index].TestResult = DutDevice.DutResult.DR_FAIL;
                m_DeviceList_UI[index].Result = UserGridClassLibrary.ItemResult.IR_FAIL;
                m_OpaqueLayer.DictionaryWorkingStatus[index] = OpaqueForm.MyResult.FAIL;
            }
            if (m_OpaqueLayer != null)
            {
                m_OpaqueLayer.EventWaitHandle[0].Reset();
            }
        }

        private void MainWindow_ProgressUpdate(object sender, TestProgressEventArgs e)
        {
            int index = e.ResultIndex;
            m_DeviceList_UI[index].SetDutStatus("Online");
            m_DeviceList_UI[index].SetDutProgress(e.Current, e.Total);

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
                m_DeviceList_UI[index].SetDutItem(e.Item, false);
            }
            else
            {
                m_DeviceList_UI[index].SetDutItem(e.Item);
            }
            m_DeviceList_UI[index].BackgroundGradientColor = System.Drawing.Color.Orange;
            m_DeviceList_UI[index].BackgroundGradientMode = UserGridClassLibrary.GridItem.DutBoxGradientMode.Vertical;
            m_DeviceList_UI[index].Result = UserGridClassLibrary.ItemResult.IR_TESTING;
            m_DeviceList[index].TestResult = DutDevice.DutResult.DR_TESTING;
        }

        private void MainWindow_ResultUpdate(object sender, ResultEventArgs e)
        {
            string resultName = e.ResultFile;
            int key = e.ResultIndex;
            if (m_OpaqueLayer != null)
            {
                m_OpaqueLayer.DictionaryWorkingStatus[key] = OpaqueForm.MyResult.WORKING;
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
                        m_OpaqueLayer.AddResult(key, e.Model, _name, OpaqueForm.MyResult.PASS, _cost);
                    }
                    else if (_result == 2)
                    {
                        m_OpaqueLayer.AddResult(key, e.Model, _name, OpaqueForm.MyResult.FAIL, _cost);
                    }
                    result = sr.ReadLine();
                }
                sr.Close();
                fs.Close();
                File.Delete(resultName);
            }
            if (e.Current < e.Total)
            {
                m_OpaqueLayer.AddResult(key, e.Model, e.Item, OpaqueForm.MyResult.WORKING, 0.0f);
            }
            //UPDATE PROGRESS BAR NUMBER
            int passCount = m_OpaqueLayer.GetTotalPassItemCount(key);
            m_DeviceList_UI[key].SetDutProgress(passCount, m_PseudoTotal);
        }

        private void MainWindow_StartUpdate(object sender, ResultEventArgs e)
        {
            int key = e.ResultIndex;
            if (m_OpaqueLayer != null)
            {
                m_OpaqueLayer.DictionaryWorkingStatus[key] = OpaqueForm.MyResult.WORKING;
            }
            switch (e.Total)
            {
                case -2:
                    m_OpaqueLayer.AddResult(key, e.Model, e.Item, OpaqueForm.MyResult.WORKING, 0.0f);
                    break;
                case -1:
                    m_OpaqueLayer.AddResult(key, e.Model, e.Item, OpaqueForm.MyResult.PASS, e.Time);
                    break;
            }
        }
        private void OnGridItemResetHandle(object sender, UserGridClassLibrary.GridItem.ResetEventArgs args)
        {
            if (m_OpaqueLayer != null)
            {
                m_OpaqueLayer.ClearResultList(args.ID);
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
            if (location.Y + m_OpaqueLayer.Height > Screen.PrimaryScreen.WorkingArea.Height)
            {
                int offset = location.Y + m_OpaqueLayer.Height - Screen.PrimaryScreen.WorkingArea.Height;
                location.Y -= offset;
                m_OpaqueLayer.Location = location;
            }
            else
            {
                m_OpaqueLayer.Location = location;
            }
            //Console.WriteLine("{0} - {1}", currentKey.ToString(), m_OpaqueLayer.CurrentKey.ToString());
            if (currentKey != m_OpaqueLayer.CurrentKey)
            {
                m_OpaqueLayer.CurrentKey = currentKey;
            }
        }
        #endregion

        #region 事件处理方法
        private void UsbDeviceNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            this.BeginInvoke(new DeviceChanged(OnDeviceChanged), new object[] { e });

        }
        private void OnTestItemChanged(object sender, OpaqueForm.ChangedEventArgs e)
        {
            int index = e.ID;
            if (index != -1)
            {
                m_DeviceList_UI[index].UpdateElapseTime(e.Elapsed);
            }
        }
        private async void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.UsbDeviceNotifier != null)
            {
                this.UsbDeviceNotifier.Enabled = false;
                this.UsbDeviceNotifier.OnDeviceNotify -= UsbDeviceNotifier_OnDeviceNotify;
            }
            if (m_DeviceList != null)
            {
                //RESET TIME
                foreach (DutDevice terminal in m_DeviceList)
                {
                    terminal.BenginTime = DateTime.Now;
                    terminal.Reset();
                }
            }
            if (m_DeviceList_UI != null)
            {
                //CHECK DEVICES 
                foreach (UserGridClassLibrary.GridItem dut in m_DeviceList_UI)
                {
                    if (!string.IsNullOrEmpty(dut.GetDutSN()))
                    {
                        int id = dut.GetDutGridID();
                        dut.Reset();
                    }
                }
            }
            await CMDHelper.Adb_KillServer();

            //清除残留的无用后台进程
            MyExcel.DeleteExcelExe();
            common.DeleteConhostExe();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("are you sure to exit?", "Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.Close();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help_form = new Help();
            help_form.ShowDialog();
        }

        private void aboutToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if (m_OpaqueLayer != null)
            {
                m_OpaqueLayer.CurrentKey = -1;
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            common.m_log.Add("Reset All test events by user.");
            //RESET TIME
            foreach (DutDevice terminal in m_DeviceList)
            {
                terminal.BenginTime = DateTime.Now;
                terminal.Reset();
            }
            //CHECK DEVICES 
            foreach (UserGridClassLibrary.GridItem dut in m_DeviceList_UI)
            {
                if (!string.IsNullOrEmpty(dut.GetDutSN()))
                {
                    int id = dut.GetDutGridID();
                    dut.Reset();
                }
            }
        }

        private void multiModelConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMapping form = new FormMapping();
            form.ShowDialog();
        }

        private void initializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This action will remove all specified data, are you sure?", "Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                SpecifiedConfigPathFactory.initialize();
            }
        }

        private void initializePortIndexTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This action will remove all specified data, are you sure??", "Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                PortToIndexFactory.inittialize();
            }
        }

        private void viewPortIndexTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IndexToPortTable form = new IndexToPortTable();
            form.ShowDialog();
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormSDCard sdcard = new FormSDCard();
            sdcard.ShowDialog();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartBtnTest();
        }

        private void startSelectedDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartCheckedBtnTest();
        }

        private void configurateToSysinfocfgLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpecifySysInfocfgFile();
        }

        private void defaultConfigPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 验证config存放路径
            string config_path = ConfigurationHelper.ReadConfig("Test_Config_Folder");
            FileInfo dir_config_path = new FileInfo(config_path + @"\cfg.ini");
            FileInfo cfg_ini_file = new FileInfo(dir_config_path.FullName);

            IniFile iniFile = new IniFile(cfg_ini_file.FullName);
            FolderBrowserDialog folder_dialog = new FolderBrowserDialog();
            folder_dialog.Description = "please specify default PQAA test config folder for all models.";
            folder_dialog.SelectedPath = this.m_default_config_folder.FullName;
            while (true)
            {
                if (folder_dialog.ShowDialog() == DialogResult.OK)
                {
                    this.m_default_config_folder = new DirectoryInfo(folder_dialog.SelectedPath);
                    iniFile.IniWriteValue("config", "Folder", this.m_default_config_folder.FullName);
                    break;
                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            printI = 0;
            if (m_DeviceList[printI].IMEI != null&& m_DeviceList[printI].IMEI !=""&&m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            printI =1;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            printI = 2;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            printI = 3;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            printI = 4;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            printI = 5;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            printI = 6;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            printI =7;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            printI = 8;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            printI = 9;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            printI = 10;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            printI =11;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            printI = 12;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            printI = 13;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            printI = 14;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            printI = 15;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            printI = 16;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            printI = 17;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            printI = 18;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem21_Click(object sender, EventArgs e)
        {
            printI = 19;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem22_Click(object sender, EventArgs e)
        {
            printI = 20;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem23_Click(object sender, EventArgs e)
        {
            printI = 21;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem24_Click(object sender, EventArgs e)
        {
            printI =22;
            if (m_DeviceList[printI].IMEI != null && m_DeviceList[printI].IMEI != "" && m_DeviceList_UI[printI].Connected)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private void toolStripMenuItem25_Click(object sender, EventArgs e)
        {
            printI = 23;
            if (m_DeviceList[printI].IMEI != null)
                this.printDocument1.Print();
            else
            {
                MessageBox.Show("This port no device or not ready to get information", "Print Lab", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }

        }

        private void viewGlobalLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", common.m_log.LogFilePath);
        }
        #endregion
    }
}

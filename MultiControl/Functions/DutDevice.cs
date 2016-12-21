using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MultiControl
{

    class DutDevice : IEqualityComparer<DutDevice>
    {
        public DutDevice()
        {

        }

        private string mmSDCard;//bonnie20160825
        private string mBuildNumber;//bonnie20160805
        private string mSerialNumber;
        private string mModel;
        private string mBuildId;
        private string mAndroidVersion;
        private string mBrand;
        private string mSDCard;
        private string mIMEI;
        private string mIMEI2;
        private string mRAM;
        private string mFlashSize;
        private int mIndex;
        private float mInstallTime;
        private float mEstimate;
        private DateTime mBegin;
        public Int32 Port_Index = -1;
        public string Port_Path = String.Empty;
        public string PQAA_Version = String.Empty;
        /// <summary>
        /// 由Port path和port index组成的port值
        /// </summary>
        public string Port
        {
            get
            {
                return $"#{Port_Index}--{Port_Path}";
            }
        }
        public string MSDCard
        {
            get { return mmSDCard; }
            set { mmSDCard = value; }
        }
        public enum DutResult
        {
            DR_NONE,
            DR_TESTING,
            DR_PASS,
            DR_FAIL
        }

        private DutResult _result;

        private string mConfigPath;

        private bool bConnected;

        private bool bExitRunningThread;

        public bool ExitRunningThread
        {
            get { return bExitRunningThread; }
            set { bExitRunningThread = value; }
        }
        private  bool isPrint;
        private string printString;
  
        public string PringString
        {
            get { return printString; }
            set { printString = value ; }
        }
        public bool IsPrint
        {
            get { return isPrint; }
            set { isPrint = value ; }
        }
        public bool Connected
        {
            get { return bConnected; }
            set { bConnected = value; }
        }

        public string SerialNumber
        {
            get { return mSerialNumber; }
            set { mSerialNumber = value; }
        }

        public string AndroidVersion
        {
            get { return mAndroidVersion; }
            set { mAndroidVersion = value; }
        }

        public string Brand
        {
            get { return mBrand; }
            set { mBrand = value; }
        }

        public string SDCard
        {
            get { return mSDCard; }
            set { mSDCard = value; }
        }

        public string Model
        {
            get { return mModel; }
            set { mModel = value; }
        }

        public string BuildId
        {
            get { return mBuildId; }
            set { mBuildId = value; }
        }

        public string IMEI
        {
            get { return mIMEI; }
            set { mIMEI = value; }
        }
        public string IMEI2
        {
            get { return mIMEI2; }
            set { mIMEI2 = value; }
        }
        public string BuildNumber//bonnie20160805
        {
            get { return mBuildNumber; }
            set { mBuildNumber = value; }
        }

        public string RAM
        {
            get { return mRAM; }
            set { mRAM = value; }
        }

        public string FLASH
        {
            get { return mFlashSize; }
            set { mFlashSize = value; }
        }

        public int ID
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        public string ConfigPath
        {
            get { return mConfigPath; }
            set { mConfigPath = value; }
        }

        public DutResult TestResult
        {
            get { return _result; }
            set { _result = value; }
        }

        public float InstallTime
        {
            get { return mInstallTime; }
            set { mInstallTime = value; }
        }

        public float Estimate
        {
            get { return mEstimate; }
            set { mEstimate = value; }
        }

        public DateTime BenginTime
        {
            get { return mBegin; }
            set { mBegin = value; }
        }

        public void Reset()
        {
            SerialNumber = string.Empty;
            Model = string.Empty;
            Connected = false;
            IMEI = string.Empty;
            BuildNumber = string.Empty;//bonnie20160805
            RAM = string.Empty;
            FLASH = string.Empty;
            ID = 0;
            ConfigPath = string.Empty;
            InstallTime = 0.0f;
            Estimate = 0.0f;
            ExitRunningThread = true;
            if (test_thread != null)
            {
                test_thread.Abort();
                test_thread = null;
            }
        }

        public bool Equals(DutDevice x, DutDevice y)
        {
            return x.SerialNumber == null || y.SerialNumber == null ? false : x.SerialNumber.Equals(y.SerialNumber);
        }

        public int GetHashCode(DutDevice obj)
        {
            return obj.GetHashCode();
        }
        public static DutDevice Default = new DutDevice();

        /// <summary>
        /// 测试线程
        /// </summary>
        public Thread test_thread = null;
    }
}

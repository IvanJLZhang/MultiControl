/* ======================================================================== 
 * 描述信息 
 *  
 * 作者：Ivan JL Zhang       
 * 时间：2016/10/25 17:08:43 
 * 文件名：EventArgsLib 
 * 版本：V1.0.0 
 * 文件说明：
 * 定义事件参数类
 * 
 * 修改者：           
 * 时间：               
 * 修改说明： 
* ======================================================================== 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiControl.Common
{
    public class TestProgressEventArgs : EventArgs
    {
        private int _resultIndex;
        private int _current;
        private int _total;
        private string _item;

        public TestProgressEventArgs()
        {

        }

        public int ResultIndex
        {
            get { return _resultIndex; }
            set { _resultIndex = value; }
        }

        public int Current
        {
            get { return _current; }
            set { _current = value; }
        }

        public int Total
        {
            get { return _total; }
            set { _total = value; }
        }

        public string Item
        {
            get { return _item; }
            set { _item = value; }
        }
    }

    public class ResultEventArgs : EventArgs
    {
        private string _resultFile;
        private int _resultIndex;

        private int _current;
        private int _total;
        private string _item;

        private string _model;

        private float _time;

        public int Current
        {
            get { return _current; }
            set { _current = value; }
        }

        public int Total
        {
            get { return _total; }
            set { _total = value; }
        }

        public string Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public ResultEventArgs(int index, string file)
        {
            this._resultIndex = index;
            this._resultFile = file;
        }

        public int ResultIndex
        {
            get { return _resultIndex; }
            set { _resultIndex = value; }
        }

        public string ResultFile
        {
            get { return _resultFile; }
            set { _resultFile = value; }
        }

        public float Time
        {
            get { return _time; }
            set { _time = value; }
        }
    }

    public class ProcessExitAgs
    {
        private string _command = String.Empty;
        /// <summary>
        /// 命令内容
        /// </summary>
        public string Command
        {
            get
            {
                return _command;
            }

            set
            {
                _command = value;
            }
        }
    }
}

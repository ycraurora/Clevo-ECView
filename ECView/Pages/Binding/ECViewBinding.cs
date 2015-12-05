using System.Collections.ObjectModel;

namespace ECView.Pages.Binding
{
    public class ECViewCollec : ObservableCollection<ECViewBinding>
    {
        /// <summary>
        /// EC参数与DataGrid绑定
        /// </summary>
        public ECViewCollec()
        {
            ;
        }
    }

    public class ECViewBinding : PropertyChangeBase
    {
        /// <summary>
        /// 模具型号
        /// </summary>
        private string _nbModel;
        public string NbModel
        {
            set
            {
                _nbModel = value;
                Notify("NbModel");
            }
            get
            {
                return _nbModel;
            }
        }
        /// <summary>
        /// EC版本
        /// </summary>
        private string _ecversion;
        public string ECVersion
        {
            set
            {
                _ecversion = value;
                Notify("ECVersion");
            }
            get
            {
                return _ecversion;
            }
        }
        /// <summary>
        /// 风扇号
        /// </summary>
        private int _fanNo;
        public int FanNo
        {
            set
            {
                _fanNo = value;
                Notify("FanNo");
            }
            get
            {
                return _fanNo;
            }
        }
        /// <summary>
        /// 风扇转速
        /// </summary>
        private string _fandutyStr;
        public string FanDutyStr
        {
            set
            {
                _fandutyStr = value;
                Notify("FanDutyStr");
            }
            get
            {
                return _fandutyStr;
            }
        }
        /// <summary>
        /// 风扇转速
        /// </summary>
        private int _fanduty;
        public int FanDuty
        {
            set
            {
                _fanduty = value;
                Notify("FanDuty");
            }
            get
            {
                return _fanduty;
            }
        }
        /// <summary>
        /// CPU温度
        /// </summary>
        private string _cpuLocal;
        public string CpuLocal
        {
            set
            {
                _cpuLocal = value;
                Notify("CpuLocal");
            }
            get
            {
                return _cpuLocal;
            }
        }
        /// <summary>
        /// 主板温度
        /// </summary>
        private string _cpuRemote;
        public string CpuRemote
        {
            set
            {
                _cpuRemote = value;
                Notify("CpuRemote");
            }
            get
            {
                return _cpuRemote;
            }
        }
        /// <summary>
        /// 风扇当前设置
        /// </summary>
        private string _fanset;
        public string FanSet
        {
            set
            {
                _fanset = value;
                Notify("FanSet");
            }
            get
            {
                return _fanset;
            }
        }
        /// <summary>
        /// 设置模式
        /// </summary>
        private int _fanSetModel;
        public int FanSetModel
        {
            set
            {
                _fanSetModel = value;
                Notify("FanSetModel");
            }
            get
            {
                return _fanSetModel;
            }
        }
        /// <summary>
        /// 更新设置标识
        /// </summary>
        private bool _updateFlag;
        public bool UpdateFlag
        {
            set
            {
                _updateFlag = value;
                Notify("UpdateFlag");
            }
            get
            {
                return _updateFlag;
            }
        }
        /// <summary>
        /// 操作
        /// </summary>
        private string _ope;
        public string Ope
        {
            set
            {
                _ope = value;
                Notify("Ope");
            }
            get
            {
                return _ope;
            }
        }
    }
}

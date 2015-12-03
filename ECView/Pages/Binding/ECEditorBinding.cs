namespace ECView.Pages.Binding
{
    public class ECEditorBinding : PropertyChangeBase
    {
        /// <summary>
        /// 风扇号
        /// </summary>
        private string _fanNo;
        public string FanNo
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
        private int _fanDuty;
        public int FanDuty
        {
            set
            {
                _fanDuty = value;
                Notify("FanDuty");
            }
            get
            {
                return _fanDuty;
            }
        }
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private string _filePath;
        public string FilePath
        {
            set
            {
                _filePath = value;
                Notify("FilePath");
            }
            get
            {
                return _filePath;
            }
        }
    }
}

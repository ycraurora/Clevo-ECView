namespace ECView.DataDefinitions
{
    public class ConfigPara
    {
        /// <summary>
        /// 主板型号
        /// </summary>
        public string NbModel
        {
            get;
            set;
        }
        /// <summary>
        /// EC版本
        /// </summary>
        public string ECVersion
        {
            get;
            set;
        }
        /// <summary>
        /// 调节模式号
        /// </summary>
        public int SetMode
        {
            get;
            set;
        }
        /// <summary>
        /// 风扇号
        /// </summary>
        public int FanNo
        {
            get;
            set;
        }
        /// <summary>
        /// 调节模式
        /// </summary>
        public string FanSet
        {
            get;
            set;
        }
        /// <summary>
        /// 风扇转速
        /// </summary>
        public int FanDuty
        {
            get;
            set;
        }
    }
}

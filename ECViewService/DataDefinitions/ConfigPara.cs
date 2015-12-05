using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECView.DataDefinitions
{
    public class ConfigPara
    {
        /// <summary>
        /// 是否自启动
        /// </summary>
        public bool IsAutoRun
        {
            get;
            set;
        }
        /// <summary>
        /// 是否最小化到托盘
        /// </summary>
        public bool IsBackRun
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

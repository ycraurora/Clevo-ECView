using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECView.DataDefinitions
{
    public class IntePara
    {
        /// <summary>
        /// 风扇号
        /// </summary>
        public int FanNo
        {
            get;
            set;
        }
        /// <summary>
        /// 智能控速模式号
        /// </summary>
        public int ControlType
        {
            get;
            set;
        }
        /// <summary>
        /// 最小转速
        /// </summary>
        public int MinFanDuty
        {
            get;
            set;
        }
        /// <summary>
        /// 范围参数列表
        /// </summary>
        public List<RangePara> RangeParaList
        {
            get;
            set;
        } 
    }

    public class RangePara
    {
        /// <summary>
        /// 范围号
        /// </summary>
        public int RangeNo
        {
            get;
            set;
        }
        /// <summary>
        /// 转速
        /// </summary>
        public int FanDuty
        {
            get;
            set;
        }
        /// <summary>
        /// 温度基础上增加百分比
        /// </summary>
        public int AddPercentage
        {
            get;
            set;
        }
        /// <summary>
        /// 范围下限
        /// </summary>
        public int InferiorLimit
        {
            get;
            set;
        }
    }
}

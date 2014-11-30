using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECView.Module.ModuleImpl;

namespace ECView.Module
{
    public class ModuleFactory
    {
        /// <summary>
        /// 风扇调节功能接口实例化
        /// </summary>
        private static IFanDutyModify moduleFanDutyModify = null;
        public static IFanDutyModify GetFanDutyModifyModule()
        {
            if (moduleFanDutyModify == null)
            {
                moduleFanDutyModify = new FanDutyModifyImpl();
            }
            return moduleFanDutyModify;
        }
    }
}

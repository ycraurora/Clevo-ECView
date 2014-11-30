using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECView.DataDefinitions;

namespace ECView.Module
{
    public interface IFanDutyModify
    {
        /// <summary>
        /// 设置风扇转速
        /// </summary>
        /// <param name="fanNo">风扇号</param>
        /// <param name="fanduty">风扇转速</param>
        /// <param name="isAuto">自动调节标识</param>
        /// <returns>风扇实际转速</returns>
        int[] SetFanduty(int fanNo, int fanduty, bool isAuto);
        /// <summary>
        /// 获取EC版本号
        /// </summary>
        /// <returns>EC版本</returns>
        string GetECVersion();
        /// <summary>
        /// 获取风扇转速与温度数据
        /// </summary>
        /// <param name="fanNo">风扇号</param>
        /// <returns>转速与温度结构体</returns>
        int[] GetTempFanDuty(int fanNo);
        /// <summary>
        /// 获取风扇数量
        /// </summary>
        /// <returns>风扇数量</returns>
        int GetFanCount();
        /// <summary>
        /// 检测服务是否启动
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务状态</returns>
        int CheckServiceState(string serviceName);
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>启动状态</returns>
        bool StartService(string serviceName);
        /// <summary>
        /// 设置程序自启动
        /// </summary>
        /// <param name="filename">程序名</param>
        /// <param name="isAutoRun">自启动标识</param>
        void SetAutoRun(string filename, bool isAutoRun);
        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="filename">配置文件名</param>
        /// <param name="configParaList">风扇配置</param>
        void WriteCfgFile(string filename, List<ConfigPara> configParaList);
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="filename">配置文件名</param>
        /// <returns>风扇配置</returns>
        List<ConfigPara> ReadCfgFile(string filename);
        /// <summary>
        /// 智能调节风扇转速
        /// </summary>
        /// <param name="intePara"></param>
        void SetInteFanduty(IntePara intePara);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECView.Tools;
using System.Threading;
using ECView.DataDefinitions;

namespace ECView.Module.ModuleImpl
{
    public class FanDutyModifyImpl : IFanDutyModify
    {
        /// <summary>
        /// 设置风扇转速
        /// </summary>
        /// <param name="fanNo">风扇号</param>
        /// <param name="fanduty">风扇转速</param>
        /// <param name="isAuto">自动调节标识</param>
        /// <returns>风扇实际转速</returns>
        public int[] SetFanduty(int fanNo, int fanduty, bool isAuto)
        {
            try
            {
                if (isAuto)
                {
                    //自动调节风扇转速
                    CallingVariation.SetFANDutyAuto(fanNo);
                }
                else
                {
                    //将风扇转速设置为定值
                    CallingVariation.SetFanDuty(fanNo, fanduty);
                }
                Thread.Sleep(500);
                int[] newFanduty = this._updateFanDuty(fanNo);
                return newFanduty;
            }
            catch (Exception e)
            {
                Console.WriteLine("设置风扇转速错误，原因：" + e.Message);
                int[] newFanduty = { -1, -1, -1 };
                return newFanduty;
            }
        }
        /// <summary>
        /// 获取EC版本号
        /// </summary>
        /// <returns>EC版本</returns>
        public string GetECVersion()
        {
            try
            {
                string ECVersion = CallingVariation.GetECVersion();
                return ECVersion;
            }
            catch (Exception e)
            {
                Console.WriteLine("获取EC版本错误，原因：" + e.Message);
                return "";
            }
        }
        /// <summary>
        /// 获取风扇转速与温度数据
        /// </summary>
        /// <returns>转速与温度结构体</returns>
        public int[] GetTempFanDuty(int fanNo)
        {
            try
            {
                var ecData = CallingVariation.GetTempFanDuty(fanNo);
                int byteData = ecData.data;
                byte[] ec = BitConverter.GetBytes(byteData);
                int[] fanduty = { 0, 0, 0 };
                fanduty[0] = (int)ec[0];
                fanduty[1] = (int)ec[1];
                fanduty[2] = (int)Math.Round(ec[2] / 2.55m);
                return fanduty;
            }
            catch (Exception e)
            {
                Console.WriteLine("获取风扇转速与温度数据错误，原因：" + e.Message);
                int[] fanduty = { -1, -1, -1 };
                return fanduty;
            }
        }
        /// <summary>
        /// 获取风扇数量
        /// </summary>
        /// <returns>风扇数量</returns>
        public int GetFanCount()
        {
            try
            {
                int FANCount = CallingVariation.GetFANCounter();
                return FANCount;
            }
            catch (Exception e)
            {
                Console.WriteLine("获取风扇数量错误，原因：" + e.Message);
                return -1;
            }
        }
        /// <summary>
        /// 检测服务是否启动
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务状态</returns>
        public int CheckServiceState(string serviceName)
        {
            try
            {
                int serviceState = ECViewTools.CheckServiceState(serviceName);
                return serviceState;
            }
            catch (Exception e)
            {
                Console.WriteLine("获取服务状态错误，原因：" + e.Message);
                return -1;
            }
        }
        /// <summary>
        /// 设置程序自启动
        /// </summary>
        /// <param name="isAutoRun">自启动标识</param>
        public void SetAutoRun(string filename, bool isAutoRun)
        {
            try
            {
                ECViewTools.SetAutoRun(filename, isAutoRun);
            }
            catch (Exception e)
            {
                Console.WriteLine("设置开机自启错误，原因：" + e.Message);
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>启动状态</returns>
        public bool StartService(string serviceName)
        {
            bool flag = ECViewTools.StartService(serviceName);
            return flag;
        }
        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="filename">配置文件名</param>
        /// <param name="configParaList">风扇配置</param>
        public void WriteCfgFile(string filename, List<ConfigPara> configParaList)
        {
            FileAnlyze.WriteCfgFile(filename, configParaList);
        }
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="filename">配置文件名</param>
        /// <returns>风扇配置</returns>
        public List<ConfigPara> ReadCfgFile(string filename)
        {
            return FileAnlyze.ReadCfgFile(filename);
        }
        /// <summary>
        /// 智能调节风扇转速
        /// </summary>
        /// <param name="intePara"></param>
        public void SetInteFanduty(IntePara intePara)
        {
            int[] ecData = GetTempFanDuty(intePara.FanNo);
            if (intePara.ControlType == 1)
            {
                for (int i = 0; i < intePara.RangeParaList.Count; i++)
                {
                    if (i == 0)
                    {
                        //若小于最小温度，设置风扇转速为最小值
                        if (ecData[0] < intePara.RangeParaList[i].InferiorLimit)
                        {
                            SetFanduty(intePara.FanNo, (int)(intePara.MinFanDuty * 2.55m), false);
                        }
                        else if (ecData[0] >= intePara.RangeParaList[i].InferiorLimit && ecData[0] < intePara.RangeParaList[i + 1].InferiorLimit)
                        {
                            if (intePara.RangeParaList[i].FanDuty < intePara.MinFanDuty)
                            {
                                SetFanduty(intePara.FanNo, (int)(intePara.MinFanDuty * 2.55m), false);
                            }
                            else
                            {
                                SetFanduty(intePara.FanNo, (int)(intePara.RangeParaList[i].FanDuty * 2.55m), false);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (i != intePara.RangeParaList.Count - 1)
                    {
                        if (ecData[0] >= intePara.RangeParaList[i].InferiorLimit && ecData[0] < intePara.RangeParaList[i + 1].InferiorLimit)
                        {
                            if (intePara.RangeParaList[i].FanDuty < intePara.MinFanDuty)
                            {
                                SetFanduty(intePara.FanNo, (int)(intePara.MinFanDuty * 2.55m), false);
                            }
                            else
                            {
                                SetFanduty(intePara.FanNo, (int)(intePara.RangeParaList[i].FanDuty * 2.55m), false);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (i == intePara.RangeParaList.Count - 1)
                    {
                        if (ecData[0] >= intePara.RangeParaList[i].InferiorLimit)
                        {
                            if (intePara.RangeParaList[i].FanDuty < intePara.MinFanDuty)
                            {
                                SetFanduty(intePara.FanNo, (int)(intePara.MinFanDuty * 2.55m), false);
                            }
                            else
                            {
                                SetFanduty(intePara.FanNo, (int)(intePara.RangeParaList[i].FanDuty * 2.55m), false);
                            }
                        }
                    }
                }
            }
            else if (intePara.ControlType == 2)
            {
                for (int i = 0; i < intePara.RangeParaList.Count; i++)
                {
                    if (i == 0)
                    {
                        //若小于最小温度，设置风扇转速为最小值
                        if (ecData[0] < intePara.RangeParaList[i].InferiorLimit)
                        {
                            SetFanduty(intePara.FanNo, (int)(intePara.MinFanDuty * 2.55m), false);
                        }
                        else if (ecData[0] > intePara.RangeParaList[i].InferiorLimit && ecData[0] < intePara.RangeParaList[i + 1].InferiorLimit)
                        {
                            if ((intePara.RangeParaList[i].AddPercentage + ecData[0]) < intePara.MinFanDuty)
                            {
                                SetFanduty(intePara.FanNo, (int)(intePara.MinFanDuty * 2.55m), false);
                            }
                            else
                            {
                                SetFanduty(intePara.FanNo, (int)((intePara.RangeParaList[i].AddPercentage + ecData[0]) * 2.55m), false);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (i != intePara.RangeParaList.Count - 1)
                    {
                        if (ecData[0] > intePara.RangeParaList[i].InferiorLimit && ecData[0] < intePara.RangeParaList[i + 1].InferiorLimit)
                        {
                            if ((intePara.RangeParaList[i].AddPercentage + ecData[0]) < intePara.MinFanDuty)
                            {
                                SetFanduty(intePara.FanNo, (int)(intePara.MinFanDuty * 2.55m), false);
                            }
                            else
                            {
                                SetFanduty(intePara.FanNo, (int)((intePara.RangeParaList[i].AddPercentage + ecData[0]) * 2.55m), false);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (i == intePara.RangeParaList.Count - 1)
                    {
                        if (ecData[0] >= intePara.RangeParaList[i].InferiorLimit)
                        {
                            if ((intePara.RangeParaList[i].AddPercentage + ecData[0]) < intePara.MinFanDuty)
                            {
                                SetFanduty(intePara.FanNo, (int)(intePara.MinFanDuty * 2.55m), false);
                            }
                            else
                            {
                                SetFanduty(intePara.FanNo, (int)((intePara.RangeParaList[i].AddPercentage + ecData[0]) * 2.55m), false);
                            }
                        }
                    }
                }
            }
        }
        /////////////////////////////////////////////////私有方法/////////////////////////////////////////////////

        /// <summary>
        /// 更新风扇转速状态
        /// </summary>
        private int[] _updateFanDuty(int fanNo)
        {
            int[] fanduty = this.GetTempFanDuty(fanNo);
            return fanduty;
        }
    }
}

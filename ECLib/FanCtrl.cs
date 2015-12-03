using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Xml;

namespace ECLib
{
    public class FanCtrl
    {
        #region 结构体定义（public）
        /// <summary>
        /// 配置参数结构体
        /// </summary>
        public class ConfigPara
        {
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
        /// <summary>
        /// 智能控制参数结构体
        /// </summary>
        private class IntePara
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
        /// <summary>
        /// 自定义范围结构体
        /// </summary>
        private class RangePara
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
        #endregion

        #region 风扇控制函数（public）
        /// <summary>
        /// 设置风扇转速
        /// </summary>
        /// <param name="fanNo">风扇号</param>
        /// <param name="fanduty">风扇转速</param>
        /// <param name="isAuto">自动调节标识</param>
        /// <returns>风扇实际转速</returns>
        public static int[] SetFanduty(int fanNo, int fanduty, bool isAuto)
        {
            try
            {
                if (isAuto)
                {
                    //自动调节风扇转速
                    _setFANDutyAuto(fanNo);
                }
                else
                {
                    //将风扇转速设置为定值
                    _setFanDuty(fanNo, fanduty);
                }
                Thread.Sleep(500);
                int[] newFanduty = _updateFanDuty(fanNo);
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
        public static string GetECVersion()
        {
            try
            {
                string ECVersion = _getECVersion();
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
        public static int[] GetTempFanDuty(int fanNo)
        {
            try
            {
                /*var ecData = _getTempFanDuty(fanNo);
                int byteData = ecData.data;
                byte[] ec = BitConverter.GetBytes(byteData);
                int[] fanduty = { 0, 0, 0 };
                fanduty[0] = (int)ec[0];
                fanduty[1] = (int)ec[1];
                fanduty[2] = (int)Math.Round(ec[2] / 2.55m);*/
                ECData ecData = _getTempFanDuty(fanNo);
                int[] fanduty = { 0, 0, 0 };
                fanduty[0] = ecData.Remote;
                fanduty[1] = ecData.Local;
                fanduty[2] = (int)Math.Round(ecData.FanDuty / 2.55m);
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
        public static int GetFanCount()
        {
            try
            {
                int FANCount = _getFANCounter();
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
        public static int CheckServiceState(string serviceName)
        {
            try
            {
                int serviceState = _checkServiceState(serviceName);
                return serviceState;
            }
            catch (Exception e)
            {
                Console.WriteLine("获取服务状态错误，原因：" + e.Message);
                return -1;
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>启动状态</returns>
        public static bool StartService(string serviceName)
        {
            bool flag = _startService(serviceName);
            return flag;
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static bool StopService(string serviceName)
        {
            bool flag = _stopService(serviceName);
            return flag;
        }
        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="filename">配置文件名</param>
        /// <param name="configParaList">风扇配置</param>
        public static void WriteCfgFile(string filename, List<ConfigPara> configParaList)
        {
            _writeCfgFile(filename, configParaList);
        }
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="filename">配置文件名</param>
        /// <returns>风扇配置</returns>
        public static List<ConfigPara> ReadCfgFile(string filename)
        {
            return _readCfgFile(filename);
        }
        public static bool InteFandutyControl(string filePath, int fanNo)
        {
            bool state = _inteFandutyControl(filePath, fanNo);
            return state;
        }
        #endregion public函数结束

        #region 风扇控制函数（private）
        /// <summary>
        /// 智能调节风扇转速
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fanNo"></param>
        /// <returns></returns>
        private static bool _inteFandutyControl(string filePath, int fanNo)
        {
            try
            {
                IntePara intePara = _readXmlFile(filePath, fanNo);
                _setInteFanduty(intePara);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        /// <summary>
        /// 智能调节风扇转速
        /// </summary>
        /// <param name="intePara"></param>
        private static void _setInteFanduty(IntePara intePara)
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
        /// <summary>
        /// 更新风扇转速状态
        /// </summary>
        private static int[] _updateFanDuty(int fanNo)
        {
            int[] fanduty = GetTempFanDuty(fanNo);
            return fanduty;
        }
        #endregion

        #region 动态库函数引用（private）
        [DllImport("ecview.dll", EntryPoint = "#3")]
        private static extern void _setFanDuty(int p1, int p2);

        [DllImport("ecview.dll", EntryPoint = "#4")]
        private static extern int _setFANDutyAuto(int p1);

        [DllImport("ecview.dll", EntryPoint = "#5")]
        private static extern ECData _getTempFanDuty(int p1);

        [DllImport("ecview.dll", EntryPoint = "#6")]
        private static extern int _getFANCounter();

        [DllImport("ecview.dll", EntryPoint = "#8")]
        private static extern string _getECVersion();

        public struct ECData
        {
            public byte Remote;
            public byte Local;
            public byte FanDuty;
        }
        #endregion

        #region 程序框架工具函数（private）
        /// <summary>
        /// 检测服务运行状态
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private static int _checkServiceState(string serviceName)
        {
            ServiceController[] service = ServiceController.GetServices();
            bool isStart = false;
            bool isExite = false;
            for (int i = 0; i < service.Length; i++)
            {
                if (service[i].ServiceName.ToUpper().Equals(serviceName.ToUpper()))
                {
                    isExite = true;
                    ServiceController server = service[i];
                    if (service[i].Status == ServiceControllerStatus.Running)
                    {
                        isStart = true;
                        break;
                    }
                }
            }

            if (!isExite)
            {
                //服务不存在
                return 0;
            }
            else
            {
                if (isStart)
                {
                    //服务存在并启动
                    return 1;
                }
                else
                {
                    //服务存在但未启动
                    return 2;
                }
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private static bool _startService(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    //服务已启动
                    return true;
                }
                else
                {
                    //服务未启动
                    //设置timeout
                    TimeSpan timeout = TimeSpan.FromMilliseconds(1000 * 10);
                    service.Start();//启动程序
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("启动服务错误，原因：" + e.Message);
                return false;
            }
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private static bool _stopService(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    //服务已停止
                    return true;
                }
                else
                {
                    //服务未停止
                    //设置timeout
                    TimeSpan timeout = TimeSpan.FromMilliseconds(1000 * 10);
                    service.Stop();//停止程序
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("停止服务错误，原因：" + e.Message);
                return false;
            }
        }
        /// <summary>
        /// 检测XML文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        private static bool _checkXmlFile(string filePath)
        {
            return false;
        }
        #endregion

        #region 文件处理函数（private）
        /// <summary>
        /// 读取XML文件
        /// </summary>
        /// <param name="filePath"></param>
        private static IntePara _readXmlFile(string filePath, int fanNo)
        {
            //声明XMLDocument对象
            XmlDocument doc = new XmlDocument();
            try
            {
                //数据存储结构体
                IntePara inte = new IntePara();
                inte.FanNo = fanNo;
                //范围数据存储结构体列表
                List<RangePara> rangeParaList = new List<RangePara>();
                //加载XML文件
                doc.Load(filePath);
                //获取XML根结点
                XmlNode root = doc.SelectSingleNode("ECView");
                //获取根结点属性--智能控速模式号
                XmlElement rootXe = (XmlElement)root;
                inte.ControlType = Convert.ToInt32(rootXe.GetAttribute("Type"));
                //获取下层结点
                XmlNodeList fanList = root.ChildNodes;
                foreach (XmlNode fan in fanList)
                {
                    //获取下层结点属性--风扇最小转速
                    XmlElement fanXe = (XmlElement)fan;
                    inte.MinFanDuty = Convert.ToInt32(fanXe.GetAttribute("MinFanduty"));
                    //获取范围数据
                    XmlNodeList rangeList = fan.ChildNodes;
                    foreach (XmlNode rangeXn in rangeList)
                    {
                        RangePara rangePara = new RangePara();
                        XmlElement rangeXe = (XmlElement)rangeXn;
                        //获取范围号
                        rangePara.RangeNo = Convert.ToInt32(rangeXe.GetAttribute("Num"));
                        //获取范围下限
                        rangePara.InferiorLimit = Convert.ToInt32(rangeXe.GetAttribute("InferiorLimit"));
                        if (inte.ControlType == 1)
                        {
                            //控速模式1下直接定义风扇转速
                            rangePara.FanDuty = Convert.ToInt32(rangeXe.GetAttribute("Fanduty"));
                        }
                        else if (inte.ControlType == 2)
                        {
                            //控速模式2下为温度+自定义增幅百分比
                            rangePara.AddPercentage = Convert.ToInt32(rangeXe.GetAttribute("AddPercentage"));
                        }
                        rangeParaList.Add(rangePara);
                    }
                }
                inte.RangeParaList = rangeParaList;
                return inte;
            }
            catch (Exception e)
            {
                Console.WriteLine("解析XML出错，原因：" + e.Message);
                return null;
            }
            finally
            {
                if (doc != null)
                {
                    //释放资源
                    doc = null;
                }
            }
        }
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="filePath">配置文件名</param>
        /// <returns>风扇配置</returns>
        private static List<ConfigPara> _readCfgFile(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            try
            {
                List<ConfigPara> configParaList = new List<ConfigPara>();

                List<string> lines = new List<string>();

                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                for (int i = 4; i < lines.Count; i++)
                {
                    ConfigPara configPara = new ConfigPara();
                    int fanNo = Convert.ToInt32(lines[i].Split(new char[] { '\t' })[1]);
                    int setMode = Convert.ToInt32(lines[i].Split(new char[] { '\t' })[3]);
                    string fanSet = lines[i].Split(new char[] { '\t' })[5];
                    int fanDuty = Convert.ToInt32(lines[i].Split(new char[] { '\t' })[7]);
                    configPara.FanNo = fanNo;
                    configPara.SetMode = setMode;
                    configPara.FanSet = fanSet;
                    configPara.FanDuty = fanDuty;
                    configParaList.Add(configPara);
                }
                return configParaList;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (sr != null)
                {
                    //释放资源
                    sr = null;
                }
            }
            
        }
        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="filePath">配置文件路径</param>
        /// <param name="configParaList">风扇配置</param>
        private static void _writeCfgFile(string filePath, List<ConfigPara> configParaList)
        {
            StreamWriter sw = new StreamWriter(filePath);
            try
            {
                sw.WriteLine("#ECView");
                sw.WriteLine("#Author YcraD");
                sw.WriteLine("#Config File -- DO NOT EDIT!");
                sw.WriteLine("FanCount" + "\t" + configParaList.Count);
                foreach (ConfigPara configPara in configParaList)
                {
                    sw.WriteLine("FanNo" + "\t" + configPara.FanNo + "\t" + "SetMode" + "\t" + configPara.SetMode + "\t" + "FanSet" + "\t" + configPara.FanSet + "\t" + "FanDuty" + "\t" + configPara.FanDuty);
                }
                sw.Close();
            }
            catch
            {
                return;
            }
            finally
            {
                if (sw != null)
                {
                    //释放资源
                    sw = null;
                }
            }
        }
        #endregion
    }
}

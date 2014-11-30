using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using ECView.DataDefinitions;

namespace ECView.Tools
{
    public class FileAnlyze
    {
        /// <summary>
        /// 读取XML文件
        /// </summary>
        /// <param name="filePath"></param>
        public static IntePara ReadXmlFile(string filePath, int fanNo)
        {
            try
            {
                //数据存储结构体
                IntePara inte = new IntePara();
                inte.FanNo = fanNo;
                //范围数据存储结构体列表
                List<RangePara> rangeParaList = new List<RangePara>();
                //声明XMLDocument对象
                XmlDocument doc = new XmlDocument();
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
        }
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="filePath">配置文件名</param>
        /// <returns>风扇配置</returns>
        public static List<ConfigPara> ReadCfgFile(string filePath)
        {
            List<ConfigPara> configParaList = new List<ConfigPara>();

            List<string> lines = new List<string>();

            StreamReader sr = new StreamReader(filePath);
            string line = "";
            while ((line=sr.ReadLine()) != null)
            {
                lines.Add(line);
            }
            bool isAutoRun = false;
            if (Convert.ToInt32(lines[3].Split(new char[] { '\t' })[1]) == 1)
            {
                isAutoRun = true;
            }
            bool isBackRun = false;
            if (Convert.ToInt32(lines[3].Split(new char[] { '\t' })[3]) == 1)
            {
                isBackRun = true;
            }
            for (int i = 5; i < lines.Count; i++ )
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
                configPara.IsAutoRun = isAutoRun;
                configPara.IsBackRun = isBackRun;
                configParaList.Add(configPara);
            }
            return configParaList;
        }
        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="filePath">配置文件路径</param>
        /// <param name="configParaList">风扇配置</param>
        public static void WriteCfgFile(string filePath, List<ConfigPara> configParaList)
        {
            StreamWriter sw = new StreamWriter(filePath);
            sw.WriteLine("#ECView");
            sw.WriteLine("#Author YcraD");
            sw.WriteLine("#Config File -- DO NOT EDIT!");
            sw.WriteLine("IsAutoRun" + "\t" + Convert.ToInt32(configParaList[0].IsAutoRun) + "\t" + "IsBackRun" + "\t" + Convert.ToInt32(configParaList[0].IsBackRun));
            sw.WriteLine("FanCount"+ "\t" + configParaList.Count);
            foreach (ConfigPara configPara in configParaList)
            {
                sw.WriteLine("FanNo" + "\t" + configPara.FanNo + "\t" + "SetMode" + "\t" + configPara.SetMode + "\t" + "FanSet" + "\t" + configPara.FanSet + "\t" + "FanDuty" + "\t" + configPara.FanDuty);
            }
            sw.Close();
        }
    }
}

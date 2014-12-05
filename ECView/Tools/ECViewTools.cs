using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.ServiceProcess;

namespace ECView.Tools
{
    public class ECViewTools
    {
        /// <summary>
        /// 设置程序自启动
        /// </summary>
        /// <param name="fileName">程序名</param>
        /// <param name="isAutoRun">自启动标识</param>
        public static void SetAutoRun(string fileName, bool isAutoRun)
        {
            RegistryKey reg = null;
            try
            {
                if (!System.IO.File.Exists(fileName))
                    throw new Exception("该文件不存在!");
                String name = fileName.Substring(fileName.LastIndexOf(@"\") + 1);
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)
                    reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (isAutoRun)
                    reg.SetValue(name, fileName);
                else
                    reg.SetValue(name, false);
            }
            catch (Exception e)
            {
                Console.WriteLine("设置开机自启错误，原因：" + e.Message);
            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }
        }
        /// <summary>
        /// 检测服务运行状态
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static int CheckServiceState(string serviceName)
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
            }else{
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
        public static bool StartService(string serviceName)
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
        /// 检测XML文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool CheckXmlFile(string filePath)
        {
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace ECViewService
{
    public partial class ECViewService : ServiceBase
    {
        //当前路径
        private string currentDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        //计时器
        private Timer timer;
        /// <summary>
        /// 初始化服务
        /// </summary>
        public ECViewService()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 循环处理事件
        /// </summary>
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //判断配置文件是否存在
            if (System.IO.File.Exists(currentDirectory + "ecview.cfg"))
            {
                List<ECLib.FanCtrl.ConfigPara> configParaList = configParaList = ECLib.FanCtrl.ReadCfgFile(currentDirectory + "ecview.cfg");
                foreach (ECLib.FanCtrl.ConfigPara configPara in configParaList)
                {
                    int fanNo = configPara.FanNo;
                    if (configPara.SetMode == 1)
                    {
                        //若配置为自动调节，设置风扇自动调节
                        ECLib.FanCtrl.SetFanduty(configPara.FanNo, 0, true);
                        //停止计时器
                        timer.Enabled = false;
                        //释放计时器资源
                        timer.Close();
                        timer.Dispose();
                    }
                    else if (configPara.SetMode == 2)
                    {
                        //若配置为手动调节，设置风扇转速
                        ECLib.FanCtrl.SetFanduty(configPara.FanNo, (int)(configPara.FanDuty * 2.55m), false);
                        //停止计时器
                        timer.Enabled = false;
                        //释放计时器资源
                        timer.Close();
                        timer.Dispose();
                    }
                    else if (configPara.SetMode == 3)
                    {
                        //若配置为智能调节，设置风扇转速
                        ECLib.FanCtrl.InteFandutyControl(currentDirectory + "conf\\Configuration_" + fanNo + ".xml", fanNo);
                    }
                    else { }
                }
                configParaList = null;
            }
        }
        /// <summary>
        /// 服务启动动作
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            //计时器初始化
            timer = new Timer(5000);
            timer.Elapsed += timer_Elapsed;
            //计时器循环设置
            timer.AutoReset = true;
            //启动主线程
            timer.Enabled = true;
        }
        /// <summary>
        /// 服务关闭动作
        /// </summary>
        protected override void OnStop()
        {
            //停止主线程
            timer.Enabled = false;
            //释放计时器资源
            timer.Close();
            timer.Dispose();
        }
    }
}

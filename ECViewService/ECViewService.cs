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
        //初始化计时器（1000ms）
        private Timer inteTimer = new Timer(1000);
        //当前路径
        private string currentDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        /// <summary>
        /// 初始化服务
        /// </summary>
        public ECViewService()
        {
            InitializeComponent();
            //计时器处理Elapsed事件
            inteTimer.Elapsed += inteTimer_Elapsed;
            //指定每次间隔结束引发Elapsed事件
            inteTimer.AutoReset = true;
        }
        /// <summary>
        /// 计时器处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void inteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //判断配置文件是否存在
            if (System.IO.File.Exists(currentDirectory + "ecview.cfg"))
            {
                List<ECLib.FanCtrl.ConfigPara> configParaList = ECLib.FanCtrl.ReadCfgFile(currentDirectory + "ecview.cfg");
                foreach (ECLib.FanCtrl.ConfigPara configPara in configParaList)
                {
                    int fanNo = configPara.FanNo;
                    if (configPara.SetMode == 1)
                    {
                        //若配置为自动调节，设置风扇自动调节
                        ECLib.FanCtrl.SetFanduty(configPara.FanNo, 0, true);
                    }
                    else if (configPara.SetMode == 2)
                    {
                        //若配置为手动调节，设置风扇转速
                        ECLib.FanCtrl.SetFanduty(configPara.FanNo, (int)(configPara.FanDuty * 2.55m), false);
                    }
                    else if (configPara.SetMode == 3)
                    {
                        //若配置为智能调节，设置风扇转速
                        bool state = ECLib.FanCtrl.InteFandutyControl(currentDirectory + "conf\\Configuration_" + fanNo + ".xml", fanNo);
                    }
                    else { }
                }
            }
        }
        /// <summary>
        /// 服务启动动作
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            //引发Elapsed事件
            inteTimer.Enabled = true;
        }
        /// <summary>
        /// 服务关闭动作
        /// </summary>
        protected override void OnStop()
        {
            //停止Elapsed事件
            inteTimer.Enabled = false;
        }
    }
}

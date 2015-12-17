using ECView.DataDefinitions;
using ECView.Module;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;

namespace ECViewService
{
    public partial class ECViewService : ServiceBase
    {
        //当前路径
        private string currentDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        //功能接口
        private IFanDutyModify iFanDutyModify;
        //线程
        private Thread t = null;
        List<ConfigPara> configParaList = null;
        /// <summary>
        /// 初始化服务
        /// </summary>
        public ECViewService()
        {
            InitializeComponent();
            iFanDutyModify = ModuleFactory.GetFanDutyModifyModule();
        }
        /// <summary>
        /// 循环处理事件
        /// </summary>
        private void setFandutyThread()
        {
            //判断配置文件是否存在
            if (System.IO.File.Exists(currentDirectory + "ecview.cfg"))
            {
                foreach (ConfigPara configPara in configParaList)
                {
                    if (configPara.SetMode == 1)
                    {
                        //若配置为自动调节，设置风扇自动调节
                        iFanDutyModify.SetFanduty(configPara.FanNo, 0, true);
                    }
                    else if (configPara.SetMode == 2)
                    {
                        //若配置为手动调节，设置风扇转速
                        iFanDutyModify.SetFanduty(configPara.FanNo, (int)(configPara.FanDuty * 2.55m), false);
                    }
                    else if (configPara.SetMode == 3)
                    {
                        while (true)
                        {
                            //线程暂停10s
                            Thread.Sleep(10 * 1000);
                            //若配置为智能调节，设置风扇转速
                            iFanDutyModify.InteFandutyControl(currentDirectory + "conf\\Configuration_" + configPara.FanNo + ".xml", configPara.FanNo);
                        }

                    }
                    else { }
                }
                configParaList = null;
                return;
            }
        }
        /// <summary>
        /// 服务启动动作
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            configParaList = iFanDutyModify.ReadCfgFile(currentDirectory + "ecview.cfg");
            t = new Thread(new ThreadStart(setFandutyThread));
            //设置线程优先级最低
            t.Priority = ThreadPriority.Lowest;
            t.Start();
        }
        /// <summary>
        /// 服务关闭动作
        /// </summary>
        protected override void OnStop()
        {
            //停止线程
            t.Abort();
        }
    }
}

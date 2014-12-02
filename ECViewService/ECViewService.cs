using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ECViewService
{
    public partial class ECViewService : ServiceBase
    {
        public ECViewService()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 服务启动动作
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
        }
        /// <summary>
        /// 服务关闭动作
        /// </summary>
        protected override void OnStop()
        {
        }
    }
}

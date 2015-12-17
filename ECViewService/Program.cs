using System.ServiceProcess;

namespace ECViewService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ECViewService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

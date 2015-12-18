using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace ECView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Mutex mut;
        public App()
        {
            //禁用重复开启
            /*bool createNew = false;
            string targetExeName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string productName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().GetName().Name);

            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, productName, out createNew))
            {
                if (createNew)
                {
                    StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
                    Run();
                }
                else
                {
                    //PTMCWin32API.SendMessage(targetExeName, "Protocol Testing Management Console", "/v:true");
                    Environment.Exit(1);
                }
            }*/
            bool requestInitialOwnership = true;
            bool mutexWasCreated;
            mut = new Mutex(requestInitialOwnership, "com.ECView.Ding", out mutexWasCreated);
            if (!(requestInitialOwnership && mutexWasCreated))
            {
                // 随意什么操作啦~
                //Current.Shutdown();
                //当前运行WPF程序的进程实例
                Process process = Process.GetCurrentProcess();
                process.Kill();
            }
        }
    }
}

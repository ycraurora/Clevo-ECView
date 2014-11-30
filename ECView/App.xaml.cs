using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ECView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /*public App()
        {
            //禁用重复开启
            bool createNew = false;
            string targetExeName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string productName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().GetName().Name);

            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, productName, out createNew))
            {
                if (createNew)
                {
                    StartupUri = new System.Uri("MainWindow.xaml", UriKind.Relative);
                    Run();
                }
                else
                {
                    //PTMCWin32API.SendMessage(targetExeName, "Protocol Testing Management Console", "/v:true");
                    System.Environment.Exit(1);
                }
            }
        }*/
    }
}

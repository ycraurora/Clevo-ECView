using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ECView.Module;
using ECView.Pages.Binding;
using System.Windows.Threading;
using System.Management;
using ECView.Pages.Windows;
using System.Threading;
using ECView.Tools;
using System.Reflection;
using ECView.DataDefinitions;
using System.Windows.Forms;

namespace ECView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //准备最小化到托盘
        NotifyIcon notifyIcon = null;

        IFanDutyModify iFanDutyModify;
        ECViewBinding ecviewData;
        ECViewCollec ecviewDataList;
        string currentDirectory;
        public ECViewCollec ECViewDataCollec
        {
            get
            {
                return ecviewDataList;
            }
        }
        //结果显示计时器
        DispatcherTimer revealTimer;
        DispatcherTimer setTimer;
        //模具型号
        ManagementObjectSearcher Searcher_BaseBoard = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
        //风扇数量
        private int FANCount=0;
        
        public MainWindow()
        {
            InitializeComponent();
            iFanDutyModify = ModuleFactory.GetFanDutyModifyModule();
            ecviewData = (ECViewBinding)ECViewGrid.DataContext;
            ecviewDataList = (ECViewCollec)ECDataGrid.DataContext;
            currentDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            InitECData();
            
            //计时器实例化
            revealTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 5)//每隔5s更新显示结果
            };
            setTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 10)//每隔10s检测温度
            };
            revealTimer.Tick += new EventHandler(_getEcData_Tick);
            setTimer.Tick += new EventHandler(_setInteFanduty_Tick);
            revealTimer.Start();
            setTimer.Start();

            InitTray();
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitECData()
        {
            //模具型号
            ecviewData.NbModel = "当前模具型号为：";
            foreach (var baseBoard in Searcher_BaseBoard.Get())
                ecviewData.NbModel += Convert.ToString((baseBoard)["Product"]);

            //EC版本
            ecviewData.ECVersion = "当前EC版本为：1.";
            ecviewData.ECVersion += iFanDutyModify.GetECVersion();

            //风扇数量
            FANCount = iFanDutyModify.GetFanCount();
            if (FANCount > 4)
                FANCount = 0;
            if (FANCount == 0)
                FANCount = 1;
            //判断配置文件是否存在
            if (System.IO.File.Exists(currentDirectory + "ecview.cfg"))
            {
                List<ConfigPara> configParaList = iFanDutyModify.ReadCfgFile(currentDirectory + "ecview.cfg");
                ecviewData.IsAutoRun = configParaList[0].IsAutoRun;
                ecviewData.IsBackRun = configParaList[0].IsBackRun;
                //风扇转速与温度信息
                for (int i = 0; i < FANCount; i++)
                {
                    int[] ecData = iFanDutyModify.GetTempFanDuty(i + 1);
                    ecviewData.FanNo = i + 1;
                    foreach (ConfigPara configPara in configParaList)
                    {
                        if (ecviewData.FanNo == configPara.FanNo)
                        {
                            ecviewData.FanSetModel = configPara.SetMode;
                            ecviewData.FanSet = configPara.FanSet;
                            if (configPara.SetMode == 1)
                            {
                                //若上次配置为自动调节，设置风扇自动调节
                                ecData = iFanDutyModify.SetFanduty(configPara.FanNo, 0, true);
                            }
                            else if (configPara.SetMode == 2)
                            {
                                //若为上次配置手动调节，设置风扇转速
                                ecData = iFanDutyModify.SetFanduty(configPara.FanNo, (int)(configPara.FanDuty * 2.55m), false);
                            }
                            else { }
                        }
                    }
                    ecviewData.CpuRemote = ecData[0] + "℃";
                    ecviewData.CpuLocal = ecData[1] + "℃";
                    ecviewData.UpdateFlag = false;
                    ecviewData.FanDutyStr = ecData[2] + "%";
                    ecviewData.FanDuty = ecData[2];
                    ecviewData.Ope = "设置";
                    ecviewDataList.Add(ecviewData);
                }
            }
            else
            {
                //风扇转速与温度信息
                for (int i = 0; i < FANCount; i++)
                {
                    int[] ecData = iFanDutyModify.GetTempFanDuty(i + 1);
                    ecviewData.CpuRemote = ecData[0] + "℃";
                    ecviewData.CpuLocal = ecData[1] + "℃";
                    ecviewData.FanDutyStr = ecData[2] + "%";
                    ecviewData.FanDuty = ecData[2];
                    ecviewData.FanNo = i + 1;
                    ecviewData.FanSet = "未设置";
                    ecviewData.FanSetModel = 0;
                    ecviewData.Ope = "设置";
                    ecviewData.UpdateFlag = false;
                    ecviewData.IsAutoRun = false;
                    ecviewData.IsBackRun = false;
                    ecviewDataList.Add(ecviewData);
                }
            }
        }
        /// <summary>
        /// 初始化托盘程序
        /// </summary>
        private void InitTray()
        {
            //设置托盘属性
            notifyIcon = new NotifyIcon();
            notifyIcon.BalloonTipText = "单击图标打开ECView";
            notifyIcon.Text = "ECView";
            notifyIcon.Icon = new System.Drawing.Icon(currentDirectory + "ECView.ico");
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(2000);
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);

            //关于选项
            System.Windows.Forms.MenuItem about = new System.Windows.Forms.MenuItem("关于");
            about.Click += new EventHandler(about_Click);
            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += new EventHandler(exit_Click);

            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { about, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);
        }
        /// <summary>
        /// 鼠标单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //如果鼠标左键单击
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    this.WindowState = WindowState.Normal;
                    this.Activate();
                }
            }
        }
        /// <summary>
        /// 退出选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exit_Click(object sender, EventArgs e)
        {
            this._exit();
        }
        /// <summary>
        /// 关于选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void about_Click(object sender, EventArgs e)
        {
            System.Windows.MessageBox.Show("ECView\n版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() +
                "\n作者：YcraD\n鸣谢：特别的帅（神舟笔记本吧）\n说明：本程序主要实现Clevo模具风扇转速控制，部分灵感来源于“特别的帅”，在此感谢",
                "关于", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        /// <summary>
        /// 检测是否更新设置
        /// </summary>
        private void CheckUpdate()
        {
            for (int i = 0; i < ecviewDataList.Count; i++)
            {
                if (ecviewDataList[i].UpdateFlag)
                {
                    revealTimer.Stop();
                    setTimer.Stop();
                    List<ConfigPara> configParaList = new List<ConfigPara>();
                    foreach (ECViewBinding ec in ecviewDataList)
                    {
                        ConfigPara configPara = new ConfigPara();
                        configPara.FanNo = ec.FanNo;
                        configPara.SetMode = ec.FanSetModel;
                        configPara.FanSet = ec.FanSet;
                        configPara.FanDuty = ec.FanDuty;
                        configPara.IsAutoRun = ec.IsAutoRun;
                        configPara.IsBackRun = ec.IsBackRun;
                        configParaList.Add(configPara);
                    }
                    //获取当前路径
                    string currentDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                    iFanDutyModify.WriteCfgFile(currentDirectory + "ecview.cfg", configParaList);
                    Thread.Sleep(1000);
                    revealTimer.Start();
                    setTimer.Start();
                }
                ecviewDataList[i].UpdateFlag = false;
            }
        }
        /// <summary>
        /// 重载关闭事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            //关闭托盘程序
            notifyIcon.Dispose();
            //关闭主程序
            base.OnClosed(e);
        }
        /// <summary>
        /// 重载即将关闭事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (ecviewData.IsBackRun)
            {
                this.Visibility = Visibility.Hidden;
                e.Cancel = true;
            }
            else if (this.Visibility == Visibility.Hidden)
            {
                if (System.Windows.MessageBox.Show("确定退出？", "ECView", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    this._exit();
                }
            }
            else
            {
                System.Windows.MessageBoxResult mesRes = System.Windows.MessageBox.Show("是否最小化到托盘？\n是--最小化到托盘；\n否--退出程序；\n取消--取消关闭程序。", "ECView", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (mesRes == MessageBoxResult.Yes)
                {
                    this.Visibility = Visibility.Hidden;
                    e.Cancel = true;
                }
                else if (mesRes == MessageBoxResult.No)
                {
                    e.Cancel = false;
                }
                else if (mesRes == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
        ////////////////////////////////////////////////////////界面事件////////////////////////////////////////////////////////
        /// <summary>
        /// DataGrid点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ECEditor_Click(object sender, RoutedEventArgs e)
        {
            //读取选择的参数行
            var datagrid = sender as System.Windows.Controls.DataGrid;
            int index = datagrid.SelectedIndex;
            ECViewBinding selectFan = (ECViewBinding)datagrid.SelectedItem;
            int fanduty = iFanDutyModify.GetTempFanDuty(index + 1)[2];
            //停止计时器
            revealTimer.Stop();
            setTimer.Stop();
            //加载窗体
            ECEditor ecWindow = new ECEditor(this, index, fanduty, selectFan.FanSetModel, revealTimer, setTimer);
            ecWindow.ShowDialog();
        }
        /// <summary>
        /// 设置自启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsAutoRunChecked(object sender, RoutedEventArgs e)
        {
            //整理数据
            ecviewData.IsAutoRun = true;
            ecviewData.UpdateFlag = true;
            iFanDutyModify.SetAutoRun(Assembly.GetExecutingAssembly().Location, true);
        }
        /// <summary>
        /// 取消自启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsAutoRunUnChecked(object sender, RoutedEventArgs e)
        {
            //整理数据
            ecviewData.IsAutoRun = false;
            ecviewData.UpdateFlag = true;
            iFanDutyModify.SetAutoRun(Assembly.GetExecutingAssembly().Location, false);
        }
        /// <summary>
        /// 开启托盘程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsBackRunChecked(object sender, RoutedEventArgs e)
        {
            //整理数据
            ecviewData.IsBackRun = true;
            ecviewData.UpdateFlag = true;
        }
        /// <summary>
        /// 取消托盘程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsBackRunUnChecked(object sender, RoutedEventArgs e)
        {
            //整理数据
            ecviewData.IsBackRun = false;
            ecviewData.UpdateFlag = true;
        }
        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            this._exit();
        }
        /// <summary>
        /// 关于提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Aboout_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("ECView\n版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() +
                "\n作者：YcraD\n鸣谢：特别的帅（神舟笔记本吧）\n说明：本程序主要实现Clevo模具风扇转速控制，部分灵感来源于“特别的帅”，在此感谢",
                "关于", MessageBoxButton.OK, MessageBoxImage.Information);
        } 
        ////////////////////////////////////////////////////////私有方法////////////////////////////////////////////////////////
        /// <summary>
        /// 实时获取风扇与温度状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _getEcData_Tick(object sender, EventArgs e)
        {
            //风扇转速与温度信息
            for (int i = 0; i < FANCount; i++)
            {
                int[] ecData = iFanDutyModify.GetTempFanDuty(i + 1);
                ecviewData.CpuRemote = ecData[0] + "℃";
                ecviewData.CpuLocal = ecData[1] + "℃";
                ecviewDataList[i].FanDuty = ecData[2];
                ecviewDataList[i].FanDutyStr = ecData[2] + "%";
            }
            CheckUpdate();
        }

        private void _setInteFanduty_Tick(object sender, EventArgs e)
        {
            //判断配置文件是否存在
            if (System.IO.File.Exists(currentDirectory + "ecview.cfg"))
            {
                List<ConfigPara> configParaList = iFanDutyModify.ReadCfgFile(currentDirectory + "ecview.cfg");
                foreach (ConfigPara configPara in configParaList)
                {
                    int fanNo = configPara.FanNo;
                    if (configPara.SetMode == 3)
                    {
                        IntePara intePara = FileAnlyze.ReadXmlFile(currentDirectory + "conf\\Configuration_" + fanNo + ".xml", fanNo);
                        iFanDutyModify.SetInteFanduty(intePara);
                    }
                }
            }
        }
        /// <summary>
        /// 退出程序
        /// </summary>
        private void _exit()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}

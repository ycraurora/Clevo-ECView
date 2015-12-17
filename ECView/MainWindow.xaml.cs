using ECView.DataDefinitions;
using ECView.Module;
using ECView.Pages.Binding;
using ECView.Pages.Windows;
using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ECView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //ECViewService状态
        private int status = 0;
        private string serviceName = "ECViewService";
        //设置行号
        private int index;
        //主窗口绑定
        ECViewBinding ecviewData = null;
        //数据列表绑定
        ECViewCollec ecviewDataList = null;
        //当前工作目录
        private string currentDirectory = "";
        //线程任务
        Task<int> tempGetter = null;
        //功能接口
        IFanDutyModify iFanDutyModify;
        /// <summary>
        /// 子窗口传参
        /// </summary>
        public ECViewCollec ECViewDataCollec
        {
            get
            {
                return ecviewDataList;
            }
        }
        //模具型号
        ManagementObjectSearcher Searcher_BaseBoard;
        //风扇数量
        private int FANCount=0;
        
        public MainWindow()
        {
            InitializeComponent();
            iFanDutyModify = ModuleFactory.GetFanDutyModifyModule();
            //检测服务
            CheckService();
            //数据绑定
            ecviewData = (ECViewBinding)ECViewGrid.DataContext;
            ecviewDataList = (ECViewCollec)ECDataGrid.DataContext;
            //当前工作目录
            currentDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //初始化数据
            InitECData();
            //CPU更新线程
            _getTemp_Task();
        }
        /// <summary>
        /// 检测服务状态
        /// </summary>
        private void CheckService()
        {
            status = iFanDutyModify.CheckServiceState(serviceName);
            if (status == 0)
            {
                MessageBox.Show("ECView服务未正确安装，无法使用智能调节。\nECView虽然仍可继续使用，但建议重新安装ECView。", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (status == 1)
            {
                iFanDutyModify.StopService(serviceName);
                return;
            }
            else if (status == 2)
            {
                MessageBox.Show("ECView服务已关闭。\n当服务设置为手动或已关闭时，无法使用智能调节以及开机自动设定风扇转速等功能。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitECData()
        {
            //判断配置文件是否存在
            if (System.IO.File.Exists(currentDirectory + "ecview.cfg"))
            {
                List<ConfigPara> configParaList = iFanDutyModify.ReadCfgFile(currentDirectory + "ecview.cfg");
                //风扇转速与温度信息
                for (int i = 0; i < configParaList.Count; i++)
                {
                    int[] ecData= { 0,0,0 };// = iFanDutyModify.GetTempFanDuty(i + 1);
                    ecviewData.NbModel = configParaList[0].NbModel;
                    ecviewData.ECVersion = configParaList[0].ECVersion;
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
                    ecviewDataList.Add(ecviewData);
                }
            }
            else
            {
                Searcher_BaseBoard = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
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
                    ecviewData.UpdateFlag = false;
                    ecviewDataList.Add(ecviewData);
                }
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            bool isUpdated = false;
            foreach (ECViewBinding ec in ecviewDataList)
            {
                if (ec.UpdateFlag)
                {
                    isUpdated = true;
                }
            }
            if (isUpdated)
            {
                MessageBoxResult res = MessageBox.Show("保存设置并退出？\n（ECView将在退出时启动ECViewService服务）", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.OK)
                {
                    SaveConfig();
                }
            }
            if (status != 0)
            {
                iFanDutyModify.StartService(serviceName);
            }
            base.OnClosing(e);
        }
        /// <summary>
        /// 检测是否更新设置
        /// </summary>
        private void SaveConfig()
        {
            for (int i = 0; i < ecviewDataList.Count; i++)
            {
                if (ecviewDataList[i].UpdateFlag)
                {
                    List<ConfigPara> configParaList = new List<ConfigPara>();
                    foreach (ECViewBinding ec in ecviewDataList)
                    {
                        ConfigPara configPara = new ConfigPara();
                        configPara.NbModel = ec.NbModel;
                        configPara.ECVersion = ec.ECVersion;
                        configPara.FanNo = ec.FanNo;
                        configPara.SetMode = ec.FanSetModel;
                        configPara.FanSet = ec.FanSet;
                        configPara.FanDuty = ec.FanDuty;
                        configParaList.Add(configPara);
                    }
                    iFanDutyModify.WriteCfgFile(currentDirectory + "ecview.cfg", configParaList);
                }
                ecviewDataList[i].UpdateFlag = false;
            }
        }
        ////////////////////////////////////////////////////////界面事件////////////////////////////////////////////////////////
        /// <summary>
        /// DataGrid行点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ECDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point aP = e.GetPosition(ECDataGrid);
            IInputElement obj = ECDataGrid.InputHitTest(aP);
            DependencyObject target = obj as DependencyObject;

            while (target != null)
            {
                if (target is DataGridRow)
                {
                    break;
                }
                target = VisualTreeHelper.GetParent(target);
            }
            if(target != null)
            {
                _ecEditorLoader(target as DataGridRow);
            }
        }

        private void _ecEditorLoader(DataGridRow row)
        {
            //读取选择行的参数
            index = row.GetIndex();
            ECViewBinding selectFan = (ECViewBinding)row.Item;
            int fanduty = iFanDutyModify.GetTempFanDuty(index + 1)[2];
            //加载窗体
            ECEditor ecWindow = new ECEditor(this, index, fanduty, selectFan.FanSetModel);
            ecWindow.ShowDialog();
        }
        /// <summary>
        /// 关于提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ECView\n版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() +
                "\n作者：YcraD\n鸣谢：特别的帅（神舟笔记本吧）\n说明：本程序主要实现Clevo模具风扇转速控制，部分灵感来源于“特别的帅”，在此感谢",
                "关于", MessageBoxButton.OK, MessageBoxImage.Information);
        } 
        /// <summary>
        /// 退出程序
        /// </summary>
        private void _exit()
        {
            System.Windows.Application.Current.Shutdown();
        }
        /// <summary>
        /// CPU温度更新线程
        /// </summary>
        private void _getTemp_Task()
        {
            tempGetter = new Task<int>(() => { return _tempGetter(); });
            tempGetter.Start();
        }
        /// <summary>
        /// 更新CPU温度
        /// </summary>
        private int _tempGetter()
        {
            try
            {
                while (true)
                {
                    int[] temp = iFanDutyModify.GetTempFanDuty(1);
                    for (int i = 0; i < ecviewDataList.Count; i++)
                    {
                        ecviewDataList[i].FanDuty = iFanDutyModify.GetTempFanDuty(i + 1)[2];
                        ecviewDataList[i].FanDutyStr = ecviewDataList[i].FanDuty + "%";
                    }
                    ecviewData.CpuRemote = temp[0] + "℃";
                    ecviewData.CpuLocal = temp[1] + "℃";
                    Thread.Sleep(5000);
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}

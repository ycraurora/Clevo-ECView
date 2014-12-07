using ECView.DataDefinitions;
using ECView.Module;
using ECView.Pages.Binding;
using ECView.Pages.Windows;
using ECView.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ECView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //功能接口
        IFanDutyModify iFanDutyModify;
        //主窗口绑定
        ECViewBinding ecviewData;
        //数据列表绑定
        ECViewCollec ecviewDataList;
        //当前工作目录
        string currentDirectory;
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
                    ecviewData.CpuRemote = ecData[1] + "℃";
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
                    ecviewData.CpuRemote = ecData[1] + "℃";
                    ecviewData.FanDutyStr = ecData[2] + "%";
                    ecviewData.FanDuty = ecData[2];
                    ecviewData.FanNo = i + 1;
                    ecviewData.FanSet = "未设置";
                    ecviewData.FanSetModel = 0;
                    ecviewData.Ope = "设置";
                    ecviewData.UpdateFlag = false;
                    ecviewDataList.Add(ecviewData);
                }
            }
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
                    List<ConfigPara> configParaList = new List<ConfigPara>();
                    foreach (ECViewBinding ec in ecviewDataList)
                    {
                        ConfigPara configPara = new ConfigPara();
                        configPara.FanNo = ec.FanNo;
                        configPara.SetMode = ec.FanSetModel;
                        configPara.FanSet = ec.FanSet;
                        configPara.FanDuty = ec.FanDuty;
                        configParaList.Add(configPara);
                    }
                    //获取当前路径
                    string currentDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                    iFanDutyModify.WriteCfgFile(currentDirectory + "ecview.cfg", configParaList);
                    Thread.Sleep(1000);
                }
                ecviewDataList[i].UpdateFlag = false;
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
            //加载窗体
            ECEditor ecWindow = new ECEditor(this, index, fanduty, selectFan.FanSetModel);
            ecWindow.ShowDialog();
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
        /// <summary>
        /// 添加受控风扇
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            int newFanNo = ecviewDataList.Count + 1;
            if (newFanNo > 4)
            {
                MessageBox.Show("无法增加更多受控风扇", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                MessageBoxResult res = MessageBox.Show("增加受控风扇？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.OK)
                {

                    ECViewBinding ecviewBinding = new ECViewBinding();
                    int[] ecData = iFanDutyModify.GetTempFanDuty(newFanNo);
                    ecviewBinding.CpuRemote = ecData[1] + "℃";
                    ecviewBinding.FanDutyStr = ecData[2] + "%";
                    ecviewBinding.FanDuty = ecData[2];
                    ecviewBinding.FanNo = newFanNo;
                    ecviewBinding.FanSet = "未设置";
                    ecviewBinding.FanSetModel = 0;
                    ecviewBinding.Ope = "设置";
                    ecviewBinding.UpdateFlag = false;
                    ecviewDataList.Add(ecviewBinding);
                }
            }
        }
        ////////////////////////////////////////////////////////私有方法////////////////////////////////////////////////////////
        /// <summary>
        /// 退出程序
        /// </summary>
        private void _exit()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}

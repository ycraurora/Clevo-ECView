using ECView.Module;
using ECView.Pages.Binding;
using System;
using System.IO;
using System.Windows;

namespace ECView.Pages.Windows
{
    /// <summary>
    /// Interaction logic for ECEditor.xaml
    /// </summary>
    public partial class ECEditor : Window
    {
        ECEditorBinding ecBinding;
        MainWindow main;
        IFanDutyModify iFanDutyModify;
        private int index;
        private int fanSetModel;
        private string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        public ECEditor(MainWindow main, int index, int fanduty, int fanSetModel)
        {
            InitializeComponent();
            ecBinding = (ECEditorBinding)ECEditGrid.DataContext;
            iFanDutyModify = ModuleFactory.GetFanDutyModifyModule();
            this.main = main;
            this.index = index;
            this.fanSetModel = fanSetModel;
            //界面初始化
            InitEcData(fanduty);
        }
        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="fanduty"></param>
        private void InitEcData(int fanduty)
        {
            int fanNo = index + 1;
            ecBinding.FanNo = "当前风扇号：" + fanNo;
            ecBinding.FanDuty = fanduty;
            switch (fanSetModel)
            {
                case 1:
                    AutoChkBox.IsChecked = true;
                    break;
                case 2:
                    ManuChkBox.IsChecked = true;
                    break;
                case 3:
                    InteChkBox.IsChecked = true;
                    break;
                default:
                    ManuChkBox.IsChecked = true;
                    break;
            }
        }
        //////////////////////////////////////////////////////界面事件//////////////////////////////////////////////////////

        /// <summary>
        /// 自动调节
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoChecked(object sender, RoutedEventArgs e)
        {
            //禁用手动与智能调节
            ManuSet.IsEnabled = false;
            InteSet.IsEnabled = false;
            //设置模式
            fanSetModel = 1;
        }
        /// <summary>
        /// 手动调节
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManuChecked(object sender, RoutedEventArgs e)
        {
            //启用手动调节
            ManuSet.IsEnabled = true;
            //禁用智能调节
            InteSet.IsEnabled = false;
            //设置模式
            fanSetModel = 2;
        }
        /// <summary>
        /// 智能调节
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InteChecked(object sender, RoutedEventArgs e)
        {
            //禁用手动调节
            ManuSet.IsEnabled = false;
            //启用智能调节
            InteSet.IsEnabled = true;
            //设置模式
            fanSetModel = 3;
        }
        /// <summary>
        /// 事件--取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //关闭窗口
            this.Close();
        }
        /// <summary>
        /// 事件--确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (fanSetModel == 1)
            {
                main.ECViewDataCollec[index].FanSet = "自动调节";
                main.ECViewDataCollec[index].FanSetModel = 1;
                iFanDutyModify.SetFanduty(index + 1, 0, true);
                main.ECViewDataCollec[index].UpdateFlag = true;

                //关闭窗口
                this.Close();
            }
            else if (fanSetModel == 2)
            {
                main.ECViewDataCollec[index].FanSet = "手动调节";
                main.ECViewDataCollec[index].FanSetModel = 2;
                main.ECViewDataCollec[index].FanDuty = ecBinding.FanDuty;
                main.ECViewDataCollec[index].FanDutyStr = ecBinding.FanDuty + "%";
                iFanDutyModify.SetFanduty(index + 1, (int)(ecBinding.FanDuty*2.55m), false);
                main.ECViewDataCollec[index].UpdateFlag = true;

                //关闭窗口
                this.Close();
            }
            else if (fanSetModel == 3)
            {
                main.ECViewDataCollec[index].FanSet = "智能调节";
                main.ECViewDataCollec[index].FanSetModel = 3;
                if (ecBinding.FilePath == null || ecBinding.FilePath == "")
                {
                    MessageBox.Show("请选择配置文件", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    main.ECViewDataCollec[index].UpdateFlag = true;
                    MessageBox.Show("智能调节将在程序关闭后启用", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);

                    //关闭窗口
                    this.Close();
                }
            }
        }
        /// <summary>
        /// 事件--选择配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSelect_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.InitialDirectory = Directory.GetCurrentDirectory();//初始目录
            fileDialog.DefaultExt = ".xml"; // 默认文件类型
            fileDialog.Filter = "XML Files (.xml)|*.xml";
            fileDialog.Multiselect = false;
            //加入选择的文件信息
            if (fileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = fileDialog.FileName;//选择配置文件
                    //风扇号
                    int fanNo = index + 1;
                    //目标文件绝对路径
                    string targetPath = path + "conf\\Configuration_" + fanNo + ".xml";
                    //检测目标文件夹是否存在
                    if (!Directory.Exists(path + "conf\\"))
                    {
                        //若不存在则建立文件夹
                        Directory.CreateDirectory(path + "conf\\");
                    }
                    //复制配置文件（覆盖同名文件）
                    File.Copy(fileDialog.FileName, targetPath, true);
                    ecBinding.FilePath = filePath;
                }
                catch (Exception ee)
                {
                    MessageBox.Show("文件读取错误！错误原因" + ee.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}

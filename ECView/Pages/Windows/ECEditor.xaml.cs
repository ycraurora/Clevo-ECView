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
using System.Windows.Shapes;
using ECView.Pages.Binding;
using System.IO;
using System.Windows.Threading;

namespace ECView.Pages.Windows
{
    /// <summary>
    /// Interaction logic for ECEditor.xaml
    /// </summary>
    public partial class ECEditor : Window
    {
        ECEditorBinding ecBinding;
        MainWindow main;
        int index;
        int fanSetModel;

        public ECEditor(MainWindow main, int index, int fanduty, int fanSetModel)
        {
            InitializeComponent();
            ecBinding = (ECEditorBinding)ECEditGrid.DataContext;
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
            if (fanSetModel == 1)
            {
                AutoChkBox.IsChecked = true;
            }
            else if (fanSetModel == 2)
            {
                ManuChkBox.IsChecked = true;
            }
            else if (fanSetModel == 3)
            {
                InteChkBox.IsChecked = true;
            }
            else
            {
                ManuChkBox.IsChecked = true;
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
                ECLib.FanCtrl.SetFanduty(index + 1, 0, true);
                main.ECViewDataCollec[index].UpdateFlag = true;

                //关闭窗口
                this.Close();
            }
            else if (fanSetModel == 2)
            {
                main.ECViewDataCollec[index].FanSet = "手动调节";
                main.ECViewDataCollec[index].FanSetModel = 2;
                main.ECViewDataCollec[index].FanDuty = ecBinding.FanDuty;
                main.ECViewDataCollec[index].FanDutyStr = ecBinding.FanDuty + "℃";
                ECLib.FanCtrl.SetFanduty(index + 1, (int)(ecBinding.FanDuty * 2.55m), false);
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
                    string targetPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "conf\\Configuration_" + fanNo + ".xml";
                    //检测目标文件夹是否存在
                    if (!Directory.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "conf\\"))
                    {
                        //若不存在则建立文件夹
                        Directory.CreateDirectory(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "conf\\");
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

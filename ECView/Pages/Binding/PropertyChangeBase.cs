using System.ComponentModel;

namespace ECView.Pages.Binding
{
    /// <summary>
    /// 属性更新基类
    /// </summary>
    public class PropertyChangeBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性更新委托
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="propertyName"></param>
        public void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

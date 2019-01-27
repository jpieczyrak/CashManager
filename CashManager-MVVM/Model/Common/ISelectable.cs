using System.ComponentModel;

namespace CashManager.WPF.Model.Common
{
    public interface ISelectable : INotifyPropertyChanged
    {
        string Name { get; }
        bool IsSelected { get; set; }
    }
}
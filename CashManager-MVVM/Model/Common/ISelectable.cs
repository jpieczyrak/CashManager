using System.ComponentModel;

namespace CashManager_MVVM.Model.Common
{
    public interface ISelectable : INotifyPropertyChanged
    {
        string Name { get; }
        bool IsSelected { get; set; }
    }
}
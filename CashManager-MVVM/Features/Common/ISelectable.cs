using System.ComponentModel;

namespace CashManager_MVVM.Features.Common
{
    public interface ISelectable : INotifyPropertyChanged
    {
        string Name { get; }
        bool IsSelected { get; set; }
    }
}
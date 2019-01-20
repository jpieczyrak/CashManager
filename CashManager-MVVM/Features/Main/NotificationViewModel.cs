using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Main
{
    public class NotificationViewModel : ViewModelBase
    {
        private string _updateStatus;

        public string UpdateStatus
        {
            get => _updateStatus;
            set => Set(ref _updateStatus, value);
        }
    }
}
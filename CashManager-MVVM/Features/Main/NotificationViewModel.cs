using CashManager_MVVM.Messages.App;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Main
{
    public class NotificationViewModel : ViewModelBase
    {
        private string _updateStatus;

        private string _tooltip;

        public string UpdateStatus
        {
            get => _updateStatus;
            set => Set(ref _updateStatus, value);
        }

        public string Tooltip
        {
            get => _tooltip;
            set => Set(ref _tooltip, value);
        }

        public NotificationViewModel() { MessengerInstance.Register<ApplicationUpdateMessage>(this, Update); }

        private void Update(ApplicationUpdateMessage message)
        {
            UpdateStatus = message.Content;
            Tooltip = message.Tooltip;
        }
    }
}
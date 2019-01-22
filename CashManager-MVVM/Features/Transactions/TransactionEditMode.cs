using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Transactions
{
    public class TransactionEditMode : ObservableObject
    {
        private TransactionEditModes _type;
        private bool _isVisible = true;
        private bool _isSelected;
        private string _tooltip;
        private string _name;

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public TransactionEditModes Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        public string Tooltip
        {
            get => _tooltip;
            set => Set(ref _tooltip, value);
        }
    }
}
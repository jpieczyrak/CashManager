using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model.Selectors
{
    public class BaseSelector : ObservableObject
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set => Set(nameof(IsChecked), ref _isChecked, value);
        }

        public string Description { get; protected set; }
    }
}
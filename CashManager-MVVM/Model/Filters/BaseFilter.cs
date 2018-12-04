using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model.Filters
{
    public class BaseFilter : ObservableObject
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
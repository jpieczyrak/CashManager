namespace CashManager_MVVM.Model.Common
{
    public class BaseSelectable : BaseObservableObject, ISelectable
    {
        private string _name;
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set => Set(nameof(Name), ref _name, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(nameof(IsSelected), ref _isSelected, value);
        }

    }
}
namespace CashManager.Model.Common
{
    public sealed class Selectable : BaseObservableObject, ISelectable
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                bool actual = IsPropertyChangedEnabled;
                IsPropertyChangedEnabled = false;
                Set(nameof(IsSelected), ref _isSelected, value);
                IsPropertyChangedEnabled = actual;
            }
        }

        public BaseObservableObject Value { get; }

        private Selectable() { }

        public Selectable(BaseObservableObject value)
        {
            Id = value.Id;
            Value = value;
            Name = value.Name;
        }

        #region Override

        public override string ToString() { return Name; }

        #endregion
    }
}
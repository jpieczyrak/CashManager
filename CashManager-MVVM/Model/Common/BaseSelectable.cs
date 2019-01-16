using System;

namespace CashManager_MVVM.Model.Common
{
    public class BaseSelectable : BaseObservableObject, ISelectable
    {
        private string _name;
        private bool _isSelected;

        public virtual string Name
        {
            get => _name;
            set => Set(nameof(Name), ref _name, value);
        }

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

        public BaseSelectable(Guid id) { Id = id; }

        protected BaseSelectable() { }

        #region Override

        public override string ToString() => Name;

        #endregion
    }
}
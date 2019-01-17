using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
{
    public class ExpandableCategory : BaseObservableObject
    {
        private ExpandableCategory _parent;
        private bool _isExpanded;
        private bool _isSelected;

        /// <summary>
        /// Used for tree view purpose
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }

        public ExpandableCategory Parent
        {
            get => _parent;
            set => Set(nameof(Parent), ref _parent, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public TrulyObservableCollection<ExpandableCategory> Children { get; set; } = new TrulyObservableCollection<ExpandableCategory>();

        #region Override

        public override string ToString() { return Name; }

        #endregion
    }
}
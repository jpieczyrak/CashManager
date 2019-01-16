using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
{
    public class TransactionType : BaseObservableObject
    {
        private bool _income;
        private bool _outcome;
        private bool _isDefault;

        private bool _isTransfer;

        public bool Income
        {
            get => _income;
            set => Set(nameof(Income), ref _income, value);
        }

        public bool Outcome
        {
            get => _outcome;
            set => Set(nameof(Outcome), ref _outcome, value);
        }

        public bool IsDefault
        {
            get => _isDefault;
            set => Set(nameof(IsDefault), ref _isDefault, value);
        }

        public bool IsTransfer
        {
            get => _isTransfer;
            set => Set(ref _isTransfer, value);
        }

        #region Override

        public override string ToString() { return Name; }

        #endregion
    }
}
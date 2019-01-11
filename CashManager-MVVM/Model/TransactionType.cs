using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
{
    public class TransactionType : BaseSelectable
    {
        private bool _income;
        private bool _outcome;
        private bool _isDefault;

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
    }
}
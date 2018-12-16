using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
{
    /// <summary>
    /// Stores name of "part of" your wallet.
    /// You can have more than one Stock in your Wallet (like bank account, physical wallet, second bank acc ect)
    /// </summary>
    public class Stock : BaseSelectable
    {
        private bool _isUserStock;
        private Balance _balance;

        public Stock()
        {
            _balance = new Balance();
        }

        public bool IsUserStock
        {
            get => _isUserStock;
            set
            {
                Set(nameof(IsUserStock), ref _isUserStock, value);
                RaisePropertyChanged(nameof(IsEditable));
            }
        }

        public Balance Balance
        {
            get => _balance;
            set => Set(nameof(Balance), ref _balance, value);
        }

        public bool IsEditable => !IsUserStock;

        #region Override

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
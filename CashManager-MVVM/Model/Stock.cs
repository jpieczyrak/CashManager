namespace CashManager_MVVM.Model
{
    /// <summary>
    /// Stores name of "part of" your wallet.
    /// You can have more than one Stock in your Wallet (like bank account, physical wallet, second bank acc ect)
    /// </summary>
    public class Stock : BaseObservableObject
    {
        private string _name;
        private bool _isUserStock;
        private double _balance;

        public string Name
        {
            get => _name;
            set => Set(nameof(Name), ref _name, value);
        }

        public bool IsUserStock
        {
            get => _isUserStock;
            set => Set(nameof(IsUserStock), ref _isUserStock, value);
        }

        public double Balance
        {
            get => _balance;
            set => Set(nameof(Balance), ref _balance, value);
        }

        #region Override

        public override string ToString()
        {
            return $"{Name}-{Id}";
        }

        #endregion
    }
}
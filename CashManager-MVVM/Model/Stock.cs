using System;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model
{
    /// <summary>
    /// Stores name of "part of" your wallet.
    /// You can have more than one Stock in your Wallet (like bank account, phisic wallet, second bank acc ect)
    /// </summary>
    public class Stock : ObservableObject
    {
        private string _name;
        private bool _isUserStock;

        /// <summary>
        /// Only for db purpose
        /// </summary>
        public Guid Id { get; private set; } = Guid.NewGuid();

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

       #region Override

        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name}-{Id}";
        }

        #endregion
    }
}
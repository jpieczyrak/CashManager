﻿using System;
using System.ComponentModel;

using CashManager.Model.Common;

using log4net;

namespace CashManager.Model
{
    /// <summary>
    /// Stores name of "part of" your wallet.
    /// You can have more than one Stock in your Wallet (like bank account, physical wallet, second bank acc ect)
    /// </summary>
    public sealed class Stock : BaseObservableObject
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(Stock)));

        private bool _isUserStock;
        private Balance _balance;
        private decimal _userOwnershipPercent = 100;

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
            set
            {
                if (_balance != null) _balance.PropertyChanged -= BalanceOnPropertyChanged;
                Set(nameof(Balance), ref _balance, value);
                if (_balance != null) _balance.PropertyChanged += BalanceOnPropertyChanged;
                RaisePropertyChanged(nameof(UserBalance));
            }
        }

        public decimal UserOwnershipPercent
        {
            get => _userOwnershipPercent;
            set
            {
                Set(nameof(UserOwnershipPercent), ref _userOwnershipPercent, value);
                RaisePropertyChanged(nameof(UserBalance));
            }
        }

        public bool IsEditable => !IsUserStock;

        public decimal UserBalance
        {
            get
            {
                try
                {
                    return UserOwnershipPercent != 0m ? Balance.Value * UserOwnershipPercent / 100m : 0m;
                }
                catch (OverflowException e)
                {
                    _logger.Value.Info("User balance overflow", e);
                    return 0m;
                }
            }
        }

        public Stock() : this(Guid.NewGuid()) { }

        public Stock(Guid id) : base(id)
        {
            LastEditDate = DateTime.Now;
            Balance = new Balance();
        }

        ~Stock()
        {
            _balance.PropertyChanged -= BalanceOnPropertyChanged;
        }

        private void BalanceOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            RaisePropertyChanged(nameof(Balance));
        }

        #region Override

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
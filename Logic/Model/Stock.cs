using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Logic.Annotations;
using Logic.LogicObjectsProviders;
using Logic.TransactionManagement.TransactionElements;
using Logic.Utils;

namespace Logic.Model
{
    /// <summary>
    ///     Stores name, starting value and actual value of "part of" your wallet.
    ///     You can have more than one Stock in your Wallet (like bank account, phisic wallet, second bank acc ect)
    ///     Actual value is based on starting value + income - outcome from transactions.
    /// </summary>
    public class Stock : INotifyPropertyChanged
    {
        private string _name;
        private double _startingValue;
        private bool _isUserStock;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public Guid Id { get; private set; }

        /// <summary>
        ///     Value of stock when stock is being added to wallet (in app)
        /// </summary>
        public double StartingValue
        {
            get { return _startingValue; }
            set
            {
                _startingValue = value;
                OnPropertyChanged(nameof(StartingValue));
            }
        }

        /// <summary>
        ///     Value calculated for choosen date, based on transactions done on stock since created
        /// </summary>
        public double ActualValue { get; private set; }

        public static Stock Unknown { get; } = new Stock("Unknown", 0) { Id = Guid.Empty };

        public bool IsUserStock
        {
            get { return _isUserStock; }
            set
            {
                _isUserStock = value;
                OnPropertyChanged(nameof(IsUserStock));
            }
        }

        private Stock() { }

        public Stock(string name, float startingValue)
        {
            Name = name;
            StartingValue = startingValue;
            Id = Guid.NewGuid();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public double GetActualValue(IEnumerable<Transaction> transactions, TimeFrame timeframe)
        {
            CalculateActualValue(transactions, timeframe);
            return ActualValue;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CalculateActualValue(IEnumerable<Transaction> transactions, TimeFrame timeframe)
        {
            ActualValue = StartingValue;
            foreach (var transaction in transactions)
            {
                if (timeframe.Contains(transaction.Date))
                {
                    bool income = StockProvider.GetStock(transaction.TargetStockId).Equals(this);
                    bool outcome = transaction.TransactionSoucePayments.Any(payment => StockProvider.GetStock(payment.StockId).Equals(this));

                    if (income)
                    {
                        ActualValue += transaction.Value;
                    }

                    if (outcome)
                    {
                        foreach (var payment in transaction.TransactionSoucePayments)
                        {
                            if (StockProvider.GetStock(payment.StockId).Equals(this))
                            {
                                double value = payment.PaymentType == ePaymentType.Value
                                                   ? payment.Value
                                                   : payment.Value * transaction.Value / 100;

                                ActualValue -= value;
                            }
                        }
                    }
                }
            }
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
            return Name;
        }

        #endregion
    }
}
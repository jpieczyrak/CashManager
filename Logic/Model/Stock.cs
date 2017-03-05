using System;
using System.Linq;
using System.Runtime.Serialization;

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
    public class Stock
    {
        public string Name { get; set; }
        
        public Guid Id { get; private set; }

        /// <summary>
        ///     Value of stock when stock is being added to wallet (in app)
        /// </summary>
        public double StartingValue { get; set; }

        /// <summary>
        ///     Value calculated for choosen date, based on transactions done on stock since created
        /// </summary>
        public double ActualValue { get; private set; }

        public static Stock Unknown { get; private set; } = new Stock("Unknown", 0, Guid.Empty);

        public bool IsUserStock { get; set; }

        private Stock() { }

        public Stock(string name, float startingValue)
        {
            Name = name;
            StartingValue = startingValue;
            Id = Guid.NewGuid();
        }

        public Stock(string name, float startingValue, Guid id) : this(name, startingValue)
        {
            Id = id;
        }

        public double GetActualValue(Transactions transactions, TimeFrame timeframe)
        {
            CalculateActualValue(transactions, timeframe);
            return ActualValue;
        }

        private void CalculateActualValue(Transactions transactions, TimeFrame timeframe)
        {
            ActualValue = StartingValue;
            foreach (var transaction in transactions.TransactionsList)
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
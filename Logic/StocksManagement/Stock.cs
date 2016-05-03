using System;
using System.Linq;
using System.Runtime.Serialization;
using Logic.TransactionManagement;

namespace Logic.StocksManagement
{
    /// <summary>
    /// Stores name, starting value and actual value of "part of" your wallet.
    /// You can have more than one Stock in your Wallet (like bank account, phisic wallet, second bank acc ect)
    /// Actual value is based on starting value + income - outcome from transactions.
    /// </summary>
    [DataContract(Namespace = "")]
    public class Stock
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid Id { get; set; }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Value of stock when stock is being added to wallet (in app)
        /// </summary>
        [DataMember]
        public double StartingValue { get; set; }

        /// <summary>
        /// Value calculated for choosen date, based on transactions done on stock since created
        /// </summary>
        [DataMember]
        public double ActualValue { get; set; }

        public static Stock Unknown { get; private set; } = new Stock(Guid.Empty);

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
                    bool income = transaction.TargetStock.Equals(this);
                    bool outcome = transaction.TransactionSoucePayments.Any(payment => payment.Stock.Equals(this));

                    if (income)
                    {
                        ActualValue += transaction.Value;
                    }

                    if (outcome)
                    {
                        foreach (TransactionPartPayment payment in transaction.TransactionSoucePayments)
                        {
                            if (payment.Stock.Equals(this))
                            {
                                double value = payment.PaymentType == ePaymentType.Value
                                    ? payment.Value
                                    : payment.Value*transaction.Value/100;

                                ActualValue -= value;
                            }
                        }
                    }
                }
            }
        }

        public Stock(string name, float startingValue)
        {
            Name = name;
            StartingValue = startingValue;
            Id = Guid.NewGuid();
        }

        private Stock(Guid empty)
        {
            Name = "Unknown";
            Id = empty;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Stock))    return false;

            return Id == ((Stock) obj).Id;
        }
    }
}
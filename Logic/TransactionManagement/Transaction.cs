using System;
using System.Collections.ObjectModel;
using Logic.StocksManagement;
using Logic.ValueCalculationStrategies;

namespace Logic.TransactionManagement
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public eTransactionType Type { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// Contribution value (0, 100 for percent; X for Value)
        /// </summary>
        public float Contribution { get; set; } = 100;

        public ePaymentType ContributionType { get; set; } = ePaymentType.Percent;

        public string Title { get; set; }

        public string Note { get; set; }

        IValueCalculationStrategy _strategy;

        public ObservableCollection<TransactionPartPayment> TransactionSoucePayments { get; set; } = new ObservableCollection<TransactionPartPayment>();

        public Stock TargetStock { get; set; }

        public float Value => _strategy.CalculateValue(Type, TransactionSoucePayments, Contribution, ContributionType);
        public ObservableCollection<Subtransation> Subtransactions { get; set; } = new ObservableCollection<Subtransation>();

        public Transaction(eTransactionType type, DateTime date, string title, string note)
        {
            _strategy = new BasicCalculationStrategy();
            Type = type;
            Date = date;
            Title = title;
            Note = note;
            Id = Guid.NewGuid();
        }

        public Transaction()
        {
            _strategy = new BasicCalculationStrategy();
            Type = eTransactionType.Buy;
            Date = DateTime.Now;
            Id = Guid.NewGuid();
        }
    }
}

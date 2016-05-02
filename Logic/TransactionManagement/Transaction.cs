using System;
using System.Collections.ObjectModel;
using Logic.ValueCalculationStrategies;

namespace Logic.TransactionManagement
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public eTransactionType Type { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// Full transaction value (total payment)
        /// </summary>
        private float RawValue { get; set; }

        /// <summary>
        /// Contribution value (0, 100 for percent; X for Value)
        /// </summary>
        public float Contribution { get; set; } = 100;

        public ePaymentType ContributionType { get; set; } = ePaymentType.Percent;

        public string Title { get; set; }

        public string Note { get; set; }

        IValueCalculationStrategy _strategy;

        public ObservableCollection<TransactionPartPayment> TransactionSoucePayments { get; set; } = new ObservableCollection<TransactionPartPayment>();

        public ObservableCollection<TransactionPartPayment> TransactionTargetPayments { get; set; } = new ObservableCollection<TransactionPartPayment>();

        public float Value => _strategy.CalculateValue(RawValue, Contribution);
        public ObservableCollection<Subtransation> Subtransactions { get; set; }

        public Transaction(eTransactionType type, DateTime date, float rawValue, string title, string note)
        {
            _strategy = new BasicCalculationStrategy();
            Type = type;
            Date = date;
            RawValue = rawValue;
            Title = title;
            Note = note;
            Id = Guid.NewGuid();
        }

        public Transaction(eTransactionType type, DateTime date, float rawValue, float contribution, string title, string note) : this(type, date, rawValue, title, note)
        {
            Contribution = contribution;
        }

        public Transaction()
        {
            _strategy = new BasicCalculationStrategy();
            Type = eTransactionType.Buy;
            Date = DateTime.Now;
            Id = Guid.NewGuid();
        }

        //TODO: delete
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", RawValue, Date, Note, Title, Value, Type);
        }
    }
}

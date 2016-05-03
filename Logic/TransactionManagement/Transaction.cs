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

        public string Title { get; set; }

        public string Note { get; set; }

        IValueCalculationStrategy _strategy;

        public ObservableCollection<TransactionPartPayment> TransactionSoucePayments { get; set; } = new ObservableCollection<TransactionPartPayment>();

        public Stock TargetStock { get; set; }

        public double Value => _strategy.CalculateValue(Type, TransactionSoucePayments, Subtransactions);
        public ObservableCollection<Subtransaction> Subtransactions { get; set; } = new ObservableCollection<Subtransaction>();

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

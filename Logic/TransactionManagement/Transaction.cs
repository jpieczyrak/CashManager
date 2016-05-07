using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Logic.Annotations;
using Logic.StocksManagement;
using Logic.Utils;
using Logic.ValueCalculationStrategies;

namespace Logic.TransactionManagement
{
    [DataContract(Namespace = "")]
    public class Transaction : INotifyPropertyChanged
    {
        private DateTime _date;
        private string _note;
        private IValueCalculationStrategy _strategy;

        private TrulyObservableCollection<Subtransaction> _subtransactions =
            new TrulyObservableCollection<Subtransaction>();

        private Stock _targetStock;
        private string _title;

        private TrulyObservableCollection<TransactionPartPayment> _transactionSoucePayments =
            new TrulyObservableCollection<TransactionPartPayment>();

        private eTransactionType _type;

        /// <summary>
        ///     TODO: remove after loading from file.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="title"></param>
        /// <param name="note"></param>
        public Transaction(eTransactionType type, DateTime date, string title, string note) : this()
        {
            _strategy = new BasicCalculationStrategy();
            Type = type;
            Date = date;
            Title = title;
            Note = note;
        }

        public Transaction()
        {
            _strategy = new BasicCalculationStrategy();
            Type = eTransactionType.Buy;
            Date = DateTime.Now;

            LastEditDate = CreationDate = DateTime.Now;

            _transactionSoucePayments.CollectionChanged += CollectionChanged;
            _subtransactions.CollectionChanged += CollectionChanged;
        }

        [DataMember]
        public eTransactionType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public TrulyObservableCollection<TransactionPartPayment> TransactionSoucePayments
        {
            get { return _transactionSoucePayments; }
            set
            {
                _transactionSoucePayments = value;
                _transactionSoucePayments.CollectionChanged += CollectionChanged;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public Stock TargetStock
        {
            get { return _targetStock; }
            set
            {
                _targetStock = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public double Value
        {
            get
            {
                if (_strategy == null)
                {
                    _strategy = new BasicCalculationStrategy();
                }
                return _strategy.CalculateValue(Type, TransactionSoucePayments, Subtransactions);
            }
            set { }
        }

        public double ValueAsProfit
        {
            get
            {
                return Type == eTransactionType.Buy || Type == eTransactionType.Reinvest 
                    ? -Value 
                    : (Type != eTransactionType.Transfer ? Value : 0);
            }
        }

        [DataMember]
        public TrulyObservableCollection<Subtransaction> Subtransactions
        {
            get { return _subtransactions; }
            set
            {
                _subtransactions = value;
                _subtransactions.CollectionChanged += CollectionChanged;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public DateTime LastEditDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(ValueAsProfit));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            LastEditDate = DateTime.Now;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Checks if subtransaction value = income value, 
        /// if not - addes new source (from unknown stock) to fullfill transaction
        /// </summary>
        public void Validate()
        {
            double subtransactionCost = _subtransactions.Sum(subtransaction => subtransaction.Value);

            //TODO: Fix (can be unsafe (wrong value) if there would be some ~incomes (same source as target))
            double value =
                _transactionSoucePayments.Sum(
                    payment =>
                        payment.PaymentType.Equals(ePaymentType.Value)
                            ? payment.Value
                            : subtransactionCost * payment.Value / 100);

            if (Math.Abs(subtransactionCost - value) > 0.0001)
            {
                double missingValue = subtransactionCost - value;
                _transactionSoucePayments.Add(new TransactionPartPayment(Stock.Unknown, missingValue, ePaymentType.Value));
            }
        }
    }
}
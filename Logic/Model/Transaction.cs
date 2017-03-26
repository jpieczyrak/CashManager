using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using Logic.LogicObjectsProviders;
using Logic.Properties;
using Logic.StocksManagement;
using Logic.TransactionManagement.TransactionElements;
using Logic.Utils;

namespace Logic.Model
{
    [DataContract(Namespace = "")]
    public class Transaction : INotifyPropertyChanged
    {
        private DateTime _date;
        private string _note;

        private TrulyObservableCollection<Subtransaction> _subtransactions =
            new TrulyObservableCollection<Subtransaction>();

        private Guid _targetStockId;
        private string _title;

        private TrulyObservableCollection<TransactionPartPayment> _transactionSoucePayments =
            new TrulyObservableCollection<TransactionPartPayment>();

        private eTransactionType _type;

        public eTransactionType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                OnPropertyChanged();
            }
        }

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

        public Guid TargetStockId
        {
            get { return _targetStockId; }
            set
            {
                _targetStockId = value;
                OnPropertyChanged();
            }
        }

        public double Value => Subtransactions?.Sum(subtransaction => subtransaction.Value) ?? 0;

        public double ValueAsProfit => Type == eTransactionType.Buy || Type == eTransactionType.Reinvest
                                           ? -Value
                                           : (Type != eTransactionType.Transfer ? Value : 0);

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

        public DateTime CreationDate { get; set; }

        public DateTime LastEditDate { get; set; }

        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        ///     TODO: remove after loading from file.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="title"></param>
        /// <param name="note"></param>
        public Transaction(eTransactionType type, DateTime date, string title, string note) : this()
        {
            Type = type;
            Date = date;
            Title = title;
            Note = note;
        }

        public Transaction()
        {
            Type = eTransactionType.Buy;
            Date = DateTime.Now;

            LastEditDate = CreationDate = DateTime.Now;

            _transactionSoucePayments.CollectionChanged += CollectionChanged;
            _subtransactions.CollectionChanged += CollectionChanged;
        }

        public Transaction(eTransactionType transactionType, DateTime date, string title, string note, Stock stock, DateTime creationDate,
            DateTime lastEdit, List<Subtransaction> subtransactions, List<TransactionPartPayment> partPayments)
        {
            Type = transactionType;
            Date = date;
            Title = title;
            Note = note;
            TargetStockId = stock.Id;
            CreationDate = creationDate;
            LastEditDate = lastEdit;
            Subtransactions = new TrulyObservableCollection<Subtransaction>(subtransactions);
            TransactionSoucePayments = new TrulyObservableCollection<TransactionPartPayment>(partPayments);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        ///     Checks if subtransaction value = income value,
        ///     if not - addes new source (from unknown stock) to fullfill transaction
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
                _transactionSoucePayments.Add(new TransactionPartPayment(StockProvider.Default, missingValue, ePaymentType.Value));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            LastEditDate = DateTime.Now;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(ValueAsProfit));
        }

        #region Override

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetHashCode().Equals(GetHashCode());
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion

        public Transaction Clone()
        {
            return (Transaction) MemberwiseClone();
        }
    }
}
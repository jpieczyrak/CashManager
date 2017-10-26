using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using Logic.Properties;
using Logic.TransactionManagement.TransactionElements;
using Logic.Utils;

namespace Logic.Model
{
    [DataContract(Namespace = "")]
    public class Transaction : INotifyPropertyChanged
    {
        private DateTime _bookDate;
        private string _note;

        private TrulyObservableCollection<Subtransaction> _subtransactions =
            new TrulyObservableCollection<Subtransaction>();
        
        private string _title;
        
        private eTransactionType _type;
        private Stock _myStock;
        private Stock _externalStock;

        public eTransactionType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Date used for register moment.
        /// E.g. When your paid in april but it was payment for march [forgot / was late]. Payment was done in April, so the CreationDate will be, but you can set BookDate to March and calculate it as it should be
        /// </summary>
        public DateTime BookDate
        {
            get { return _bookDate; }
            set
            {
                _bookDate = value;
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

        /// <summary>
        /// Used for our stocks like: my wallet, my bank account etc
        /// </summary>
        public Stock MyStock
        {
            get { return _myStock; }
            set
            {
                _myStock = value;
                OnPropertyChanged(nameof(MyStock));
            }
        }

        /// <summary>
        /// Used for stocks like: shop, employer etc
        /// </summary>
        public Stock ExternalStock
        {
            get { return _externalStock; }
            set
            {
                _externalStock = value;
                OnPropertyChanged(nameof(ExternalStock));
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

        public DateTime CreationDate { get; private set; }

        public DateTime LastEditDate { get; private set; }
        
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        ///     TODO: remove after loading from file.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bookDate"></param>
        /// <param name="title"></param>
        /// <param name="note"></param>
        public Transaction(eTransactionType type, DateTime bookDate, string title, string note) : this()
        {
            Type = type;
            BookDate = bookDate;
            Title = title;
            Note = note;
        }

        public Transaction()
        {
            Type = eTransactionType.Buy;
            BookDate = DateTime.Now;

            BookDate = LastEditDate = CreationDate = DateTime.Now;
            
            _subtransactions.CollectionChanged += CollectionChanged;
        }

        public Transaction(eTransactionType transactionType, DateTime bookDate, string title, string note, DateTime creationDate, List<Subtransaction> subtransactions, Stock myStock, Stock externalStock)
        {
            Type = transactionType;
            BookDate = bookDate;
            Title = title;
            Note = note;
            LastEditDate = CreationDate = creationDate;
            Subtransactions = new TrulyObservableCollection<Subtransaction>(subtransactions);
            _myStock = myStock;
            _externalStock = externalStock;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        
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
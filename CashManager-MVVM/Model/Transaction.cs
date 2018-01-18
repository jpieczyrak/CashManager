using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using GalaSoft.MvvmLight;

using Logic.TransactionManagement.TransactionElements;
using Logic.Utils;

namespace CashManager_MVVM.Model
{
    public class Transaction : ObservableObject
    {
        private string _title;
        private string _note;

        private DateTime _bookDate;

        private Stock _userStock;
        private Stock _externalStock;
        private eTransactionType _type;

        private TrulyObservableCollection<Subtransaction> _subtransactions;
        private DateTime _lastEditDate;

        /// <summary>
        /// Date when transaction was first created within application
        /// </summary>
        public DateTime InstanceCreationDate { get; }

        /// <summary>
        /// Date when transaction was performed (in real life, like going to shop or receiving payment)
        /// </summary>
        public DateTime TransationSourceCreationDate { get; }

        /// <summary>
        /// Last time when transation was edited by user (within app)
        /// </summary>
        public DateTime LastEditDate
        {
            get => _lastEditDate;
            private set => Set(nameof(LastEditDate), ref _lastEditDate, value);
        }

        /// <summary>
        /// Unique id to recognize transactions
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Title of transaction
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(nameof(Title), ref _title, value);
        }

        /// <summary>
        /// Additional notes
        /// </summary>
        public string Note
        {
            get => _note;
            set => Set(nameof(Note), ref _note, value);
        }

        /// <summary>
        /// Used for our stocks like: my wallet, my bank account etc
        /// </summary>
        public Stock UserStock
        {
            get => _userStock;
            set => Set(nameof(UserStock), ref _userStock, value);
        }

        /// <summary>
        /// Used for stocks like: shop, employer etc
        /// </summary>
        public Stock ExternalStock
        {
            get => _externalStock;
            set => Set(nameof(ExternalStock), ref _externalStock, value);
        }

        /// <summary>
        /// List of subtransactions (like positions on bill)
        /// </summary>
        public TrulyObservableCollection<Subtransaction> Subtransactions
        {
            get => _subtransactions;
            set
            {
                Set(nameof(Subtransactions), ref _subtransactions, value);
                _subtransactions.CollectionChanged += CollectionChanged;
            }
        }

        /// <summary>
        /// Transaction type - buying/selling/working - for accounting purpose
        /// </summary>
        public eTransactionType Type
        {
            get => _type;
            set => Set(nameof(Type), ref _type, value);
        }

        /// <summary>
        /// Date used for register moment.
        /// E.g. When your paid in april but it was payment for march [forgot / was late]. Payment was done in April, so the
        /// CreationDate will be, but you can set BookDate to March and calculate it as it should be
        /// </summary>
        public DateTime BookDate
        {
            get => _bookDate;
            set => Set(nameof(BookDate), ref _bookDate, value);
        }

        /// <summary>
        /// Total value of transaction
        /// </summary>
        public double Value => Subtransactions?.Sum(subtransaction => subtransaction.Value.Value) ?? 0;

        /// <summary>
        /// Total value of transaction as profit of user (negative when buying, positive when receiving payments)
        /// </summary>
        public double ValueAsProfit => Type == eTransactionType.Buy || Type == eTransactionType.Reinvest
                                           ? -Value
                                           : (Type != eTransactionType.Transfer ? Value : 0);

        public Transaction()
        {
            Id = Guid.NewGuid();
            Type = eTransactionType.Buy;
            BookDate = LastEditDate = InstanceCreationDate = DateTime.Now;

            Subtransactions = new TrulyObservableCollection<Subtransaction>();
            PropertyChanged += OnPropertyChanged;
        }



        public Transaction Clone()
        {
            return (Transaction) MemberwiseClone();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            LastEditDate = DateTime.Now;
        }
        
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Value));
            RaisePropertyChanged(nameof(ValueAsProfit));
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
    }
}
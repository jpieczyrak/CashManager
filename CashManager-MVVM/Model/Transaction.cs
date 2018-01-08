using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using GalaSoft.MvvmLight;

using Logic.Model;
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
        private TrulyObservableCollection<Tag> _tags;
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
        /// Optional tags for whole transaction (like: buying PC 2015)
        /// </summary>
        public TrulyObservableCollection<Tag> Tags
        {
            get => _tags;
            set
            {
                Set(nameof(Tags), ref _tags, value);
                _tags.CollectionChanged += CollectionChanged;
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
        public double Value => Subtransactions?.Sum(subtransaction => subtransaction.Value) ?? 0;

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

            Tags = new TrulyObservableCollection<Tag>();
            Subtransactions = new TrulyObservableCollection<Subtransaction>();
            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Should be used only after parsing data or for test purpose.
        /// Otherwise please use paramless constructor
        /// </summary>
        /// <param name="transactionType">Tranasction type</param>
        /// <param name="sourceTransactionCreationDate">When transaction was performed</param>
        /// <param name="title">Title of transaction</param>
        /// <param name="note">Additional notes</param>
        /// <param name="subtransactions">Subtransactions - like positions from bill</param>
        /// <param name="userStock">User stock like wallet / bank account</param>
        /// <param name="externalStock">External stock like employer / shop</param>
        /// <param name="sourceInput">Text source of transaction (for parsing purpose) to provide unique id</param>
        public Transaction(eTransactionType transactionType, DateTime sourceTransactionCreationDate, string title, string note,
            List<Subtransaction> subtransactions, Stock userStock, Stock externalStock, string sourceInput)
        {
            Id = GenerateGUID(sourceInput);
            Type = transactionType;
            Title = title;
            Note = note;
            _bookDate = TransationSourceCreationDate = sourceTransactionCreationDate;
            LastEditDate = InstanceCreationDate = DateTime.Now;
            Subtransactions = new TrulyObservableCollection<Subtransaction>(subtransactions);
            Tags = new TrulyObservableCollection<Tag>();
            _userStock = userStock;
            _externalStock = externalStock;
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

        /// <summary>
        /// Generates GUID based on input (original transaction text - from excel / bank import etc)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Guid GenerateGUID(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                    return new Guid(hash);
                }
            }

            return Guid.NewGuid();
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
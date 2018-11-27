using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

using GalaSoft.MvvmLight;

using JetBrains.Annotations;

using LogicOld.TransactionManagement.TransactionElements;
using LogicOld.Utils;

namespace LogicOld.Model
{
    [DataContract(Namespace = "")]
    public class Transaction : ObservableObject
    {
        private string _title;
        private string _note;

        private DateTime _bookDate;

        private Stock _userStock;
        private Stock _externalStock;
        private eTransactionType _type;

        private TrulyObservableCollection<Position> _positions = new TrulyObservableCollection<Position>();
        private TrulyObservableCollection<Tag> _tags = new TrulyObservableCollection<Tag>();

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
        public DateTime LastEditDate { get; private set; }

        /// <summary>
        /// Unique id to recognize transactions
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Title of transaction
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Additional notes
        /// </summary>
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
        public Stock UserStock
        {
            get { return _userStock; }
            set
            {
                _userStock = value;
                OnPropertyChanged(nameof(UserStock));
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

        /// <summary>
        /// List of positions (like positions on bill)
        /// </summary>
        public TrulyObservableCollection<Position> Positions
        {
            get { return _positions; }
            set
            {
                _positions = value;
                _positions.CollectionChanged += CollectionChanged;
                OnPropertyChanged(nameof(Positions));
            }
        }

        /// <summary>
        /// Optional tags for whole transaction (like: buying PC 2015)
        /// </summary>
        public TrulyObservableCollection<Tag> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                _tags.CollectionChanged += CollectionChanged;
                OnPropertyChanged(nameof(Tags));
            }
        }

        /// <summary>
        /// Transaction type - buying/selling/working - for accounting purpose
        /// </summary>
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
        /// E.g. When your paid in april but it was payment for march [forgot / was late]. Payment was done in April, so the
        /// CreationDate will be, but you can set BookDate to March and calculate it as it should be
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

        /// <summary>
        /// Total value of transaction
        /// </summary>
        public double Value => Positions?.Sum(position => position.Value) ?? 0;

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
            BookDate = DateTime.Now;

            LastEditDate = InstanceCreationDate = DateTime.Now;

            _tags = new TrulyObservableCollection<Tag>();
            
            _positions.CollectionChanged += CollectionChanged;
            _tags.CollectionChanged += CollectionChanged;
        }

        /// <summary>
        /// Should be used only after parsing data or for test purpose.
        /// Otherwise please use pramless constructor
        /// </summary>
        /// <param name="transactionType">Tranasction type</param>
        /// <param name="sourceTransactionCreationDate">When transaction was performed</param>
        /// <param name="title">Title of transaction</param>
        /// <param name="note">Additional notes</param>
        /// <param name="positions">Positions - like positions from bill</param>
        /// <param name="userStock">User stock like wallet / bank account</param>
        /// <param name="externalStock">External stock like employer / shop</param>
        /// <param name="sourceInput">Text source of transaction (for parsing purpose) to provide unique id</param>
        public Transaction(eTransactionType transactionType, DateTime sourceTransactionCreationDate, string title, string note,
            List<Position> positions, Stock userStock, Stock externalStock, string sourceInput)
        {
            Id = GenerateGUID(sourceInput);
            Type = transactionType;
            Title = title;
            Note = note;
            _bookDate = TransationSourceCreationDate = sourceTransactionCreationDate;
            LastEditDate = InstanceCreationDate = DateTime.Now;
            Positions = new TrulyObservableCollection<Position>(positions);
            _userStock = userStock;
            _externalStock = externalStock;
        }

        /// <summary>
        /// Generates GUID based on input (original transaction text - from excel / bank import etc)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Guid GenerateGUID(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash);
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public Transaction Clone()
        {
            return (Transaction) MemberwiseClone();
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
    }
}
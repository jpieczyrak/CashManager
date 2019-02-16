using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using CashManager.Data.Extensions;
using CashManager.Model.Common;

using log4net;

namespace CashManager.Model
{
    public class Transaction : BaseObservableObject, IBookable
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(Transaction)));
        private string _title;
        private string _note;

        private DateTime _bookDate;

        private Stock _userStock;
        private Stock _externalStock;
        private TransactionType _type;

        private TrulyObservableCollection<Position> _positions;
        private ObservableCollection<StoredFileInfo> _storedFiles;

        /// <summary>
        /// Date when transaction was performed (in real life, like going to shop or receiving payment)
        /// </summary>
        public DateTime TransactionSourceCreationDate { get; private set; }

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
        /// List of positions (like positions on bill)
        /// </summary>
        public TrulyObservableCollection<Position> Positions
        {
            get => _positions;
            set
            {
                if (_positions != null) _positions.CollectionChanged -= PositionsCollectionChanged;
                Set(nameof(Positions), ref _positions, value);
                _positions.CollectionChanged += PositionsCollectionChanged;
            }
        }

        /// <summary>
        /// List of bills
        /// </summary>
        public ObservableCollection<StoredFileInfo> StoredFiles
        {
            get => _storedFiles;
            private set
            {
                if (_storedFiles != null) _storedFiles.CollectionChanged -= StoredFilesOnCollectionChanged;
                Set(nameof(StoredFiles), ref _storedFiles, value);
                _storedFiles.CollectionChanged += StoredFilesOnCollectionChanged;
            }
        }

        /// <summary>
        /// Transaction type - buying/selling/working - for accounting purpose
        /// </summary>
        public TransactionType Type
        {
            get => _type;
            set
            {
                _type = value;
                RaisePropertyChanged(nameof(Type));
            }
        }

        /// <summary>
        /// Date used for register moment.
        /// E.g. When your paid in april but it was payment for march [forgot / was late]. Payment was done in April, so the
        /// CreationDate will be, but you can set BookDate to March and calculate it as it should be
        /// </summary>
        public DateTime BookDate
        {
            get => _bookDate;
            set
            {
                if (value > DateTime.Today) value = DateTime.Today;
                Set(nameof(BookDate), ref _bookDate, value);
            }
        }

        /// <summary>
        /// Total value of transaction
        /// </summary>
        public decimal Value
        {
            get
            {
                try
                {
                    return Positions?.Sum(position => position.Value?.GrossValue) ?? 0;
                }
                catch (OverflowException e)
                {
                    _logger.Value.Info("Transaction value overflow", e);
                    return 0m;
                }
            }
        }

        /// <summary>
        /// Total value of transaction as profit of user (negative when buying, positive when receiving payments)
        /// </summary>
        public decimal ValueWithSign => Type == null
                                            ? 0m
                                            : Type.Outcome
                                                ? -Value
                                                : Type.Income
                                                    ? Value
                                                    : 0m;


        public decimal ValueAsProfit => Type == null || Type.IsTransfer ? 0m : ValueWithSign;

        public bool IsValid => Type != null
                               && UserStock != null
                               && (Positions?.Any() ?? false)
                               && !string.IsNullOrWhiteSpace(Title)
                               && UserStock != null
                               && Value != 0m;

        public string CategoriesForGui => Positions == null || !Positions.Any()
                                              ? string.Empty
                                              : string.Join(", ", Positions.Select(x => x.Category?.Name).Distinct().Where(x => x != null));

        public Transaction(Guid id) : this() { Id = id; }

        public Transaction()
        {
            _bookDate = LastEditDate = InstanceCreationDate = DateTime.Now;

            Positions = new TrulyObservableCollection<Position>();
            StoredFiles = new ObservableCollection<StoredFileInfo>();
            IsPropertyChangedEnabled = true;
        }

        private void PositionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsPropertyChangedEnabled)
            {
                RaisePropertyChanged(nameof(Value));
                RaisePropertyChanged(nameof(ValueWithSign));
                RaisePropertyChanged(nameof(ValueAsProfit));
            }
        }

        private void StoredFilesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsPropertyChangedEnabled) RaisePropertyChanged(nameof(StoredFiles));
        }

        public static Transaction Copy(Transaction source)
        {
            if (source == null) return null;
            var dto = new Data.DTO.Transaction($"{source.Id}{DateTime.Now}".GenerateGuid())
            {
                TransactionSourceCreationDate = source.TransactionSourceCreationDate
            };

            return BuildTransaction(source, dto);
        }

        public static Transaction Clone(Transaction source)
        {
            if (source == null) return null;
            var dto = new Data.DTO.Transaction(source.Id)
            {
                TransactionSourceCreationDate = source.TransactionSourceCreationDate
            };

            return BuildTransaction(source, dto);
        }

        private static Transaction BuildTransaction(Transaction source, Data.DTO.Transaction dto)
        {
            var transaction = Mapper.Map<Transaction>(dto);
            transaction.IsPropertyChangedEnabled = false;
            transaction.BookDate = source.BookDate;
            transaction.Title = source.Title;
            transaction.Note = source.Note;
            transaction.UserStock = source.UserStock;
            transaction.ExternalStock = source.ExternalStock;
            transaction.Type = source.Type;

            transaction.Positions = new TrulyObservableCollection<Position>(source.Positions
                                                                                  .Where(x => x != null)
                                                                                  .Select(Position.Copy));

            transaction.IsPropertyChangedEnabled = source.IsPropertyChangedEnabled;

            return transaction;
        }
    }
}
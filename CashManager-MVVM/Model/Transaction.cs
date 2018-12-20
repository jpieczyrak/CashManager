﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
{
    public class Transaction : BaseObservableObject, IBookable
    {
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
        public decimal Value => Positions?.Sum(position => position.Value?.GrossValue) ?? 0;

        /// <summary>
        /// Total value of transaction as profit of user (negative when buying, positive when receiving payments)
        /// </summary>
        public decimal ValueAsProfit => Type.Outcome
                                           ? -Value
                                           : Type.Income
                                               ? Value
                                               : 0m;

        public Transaction()
        {
            _bookDate = LastEditDate = InstanceCreationDate = DateTime.Now;

            Positions = new TrulyObservableCollection<Position>();
            StoredFiles = new ObservableCollection<StoredFileInfo>();
        }
        
        private void PositionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Value));
            RaisePropertyChanged(nameof(ValueAsProfit));
        }

        private void StoredFilesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(StoredFiles));
        }
    }
}
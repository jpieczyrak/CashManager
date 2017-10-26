﻿using System;
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
        private string _title;
        private string _note;

        private DateTime _bookDate;

        private Stock _myStock;
        private Stock _externalStock;
        private eTransactionType _type;

        private TrulyObservableCollection<Subtransaction> _subtransactions = new TrulyObservableCollection<Subtransaction>();
        private TrulyObservableCollection<Tag> _tags = new TrulyObservableCollection<Tag>();

        public DateTime InstanceCreationDate { get; }

        public DateTime TransationSourceCreationDate { get; }

        public DateTime LastEditDate { get; private set; }

        public Guid Id { get; private set; } = Guid.NewGuid();

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
        ///     Used for our stocks like: my wallet, my bank account etc
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
        ///     Used for stocks like: shop, employer etc
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
        public TrulyObservableCollection<Subtransaction> Subtransactions
        {
            get { return _subtransactions; }
            set
            {
                _subtransactions = value;
                _subtransactions.CollectionChanged += CollectionChanged;
                OnPropertyChanged(nameof(Subtransactions));
            }
        }

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
        ///     Date used for register moment.
        ///     E.g. When your paid in april but it was payment for march [forgot / was late]. Payment was done in April, so the
        ///     CreationDate will be, but you can set BookDate to March and calculate it as it should be
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

        public double Value => Subtransactions?.Sum(subtransaction => subtransaction.Value) ?? 0;

        public double ValueAsProfit => Type == eTransactionType.Buy || Type == eTransactionType.Reinvest
                                           ? -Value
                                           : (Type != eTransactionType.Transfer ? Value : 0);

        public Transaction()
        {
            Type = eTransactionType.Buy;
            BookDate = DateTime.Now;

            LastEditDate = InstanceCreationDate = DateTime.Now;

            _tags = new TrulyObservableCollection<Tag>();
            
            _subtransactions.CollectionChanged += CollectionChanged;
            _tags.CollectionChanged += CollectionChanged;
        }

        public Transaction(eTransactionType transactionType, DateTime sourceTransactionCreationDate, string title, string note,
            List<Subtransaction> subtransactions, Stock myStock, Stock externalStock)
        {
            Type = transactionType;
            Title = title;
            Note = note;
            _bookDate = TransationSourceCreationDate = sourceTransactionCreationDate;
            LastEditDate = InstanceCreationDate = DateTime.Now;
            Subtransactions = new TrulyObservableCollection<Subtransaction>(subtransactions);
            _myStock = myStock;
            _externalStock = externalStock;
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
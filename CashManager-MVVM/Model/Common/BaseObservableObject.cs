using System;
using System.ComponentModel;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model.Common
{
    public abstract class BaseObservableObject : ObservableObject, IEditable
    {
        private DateTime _lastEditDate;

        public Guid Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// Date when transaction was first created within application
        /// </summary>
        public DateTime InstanceCreationDate { get; protected set; } = DateTime.Now;

        /// <summary>
        /// Last time when transaction was edited by user (within app)
        /// </summary>
        public DateTime LastEditDate
        {
            get => _lastEditDate;
            protected set => Set(ref _lastEditDate, value);
        }

        public bool IsPropertyChangedEnabled { get; set; }

        protected BaseObservableObject()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (IsPropertyChangedEnabled) LastEditDate = DateTime.Now;
        }

        #region Override

        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }
}
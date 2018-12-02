using System;
using System.ComponentModel;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model
{
    public abstract class BaseObservableObject : ObservableObject
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// Date when transaction was first created within application
        /// </summary>
        public DateTime InstanceCreationDate { get; protected set; } = DateTime.Now;

        /// <summary>
        /// Last time when transaction was edited by user (within app)
        /// </summary>
        public DateTime LastEditDate { get; protected set; }

        protected BaseObservableObject()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            LastEditDate = DateTime.Now;
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
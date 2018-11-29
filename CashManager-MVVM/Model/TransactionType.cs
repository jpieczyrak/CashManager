using System;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model
{
    public class TransactionType : ObservableObject
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; set; }

        public bool Income { get; set; }

        public bool Outcome { get; set; }

        public bool IsDefault { get; set; }

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
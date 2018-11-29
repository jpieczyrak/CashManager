using System;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model
{
    public abstract class BaseObservableObject : ObservableObject
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

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
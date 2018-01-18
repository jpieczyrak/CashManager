using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model
{
    /// <summary>
    /// Stores "tag" information of transaction.
    /// Each transaction can contains unlimited tags.
    /// Tag can be assigned to many transactions.
    /// Sum of values of all tags dont have to be equal to sum of all transaction value (one tag can be assigned to one or more transaction)
    /// </summary>
    public class Tag : ObservableObject
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => Set(nameof(Name), ref _name, value);
        }

        #region Override

        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
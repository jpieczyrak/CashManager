using System.ComponentModel;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace LogicOld.Model
{
    /// <summary>
    /// Stores "tag" information of transaction.
    /// Each transaction can contains unlimited tags.
    /// Tag can be assigned to many transactions.
    /// Sum of values of all tags dont have to be equal to sum of all transaction value (one tag can be assigned to one or more transaction)
    /// </summary>
    public class Tag : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public Tag(string name)
        {
            Name = name;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
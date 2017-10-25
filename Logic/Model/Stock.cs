using System.ComponentModel;
using System.Runtime.CompilerServices;

using Logic.Annotations;

namespace Logic.Model
{
    /// <summary>
    ///     Stores name of "part of" your wallet.
    ///     You can have more than one Stock in your Wallet (like bank account, phisic wallet, second bank acc ect)
    /// </summary>
    public class Stock : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private Stock() { }

        public Stock(string name)
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
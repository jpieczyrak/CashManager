using System;
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
        private bool _isUserStock;

        /// <summary>
        /// Only for db purpose
        /// </summary>
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public bool IsUserStock
        {
            get { return _isUserStock; }
            set
            {
                _isUserStock = value;
                OnPropertyChanged(nameof(IsUserStock));
            }
        }

        private Stock() { }

        public Stock(string name)
        {
            _name = name;
            _isUserStock = false;
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
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name}-{Id}";
        }

        #endregion
    }
}
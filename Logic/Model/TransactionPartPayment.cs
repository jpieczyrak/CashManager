using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using Logic.Properties;
using Logic.StocksManagement;
using Logic.TransactionManagement.TransactionElements;

namespace Logic.Model
{
    [DataContract(Namespace = "")]
    public class TransactionPartPayment : INotifyPropertyChanged
    {
        private double _value;
        private ePaymentType _paymentType;

        public Guid StockId { get; set; }

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public ePaymentType PaymentType
        {
            get { return _paymentType; }
            set
            {
                _paymentType = value;
                OnPropertyChanged();
            }
        }

        public Guid Id { get; private set; }

        public TransactionPartPayment()
        {
            Id = Guid.NewGuid();
        }

        public TransactionPartPayment(Stock stock, double value, ePaymentType paymentType) : this()
        {
            StockId = Guid.Empty;
            Value = value;
            PaymentType = paymentType;
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
            return obj != null && obj.GetHashCode().Equals(GetHashCode());
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }
}
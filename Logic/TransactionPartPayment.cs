using System.ComponentModel;
using System.Runtime.CompilerServices;
using Logic.Annotations;

namespace Logic
{
    public class TransactionPartPayment : INotifyPropertyChanged
    {
        private double _value;
        private ePaymentType _paymentType;

        public TransactionPartPayment(string stock, double value, ePaymentType paymentType)
        {
            Stock = stock;
            Value = value;
            PaymentType = paymentType;
        }

        public string Stock { get; set; }

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
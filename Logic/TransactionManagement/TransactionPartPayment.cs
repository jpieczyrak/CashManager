using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Logic.Annotations;

namespace Logic.TransactionManagement
{
    [DataContract(Namespace = "")]
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

        [DataMember]
        public string Stock { get; set; }

        [DataMember]
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
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
using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model
{
    public class PaymentValue : ObservableObject
    {
        private double _value;

        public double Value
        {
            get => _value;
            set => Set(nameof(Value), ref _value, value);
        }
    }
}
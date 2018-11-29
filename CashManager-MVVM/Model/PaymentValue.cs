namespace CashManager_MVVM.Model
{
    public class PaymentValue : BaseObservableObject
    {
        private double _value;

        public double Value
        {
            get => _value;
            set => Set(nameof(Value), ref _value, value);
        }
    }
}
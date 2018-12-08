using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
{
    public class PaymentValue : BaseObservableObject
    {
        private double _grossValue;
        private double _netValue;
        private double _taxPercentValue;

        public double GrossValue
        {
            get => _grossValue;
            set => Set(nameof(GrossValue), ref _grossValue, value);
        }

        public double NetValue
        {
            get => _netValue;
            set => Set(nameof(NetValue), ref _netValue, value);
        }

        public double TaxPercentValue
        {
            get => _taxPercentValue;
            set => Set(nameof(TaxPercentValue), ref _taxPercentValue, value);
        }
    }
}
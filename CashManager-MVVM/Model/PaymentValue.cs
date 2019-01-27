using CashManager.WPF.Model.Common;

namespace CashManager.WPF.Model
{
    public sealed class PaymentValue : BaseObservableObject
    {
        private decimal _grossValue;
        private decimal _netValue;
        private decimal _taxPercentValue;
        private bool _taxLocked = true;

        public decimal GrossValue
        {
            get => _grossValue;
            set
            {
                Set(nameof(GrossValue), ref _grossValue, value);
                if (_taxLocked) UpdateNet();
                else UpdateTax();
            }
        }

        public decimal NetValue
        {
            get => _netValue;
            set
            {
                Set(nameof(NetValue), ref _netValue, value);
                if (_taxLocked) UpdateGross();
                else UpdateTax();
            }
        }

        public decimal TaxPercentValue
        {
            get => _taxPercentValue;
            set
            {
                Set(nameof(TaxPercentValue), ref _taxPercentValue, value);
                UpdateGross();
                RaisePropertyChanged(nameof(TaxGuiString));
            }
        }

        public bool TaxLocked
        {
            get => _taxLocked;
            set => Set(nameof(TaxLocked), ref _taxLocked, value);
        }

        private void UpdateTax()
        {
            if (_netValue > 0m)
            {
                decimal oldValue = _taxPercentValue;
                _taxPercentValue = (_grossValue - _netValue) * 100m / _netValue;
                if (_taxPercentValue != oldValue) RaisePropertyChanged(nameof(TaxPercentValue));
            }
        }

        private void UpdateNet()
        {
            decimal oldNet = _netValue;
            _netValue = _grossValue / (100m + _taxPercentValue) * 100m;
            if (oldNet != _netValue) RaisePropertyChanged(nameof(NetValue));
        }

        private void UpdateGross()
        {
            decimal oldValue = _grossValue;
            _grossValue = _netValue * (100m + _taxPercentValue) / 100m;
            if (_grossValue != oldValue) RaisePropertyChanged(nameof(GrossValue));
        }

        public string TaxGuiString => $"{TaxPercentValue:###}%";

        /// <summary>
        /// Used by mapper
        /// </summary>
        public PaymentValue(decimal netValue, decimal grossValue, decimal taxPercentValue)
        {
            _netValue = netValue;
            _grossValue = grossValue;
            _taxPercentValue = taxPercentValue;
            if (_netValue == 0m) UpdateNet();
            if (_grossValue == 0m) UpdateGross();
            if (_taxPercentValue == 0m) UpdateTax();
            RaisePropertyChanged(nameof(TaxGuiString));
        }

        public PaymentValue()
        {
            _taxPercentValue = 23;
        }
    }
}
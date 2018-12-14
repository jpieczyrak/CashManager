using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
{
    public class PaymentValue : BaseObservableObject
    {
        private double _grossValue;
        private double _netValue;
        private double _taxPercentValue;
        private bool _taxLocked = true;

        public double GrossValue
        {
            get => _grossValue;
            set
            {
                Set(nameof(GrossValue), ref _grossValue, value);
                if (_taxLocked) UpdateNet();
                else UpdateTax();
            }
        }

        public double NetValue
        {
            get => _netValue;
            set
            {
                Set(nameof(NetValue), ref _netValue, value);
                if (_taxLocked) UpdateGross();
                else UpdateTax();
            }
        }

        public double TaxPercentValue
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
            if (_netValue > 0)
            {
                double oldValue = _taxPercentValue;
                _taxPercentValue = (_grossValue - _netValue) * 100d / _netValue;
                if (_taxPercentValue != oldValue) RaisePropertyChanged(nameof(TaxPercentValue));
            }
        }

        private void UpdateNet()
        {
            double oldNet = _netValue;
            _netValue = _grossValue / (100d + _taxPercentValue) * 100d;
            if (oldNet != _netValue) RaisePropertyChanged(nameof(NetValue));
        }

        private void UpdateGross()
        {
            double oldValue = _grossValue;
            _grossValue = _netValue * (100d + _taxPercentValue) / 100d;
            if (_grossValue != oldValue) RaisePropertyChanged(nameof(GrossValue));
        }

        public string TaxGuiString => $"{TaxPercentValue:###}%";

        /// <summary>
        /// Used by mapper
        /// </summary>
        public PaymentValue(double netValue, double grossValue, double taxPercentValue)
        {
            _netValue = netValue;
            _grossValue = grossValue;
            _taxPercentValue = taxPercentValue;
            if (_netValue == 0) UpdateNet();
            if (_grossValue == 0) UpdateGross();
            if (_taxPercentValue == 0) UpdateTax();
            RaisePropertyChanged(nameof(TaxGuiString));
        }

        public PaymentValue()
        {
            _taxPercentValue = 23;
        }
    }
}
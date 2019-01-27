using CashManager.Properties;

namespace CashManager.Model.Selectors
{
    public class RangeSelector : BaseSelector
    {
        private decimal _min;
        private decimal _max;
        private decimal _minimumValue;
        private decimal _maximumValue;

        public decimal Min
        {
            get => _min;
            set => Set(nameof(Min), ref _min, value);
        }

        public decimal Max
        {
            get => _max;
            set => Set(nameof(Max), ref _max, value);
        }

        public decimal MinimumValue
        {
            get => _minimumValue;
            set => Set(ref _minimumValue, value);
        }

        public decimal MaximumValue
        {
            get => _maximumValue;
            set => Set(ref _maximumValue, value);
        }

        public RangeSelectorType Type { get; private set; }

        public RangeSelector(RangeSelectorType type)
        {
            Type = type;
            switch (type)
            {
                case RangeSelectorType.GrossValue:
                    Description = Strings.Value;
                    break;
            }
        }

        public void Apply(RangeSelector source)
        {
            Min = source.Min;
            Max = source.Max;
            Type = source.Type;
            IsChecked = source.IsChecked;
        }
    }
}
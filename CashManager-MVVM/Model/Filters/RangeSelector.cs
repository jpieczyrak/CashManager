namespace CashManager_MVVM.Model.Filters
{
    public class RangeSelector : BaseSelector
    {
        private double _min;
        private double _max;

        public double Min
        {
            get => _min;
            set => Set(nameof(Min), ref _min, value);
        }

        public double Max
        {
            get => _max;
            set => Set(nameof(Max), ref _max, value);
        }

        public RangeSelector(string description)
        {
            Description = description;
        }
    }
}
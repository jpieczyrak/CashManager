namespace CashManager_MVVM.Model.Filters
{
    public class RangeFilter : BaseFilter
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

        public RangeFilter(string description)
        {
            Description = description;
        }
    }
}
namespace CashManager_MVVM.Model.Selectors
{
    public class RangeSelector : BaseSelector
    {
        private decimal _min;
        private decimal _max;

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

        public RangeSelector(string description)
        {
            Description = description;
        }
    }
}
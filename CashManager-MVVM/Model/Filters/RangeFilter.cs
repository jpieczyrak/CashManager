namespace CashManager_MVVM.Model.Filters
{
    public class RangeFilter : BaseFilter
    {
        public double Min { get; set; }

        public double Max { get; set; }

        public RangeFilter(string description)
        {
            Description = description;
        }
    }
}
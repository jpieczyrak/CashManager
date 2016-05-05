using Logic.Specification;
using Logic.TransactionManagement;

namespace Logic.FindingFilters
{
    public class TimeFramRule : CompositeSpecification<Transaction>
    {
        private readonly TimeFrame _timeFrame;

        public TimeFramRule(TimeFrame timeFrame)
        {
            _timeFrame = timeFrame;
        }

        public override bool IsSatisfiedBy(Transaction o)
        {
            return _timeFrame.Contains(o.Date);
        }
    }
}
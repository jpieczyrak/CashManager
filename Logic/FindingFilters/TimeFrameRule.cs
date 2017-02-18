using Logic.Specification;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

namespace Logic.FindingFilters
{
    public class TimeFrameRule : CompositeSpecification<Transaction>
    {
        private readonly TimeFrame _timeFrame;

        public TimeFrameRule(TimeFrame timeFrame)
        {
            _timeFrame = timeFrame;
        }

        public override bool IsSatisfiedBy(Transaction o)
        {
            return _timeFrame.Contains(o.Date);
        }
    }
}
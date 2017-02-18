using Logic.Specification;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

namespace Logic.FindingFilters
{
    public class TransactionMaxValueRule : CompositeSpecification<Transaction>
    {
        private readonly double _maxValue;

        public TransactionMaxValueRule(double maxValue)
        {
            _maxValue = maxValue;
        }

        public override bool IsSatisfiedBy(Transaction o)
        {
            return _maxValue >= o.Value;
        }
    }
}
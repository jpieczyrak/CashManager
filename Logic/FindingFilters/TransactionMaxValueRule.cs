using Logic.Model;
using Logic.Specification;

namespace Logic.FindingFilters
{
    public class TransactionMaxValueRule : CompositeSpecification<Transaction>
    {
        private readonly double _maxValue;

        public TransactionMaxValueRule(double maxValue)
        {
            _maxValue = maxValue;
        }

        public override bool IsSatisfiedBy(Transaction transaction)
        {
            return _maxValue >= transaction.Value;
        }
    }
}
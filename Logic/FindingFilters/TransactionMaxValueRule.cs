using LogicOld.Model;
using LogicOld.Specification;

namespace LogicOld.FindingFilters
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
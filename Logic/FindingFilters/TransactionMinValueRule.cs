using LogicOld.Model;
using LogicOld.Specification;

namespace LogicOld.FindingFilters
{
    public class TransactionMinValueRule : CompositeSpecification<Transaction>
    {
        private readonly double _minValue;

        public TransactionMinValueRule(double minValue)
        {
            _minValue = minValue;
        }

        public override bool IsSatisfiedBy(Transaction transaction)
        {
            return _minValue <= transaction.Value;
        }
    }
}
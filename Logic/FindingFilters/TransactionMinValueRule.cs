using System;
using Logic.Specification;
using Logic.TransactionManagement;

namespace Logic.FindingFilters
{
    public class TransactionMinValueRule : CompositeSpecification<Transaction>
    {
        private readonly double _minValue;

        public TransactionMinValueRule(double minValue)
        {
            _minValue = minValue;
        }

        public override bool IsSatisfiedBy(Transaction o)
        {
            return _minValue <= o.Value;
        }
    }
}
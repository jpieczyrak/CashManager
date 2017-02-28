using System;

using Logic.Model;
using Logic.Specification;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

namespace Logic.FindingFilters
{
    public class TargetStockRule : CompositeSpecification<Transaction>
    {
        private Guid _stockId;

        public TargetStockRule(Guid stockId)
        {
            _stockId = stockId;
        }

        public override bool IsSatisfiedBy(Transaction o)
        {
            return _stockId.Equals(o.TargetStockId);
        }
    }
}

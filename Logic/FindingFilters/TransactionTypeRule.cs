using Logic.Specification;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

namespace Logic.FindingFilters
{
    public class TransactionTypeRule : CompositeSpecification<Transaction>
    {
        private readonly eTransactionType _transactionType;

        public TransactionTypeRule(eTransactionType transactionType)
        {
            _transactionType = transactionType;
        }
        
        public override bool IsSatisfiedBy(Transaction o)
        {
            return o.Type.Equals(_transactionType);
        }
    }
}
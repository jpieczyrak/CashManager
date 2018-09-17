using Logic.Model;
using Logic.Specification;
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
        
        public override bool IsSatisfiedBy(Transaction transaction)
        {
            return transaction.Type.Equals(_transactionType);
        }
    }
}
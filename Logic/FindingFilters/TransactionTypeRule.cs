using LogicOld.Model;
using LogicOld.Specification;
using LogicOld.TransactionManagement.TransactionElements;

namespace LogicOld.FindingFilters
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
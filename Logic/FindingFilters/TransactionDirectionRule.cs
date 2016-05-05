using Logic.Specification;
using Logic.TransactionManagement;

namespace Logic.FindingFilters
{
    public class TransactionDirectionRule : CompositeSpecification<Transaction>
    {
        private readonly eTransactionDirection _transactionDirection;

        public TransactionDirectionRule(eTransactionDirection transactionDirection)
        {
            this._transactionDirection = transactionDirection;
        }

        public override bool IsSatisfiedBy(Transaction o)
        {
            switch (o.Type)
            {
                case eTransactionType.Buy:
                case eTransactionType.Reinvest:
                    return _transactionDirection == eTransactionDirection.Outcome;
                case eTransactionType.Resell:
                case eTransactionType.Sell:
                case eTransactionType.Work:
                    return _transactionDirection == eTransactionDirection.Income;
                case eTransactionType.Transfer:
                    return _transactionDirection == eTransactionDirection.Transfer;
            }
            return false;
        }
    }
}
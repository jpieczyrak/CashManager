using System;

namespace CashManager.Logic.Calculators
{
    public class TransactionBalance
    {
        public TransactionBalance(DateTime bookDate, decimal value)
        {
            BookDate = bookDate;
            Value = value;
        }

        public DateTime BookDate { get; }

        public decimal Value { get; }

        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + BookDate.GetHashCode();
                hash = hash * 23 + Value.GetHashCode();

                return hash;
            }
        }
    }
}
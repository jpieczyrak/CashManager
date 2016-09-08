using System;

namespace Logic.TransactionManagement.BulkModifications
{
    public static class BulkTransactionParametersChanger
    {
        public static void Change(Transactions transactions, Func<Transaction, bool> condition, Action<Transaction>[] actions)
        {
            foreach (var transaction in transactions.TransactionsList)
            {
                if (condition(transaction))
                {
                    foreach (var action in actions)
                    {
                        action(transaction);
                    }
                }
            }
        }
    }
}
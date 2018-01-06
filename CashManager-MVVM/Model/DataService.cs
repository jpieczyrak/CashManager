using System;
using System.Collections.Generic;
using System.Linq;

using Logic.LogicObjectsProviders;
using Logic.Model;
using Logic.TransactionManagement.TransactionElements;
using Logic.Utils;

namespace CashManager_MVVM.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<TrulyObservableCollection<Transaction>, Exception> callback)
        {
            var transactions = TransactionProvider.Transactions;
#if DEBUG
            if (!transactions.Any())
            {
                transactions = new TrulyObservableCollection<Transaction>
                {
                    new Transaction(eTransactionType.Buy, DateTime.Now, "title1", "run", new List<Subtransaction>
                    {
                        new Subtransaction("sub1", 12, "cat1")
                    }, new Stock("test1"), new Stock("test2"), "run"),
                    new Transaction(eTransactionType.Buy, DateTime.Now, "title2", "run", new List<Subtransaction>
                    {
                        new Subtransaction("sub1", 100, "cat2")
                    }, new Stock("test1"), new Stock("test2"), "run"),
                };
            }
#endif
            callback(transactions, null);
        }
    }
}
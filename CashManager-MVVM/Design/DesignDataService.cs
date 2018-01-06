using System;
using System.Collections.Generic;

using CashManager_MVVM.Model;

using Logic.Model;
using Logic.TransactionManagement.TransactionElements;
using Logic.Utils;

namespace CashManager_MVVM.Design
{
    public class DesignDataService : IDataService
    {
        #region IDataService

        public void GetData(Action<TrulyObservableCollection<Transaction>, Exception> callback)
        {
            var transactions = new TrulyObservableCollection<Transaction>
            {
                new Transaction(eTransactionType.Buy, DateTime.Now, "title1", "notes1", new List<Subtransaction>
                {
                    new Subtransaction("sub1", 12, "cat1")
                }, new Stock("test1"), new Stock("test2"), "design"),
                new Transaction(eTransactionType.Buy, DateTime.Now, "title2", "notes2", new List<Subtransaction>
                {
                    new Subtransaction("sub1", 100, "cat2")
                }, new Stock("test1"), new Stock("test2"), "design"),
                new Transaction(eTransactionType.Buy, DateTime.Now, "title3", "notes3", new List<Subtransaction>
                {
                    new Subtransaction("sub1", 100, "cat3")
                }, new Stock("test1"), new Stock("test2"), "design"),
            };
            callback(transactions, null);
        }

        #endregion
    }
}
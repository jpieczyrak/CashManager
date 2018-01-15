using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager_MVVM.Model.DataProviders;

using Logic.TransactionManagement.TransactionElements;
using Logic.Utils;

using Transaction = CashManager_MVVM.Model.Transaction;

namespace CashManager_MVVM.Design
{
    public class DesignDataService : IDataService
    {
        #region IDataService

        public void GetData(Action<TrulyObservableCollection<Transaction>, Exception> callback)
        {
            var transactions = new List<Logic.DTO.Transaction>
            {
                new Logic.DTO.Transaction(eTransactionType.Buy, DateTime.Now, "title1", "notes1", new List<Logic.DTO.Subtransaction>
                    {
                        new Logic.DTO.Subtransaction
                        {
                            CategoryId = Guid.NewGuid(),
                            Value = 12,
                            Name = "cat1"
                        }
                    }, new Logic.DTO.Stock
                    {
                        Name = "test1"
                    },
                    new Logic.DTO.Stock
                    {
                        Name = "test2"
                    }, "design1"),
                new Logic.DTO.Transaction(eTransactionType.Buy, DateTime.Now, "title2", "notes2", new List<Logic.DTO.Subtransaction>
                    {
                        new Logic.DTO.Subtransaction
                        {
                            CategoryId = Guid.NewGuid(),
                            Value = 12,
                            Name = "cat2"
                        }
                    }, new Logic.DTO.Stock
                    {
                        Name = "test1"
                    },
                    new Logic.DTO.Stock
                    {
                        Name = "test2"
                    }, "design2"),
            };
            callback(new TrulyObservableCollection<Transaction>(transactions.Select(Mapper.Map<Transaction>)), null);
        }

        #endregion
    }
}
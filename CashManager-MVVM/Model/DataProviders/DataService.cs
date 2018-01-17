using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Logic.Model;
using Logic.TransactionManagement.TransactionElements;
using Logic.Utils;

namespace CashManager_MVVM.Model.DataProviders
{
    public class DataService : IDataService
    {
        public void GetData(Action<TrulyObservableCollection<Transaction>, Exception> callback)
        {
            TrulyObservableCollection<Transaction> transactions = null;// = TransactionProvider.Transactions;
#if DEBUG
            if (transactions == null || !transactions.Any())
            {
                var trans = new List<Logic.DTO.Transaction>
                {
                    new Logic.DTO.Transaction(eTransactionType.Buy, DateTime.Now, "title1", "notes1", new List<Logic.DTO.Subtransaction>
                        {
                            new Logic.DTO.Subtransaction
                            {
                                CategoryId = Guid.NewGuid(),
                                Value = 12,
                                Title = "cat1"
                            }
                        }, new Logic.DTO.Stock
                        {
                            Name = "test1"
                        },
                        new Logic.DTO.Stock
                        {
                            Name = "test2"
                        }, "test1"),
                    new Logic.DTO.Transaction(eTransactionType.Buy, DateTime.Now, "title2", "notes2", new List<Logic.DTO.Subtransaction>
                        {
                            new Logic.DTO.Subtransaction
                            {
                                CategoryId = Guid.NewGuid(),
                                Value = 12,
                                Title = "cat2"
                            }
                        }, new Logic.DTO.Stock
                        {
                            Name = "test1"
                        },
                        new Logic.DTO.Stock
                        {
                            Name = "test2"
                        }, "test2"),
                };
                transactions = new TrulyObservableCollection<Transaction>(trans.Select(Mapper.Map<Transaction>));
            }
#endif
            callback(transactions, null);
        }
    }
}
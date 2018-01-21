using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Logic.TransactionManagement.TransactionElements;

namespace CashManager_MVVM.Model.DataProviders
{
    public class DataService : IDataService
    {
        public void GetTransactions(Action<IEnumerable<Transaction>, Exception> callback)
        {
            IEnumerable<Transaction> transactions = null;// = TransactionProvider.Transactions;
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
                                Value = new Logic.DTO.PaymentValue { Value = 12 },
                                Title = "title cat1"
                            },
                            new Logic.DTO.Subtransaction
                            {
                                CategoryId = Guid.NewGuid(),
                                Value = new Logic.DTO.PaymentValue { Value = 15 },
                                Title = "title cat2"
                            },
                        }, 
                        new Logic.DTO.Stock
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
                                Value = new Logic.DTO.PaymentValue { Value = 24 },
                                Title = "cat2"
                            }
                        }, 
                        new Logic.DTO.Stock
                        {
                            Name = "test1"
                        },
                        new Logic.DTO.Stock
                        {
                            Name = "test2"
                        }, "test2"),
                };
                transactions = trans.Select(Mapper.Map<Transaction>);
            }
#endif
            callback(transactions, null);
        }

        public void GetCategories(Action<IEnumerable<Category>, Exception> callback)
        {
            IEnumerable<Category> categories = null;
#if DEBUG
            if (categories == null || !categories.Any())
            {
                var dtoCategories = new List<Logic.DTO.Category>
                {

                };
                categories = dtoCategories.Select(Mapper.Map<Category>);
            }
#endif
            callback(categories, null);
        }

        public void GetStocks(Action<IEnumerable<Stock>, Exception> callback)
        {
            callback(
                new List<Stock>
                {
                    new Stock { Name = "User1", IsUserStock = true },
                    new Stock { Name = "Ex1" },
                    new Stock { Name = "Ex2" }
                }, null);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.DataProviders;

using Logic.TransactionManagement.TransactionElements;
using Logic.Utils;

using PaymentValue = Logic.DTO.PaymentValue;
using Subtransaction = Logic.DTO.Subtransaction;
using Tag = Logic.DTO.Tag;

namespace CashManager_MVVM.Design
{
    public class DesignDataService : IDataService
    {
        #region IDataService

        public void GetData(Action<TrulyObservableCollection<Transaction>, Exception> callback)
        {
            var transactions = new List<Logic.DTO.Transaction>
            {
                new Logic.DTO.Transaction(eTransactionType.Buy, DateTime.Now, "title1", "notes1", new List<Subtransaction>
                    {
                        new Subtransaction
                        {
                            CategoryId = Guid.NewGuid(),
                            Value = new PaymentValue { Value = 12 },
                            Title = "cat1",
                            Tags = new List<Tag> { new Tag { Name = "tag1" } }
                        }
                    }, new Logic.DTO.Stock
                    {
                        Name = "test1"
                    },
                    new Logic.DTO.Stock
                    {
                        Name = "test2"
                    }, "design1"),
                new Logic.DTO.Transaction(eTransactionType.Buy, DateTime.Now, "title2", "notes2", new List<Subtransaction>
                    {
                        new Subtransaction
                        {
                            CategoryId = Guid.NewGuid(),
                            Value = new PaymentValue { Value = 24 },
                            Title = "cat2",
                            Tags = new List<Tag> { new Tag { Name = "tag123" } }
                        }
                    }, new Logic.DTO.Stock
                    {
                        Name = "test1"
                    },
                    new Logic.DTO.Stock
                    {
                        Name = "test2"
                    }, "design2")
            };
            callback(new TrulyObservableCollection<Transaction>(transactions.Select(Mapper.Map<Transaction>)), null);
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

        #endregion
    }
}
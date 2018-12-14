using System;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Queries.Stock
{
    public class StockTests
    {
        [Fact]
        public void StockQueryHandler_StockQueryEmptyDatabase_EmptyArray()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new StockQueryHandler(repository);
            var query = new StockQuery();

            //when
            var result = handler.Execute(query);

            //then
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void StockQueryHandler_StockQueryNotEmptyDatabase_Array()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new StockQueryHandler(repository);
            var query = new StockQuery();
            var stocks = new[]
            {
                new Data.DTO.Stock(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1))
                {
                    Name = "1",
                    IsUserStock = true,
                    Balance = new Balance { Value = 12.34m }
                },
                new Data.DTO.Stock(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 2))
                {
                    Name = "2",
                    IsUserStock = false
                }
            };
            repository.Database.GetCollection<Data.DTO.Stock>().InsertBulk(stocks);

            //when
            var result = handler.Execute(query);

            //then
            Assert.Equal(stocks, result);
            Assert.Equal(stocks[0].Balance, result[0].Balance);
            Assert.Equal(stocks[0].Balance.Value, result[0].Balance.Value);
        }
    }
}
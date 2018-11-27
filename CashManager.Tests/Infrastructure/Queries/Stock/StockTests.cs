using System;
using System.IO;

using CashManager.Infrastructure.Query.Stocks;

using LiteDB;

using Xunit;

namespace CashManager.Tests.Infrastructure.Queries.Stock
{
    public class StockTests
    {
        [Fact]
        public void StockQueryHandler_StockQueryEmptyDatabase_EmptyArray()
        {
            //given
            var repository = GetEmptyDatabase();
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
            var repository = GetEmptyDatabase();
            var handler = new StockQueryHandler(repository);
            var query = new StockQuery();
            var stocks = new[]
            {
                new Data.DTO.Stock
                {
                    Id = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1),
                    Name = "1",
                    IsUserStock = true
                },
                new Data.DTO.Stock
                {
                    Id = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 2),
                    Name = "2",
                    IsUserStock = false
                }
            };
            repository.Database.GetCollection<Data.DTO.Stock>().InsertBulk(stocks);

            //when
            var result = handler.Execute(query);

            //then
            Assert.Equal(stocks, result);
        }

        private static LiteRepository GetEmptyDatabase()
        {
            return new LiteRepository(new LiteDatabase(new MemoryStream()));
        }
    }
}
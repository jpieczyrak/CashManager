using System;
using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Command.Stocks;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Commands.Stocks
{
    public class UpsertCommandTests
    {
        [Fact]
        public void UpsertStockCommandHandler_NullEmptyDb_EmptyDb()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertStocksCommandHandler(repository);
            var command = new UpsertStocksCommand(null);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Stock>());
        }

        [Fact]
        public void UpsertStockCommandHandler_EmptyDbUpsertOneObject_ObjectSaved()
        {
            //given
            var stocks = new[]
            {
                new Stock { Name = "test1", Balance = new Balance { Date = DateTime.Today, Value = 12.45 } }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertStocksCommandHandler(repository);
            var command = new UpsertStocksCommand(stocks);

            //when
            handler.Execute(command);

            //then
            var orderedStocksInDatabase = repository.Database.Query<Stock>().OrderBy(x => x.Id).ToArray();
            Assert.Equal(stocks.OrderBy(x => x.Id), orderedStocksInDatabase);
            Assert.Equal(stocks[0].Balance, orderedStocksInDatabase[0].Balance);
            Assert.Equal(stocks[0].Balance.Value, orderedStocksInDatabase[0].Balance.Value);
            Assert.Equal(stocks[0].Balance.Date, orderedStocksInDatabase[0].Balance.Date);
        }

        [Fact]
        public void UpsertStockCommandHandler_EmptyDbUpsertList_ListSaved()
        {
            //given
            var Stocks = new[]
            {
                new Stock { Name = "test1" },
                new Stock { Name = "test2" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertStocksCommandHandler(repository);
            var command = new UpsertStocksCommand(Stocks);

            //when
            handler.Execute(command);

            //then
            var orderedStocksInDatabase = repository.Database.Query<Stock>().OrderBy(x => x.Id);
            Assert.Equal(Stocks.OrderBy(x => x.Id), orderedStocksInDatabase);
        }

        [Fact]
        public void UpsertStockCommandHandler_NotEmptyDbUpsertList_ListUpdated()
        {
            //given
            var stocks = new[]
            {
                new Stock { Name = "test1" },
                new Stock { Name = "test2" }
            };

            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new UpsertStocksCommandHandler(repository);
            var command = new UpsertStocksCommand(stocks);
            repository.Database.UpsertBulk(stocks);
            foreach (var stock in stocks) stock.Name += " - updated";

            //when
            handler.Execute(command);

            //then
            var orderedStocksInDatabase = repository.Database.Query<Stock>().OrderBy(x => x.Id).ToArray();
            stocks = stocks.OrderBy(x => x.Id).ToArray();
            Assert.Equal(stocks, orderedStocksInDatabase);
            for (int i = 0; i < stocks.Length; i++) Assert.Equal(stocks[i].Name, orderedStocksInDatabase[i].Name);
        }
    }
}
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
            var Stocks = new[]
            {
                new Stock { Name = "test1" }
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
        public void UpsertStockCommandHandler_NamemptyDbUpsertList_ListUpdated()
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
            repository.Database.UpsertBulk(Stocks);
            foreach (var Stock in Stocks) Stock.Name += " - updated";

            //when
            handler.Execute(command);

            //then
            var orderedStocksInDatabase = repository.Database.Query<Stock>().OrderBy(x => x.Id).ToArray();
            Stocks = Stocks.OrderBy(x => x.Id).ToArray();
            Assert.Equal(Stocks, orderedStocksInDatabase);
            for (int i = 0; i < Stocks.Length; i++)
            {
                Assert.Equal(Stocks[i].Name, orderedStocksInDatabase[i].Name);
            }
        }
    }
}
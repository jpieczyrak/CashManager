using System.Linq;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Command.Stocks;
using CashManager.Infrastructure.DbConnection;
using CashManager.Tests.Utils;

using Xunit;

namespace CashManager.Tests.Infrastructure.Commands.Stocks
{
    public class DeleteStockCommandTests
    {
        [Fact]
        public void DeleteStockCommandHandler_NullEmptyDb_EmptyDb()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteStockCommandHandler(repository);
            var command = new DeleteStockCommand(null);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Stock>());
        }

        [Fact]
        public void DeleteStockCommandHandler_NotExisting_NoChange()
        {
            //given
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteStockCommandHandler(repository);
            var command = new DeleteStockCommand(new Stock());

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Stock>());
        }

        [Fact]
        public void DeleteStockCommandHandler_Existing_Removed()
        {
            //given
            var stock = new Stock();
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteStockCommandHandler(repository);
            var command = new DeleteStockCommand(stock);
            repository.Database.Upsert(stock);

            //when
            handler.Execute(command);

            //then
            Assert.Empty(repository.Database.Query<Stock>());
        }

        [Fact]
        public void DeleteStockCommandHandler_MoreObjects_RemovedProperOne()
        {
            //given
            var targetStock = new Stock();
            var stocks = new[] { targetStock, new Stock(), new Stock(), new Stock() };
            var repository = LiteDbHelper.CreateMemoryDb();
            var handler = new DeleteStockCommandHandler(repository);
            var command = new DeleteStockCommand(targetStock);
            repository.Database.UpsertBulk(stocks);

            //when
            handler.Execute(command);

            //then
            stocks = stocks.Skip(1).OrderBy(x => x.Id).ToArray();
            var actualStocks = repository.Database.Query<Stock>().OrderBy(x => x.Id).ToArray();
            Assert.Equal(stocks.Length, actualStocks.Length);
            Assert.Equal(stocks, actualStocks);
        }
    }
}
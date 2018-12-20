using System.Linq;

using Autofac;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Categories;
using CashManager.Infrastructure.Command.Stocks;
using CashManager.Infrastructure.Command.Tags;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Command.TransactionTypes;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Transactions;
using CashManager.Logic.DefaultData;

using DtoTransaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.Utils
{
    public class DbFiller
    {
        public static void Fill(IContainer container)
        {
            var queryDispatcher = container.Resolve<IQueryDispatcher>();
            var commandDispatcher = container.Resolve<ICommandDispatcher>();

            if (!queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery()).Any())
            {
                var defaultDataProvider = new DefaultDataProvider();

                var stocks = defaultDataProvider.GetStocks();
                var categories = defaultDataProvider.GetCategories();
                var types = defaultDataProvider.GetTransactionTypes();
                var tags = defaultDataProvider.GetTags();
                var transactions = defaultDataProvider.GetTransactions(stocks, categories, types, tags);

                commandDispatcher.Execute(new UpsertStocksCommand(stocks));
                commandDispatcher.Execute(new UpsertTransactionTypesCommand(types));
                commandDispatcher.Execute(new UpsertCategoriesCommand(categories));
                commandDispatcher.Execute(new UpsertTagsCommand(tags));
                commandDispatcher.Execute(new UpsertTransactionsCommand(transactions));
            }
        }
    }
}
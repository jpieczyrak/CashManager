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

using DtoPaymentValue = CashManager.Data.DTO.PaymentValue;
using DtoCategory = CashManager.Data.DTO.Category;
using DtoTransaction = CashManager.Data.DTO.Transaction;
using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.Temps
{
    public class DbFiller
    {
        public static void Fill(IContainer container)
        {
#if DEBUG
            var queryDispatcher = container.Resolve<IQueryDispatcher>();
            var commandDispatcher = container.Resolve<ICommandDispatcher>();

            if (!queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(new TransactionQuery()).Any())
            {
                var stocks = DefaultDataProvider.GetStocks();
                var categories = DefaultDataProvider.GetCategories();
                var types = DefaultDataProvider.GetTransactionTypes();
                var tags = DefaultDataProvider.GetTags();
                var transactions = DefaultDataProvider.GetTransactions(stocks, categories, types, tags);
                var positions = transactions.SelectMany(x => x.Positions).ToArray();

                commandDispatcher.Execute(new UpsertStocksCommand(stocks));
                commandDispatcher.Execute(new UpsertTransactionTypesCommand(types));
                commandDispatcher.Execute(new UpsertCategoriesCommand(categories));
                commandDispatcher.Execute(new UpsertTagsCommand(positions.Where(x => x.Tags != null).SelectMany(x => x.Tags).ToArray()));
                commandDispatcher.Execute(new UpsertTransactionsCommand(transactions));
            }
#endif
        }
    }
}
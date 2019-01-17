using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Transactions;
using CashManager.Logic.Wrappers;

using CashManager_MVVM.Model;

using DtoTransaction = CashManager.Data.DTO.Transaction;

namespace CashManager_MVVM.CommonData
{
    public class TransactionsProvider
    {
        public TrulyObservableCollection<Transaction> AllTransactions { get; private set; }

        public TransactionsProvider(IQueryDispatcher queryDispatcher)
        {
            //lets cache categories [needed after not loading full categories in transaction query]:
            var catDtos = queryDispatcher.Execute<CategoryQuery, CashManager.Data.DTO.Category[]>(new CategoryQuery());
            var categories = Mapper.Map<Category[]>(catDtos);

            var query = new TransactionQuery();
            DtoTransaction[] dtos = null;
            using (new MeasureTimeWrapper(() => dtos = queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(query), "query transactions")) { }

            Transaction[] transactions = null;
            using (new MeasureTimeWrapper(() => transactions = Mapper.Map<Transaction[]>(dtos), $"map transactions [{dtos.Length}]")) { }
            using (new MeasureTimeWrapper(() => AllTransactions = new TrulyObservableCollection<Transaction>(transactions), "create tru. ob. coll")) { }
        }
    }
}
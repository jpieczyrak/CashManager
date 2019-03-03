using System;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Transactions;
using CashManager.Logic.Wrappers;
using CashManager.Model;

using DtoTransaction = CashManager.Data.DTO.Transaction;

namespace CashManager.CommonData
{
    public class TransactionsProvider
    {
        public TrulyObservableCollection<Transaction> AllTransactions { get; private set; }

        public TransactionsProvider(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            //lets cache categories [needed after not loading full categories in transaction query]:
            Mapper.Map<Category[]>(queryDispatcher.Execute<CategoryQuery, Data.DTO.Category[]>(new CategoryQuery()));

            var query = new TransactionQuery();
            DtoTransaction[] dtos = null;
            using (new MeasureTimeWrapper(() => dtos = queryDispatcher.Execute<TransactionQuery, DtoTransaction[]>(query), "query transactions")) { }


            //todo: remove after migration
            foreach (var dto in dtos)
            {
                dto.Notes.Add(dto.Note);
                dto.Notes = dto.Notes.Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
                dto.Note = string.Empty;
            }
            commandDispatcher.Execute(new UpsertTransactionsCommand(dtos));
            //end todo

            Transaction[] transactions = null;
            using (new MeasureTimeWrapper(() => transactions = Mapper.Map<Transaction[]>(dtos), $"map transactions [{dtos.Length}]")) { }
            using (new MeasureTimeWrapper(() => AllTransactions = new TrulyObservableCollection<Transaction>(transactions), "create tru. ob. coll")) { }
        }
    }
}
using CashManager.Data.DTO;

namespace CashManager.Logic.DefaultData
{
    public interface IDataProvider
    {
        Transaction[] GetTransactions(Stock[] stocks, Category[] categories, TransactionType[] types, Tag[] tags);

        Category[] GetCategories();

        TransactionType[] GetTransactionTypes();

        Tag[] GetTags();

        Stock[] GetStocks();
    }
}
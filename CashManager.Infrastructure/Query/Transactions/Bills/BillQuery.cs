namespace CashManager.Infrastructure.Query.Transactions.Bills
{
    public class BillQuery : IQuery<byte[]>
    {
        public string Id { get; }

        public BillQuery(string id)
        {
            Id = id;
        }
    }
}

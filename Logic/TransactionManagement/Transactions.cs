using System.Collections.ObjectModel;

namespace Logic.TransactionManagement
{
    /// <summary>
    /// List of all transaction saved in app.
    /// </summary>
    public class Transactions
    {
        public ObservableCollection<Transaction> TransactionsList { get; set; } = new ObservableCollection<Transaction>();

        public void Add(Transaction transaction)
        {
            TransactionsList.Add(transaction);
        }
    }
}
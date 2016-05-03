using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Logic.TransactionManagement
{
    /// <summary>
    /// List of all transaction saved in app.
    /// </summary>
    [DataContract(Namespace = "")]
    public class Transactions
    {
        [DataMember]
        public ObservableCollection<Transaction> TransactionsList { get; set; } = new ObservableCollection<Transaction>();

        public void Add(Transaction transaction)
        {
            TransactionsList.Add(transaction);
        }
    }
}
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;

using Logic.FilesOperations;
using Logic.Model;

namespace Logic.TransactionManagement.TransactionElements
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

        public void Save(CSVFormater formater, string filename)
        {
            File.WriteAllText(filename, CSVFormater.ToCSV(this));
        }
    }
}
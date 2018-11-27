using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;

using LogicOld.FilesOperations;
using LogicOld.Model;

namespace LogicOld.TransactionManagement.TransactionElements
{
    /// <summary>
    /// List of all transaction saved in app.
    /// Obsolate. todo: remove
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
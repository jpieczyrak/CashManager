using Logic.TransactionManagement;

namespace Logic
{
    public class TransactionGUI
    {
        public TransactionGUI(Transaction transaction)
        {
            Transaction = transaction;
        }
        public Transaction Transaction { get; set; }
    }
}
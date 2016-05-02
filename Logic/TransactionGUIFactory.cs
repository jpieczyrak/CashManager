using Logic.TransactionManagement;

namespace Logic
{
    public class TransactionGUIFactory
    {
        public static TransactionGUI Create(Transaction transaction)
        {
            return new TransactionGUI(transaction);
        }
    }
}
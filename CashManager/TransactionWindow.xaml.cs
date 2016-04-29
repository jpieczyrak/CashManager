using System.Windows;
using Logic.TransactionManagement;

namespace CashManager
{
    /// <summary>
    /// Interaction logic for TransactionWindow.xaml
    /// </summary>
    public partial class TransactionWindow : Window
    {
        private readonly Transaction _transaction;

        public TransactionWindow(Transaction transaction)
        {
            _transaction = transaction;     //make copy not copy ref!
            InitializeComponent();

            //TODO: make copy of actual transaction
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            //TODO: restore transaction from copy
            //Close();
        }
    }
}

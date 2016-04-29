using System.Windows;

namespace CashManager
{
    /// <summary>
    /// Interaction logic for TransactionWindow.xaml
    /// </summary>
    public partial class TransactionWindow : Window
    {
        public TransactionWindow()
        {
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

using System.Windows;
using System.Windows.Controls;

using Autofac;

namespace CashManager_MVVM.Features.Main.Init
{
    public partial class InitWindow : Window
    {
        public InitWindow(ContainerBuilder builder, string databaseFilepath)
        {
            InitializeComponent();

            DataContext = new InitViewModel(builder, databaseFilepath, Close);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                //could be used "SecurePassword", but we do not need such security
                //someone accessing machine ram is bigger problem than knowing a password
                //we are using passwordbox only to no let know other what password are we typing...
                ((dynamic)DataContext).Password = ((PasswordBox)sender).Password;
            }
        }
    }
}

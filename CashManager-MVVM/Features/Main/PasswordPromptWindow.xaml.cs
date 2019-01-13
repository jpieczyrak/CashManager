using System.Windows;
using System.Windows.Controls;

namespace CashManager_MVVM.Features.Main
{
    public partial class PasswordPromptWindow : Window
    {
        public string PasswordText { get; private set; }

        public PasswordPromptWindow()
        {
            InitializeComponent();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordText = ((PasswordBox) sender).Password;
        }
    }
}
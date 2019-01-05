using System.Windows;

namespace CashManager_MVVM.Features.Main
{
    public partial class PasswordPromptWindow : Window
    {
        public PasswordPromptWindow()
        {
            InitializeComponent();
        }

        public string PasswordText => PasswordTextBox.Text;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

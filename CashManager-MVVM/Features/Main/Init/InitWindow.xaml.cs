using System.Windows;

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
    }
}

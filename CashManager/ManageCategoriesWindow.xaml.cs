using System.Windows;
using LogicOld.LogicObjectsProviders;

namespace CashManager
{
    /// <summary>
    /// Interaction logic for ManageCategoriesWindow.xaml
    /// </summary>
    public partial class ManageCategoriesWindow : Window
    {
        public ManageCategoriesWindow()
        {
            InitializeComponent();

            dataGridCategories.ItemsSource = CategoryProvider.Categories;
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonAddEmpty_Click(object sender, RoutedEventArgs e)
        {
            CategoryProvider.FindOrCreate("empty");
        }
    }
}

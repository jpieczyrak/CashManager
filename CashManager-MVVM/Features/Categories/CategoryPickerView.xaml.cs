using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CashManager_MVVM.Features.Categories
{
    /// <summary>
    /// Interaction logic for CategoryPickerView.xaml
    /// </summary>
    public partial class CategoryPickerView : Window
    {
        public CategoryPickerView(CategoryViewModel viewmodel, Model.Category category)
        {
            InitializeComponent();
			DataContext = viewmodel;
			UpdatedSelectedCategories(category, viewmodel.Categories);
        }

        private static void UpdatedSelectedCategories(Model.Category category, IEnumerable<Model.Category> categories)
        {
            foreach (var cat in categories)
            {
                cat.IsSelected = category != null && cat.Name == category.Name;
                if (cat.Children.Any())
                {
                    UpdatedSelectedCategories(category, cat.Children);
                }
            }
        }
    }
}

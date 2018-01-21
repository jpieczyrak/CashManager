using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CashManager_MVVM.Model;
using CashManager_MVVM.ViewModel;

namespace CashManager_MVVM.View
{
    /// <summary>
    /// Interaction logic for CategoryPickerView.xaml
    /// </summary>
    public partial class CategoryPickerView : Window
    {
        public CategoryPickerView(Category category)
        {
            InitializeComponent();
            if (DataContext is CategoriesViewModel dataContext)
            {
                dataContext.SelectedCategory = category;
                if (category != null)
                {
                    UpdatedSelectedCategories(category, dataContext.Categories);
                }
            }
        }

        private static void UpdatedSelectedCategories(Category category, IEnumerable<Category> categories)
        {
            foreach (var cat in categories)
            {
                cat.IsSelected = cat.Value == category.Value;
                if (cat.Children.Any())
                {
                    UpdatedSelectedCategories(category, cat.Children);
                }
            }
        }
    }
}

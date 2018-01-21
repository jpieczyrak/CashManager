using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.DataProviders;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CashManager_MVVM.ViewModel
{
    public class CategoriesViewModel : ViewModelBase
    {
        public IEnumerable<Category> Categories { get; set; }
        public Category SelectedCategory { get; set; }

        public RelayCommand<Window> CloseCommand { get; set; }
        public RelayCommand<Category> UpdateSelectedCategory { get; set; }

        public CategoriesViewModel(IDataService dataService)
        {
            CloseCommand = new RelayCommand<Window>(window => window?.Close());
            UpdateSelectedCategory = new RelayCommand<Category>(category => SelectedCategory = category);
            dataService.GetCategories((categories, exception) =>
            {
                if (categories != null)
                {
                    foreach (var category in categories.ToArray())
                    {
                        category.Children = categories.Where(x => x.Parent?.Id == category?.Id).ToArray();
                    }
                    Categories = categories.Where(x => x.Parent == null).ToArray();
                }
            });
        }
    }
}
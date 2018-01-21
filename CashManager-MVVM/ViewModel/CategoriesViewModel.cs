using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.DataProviders;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.ViewModel
{
    public class CategoriesViewModel : ViewModelBase
    {
        public IEnumerable<Category> Categories { get; set; }

        public CategoriesViewModel(IDataService dataService)
        {
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
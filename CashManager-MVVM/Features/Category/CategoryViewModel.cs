using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CashManager_MVVM.Model.DataProviders;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CashManager_MVVM.Features.Category
{
    public class CategoryViewModel : ViewModelBase
    {
		public IEnumerable<Model.Category> Categories { get; set; }

		public Model.Category SelectedCategory { get; set; }

		public RelayCommand<Window> CloseCommand => new RelayCommand<Window>(window => window?.Close());
        public RelayCommand<Model.Category> UpdateSelectedCategory => new RelayCommand<Model.Category>(category => SelectedCategory = category);

        public CategoryViewModel(IDataService dataService)
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
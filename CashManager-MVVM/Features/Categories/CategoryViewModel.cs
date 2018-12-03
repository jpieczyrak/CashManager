using System.Collections.Generic;
using System.Linq;
using System.Windows;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CashManager_MVVM.Features.Categories
{
    public class CategoryViewModel : ViewModelBase
    {
        public IEnumerable<Model.Category> Categories { get; set; }

        public Model.Category SelectedCategory { get; set; }

        public RelayCommand<Window> CloseCommand => new RelayCommand<Window>(window => window?.Close());

        public RelayCommand<Model.Category> UpdateSelectedCategory =>
            new RelayCommand<Model.Category>(category => SelectedCategory = category);

        public CategoryViewModel(IQueryDispatcher queryDispatcher)
        {
            var categories = queryDispatcher.Execute<CategoryQuery, CashManager.Data.DTO.Category[]>(new CategoryQuery())
                                            .Select(Mapper.Map<Model.Category>)
                                            .ToArray();

            foreach (var category in categories) category.Children = new TrulyObservableCollection<Category>(categories.Where(x => x.Parent?.Id == category?.Id));

            Categories = categories.Where(x => x.Parent == null).ToArray(); //find the root(s)
        }
    }
}
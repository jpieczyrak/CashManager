using System.Collections.Generic;
using System.Linq;
using System.Windows;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using DtoCategory = CashManager.Data.DTO.Category;

namespace CashManager_MVVM.Features.Categories
{
    public class CategoryPickerViewModel : ViewModelBase
    {
        private Category _selectedCategory;

        public IEnumerable<Category> Categories { get; }
        public Category[] FlatCategories { get; }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set => Set(ref _selectedCategory, value);
        }

        public RelayCommand<Window> CloseCommand => new RelayCommand<Window>(window => window?.Close());

        public RelayCommand<Category> UpdateSelectedCategory => new RelayCommand<Category>(ExecuteUpdateSelectedCategory);

        public CategoryPickerViewModel(IQueryDispatcher queryDispatcher, Category selectedCategory = null)
        {
            var categories = queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery())
                                            .Select(Mapper.Map<Category>)
                                            .ToArray();

            foreach (var category in categories)
                category.Children = new TrulyObservableCollection<Category>(categories.Where(x => x.Parent?.Id == category.Id).OrderBy(x => x.Name));

            FlatCategories = categories.ToArray();
            Categories = categories.Where(x => x.Parent == null).OrderBy(x => x.Name).ToArray(); //find the root(s)
            SelectedCategory = selectedCategory;

            if (selectedCategory != null)
            {
                var selected = categories.FirstOrDefault(x => x.Id == selectedCategory.Id);
                if (selected != null) selected.IsSelected = true;
                ExpandParents(selected);
            }
        }

        private void ExecuteUpdateSelectedCategory(Category category)
        {
            if (category != null) SelectedCategory = category;
        }

        private void ExpandParents(Category selected)
        {
            var parent = FlatCategories.FirstOrDefault(x => x.Children.Contains(selected));
            if (parent != null)
            {
                parent.IsExpanded = true;
                ExpandParents(parent);
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;

using CashManager.WPF.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using DtoCategory = CashManager.Data.DTO.Category;

namespace CashManager.WPF.Features.Categories
{
    public class CategoryPickerViewModel : ViewModelBase
    {
        private readonly ExpandableCategory[] _flatCategories;
        private Category _selectedCategory;

        public IEnumerable<ExpandableCategory> Categories { get; }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set => Set(ref _selectedCategory, value);
        }

        public Category DefaultCategory { get; }

        public RelayCommand<ExpandableCategory> UpdateSelectedCategory => new RelayCommand<ExpandableCategory>(ExecuteUpdateSelectedCategory);

        public CategoryPickerViewModel(IQueryDispatcher queryDispatcher, Category selectedCategory = null)
        {
            var categories = queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery())
                                            .Select(Mapper.Map<ExpandableCategory>)
                                            .ToArray();

            foreach (var category in categories)
                category.Children = new TrulyObservableCollection<ExpandableCategory>(categories.Where(x => x.Parent?.Id == category.Id).OrderBy(x => x.Name));

            _flatCategories = categories.ToArray();
            Categories = categories.Where(x => x.Parent == null).OrderBy(x => x.Name).ToArray(); //find the root(s)
            DefaultCategory = Mapper.Map<Category>(categories.FirstOrDefault());
            SelectedCategory = selectedCategory;

            if (selectedCategory != null)
            {
                var selected = categories.FirstOrDefault(x => x.Id == selectedCategory.Id);
                if (selected != null) selected.IsSelected = true;
                ExpandParents(selected);
            }
        }

        private void ExecuteUpdateSelectedCategory(ExpandableCategory category)
        {
            if (category != null) SelectedCategory = Mapper.Map<Category>(category);
        }

        private void ExpandParents(ExpandableCategory selected)
        {
            var parent = _flatCategories.FirstOrDefault(x => x.Children.Contains(selected));
            if (parent != null)
            {
                parent.IsExpanded = true;
                ExpandParents(parent);
            }
        }
    }
}
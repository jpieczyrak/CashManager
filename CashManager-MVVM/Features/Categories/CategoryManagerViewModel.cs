﻿using System;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Categories;
using CashManager.Infrastructure.Command.Stocks;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using DtoCategory = CashManager.Data.DTO.Category;

namespace CashManager_MVVM.Features.Categories
{
    public class CategoryManagerViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public TrulyObservableCollection<Category> Categories { get; private set; }

        public RelayCommand<Category> UpdateSelectedCategory => new RelayCommand<Category>(category => SelectedCategory = category);

        public RelayCommand AddCategoryCommand => new RelayCommand(ExecuteAddCategoryCommand);

        public RelayCommand RemoveCategoryCommand => new RelayCommand(ExecuteRemoveCategoryCommand);

        public Category SelectedCategory { get; private set; }

        public CategoryManagerViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            var categories = _queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery())
                                             .Select(Mapper.Map<Category>)
                                             .ToArray();

            foreach (var category in categories)
                category.Children = new TrulyObservableCollection<Category>(categories.Where(x => x.Parent?.Id == category.Id));

            Categories = new TrulyObservableCollection<Category>(categories.Where(x => x.Parent == null)); //find the root(s)
        }

        public void Move(Category sourceCategory, Category targetCategory)
        {
            if (sourceCategory != null && targetCategory != null && sourceCategory.Parent != null && sourceCategory.Id != targetCategory.Id)
            {
                var sourceParentId = sourceCategory.Parent.Id;

                //swap
                sourceCategory.Parent = targetCategory;
                targetCategory.Children.Add(sourceCategory);

                //remove from old position
                var previousParent = Find(Categories.ToArray(), sourceParentId);
                if (previousParent != null)
                {
                    previousParent.Children.Remove(sourceCategory);
                    UpsertCategory(sourceCategory);
                }
            }
        }

        public Category Find(Category[] categories, Guid id)
        {
            foreach (var category in categories)
            {
                if (category.Id == id) return category;
                if (category.Children?.Any() ?? false)
                {
                    var result = Find(category.Children.ToArray(), id);
                    if (result != null) return result;
                }
            }

            return null;
        }

        private void ExecuteAddCategoryCommand()
        {
            var parent = SelectedCategory ?? Categories.FirstOrDefault();
            var category = parent != null ? new Category { Value = "New category" } : new Category { Value = "Root" };
            if (parent != null)
            {
                parent.Children.Add(category);
                category.Parent = parent;
            }
            else Categories.Add(category);
            UpsertCategory(category);
        }

        private void ExecuteRemoveCategoryCommand()
        {
            if (SelectedCategory != null && SelectedCategory.Parent != null)
            {
                var input = Categories.ToArray();
                var selected = Find(input, SelectedCategory.Id);
                var parent = Find(input, selected.Parent.Id);
                foreach (var child in selected.Children)
                {
                    child.Parent = parent;
                    parent.Children.Add(child);
                }

                parent.Children.Remove(selected);
                UpsertCategories(selected.Children.ToArray());
                _commandDispatcher.Execute(new DeleteCategoryCommand(Mapper.Map<DtoCategory>(selected)));
                //todo: find all tran.positions which used selected category and change it for parent. then save.
            }
        }

        private void UpsertCategory(Category category)
        {
            var categories = new [] { category };
            UpsertCategories(categories);
        }

        private void UpsertCategories(Category[] categories)
        {
            _commandDispatcher.Execute(new UpsertCategoriesCommand(Mapper.Map<DtoCategory[]>(categories)));
        }
    }
}
﻿using System;
using System.Linq;
using System.Windows;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Categories;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using GongSolutions.Wpf.DragDrop;

using DtoCategory = CashManager.Data.DTO.Category;

namespace CashManager_MVVM.Features.Categories
{
    public class CategoryManagerViewModel : ViewModelBase, IDropTarget
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private string _categoryName;
        private Category _selectedCategory;

        public TrulyObservableCollection<Category> Categories { get; private set; }

        public RelayCommand AddCategoryCommand => new RelayCommand(ExecuteAddCategoryCommand);

        public RelayCommand RemoveCategoryCommand => new RelayCommand(ExecuteRemoveCategoryCommand, CanExecuteRemoveCategoryCommand);

        public Category SelectedCategory
        {
            get => _selectedCategory;
            private set
            {
                Set(ref _selectedCategory, value);
                RaisePropertyChanged(nameof(RemoveCategoryCommand));
            }
        }

        public string CategoryName
        {
            get => _categoryName;
            set => Set(nameof(CategoryName), ref _categoryName, value);
        }

        public CategoryManagerViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _categoryName = "New category";
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            var categories = _queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery())
                                             .Select(Mapper.Map<Category>)
                                             .ToArray();

            foreach (var category in categories)
            {
                category.Children = new TrulyObservableCollection<Category>(categories.Where(x => x.Parent?.Id == category.Id));
                category.PropertyChanged += (sender, args) =>
                {
                    if (category.IsSelected) SelectedCategory = category;
                };
            }

            Categories = new TrulyObservableCollection<Category>(categories.Where(x => x.Parent == null)); //find the root(s)
            
            SelectedCategory = categories.FirstOrDefault(x => x.IsSelected);
        }

        private void Move(Category sourceCategory, Category targetCategory)
        {
            if (sourceCategory == null || targetCategory == null) return;
            if (sourceCategory.Id == targetCategory.Id) return;

            //target can not be a (grand)children
            if (Find(sourceCategory.Children.ToArray(), targetCategory.Id) != null) return;

            var sourceParentId = sourceCategory.Parent?.Id;

            //swap
            sourceCategory.Parent = targetCategory;
            targetCategory.Children.Add(sourceCategory);

            //remove from old position
            if (sourceParentId == null)
                Categories.Remove(sourceCategory);
            else
                Find(Categories.ToArray(), sourceParentId.Value)?.Children.Remove(sourceCategory);

            //update parent in db
            UpsertCategory(sourceCategory);
        }

        private Category Find(Category[] categories, Guid id)
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
            var parent = SelectedCategory;
            var category = new Category { Name = CategoryName };
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
            if (SelectedCategory?.Parent != null)
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

        private bool CanExecuteRemoveCategoryCommand() => SelectedCategory?.Parent != null;

        private void UpsertCategory(Category category)
        {
            var categories = new [] { category };
            UpsertCategories(categories);
        }

        private void UpsertCategories(Category[] categories)
        {
            _commandDispatcher.Execute(new UpsertCategoriesCommand(Mapper.Map<DtoCategory[]>(categories)));
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = dropInfo.Data is Category
                                   ? DragDropEffects.Copy
                                   : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Category source && dropInfo.TargetItem is Category target) Move(source, target);
        }
    }
}
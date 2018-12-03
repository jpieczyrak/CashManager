using System;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Categories;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;

using DtoCategory = CashManager.Data.DTO.Category;

namespace CashManager_MVVM.Features.Categories
{
    public class CategoryManagerViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public TrulyObservableCollection<Category> Categories { get; private set; }

        public CategoryManagerViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            var categories = _queryDispatcher.Execute<CategoryQuery, CashManager.Data.DTO.Category[]>(new CategoryQuery())
                                            .Select(Mapper.Map<Category>)
                                            .ToArray();

            foreach (var category in categories) category.Children = new TrulyObservableCollection<Category>(categories.Where(x => x.Parent?.Id == category.Id));
            
            Categories = new TrulyObservableCollection<Category>(categories.Where(x => x.Parent == null)); //find the root(s)
        }

        public void Move(Category sourceCategory, Category targetCategory)
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

        private void UpsertCategory(Category sourceCategory)
        {
            _commandDispatcher.Execute(new UpsertCategoriesCommand(new[] { Mapper.Map<DtoCategory>(sourceCategory) }));
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
    }
}
using System;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Categories
{
    public class CategoryManagerViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public TrulyObservableCollection<Category> Categories { get; private set; }

        public CategoryManagerViewModel(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
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
            Find(Categories.ToArray(), sourceParentId)?.Children.Remove(sourceCategory);
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
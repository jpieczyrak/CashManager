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

            foreach (var category in categories) category.Children = categories.Where(x => x.Parent?.Id == category.Id).ToList();
            
            Categories = new TrulyObservableCollection<Category>(categories.Where(x => x.Parent == null)); //find the root(s)
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Model.Common;

namespace CashManager.Model
{
    public class Category : BaseObservableObject
    {
        private Category _parent;

        public Category Parent
        {
            get => _parent;
            set => Set(nameof(Parent), ref _parent, value);
        }

        public Category() { }

        public Category(Guid id) : base(id) { }

        public bool MatchCategoryFilter(Category category)
        {
            return category?.GetParentsId().Contains(Id) ?? false;
        }

        public bool MatchCategoryFilter(List<Guid> filter)
        {
            var ids = GetCategoriesChain(new Stack<Guid>());

            return filter.Count <= ids.Count && filter.All(guid => guid == ids.Pop());
        }

        /// <summary>
        ///     Gets category ids from parents - making a chain of ids.
        /// </summary>
        /// <param name="ids">Stack of ids given from child or new if its "leaf" children</param>
        /// <returns></returns>
        public Stack<Guid> GetCategoriesChain(Stack<Guid> ids)
        {
            ids.Push(Id);
            Parent?.GetCategoriesChain(ids);

            return ids;
        }

        public Guid[] GetParentsId()
        {
            var results = new[] { Id };
            return Parent?.GetParentsId().Concat(results).ToArray() ?? results;
        }

        public int CountParents()
        {
            return Parent?.CountParents() + 1 ?? 0;
        }

        #region Override

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
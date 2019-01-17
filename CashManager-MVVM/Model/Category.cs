using System;
using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
{
    public class Category : BaseObservableObject
    {
        private Category _parent;
        private bool _isExpanded;
        private bool _isSelected;

        /// <summary>
        /// Used for tree view purpose
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }

        public Category Parent
        {
            get => _parent;
            set => Set(nameof(Parent), ref _parent, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public TrulyObservableCollection<Category> Children { get; set; } = new TrulyObservableCollection<Category>();

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
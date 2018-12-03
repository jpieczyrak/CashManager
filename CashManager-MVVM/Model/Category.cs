using System;
using System.Collections.Generic;
using System.Linq;

namespace CashManager_MVVM.Model
{
    public class Category : BaseObservableObject
    {
        private string _value;

        private Category _parent;

        public bool IsExpanded { get; set; } = true;

        public string Value
        {
            get => _value;
            set => Set(nameof(Value), ref _value, value);
        }

        public Category Parent
        {
            get => _parent;
            set => Set(nameof(Parent), ref _parent, value);
        }

        public TrulyObservableCollection<Category> Children { get; set; } = new TrulyObservableCollection<Category>();

        public bool IsSelected { get; set; }

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
        
        #region Override
        
        public override string ToString()
        {
            return Value;
        }

        #endregion
    }
}
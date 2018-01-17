using System;
using System.Collections.Generic;
using System.Linq;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model
{
    public class Category : ObservableObject
    {
        private string _value;

        private Category _parent;

        public Guid Id { get; private set; }

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

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        #endregion
    }
}
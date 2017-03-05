using System;
using System.Collections.Generic;
using System.Linq;

using Logic.LogicObjectsProviders;

namespace Logic.Model
{
    public class Category
    {
        private string _value;

        private Category _parent;

        public Guid ParentId { get; set; }

        public Guid Id { get; private set; }

        public string Value
        {
            get { return _value; }
            set
            {
                if (value != null)
                {
                    _value = value;
                }
            }
        }

        public Category Parent => _parent ?? (_parent = CategoryProvider.GetById(ParentId));

        public bool MatchCategoryFilter(List<Guid> filter)
        {
            var ids = GetCategoriesChain(new Stack<Guid>());

            return filter.Count <= ids.Count && filter.All(guid => guid == ids.Pop());
        }

        /// <summary>
        /// Gets category ids from parents - making a chain of ids.
        /// </summary>
        /// <param name="ids">Stack of ids given from child or new if its "leaf" children</param>
        /// <returns></returns>
        public Stack<Guid> GetCategoriesChain(Stack<Guid> ids)
        {
            ids.Push(Id);
            Parent?.GetCategoriesChain(ids);

            return ids;
        }

        public Category(string value)
        {
            Value = value;
            Id = Guid.NewGuid();
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
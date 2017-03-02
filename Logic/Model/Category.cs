using System;
using System.Collections.Generic;

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
            List<Guid> ids = new List<Guid>();
            GetCategoriesChain(ids);
            
            if (filter.Count > ids.Count) return false;

            ids.Reverse();

            for (int i = 0; i < filter.Count; i++)
            {
                if (filter[i] != ids[i]) return false;
            }
            return true;
        }

        public void GetCategoriesChain(List<Guid> ids)
        {
            ids.Add(Id);
            Parent?.GetCategoriesChain(ids);
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Logic.Annotations;
using Logic.LogicObjectsProviders;

namespace Logic.Model
{
    public class Category : INotifyPropertyChanged
    {
        private string _value;

        private Category _parent;
        private Guid _parentId;

        public Guid ParentId
        {
            get { return _parentId; }
            set
            {
                _parentId = value;
                OnPropertyChanged(nameof(ParentId));
            }
        }

        public Guid Id { get; private set; }

        public string Value
        {
            get { return _value; }
            set
            {
                if (value != null)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public Category Parent => _parent ?? (_parent = CategoryProvider.GetById(ParentId));

        public Category(string value)
        {
            Value = value;
            Id = Guid.NewGuid();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
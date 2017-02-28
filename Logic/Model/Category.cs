using System;

namespace Logic.Model
{
    public class Category
    {
        public Guid Id { get; set; }

        private string _value;

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
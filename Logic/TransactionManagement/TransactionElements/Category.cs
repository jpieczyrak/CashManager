using System;
using System.Runtime.Serialization;

namespace Logic.TransactionManagement.TransactionElements
{
    [DataContract]
    public class Category
    {
        private string _value;

        [DataMember] private readonly Guid _id;

        [DataMember]
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

        private Category() { }

        public Category(string value)
        {
            Value = value;
            _id = Guid.NewGuid();
        }

        #region Override

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        #endregion
    }
}
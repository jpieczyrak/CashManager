using System.Runtime.Serialization;

namespace Logic.TransactionManagement.TransactionElements
{
    [DataContract]
    public class Category
    {
        private string _value;

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
        }

        #region Override

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            if (Value != null)
            {
                return Value.GetHashCode();
            }
            return -1;
        }

        public override string ToString()
        {
            return Value;
        }

        #endregion
    }
}
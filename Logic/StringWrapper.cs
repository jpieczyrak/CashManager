using System.Runtime.Serialization;

namespace Logic
{
    [DataContract]
    public class StringWrapper
    {
        [DataMember]
        public string Value { get; set; }

        public StringWrapper(string value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            StringWrapper stringWrapper = obj as StringWrapper;
            if (stringWrapper != null)
            {
                string val = stringWrapper.Value;
                return Value.Equals(val);
            }
            return false;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
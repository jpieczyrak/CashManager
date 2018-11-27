using System;
using System.Runtime.Serialization;

namespace LogicOld.DTO
{
    [DataContract]
    public class Category
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Category Parent { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}
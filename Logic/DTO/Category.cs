using System;
using System.Runtime.Serialization;

namespace Logic.DTO
{
    [DataContract]
    public class Category
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Value { get; set; }

        public Category() { }
    }
}
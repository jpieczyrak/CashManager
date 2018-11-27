using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LogicOld.DTO
{
    [DataContract(Namespace = "")]
    public class Position
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public PaymentValue Value { get; set; }
        
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public List<Tag> Tags { get; set; }

        [DataMember]
        public Category Category { get; set; }
    }
}
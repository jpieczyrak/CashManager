using System;
using System.Runtime.Serialization;

namespace Logic.DTO
{
    [DataContract(Namespace = "")]
    public class Subtransaction
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public double Value { get; set; }

        [DataMember]
        public Guid CategoryId { get; set; }

        [DataMember]
        public string Tags { get; set; }

        [DataMember]
        public Guid Id { get; set; }
    }
}
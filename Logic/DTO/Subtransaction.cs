using System;
using System.Runtime.Serialization;

namespace Logic.DTO
{
    [DataContract(Namespace = "")]
    public class Subtransaction
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public PaymentValue Value { get; set; }

        [DataMember]
        public Guid CategoryId { get; set; }
        
        [DataMember]
        public Guid Id { get; set; }
    }
}
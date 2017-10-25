using System;
using System.Runtime.Serialization;

using Logic.TransactionManagement.TransactionElements;

namespace Logic.DTO
{
    [DataContract(Namespace = "")]
    public class Payment
    {
        [DataMember]
        public Stock Source { get; set; }
        public Stock Target { get; set; }

        [DataMember]
        public double Value { get; set; }
    }
}
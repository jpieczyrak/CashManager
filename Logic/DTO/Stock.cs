using System;
using System.Runtime.Serialization;

namespace Logic.DTO
{
    [DataContract(Namespace = "")]
    public class Stock
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid Id { get; private set; }

        [DataMember]
        public double StartingValue { get; set; }

        [DataMember]
        public double ActualValue { get; private set; }

        [DataMember]
        public bool IsUserStock { get; set; }
    }
}
using System;
using System.Runtime.Serialization;

namespace LogicOld.DTO
{
    [DataContract(Namespace = "")]
    public class Stock
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsUserStock { get; set; }
        
        [DataMember]
        public Guid Id { get; set; }
    }
}
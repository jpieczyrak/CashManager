using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Logic.TransactionManagement.TransactionElements;

namespace Logic.DTO
{
    [DataContract(Namespace = "")]
    public class Transaction
    {
        [DataMember]
        public eTransactionType Type { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Note { get; set; }
        
        [DataMember]
        public List<Subtransaction> Subtransactions { get; set; }

        [DataMember]
        public List<Tag> Tags { get; set; }

        [DataMember]
        public DateTime InstanceCreationDate { get; set; }

        [DataMember]
        public DateTime TransationSourceCreationDate { get; set; }

        [DataMember]
        public DateTime LastEditDate { get; set; }

        [DataMember]
        public DateTime BookDate { get; set; }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Stock UserStock { get; set; }

        [DataMember]
        public Stock ExternalStock { get; set; }
    }
}
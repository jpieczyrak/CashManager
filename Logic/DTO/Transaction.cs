using System;
using System.Runtime.Serialization;

using Logic.Model;
using Logic.TransactionManagement.TransactionElements;
using Logic.Utils;

namespace Logic.DTO
{
    [DataContract(Namespace = "")]
    public class Transaction
    {
        [DataMember]
        public eTransactionType Type { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public TrulyObservableCollection<TransactionPartPayment> TransactionSoucePayments { get; set; }

        [DataMember]
        public Guid TargetStockId { get; set; }

        [DataMember]
        public TrulyObservableCollection<Subtransaction> Subtransactions { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public DateTime LastEditDate { get; set; }

        [DataMember]
        public Guid Id { get; set; }
    }
}
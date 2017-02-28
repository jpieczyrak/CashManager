using System;
using System.Runtime.Serialization;

using Logic.TransactionManagement.TransactionElements;

namespace Logic.DTO
{
    [DataContract(Namespace = "")]
    public class TransactionPartPayment
    {
        [DataMember]
        public Guid StockId { get; set; }

        [DataMember]
        public double Value { get; set; }

        [DataMember]
        public ePaymentType PaymentType { get; set; }

        [DataMember]
        public Guid Id { get; set; }
    }
}
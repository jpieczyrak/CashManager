using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

using LogicOld.TransactionManagement.TransactionElements;

namespace LogicOld.DTO
{
    [DataContract(Namespace = "")]
    public class Transaction
    {
        private Transaction() { }

        [DataMember]
        public eTransactionType Type { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Note { get; set; }
        
        [DataMember]
        public List<Position> Positions { get; set; }

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

        /// <summary>
        /// Should be used only after parsing data or for test purpose.
        /// Otherwise please use paramless constructor
        /// </summary>
        /// <param name="transactionType">Tranasction type</param>
        /// <param name="sourceTransactionCreationDate">When transaction was performed</param>
        /// <param name="title">Title of transaction</param>
        /// <param name="note">Additional notes</param>
        /// <param name="positions">Positions - like positions from bill</param>
        /// <param name="userStock">User stock like wallet / bank account</param>
        /// <param name="externalStock">External stock like employer / shop</param>
        /// <param name="sourceInput">Text source of transaction (for parsing purpose) to provide unique id</param>
        public Transaction(eTransactionType transactionType, DateTime sourceTransactionCreationDate, string title, string note,
            List<Position> positions, Stock userStock, Stock externalStock, string sourceInput)
        {
            Id = GenerateGUID(sourceInput);
            Type = transactionType;
            Title = title;
            Note = note;
            BookDate = TransationSourceCreationDate = sourceTransactionCreationDate;
            LastEditDate = InstanceCreationDate = DateTime.Now;
            Positions = new List<Position>(positions);
            UserStock = userStock;
            ExternalStock = externalStock;
        }

        /// <summary>
        /// Generates GUID based on input (original transaction text - from excel / bank import etc)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Guid GenerateGUID(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                    return new Guid(hash);
                }
            }

            return Guid.NewGuid();
        }
    }
}
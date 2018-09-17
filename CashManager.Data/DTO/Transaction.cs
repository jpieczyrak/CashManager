using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CashManager.Data.DTO
{
    public class Transaction
    {
		public eTransactionType Type { get; set; }

		public string Title { get; set; }

		public string Note { get; set; }

		public List<Subtransaction> Subtransactions { get; set; }

		public DateTime InstanceCreationDate { get; set; }

		public DateTime TransationSourceCreationDate { get; set; }

		public DateTime LastEditDate { get; set; }

		public DateTime BookDate { get; set; }

		public Guid Id { get; set; }

		public Stock UserStock { get; set; }

		public Stock ExternalStock { get; set; }

		public Transaction()
		{
			Id = Guid.NewGuid();
		}

		/// <summary>
        /// Should be used only after parsing data or for test purpose.
        /// Otherwise please use paramless constructor
        /// </summary>
        /// <param name="transactionType">Tranasction type</param>
        /// <param name="sourceTransactionCreationDate">When transaction was performed</param>
        /// <param name="title">Title of transaction</param>
        /// <param name="note">Additional notes</param>
        /// <param name="subtransactions">Subtransactions - like positions from bill</param>
        /// <param name="userStock">User stock like wallet / bank account</param>
        /// <param name="externalStock">External stock like employer / shop</param>
        /// <param name="sourceInput">Text source of transaction (for parsing purpose) to provide unique id</param>
        public Transaction(eTransactionType transactionType, DateTime sourceTransactionCreationDate, string title, string note,
            List<Subtransaction> subtransactions, Stock userStock, Stock externalStock, string sourceInput)
        {
            Id = GenerateGUID(sourceInput);
            Type = transactionType;
            Title = title;
            Note = note;
            BookDate = TransationSourceCreationDate = sourceTransactionCreationDate;
            LastEditDate = InstanceCreationDate = DateTime.Now;
            Subtransactions = new List<Subtransaction>(subtransactions);
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
		
		public override bool Equals(object obj) => obj?.GetHashCode() == GetHashCode();

		public override int GetHashCode() => Id.GetHashCode();
    }
}
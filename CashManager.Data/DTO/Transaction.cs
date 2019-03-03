using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.Extensions;

namespace CashManager.Data.DTO
{
    public class Transaction : Dto
    {
		public TransactionType Type { get; set; }

		public string Title { get; set; }

		public List<string> Notes { get; set; }

		public List<Position> Positions { get; set; }

		public List<StoredFileInfo> StoredFiles { get; set; }

        public DateTime TransactionSourceCreationDate { get; set; }

		public DateTime BookDate { get; set; }

		public Stock UserStock { get; set; }

		public Stock ExternalStock { get; set; }

        public string Note { get; set; }

        public Transaction()
        {
            TransactionSourceCreationDate = DateTime.MinValue;
            BookDate = LastEditDate = InstanceCreationDate = DateTime.Now;
            Positions = new List<Position>();
            Notes = new List<string>();
        }

        public Transaction(Guid id) : this()
        {
            Id = id;
            StoredFiles = new List<StoredFileInfo>();
        }

        /// <summary>
        /// Should be used only after parsing data or for test purpose.
        /// Otherwise please use paramless constructor
        /// </summary>
        /// <param name="type">Transaction type</param>
        /// <param name="sourceTransactionCreationDate">When transaction was performed</param>
        /// <param name="title">Title of transaction</param>
        /// <param name="note">Additional notes</param>
        /// <param name="positions">Positions - like positions from bill</param>
        /// <param name="userStock">User stock like wallet / bank account</param>
        /// <param name="externalStock">External stock like employer / shop</param>
        public Transaction(TransactionType type, DateTime sourceTransactionCreationDate, string title, string note,
            IEnumerable<Position> positions, Stock userStock, Stock externalStock) : this()
        {
            Id = ImportGuid(type, sourceTransactionCreationDate, title, positions);
            Type = type;
            Title = title;
            Notes = new List<string> { note };
            BookDate = TransactionSourceCreationDate = sourceTransactionCreationDate;
            Positions = new List<Position>(positions);
            UserStock = userStock;
            ExternalStock = externalStock;
        }

        private static Guid ImportGuid(TransactionType type, DateTime sourceTransactionCreationDate, string title, IEnumerable<Position> positions)
        {
            return $"{sourceTransactionCreationDate};{title};{((type?.Income ?? false) ? 1 : -1) * positions.Sum(x => x.Value.GrossValue)}".GenerateGuid();
        }

        public void RecalculateId()
        {
            Id = ImportGuid(Type, TransactionSourceCreationDate, Title, Positions);
        }
    }
}
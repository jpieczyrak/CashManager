using System.Collections.Generic;

namespace CashManager.Data.DTO
{
	public class Subtransaction : Dto
	{
		public string Title { get; set; }

		public PaymentValue Value { get; set; }

		public List<Tag> Tags { get; set; }

		public Category Category { get; set; }
	}
}

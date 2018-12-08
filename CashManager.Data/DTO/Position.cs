using System.Collections.Generic;

namespace CashManager.Data.DTO
{
	public class Position : Dto
	{
	    public Position()
	    {
            Value = new PaymentValue();
	        Tags = new List<Tag>();
        }

	    public Position(string title, double value) : this()
        {
            Title = title;
            Value = new PaymentValue { GrossValue = value };
        }

        public string Title { get; set; }

		public PaymentValue Value { get; set; }

		public List<Tag> Tags { get; set; }

		public Category Category { get; set; }
	}
}

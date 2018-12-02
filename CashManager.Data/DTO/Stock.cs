using System;

namespace CashManager.Data.DTO
{
	public class Stock : Dto
	{
	    public Stock() { }

	    public Stock(Guid id) { Id = id; }

	    public string Name { get; set; }

		public bool IsUserStock { get; set; }

		public double Balance { get; set; }
	}
}

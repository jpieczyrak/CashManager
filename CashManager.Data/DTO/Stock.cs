using System;

namespace CashManager.Data.DTO
{
	public class Stock : Dto
	{
	    public Stock() { Balance = new Balance(); }

	    public Stock(Guid id) { Id = id; }

	    public string Name { get; set; }

	    public bool IsUserStock { get; set; }

		public Balance Balance { get; set; }
	}
}

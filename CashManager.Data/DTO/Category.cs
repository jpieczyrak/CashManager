namespace CashManager.Data.DTO
{
	public class Category : Dto
	{
		public Category Parent { get; set; }

		public string Value { get; set; }
	}
}

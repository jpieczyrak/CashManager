using CashManager.Infrastructure.Query.NoQueries;

using Xunit;

namespace CashManager.Tests.Infrastructure.Queries.NoQuery
{
	public class NoQueryTests
	{
		[Fact]
		public void NoQueryHandler_NoQueryEmptyDatabase_EmptyArray()
		{
            //given
			var handler = new NoQueryHandler();
			var query = new CashManager.Infrastructure.Query.NoQueries.NoQuery();

			//when
			var result = handler.Execute(query);

			//then
			Assert.NotNull(result);
			Assert.Empty(result);
		}
    }
}

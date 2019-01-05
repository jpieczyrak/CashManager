using CashManager.Logic.Parsers;

using Xunit;

namespace CashManager.Tests.Parsers
{
    public class CustomCsvParserTests : BaseParserTests
    {
        [Fact]
        public void Parse_None_Empty()
        {
            //given
            var rules = new Rule[0];
            var parser = new CustomCsvParser(rules);
            var input = "";

            //when
            var result = parser.Parse(input, null, null, null, null);

            //then
            Assert.Empty(result);
        }
    }
}
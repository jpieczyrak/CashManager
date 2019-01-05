using CashManager.Logic.Parsers.Custom;

using Xunit;

namespace CashManager.Tests.Parsers.Custom
{
    public class CustomCsvParserTests : BaseParserTests
    {
        [Fact]
        public void Parse_NoneRulesEmptyInput_Empty()
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

        [Fact]
        public void Parse_SomeRulesEmptyInput_Empty()
        {
            //given
            var rules = new[] { new Rule() };
            var parser = new CustomCsvParser(rules);
            var input = "";

            //when
            var result = parser.Parse(input, null, null, null, null);

            //then
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_NoRulesNoneEmptyInput_Empty()
        {
            //given
            var rules = new Rule[0];
            var parser = new CustomCsvParser(rules);
            var input = ";;;1\r\n;;;2\r\n;;;3\r\n;;;4\r\n;;;5\r\n;;;6\r\n;;;7\r\n;;;\r\n;;;\r\n;;;8\r\n";

            //when
            var result = parser.Parse(input, null, null, null, null);

            //then
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_SomeRulesNoneEmptyInput_NonEmpty()
        {
            //given
            var rules = new[]
            {
                new Rule
                {
                    Property = TransactionField.Title,
                    Column = 4,
                    IsOptional = false
                }
            };
            var parser = new CustomCsvParser(rules);
            var input = ";;;1\r\n;;;2\r\n;;;3\r\n;;;4\r\n;;;5\r\n;;;6\r\n;;;7\r\n;;;\r\n;;;\r\n;;;8\r\n";

            //when
            var result = parser.Parse(input, null, null, null, null);

            //then
            Assert.NotEmpty(result);
            Assert.Equal(8, result.Length);
        }
    }
}
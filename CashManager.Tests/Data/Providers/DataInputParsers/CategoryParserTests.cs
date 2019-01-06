using CashManager.Logic.DefaultData.Builders;
using CashManager.Logic.DefaultData.InputParsers;

using Xunit;

namespace CashManager.Tests.Data.Providers.DataInputParsers
{
    public class CategoryParserTests
    {
        [Fact]
        public void Parse_ValidSpaceInput_Array()
        {
            //given
            string input = "A\n"
                           + " B\n"
                           + " C\n"
                           + "  D\n"
                           + " E\n"
                           + "F\n";
            var expected = new CategoryBuilder()
                           .AddTopCategory("A")
                           .AddChildrenCategoryAndGoUp("B")
                           .AddChildrenCategory("C")
                           .AddChildrenCategory("D")
                           .GoUp()
                           .GoUp()
                           .AddChildrenCategory("E")
                           .AddTopCategory("F")
                           .Build();
            var parser = new CategoryParser();

            //when
            var result = parser.Parse(input);

            //then
            Assert.Equal(expected.Length, result.Length);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i].Name, result[i].Name);
                Assert.Equal(expected[i].Parent?.Name, result[i].Parent?.Name);
            }
        }

        [Fact]
        public void Parse_ValidDotInput_Array()
        {
            //given
            string input = "A\n"
                           + ".B\n"
                           + ".C\n"
                           + "..D\n"
                           + ".E\n"
                           + "F\n";
            var expected = new CategoryBuilder()
                           .AddTopCategory("A")
                           .AddChildrenCategoryAndGoUp("B")
                           .AddChildrenCategory("C")
                           .AddChildrenCategory("D")
                           .GoUp()
                           .GoUp()
                           .AddChildrenCategory("E")
                           .AddTopCategory("F")
                           .Build();
            var parser = new CategoryParser();

            //when
            var result = parser.Parse(input);

            //then
            Assert.Equal(expected.Length, result.Length);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i].Name, result[i].Name);
                Assert.Equal(expected[i].Parent?.Name, result[i].Parent?.Name);
            }
        }
    }
}
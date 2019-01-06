using System.Linq;

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
            string input =   "A\n"
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
            string input =   "A\n"
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

        [Fact]
        public void Parse_SimpleDuplicateNames_ValidDistinctIdsInArray()
        {
            //given
            string input =   "A\n"
                           + "B\n"
                           + ".A";
            var parser = new CategoryParser();

            //when
            var result = parser.Parse(input);

            //then
            Assert.Equal(3, result.Length);
            Assert.Equal(3, result.Distinct().ToArray().Length);
        }

        [Fact]
        public void Parse_DuplicateNames_ValidDistinctIdsInArray()
        {
            //given
            string input =   "A\n"
                           + "B\n"
                           + ".A\n"
                           + ".B\n"
                           + "C\n"
                           + ".B";
            var parser = new CategoryParser();

            //when
            var result = parser.Parse(input);

            //then
            Assert.Equal(6, result.Length);
            Assert.Equal(6, result.Distinct().ToArray().Length);
        }

        [Fact]
        public void Parse_DuplicateNamesOnSameLevel_SingleInstance()
        {
            //given
            string input =   "A\n"
                           + "A"; //should be discarded
            var parser = new CategoryParser();

            //when
            var result = parser.Parse(input);

            //then
            Assert.Single(result);
            Assert.Single(result.Distinct().ToArray());
        }

        [Fact]
        public void Parse_DuplicateNamesOnSameLevel_SingleInstanceOfNameAtGivenLevel()
        {
            //given
            string input =   "A\n"
                           + ".B\n"
                           + "Z\n"
                           + "A\n"  //should be discarded as "second A"
                           + ".C\n" //should be attached to 1st A
                           + "..D"; //should be attached to C in 1st A
            var parser = new CategoryParser();

            //when
            var result = parser.Parse(input);

            //then
            Assert.Equal(5, result.Length);
            Assert.Equal(5, result.Distinct().ToArray().Length);
        }
    }
}
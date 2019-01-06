using CashManager.Data.DTO;
using CashManager.Logic.DefaultData.Builders;

using Xunit;

namespace CashManager.Tests.DataBuilderTests
{
    public class CategoryBuilderTests
    {
        [Fact]
        public void AddCategory_Null_NoError()
        {
            //given
            var builder = new CategoryBuilder();

            //when
            var result = builder.AddCategory(null);

            //then
            Assert.Null(result.LastCategory);
            Assert.Empty(result.Categories);
        }

        [Fact]
        public void AddCategory_ValidCategory_ProperlySet()
        {
            //given
            var builder = new CategoryBuilder();
            var category = new Category();

            //when
            var result = builder.AddCategory(category);

            //then
            Assert.Equal(category, result.LastCategory);
            Assert.Contains(category, result.Categories);
        }
    }
}
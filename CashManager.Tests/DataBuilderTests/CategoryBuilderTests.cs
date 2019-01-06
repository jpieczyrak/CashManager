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
            var result = builder.AddTopCategory(null);

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
            var result = builder.AddTopCategory(category);

            //then
            Assert.Equal(category, result.LastCategory);
            Assert.Contains(category, result.Categories);
        }

        [Fact]
        public void AddChildrenCategory_ValidCategoryNoParent_Nothing()
        {
            //given
            var builder = new CategoryBuilder();
            var category = new Category();

            //when
            var result = builder.AddChildrenCategory(category);

            //then
            Assert.Null(result.LastCategory);
            Assert.Empty(result.Categories);
        }

        [Fact]
        public void AddChildrenCategory_ValidCategoryValidParent_ProperlySet()
        {
            //given
            var builder = new CategoryBuilder();
            var child = new Category();
            var parent = new Category();
            builder = builder.AddTopCategory(parent);

            //when
            var result = builder.AddChildrenCategory(child);

            //then
            Assert.Equal(child, result.LastCategory);
            Assert.DoesNotContain(child, result.Categories);
            Assert.Equal(parent, result.LastCategory.Parent);
        }
    }
}
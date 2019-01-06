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

        [Fact]
        public void AddChildrenCategoryAndGoUp_ValidCategoryValidParent_ProperlySet()
        {
            //given
            var builder = new CategoryBuilder();
            var parent = new Category();
            var child1 = new Category();
            var child2 = new Category();
            builder = builder.AddTopCategory(parent);

            //when
            var result = builder.AddTopCategory(parent)
                                .AddChildrenCategoryAndGoUp(child1)
                                .AddChildrenCategoryAndGoUp(child2);

            //then
            Assert.Equal(parent, result.LastCategory);
            Assert.DoesNotContain(child1, result.Categories);
            Assert.DoesNotContain(child2, result.Categories);
        }

        [Fact]
        public void Build_ValidChain_ArrayOfTopParents()
        {
            //given
            var builder = new CategoryBuilder();
            var parent = new Category();
            var child1 = new Category();
            var child2 = new Category();
            
            //when
            var result = builder.AddTopCategory(parent)
                                .AddChildrenCategory(child1)
                                .AddChildrenCategory(child2)
                                .Build();

            //then
            Assert.Null(parent.Parent);
            Assert.Equal(parent, child1.Parent);
            Assert.Equal(child1, child2.Parent);
            Assert.Contains(parent, result);
            Assert.DoesNotContain(child1, result);
            Assert.DoesNotContain(child2, result);
        }

        [Fact]
        public void Build_ValidChainMultipleRoots_ArrayOfTopParents()
        {
            //given
            var builder = new CategoryBuilder();
            var root1 = new Category();
            var root2 = new Category();
            var root3 = new Category();
            var root1Child1 = new Category();
            var root1Child2 = new Category();
            var root2Child1 = new Category();
            var root2Child1Child1 = new Category();
            
            //when
            var result = builder.AddTopCategory(root1)
                                .AddChildrenCategoryAndGoUp(root1Child1)
                                .AddChildrenCategoryAndGoUp(root1Child2)
                                .AddTopCategory(root2)
                                .AddChildrenCategory(root2Child1)
                                .AddChildrenCategory(root2Child1Child1)
                                .AddTopCategory(root3)
                                .Build();

            //then
            Assert.Null(root1.Parent);
            Assert.Null(root2.Parent);
            Assert.Null(root3.Parent);

            Assert.Equal(root1, root1Child1.Parent);
            Assert.Equal(root1, root1Child2.Parent);

            Assert.Equal(root2, root2Child1.Parent);
            Assert.Equal(root2Child1, root2Child1Child1.Parent);

            Assert.Contains(root1, result);
            Assert.Contains(root2, result);
            Assert.Contains(root3, result);

            Assert.DoesNotContain(root1Child1, result);
            Assert.DoesNotContain(root1Child2, result);
        }
    }
}
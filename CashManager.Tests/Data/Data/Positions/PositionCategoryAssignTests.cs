using System;

using CashManager.Model;

using Xunit;

namespace CashManager.Tests.Data.Data.Positions
{
    public class PositionCategoryAssignTests
    {
        [Fact]
        public void CategoryWithSameIdShouldBeAssignedProperly()
        {
            //given
            var id = Guid.NewGuid();
            var category1 = new Model.Category(id) { Name = "cat 1" };
            var category2 = new Model.Category(id) { Name = "cat 2" };
            var position = new Position { Category = category1 };

            //when
            position.Category = category2;

            //then
            Assert.Same(category2, position.Category);
            Assert.Equal(category2, position.Category);
            Assert.Equal(category2.Name, position.Category.Name);

            Assert.NotSame(category1, position.Category);
            //test will pass when Category setter will assign value instead of calling mvvmlight setter
            //mvvmlight setter (based on id) will not assign field, coz it is "the same value"
        }
    }
}
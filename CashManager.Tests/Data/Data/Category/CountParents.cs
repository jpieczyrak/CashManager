using Xunit;

namespace CashManager.Tests.Data.Data.Category
{
    public class CountParents
    {
        [Fact]
        public void ShouldProperlyCountParents()
        {
            //given
            var child = new CashManager_MVVM.Model.Category { Name = "a1" };
            var parent = new CashManager_MVVM.Model.Category { Name = "a2" };
            var root = new CashManager_MVVM.Model.Category { Name = "a3" };
            child.Parent = parent;
            parent.Parent = root;

            //when
            int childResult = child.CountParents();
            int parentResult = parent.CountParents();
            int rootResult = root.CountParents();

            //then
            Assert.Equal(2, childResult);
            Assert.Equal(1, parentResult);
            Assert.Equal(0, rootResult);
        }
    }
}
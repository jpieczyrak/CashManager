using System;
using System.Collections.Generic;

using Xunit;

namespace CashManager.Tests.Data.Data.Category
{
    public class FilteringTests
    {
        [Fact]
        public void GetCategoriesChainTests()
        {
            //given
            var child = new CashManager_MVVM.Model.Category { Name = "a1" };
            var parent = new CashManager_MVVM.Model.Category { Name = "a2" };
            var root = new CashManager_MVVM.Model.Category { Name = "a3" };

            child.Parent = parent;
            parent.Parent = root;

            var expectedFilter = new Stack<Guid>();
            expectedFilter.Push(child.Id);
            expectedFilter.Push(parent.Id);
            expectedFilter.Push(root.Id);

            //when
            var result = child.GetCategoriesChain(new Stack<Guid>());

            //then
            Assert.Equal(expectedFilter, result);
        }

        [Fact]
        public void GetParentsIdTests()
        {
            //given
            var child = new CashManager_MVVM.Model.Category { Name = "a1" };
            var parent = new CashManager_MVVM.Model.Category { Name = "a2" };
            var root = new CashManager_MVVM.Model.Category { Name = "a3" };
            child.Parent = parent;
            parent.Parent = root;

            var expected = new[] { root.Id, parent.Id, child.Id };

            //when
            var result = child.GetParentsId();

            //then
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MatchCategoryFilterByFilterList()
        {
            //given
            var child = new CashManager_MVVM.Model.Category { Name = "a1" };
            var parent = new CashManager_MVVM.Model.Category { Name = "a2" };
            var root = new CashManager_MVVM.Model.Category { Name = "a3" };

            child.Parent = parent;
            parent.Parent = root;

            var filter = new List<Guid> { root.Id, parent.Id };

            //when
            bool childMatchesFilter = child.MatchCategoryFilter(filter);
            bool parentMatchesFilter = parent.MatchCategoryFilter(filter);
            bool rootMatchesFilter = root.MatchCategoryFilter(filter);

            //then
            Assert.True(childMatchesFilter);
            Assert.True(parentMatchesFilter);
            Assert.False(rootMatchesFilter);
        }

        [Fact]
        public void MatchCategoryFilter_Null_False()
        {
            //given
            var category = new CashManager_MVVM.Model.Category { Name = "a1" };

            //when
            bool result = category.MatchCategoryFilter((CashManager_MVVM.Model.Category) null);

            //then
            Assert.False(result);
        }

        [Fact]
        public void MatchCategoryFilter_Child_True()
        {
            //given
            var child = new CashManager_MVVM.Model.Category { Name = "a1" };
            var parent = new CashManager_MVVM.Model.Category { Name = "a2" };
            child.Parent = parent;

            //when
            bool result = parent.MatchCategoryFilter(child);

            //then
            Assert.True(result);
        }

        [Fact]
        public void MatchCategoryFilter_ChildToParent_False()
        {
            //given
            var child = new CashManager_MVVM.Model.Category { Name = "a1" };
            var parent = new CashManager_MVVM.Model.Category { Name = "a2" };
            child.Parent = parent;

            //when
            bool result = child.MatchCategoryFilter(parent);

            //then
            Assert.False(result);
        }

        [Fact]
        public void MatchCategoryFilter_NonRelated_False()
        {
            //given
            var child = new CashManager_MVVM.Model.Category { Name = "a1" };
            var parent = new CashManager_MVVM.Model.Category { Name = "a2" };
            var random = new CashManager_MVVM.Model.Category { Name = "a3" };
            child.Parent = parent;

            //when
            bool result = parent.MatchCategoryFilter(random);

            //then
            Assert.False(result);
        }
    }
}
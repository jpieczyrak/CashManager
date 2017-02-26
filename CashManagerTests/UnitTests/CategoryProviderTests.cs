using Logic.LogicObjectsProviders;
using Logic.TransactionManagement.TransactionElements;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NUnit.Framework;

using Assert = NUnit.Framework.Assert;

namespace CashManagerTests.UnitTests
{
    [TestFixture]
    public class CategoryProviderTests
    {
        [Test]
        public void ShouldCreateUniqueCategories()
        {
            var category1 = CategoryProvider.FindOrCreate("A1");
            var category2 = CategoryProvider.FindOrCreate("A2");

            Assert.AreNotEqual(category1, category2);
        }

        [Test]
        public void ShouldReturnSameCategory()
        {
            var category1 = CategoryProvider.FindOrCreate("A1");
            var category2 = CategoryProvider.FindOrCreate("A1");

            Assert.AreEqual(category1.GetHashCode(), category2.GetHashCode());
        }
    }
}

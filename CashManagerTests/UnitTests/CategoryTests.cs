using Logic.FilesOperations;
using Logic.TransactionManagement.TransactionElements;

using NUnit.Framework;

namespace CashManagerTests.UnitTests
{
    [TestFixture]
    public class CategoryTests
    {
        [Test]
        public void SerializationTests()
        {
            var expected = new Category("A1");

            var serializedObject = Serializer.XMLSerializeObject(expected);
            var deserialized = Deserializer.Deserialize(serializedObject, typeof(Category));

            Assert.AreEqual(expected, deserialized);
        }
    }
}
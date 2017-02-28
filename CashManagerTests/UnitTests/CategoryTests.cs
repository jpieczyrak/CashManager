using System;

using AutoMapper;

using Logic.DTO;
using Logic.FilesOperations;

using NUnit.Framework;

using MapperConfiguration = Logic.Mapping.MapperConfiguration;

namespace CashManagerTests.UnitTests
{
    [TestFixture]
    public class CategoryTests
    {
        [SetUp]
        public void Setup()
        {
            MapperConfiguration.Configure();
        }

        [Test]
        public void SerializationTests()
        {
            var dto = new Category { Value = "A1", Id = Guid.NewGuid() };
            var expected = Mapper.Map<Logic.Model.Category>(dto);

            string serializedObject = Serializer.XMLSerializeObject(dto);
            var deserialized = Deserializer.Deserialize(serializedObject, typeof(Category));
            var mappedAfterDeserialization = Mapper.Map<Logic.Model.Category>(deserialized);

            Assert.AreEqual(expected, mappedAfterDeserialization);
        }
    }
}
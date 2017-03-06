using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Logic.Database;
using Logic.DTO;
using Logic.FilesOperations;
using Logic.LogicObjectsProviders;

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
        public void DBTests()
        {
            var parent = CategoryProvider.FindOrCreate("parent");
            var dto = new Category { Value = "A1", Id = Guid.NewGuid(), ParentId = parent.Id };
            var expected = Mapper.Map<Logic.Model.Category>(dto);

            DatabaseProvider.DB.Save(dto);
            var loaded = DatabaseProvider.DB.Read<Category>().FirstOrDefault(c => c.Id == dto.Id);
            var mapped = Mapper.Map<Logic.Model.Category>(loaded);

            Assert.AreEqual(expected, mapped);
            Assert.AreEqual(expected.Parent, mapped.Parent);
            Assert.AreEqual(expected.ParentId, mapped.ParentId);
        }

        [Test]
        public void FilteringTests()
        {
            var child = CategoryProvider.FindOrCreate("a1");
            var parent = CategoryProvider.FindOrCreate("a2");
            var root = CategoryProvider.FindOrCreate("a3");

            child.ParentId = parent.Id;
            parent.ParentId = root.Id;

            var filter = new List<Guid> { root.Id, parent.Id };

            //when
            Assert.IsTrue(child.MatchCategoryFilter(filter));
            Assert.IsTrue(parent.MatchCategoryFilter(filter));
            Assert.IsFalse(root.MatchCategoryFilter(filter));
        }

        [Test]
        public void SerializationTests()
        {
            var parent = CategoryProvider.FindOrCreate("parent");
            var dto = new Category { Value = "A1", Id = Guid.NewGuid(), ParentId = parent.Id };
            var expected = Mapper.Map<Logic.Model.Category>(dto);

            string serializedObject = Serializer.XMLSerializeObject(dto);
            var loaded = Deserializer.Deserialize(serializedObject, typeof(Category));
            var mapped = Mapper.Map<Logic.Model.Category>(loaded);

            Assert.AreEqual(expected, mapped);
            Assert.AreEqual(expected.Parent, mapped.Parent);
            Assert.AreEqual(expected.ParentId, mapped.ParentId);
        }
    }
}
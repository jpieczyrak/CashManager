
using AutoMapper;

using CashManager.Model;

using Xunit;

using MapperConfiguration = CashManager.Configuration.Mapping.MapperConfiguration;

namespace CashManager.Tests.Mappers
{
    public class ReferenceCreationTests
    {
        [Fact]
        public void AutoMapperMap_TransactionType_SameTransactionTypeReference()
        {
            //given
            MapperConfiguration.Configure();
            var type = new CashManager.Data.DTO.TransactionType { Name = "refrence test", IsDefault = true };

            var firstMap = Mapper.Map<TransactionType>(type);
            var secondMap = Mapper.Map<TransactionType>(type);

            //when
            firstMap.IsDefault = false;

            //then
            Assert.Equal(type.Id, firstMap.Id);
            Assert.Equal(type.Id, secondMap.Id);

            Assert.Same(firstMap, secondMap);

            Assert.False(firstMap.IsDefault);
            Assert.False(secondMap.IsDefault);
        }

        [Fact]
        public void AutoMapperMap_Transaction_SameTransactionTypeReference()
        {
            //given
            MapperConfiguration.Configure();
            var type = new CashManager.Data.DTO.TransactionType { Name = "refrence test", IsDefault = true };
            var transaction = new CashManager.Data.DTO.Transaction { Type = type };

            //when
            var firstMap = Mapper.Map<Transaction>(transaction);
            var secondMap = Mapper.Map<Transaction>(transaction);

            //then
            Assert.Equal(type.Id, firstMap.Type.Id);
            Assert.Equal(type.Id, secondMap.Type.Id);

            Assert.Same(firstMap.Type, secondMap.Type);
        }
    }
}
using AutoMapper;

using CashManager_MVVM.Model;

using Xunit;

using MapperConfiguration = CashManager_MVVM.Mapping.MapperConfiguration;

namespace CashManager.Tests.Mappers
{
    public class ModelAndDtoMappingTests
    {
        static ModelAndDtoMappingTests()
        {
            MapperConfiguration.Configure(); //call it only once
        }

        [Fact]
        public void ModelToDtoTest()
        {
            //given 
            var model1 = new Category { Value = "parent" };
            var model2 = new Category { Value = "child", Parent = model1 };

            //when
            var result1 = Mapper.Map<Data.DTO.Category>(model1);
            var result2 = Mapper.Map<Data.DTO.Category>(model2);

            //then
            Assert.Equal(model1.Id, result1.Id);
            Assert.Equal(model1.Value, result1.Value);

            Assert.Equal(model2.Id, result2.Id);
            Assert.Equal(model2.Value, result2.Value);
            Assert.Equal(model2.Parent.Id, result2.Parent.Id);
            Assert.Equal(model2.Parent.Value, result2.Parent.Value);
        }

        [Fact]
        public void DtoToModelTest()
        {
            //given 
            var dto1 = new Data.DTO.Category { Value = "parent" };
            var dto2 = new Data.DTO.Category { Value = "child", Parent = dto1 };

            //when
            var result1 = Mapper.Map<Category>(dto1);
            var result2 = Mapper.Map<Category>(dto2);

            //then
            Assert.Equal(dto1.Id, result1.Id);
            Assert.Equal(dto1.Value, result1.Value);

            Assert.Equal(dto2.Id, result2.Id);
            Assert.Equal(dto2.Value, result2.Value);
            Assert.Equal(dto2.Parent.Id, result2.Parent.Id);
            Assert.Equal(dto2.Parent.Value, result2.Parent.Value);
        }

        [Fact]
        public void ModelToDtoToModelTest()
        {
            //given 
            var model1 = new Category { Value = "parent" };
            var model2 = new Category { Value = "child", Parent = model1 };

            //when
            var result1 = Mapper.Map<Category>(Mapper.Map<Data.DTO.Category>(model1));
            var result2 = Mapper.Map<Category>(Mapper.Map<Data.DTO.Category>(model2));

            //then
            Assert.Equal(model1.Id, result1.Id);
            Assert.Equal(model1.Value, result1.Value);

            Assert.Equal(model2.Id, result2.Id);
            Assert.Equal(model2.Value, result2.Value);
            Assert.Equal(model2.Parent.Id, result2.Parent.Id);
            Assert.Equal(model2.Parent.Value, result2.Parent.Value);
        }
    }
}
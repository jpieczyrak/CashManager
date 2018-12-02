using System;
using System.Linq;

using AutoMapper;

using CashManager_MVVM;
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

        [Fact]
        public void PaymentValueModelToDtoToModelTest()
        {
            //given 
            var model1 = new PaymentValue { Value = 12 };
            var model2 = new PaymentValue { Value = 15 };

            //when
            var result1 = Mapper.Map<PaymentValue>(Mapper.Map<Data.DTO.PaymentValue>(model1));
            var result2 = Mapper.Map<PaymentValue>(Mapper.Map<Data.DTO.PaymentValue>(model2));

            //then
            Assert.Equal(model1.Id, result1.Id);
            Assert.Equal(model1.Value, result1.Value);

            Assert.Equal(model2.Id, result2.Id);
            Assert.Equal(model2.Value, result2.Value);
        }

        [Fact]
        public void TransactionModelToDtoToModelTest()
        {
            //given 
            var categoryParent = new Category { Value = "cat1", Parent = null };
            double balance = 123.45d;
            var model = new Transaction
            {
                UserStock = new Stock { Name = "1st", IsUserStock = true, Balance = balance },
                ExternalStock = new Stock { Name = "2nd", IsUserStock = false },
                BookDate = DateTime.Today,
                Note = "note",
                Title = "title",
                Type = new TransactionType { Outcome = true, Name = "buy", IsDefault = true },
                Positions = new TrulyObservableCollection<Position>(new[]
                {
                    new Position
                    {
                        Value = new PaymentValue { Value = 12 },
                        Tags = new TrulyObservableCollection<Tag>(new[]
                        {
                            new Tag { Name = "asd" }
                        }),
                        Category = categoryParent,
                        Title = "title1"
                    },
                    new Position
                    {
                        Value = new PaymentValue { Value = 22 },
                        Tags = new TrulyObservableCollection<Tag>(new[]
                        {
                            new Tag { Name = "dsa" }
                        }),
                        Category = new Category { Value = "cat2", Parent = categoryParent },
                        Title = "title2"
                    },
                })
            };

            //when
            var dto = Mapper.Map<Data.DTO.Transaction>(model);
            var result = Mapper.Map<Transaction>(dto);

            //then
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.BookDate, result.BookDate);
            Assert.Equal(model.ExternalStock.Name, result.ExternalStock.Name);
            Assert.Equal(model.ExternalStock.Id, result.ExternalStock.Id);
            Assert.Equal(model.ExternalStock.Balance, result.ExternalStock.Balance);
            Assert.Equal(model.UserStock.Name, result.UserStock.Name);
            Assert.Equal(model.UserStock.Id, result.UserStock.Id);
            Assert.Equal(model.UserStock.Balance, result.UserStock.Balance);
            Assert.Equal(model.InstanceCreationDate, result.InstanceCreationDate);
            Assert.NotEqual(model.LastEditDate, result.LastEditDate);
            Assert.Equal(model.TransactionSourceCreationDate, result.TransactionSourceCreationDate);
            Assert.Equal(model.Note, result.Note);
            Assert.Equal(model.Title, result.Title);
            Assert.Equal(model.Type, result.Type);
            Assert.Equal(model.Type.Id, result.Type.Id);
            Assert.Equal(model.Type.Name, result.Type.Name);
            Assert.Equal(model.Type.Income, result.Type.Income);
            Assert.Equal(model.Type.Outcome, result.Type.Outcome);
            Assert.Equal(model.Type.IsDefault, result.Type.IsDefault);

            Assert.Equal(model.Positions.Select(x => x.Value.Id), dto.Positions.Select(x => x.Value.Id));

            Assert.Equal(model.Positions.Count, result.Positions.Count);
            Assert.Equal(model.Positions.Select(x => x.Id), result.Positions.Select(x => x.Id));

            Assert.Equal(model.Positions.Select(x => x.Value.Value), result.Positions.Select(x => x.Value.Value));
            Assert.Equal(model.Positions.Select(x => x.Value.Id), result.Positions.Select(x => x.Value.Id));

            Assert.Equal(model.Positions.SelectMany(x => x.Tags).Count(), result.Positions.SelectMany(x => x.Tags).Count());
            Assert.Equal(model.Positions.SelectMany(x => x.Tags).Select(x => x.Name), result.Positions.SelectMany(x => x.Tags).Select(x => x.Name));
            Assert.Equal(model.Positions.SelectMany(x => x.Tags).Select(x => x.Id), result.Positions.SelectMany(x => x.Tags).Select(x => x.Id));

            Assert.Equal(model.Positions.Select(x => x.Category), result.Positions.Select(x => x.Category));
            Assert.Equal(model.Positions.Select(x => x.Category.Value), result.Positions.Select(x => x.Category.Value));
            Assert.Equal(model.Positions.Select(x => x.Category.Id), result.Positions.Select(x => x.Category.Id));
            Assert.Equal(model.Positions.Select(x => x.Category.Parent), result.Positions.Select(x => x.Category.Parent));
        }
    }
}
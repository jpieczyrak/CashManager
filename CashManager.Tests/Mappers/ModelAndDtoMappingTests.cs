using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AutoMapper;

using CashManager.Model;

using Xunit;

using MapperConfiguration = CashManager.Configuration.Mapping.MapperConfiguration;
using DtoCategory = CashManager.Data.DTO.Category;

namespace CashManager.Tests.Mappers
{
    public class ModelAndDtoMappingTests
    {
        public ModelAndDtoMappingTests()
        {
            MapperConfiguration.Configure();
        }

        [Fact]
        public void ModelToDtoTest()
        {
            //given
            var model1 = new Category { Name = "parent" };
            var model2 = new Category { Name = "child", Parent = model1 };

            //when
            var result1 = Mapper.Map<DtoCategory>(model1);
            var result2 = Mapper.Map<DtoCategory>(model2);

            //then
            Assert.Equal(model1.Id, result1.Id);
            Assert.Equal(model1.Name, result1.Name);

            Assert.Equal(model2.Id, result2.Id);
            Assert.Equal(model2.Name, result2.Name);
            Assert.Equal(model2.Parent.Id, result2.Parent.Id);
            Assert.Equal(model2.Parent.Name, result2.Parent.Name);
        }

        [Fact]
        public void DtoToModelTest()
        {
            //given
            var dto1 = new DtoCategory { Name = "parent" };
            var dto2 = new DtoCategory { Name = "child", Parent = dto1 };

            //when
            var result1 = Mapper.Map<Category>(dto1);
            var result2 = Mapper.Map<Category>(dto2);

            //then
            Assert.Equal(dto1.Id, result1.Id);
            Assert.Equal(dto1.Name, result1.Name);

            Assert.Equal(dto2.Id, result2.Id);
            Assert.Equal(dto2.Name, result2.Name);
            Assert.Equal(dto2.Parent.Id, result2.Parent.Id);
            Assert.Equal(dto2.Parent.Name, result2.Parent.Name);
        }

        [Fact]
        public void ModelToDtoToModelTest()
        {
            //given
            var model1 = new Category { Name = "parent" };
            var model2 = new Category { Name = "child", Parent = model1 };

            //when
            var result1 = Mapper.Map<Category>(Mapper.Map<DtoCategory>(model1));
            var result2 = Mapper.Map<Category>(Mapper.Map<DtoCategory>(model2));

            //then
            Assert.Equal(model1.Id, result1.Id);
            Assert.Equal(model1.Name, result1.Name);

            Assert.Equal(model2.Id, result2.Id);
            Assert.Equal(model2.Name, result2.Name);
            Assert.Equal(model2.Parent.Id, result2.Parent.Id);
            Assert.Equal(model2.Parent.Name, result2.Parent.Name);
        }

        [Fact]
        public void PaymentValueModelToDtoToModelTest()
        {
            //given
            var model = new PaymentValue { GrossValue = 12 };

            //when
            var result = Mapper.Map<PaymentValue>(Mapper.Map<CashManager.Data.DTO.PaymentValue>(model));

            //then
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.GrossValue, result.GrossValue);
        }

        [Fact]
        public void SimpleTransactionLastEditDateChangeMappingTest()
        {
            //given
            var model = new Transaction { Notes = new TrulyObservableCollection<Note> { new Note("now last edit date should be updated") } };

            //when
            var dto = Mapper.Map<CashManager.Data.DTO.Transaction>(model);
            var result = Mapper.Map<Transaction>(dto);

            //then
            Assert.Equal(model.Id, dto.Id);
            Assert.Equal(model.Notes.Select(x => x.Value), dto.Notes);
            Assert.Equal(model.LastEditDate, dto.LastEditDate);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.Notes.Select(x => x.Value), result.Notes.Select(x => x.Value));
            Assert.Equal(model.LastEditDate, result.LastEditDate);
        }

        [Fact]
        public void TransactionModelToDtoToModelTest()
        {
            //given
            var categoryParent = new Category { Name = "cat1", Parent = null };
            decimal balance = 123.45m;
            var model = new Transaction
            {
                UserStock = new Stock { Name = "1st", IsUserStock = true, Balance = new Balance { Value = balance } },
                ExternalStock = new Stock { Name = "2nd", IsUserStock = false },
                BookDate = DateTime.Today,
                Notes = new TrulyObservableCollection<Note> { new Note("note") },
                Title = "title",
                Type = new TransactionType { Outcome = true, Name = "buy", IsDefault = true },
                Positions = new TrulyObservableCollection<Position>(new[]
                {
                    new Position
                    {
                        Value = new PaymentValue { GrossValue = 12 },
                        Tags = new[]
                        {
                            new Tag { Name = "asd" }
                        },
                        Category = categoryParent,
                        Title = "title1"
                    },
                    new Position
                    {
                        Value = new PaymentValue { GrossValue = 22 },
                        Tags = new[]
                        {
                            new Tag { Name = "dsa" }
                        },
                        Category = new Category { Name = "cat2", Parent = categoryParent },
                        Title = "title2"
                    },
                })
            };

            //when
            var dto = Mapper.Map<CashManager.Data.DTO.Transaction>(model);
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
            Assert.Equal(model.UserStock.Balance.Value, result.UserStock.Balance.Value);
            Assert.Equal(model.InstanceCreationDate, result.InstanceCreationDate);
            Assert.Equal(model.LastEditDate, result.LastEditDate);
            Assert.Equal(model.TransactionSourceCreationDate, result.TransactionSourceCreationDate);
            Assert.Equal(model.Notes.Select(x => x.Value), result.Notes.Select(x => x.Value));
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

            Assert.Equal(model.Positions.Select(x => x.Value.GrossValue), result.Positions.Select(x => x.Value.GrossValue));
            Assert.Equal(model.Positions.Select(x => x.Value.Id), result.Positions.Select(x => x.Value.Id));

            Assert.Equal(model.Positions.SelectMany(x => x.Tags).Count(), result.Positions.SelectMany(x => x.Tags).Count());
            Assert.Equal(model.Positions.SelectMany(x => x.Tags).Select(x => x.Name), result.Positions.SelectMany(x => x.Tags).Select(x => x.Name));
            Assert.Equal(model.Positions.SelectMany(x => x.Tags).Select(x => x.Id), result.Positions.SelectMany(x => x.Tags).Select(x => x.Id));

            Assert.Equal(model.Positions.Select(x => x.Category), result.Positions.Select(x => x.Category));
            Assert.Equal(model.Positions.Select(x => x.Category.Name), result.Positions.Select(x => x.Category.Name));
            Assert.Equal(model.Positions.Select(x => x.Category.Id), result.Positions.Select(x => x.Category.Id));
            Assert.Equal(model.Positions.Select(x => x.Category.Parent), result.Positions.Select(x => x.Category.Parent));
        }
    }
}
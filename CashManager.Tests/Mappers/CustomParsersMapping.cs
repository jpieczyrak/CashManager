using System.Linq.Expressions;
using System.Runtime.InteropServices;

using AutoMapper;

using CashManager.Logic.Parsers.Custom;

using Xunit;

using MapperConfiguration = CashManager.Configuration.Mapping.MapperConfiguration;

namespace CashManager.Tests.Mappers
{
    public class CustomParsersMapping
    {
        [Fact]
        public void CustomCsvParserModelToDtoTests()
        {
            //given
            MapperConfiguration.Configure();
            var rules = new[]
            {
                new Rule { IsOptional = true, Column = 5, Property = TransactionField.Currency }
            };
            string columnSplitter = ";";
            var parser = new CustomCsvParser(rules, null, columnSplitter);

            //when
            var result = Mapper.Map<CashManager.Data.ViewModelState.Parsers.CustomCsvParser>(parser);

            //then
            Assert.NotNull(result);
            Assert.NotNull(result.ColumnSplitter);
            Assert.Equal(columnSplitter, result.ColumnSplitter);

            Assert.NotNull(result.Rules);
            Assert.Equal(rules[0].IsOptional, result.Rules[0].IsOptional);
            Assert.Equal(rules[0].Column, result.Rules[0].Column);
            Assert.Equal((int)rules[0].Property, result.Rules[0].Property);
        }

        [Fact]
        public void CustomCsvParserDtoToModelTests()
        {
            //given
            MapperConfiguration.Configure();
            var rules = new[]
            {
                new CashManager.Data.ViewModelState.Parsers.Rule { IsOptional = true, Column = 5, Property = (int)TransactionField.Currency }
            };
            string columnSplitter = ";";
            var parser = new CashManager.Data.ViewModelState.Parsers.CustomCsvParser
            {
                ColumnSplitter = columnSplitter,
                Rules = rules
            };

            //when
            var result = Mapper.Map<CustomCsvParser>(parser);

            //then
            Assert.NotNull(result);
            Assert.NotNull(result.ColumnSplitter);
            Assert.Equal(columnSplitter, result.ColumnSplitter);

            Assert.NotNull(result.Rules);
            Assert.Equal(rules[0].IsOptional, result.Rules[0].IsOptional);
            Assert.Equal(rules[0].Column, result.Rules[0].Column);
            Assert.Equal(rules[0].Property, (int)result.Rules[0].Property);
        }
    }
}
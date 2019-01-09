using System;

using CashManager_MVVM;
using CashManager_MVVM.Configuration.Mapping;
using CashManager_MVVM.Model;

using Xunit;

namespace CashManager.Tests.Data.Data.Positions
{
    public class PositionsCopyTests
    {
        [Fact]
        public void Copy_Null_Null()
        {
            //given

            //when
            var result = Position.Copy(null);

            //then
            Assert.Null(result);
        }

        [Fact]
        public void Copy_NotNull_SameTransactionDifferentId()
        {
            //given
            var user = new Stock { Name = "User1", IsUserStock = true };
            var external = new Stock { Name = "Ex" };
            var type = new TransactionType { Income = true, IsDefault = true };
            var tag = new Tag { Name = "tag" };
            var category = new CashManager_MVVM.Model.Category { Name = "cat" };
            var parent = new Transaction
            {
                Title = "Title",
                Note = "Note",
                BookDate = DateTime.Today,
                UserStock = user,
                ExternalStock = external,
                Type = type
            };
            var storedFileInfo = new StoredFileInfo("file", parent.Id);
            parent.StoredFiles.Add(storedFileInfo);
            var source = new Position
            {
                Value = new PaymentValue(10, 10, 0),
                Category = category,
                Title = "pos",
                Tags = new[] { tag },
                Parent = parent
            };
            parent.Positions = new TrulyObservableCollection<Position>(new[] { source });
            MapperConfiguration.Configure();

            //when
            var result = Position.Copy(source);

            //then
            Assert.NotNull(result);
            Assert.NotEqual(source.Id, result.Id);
            Assert.Equal(source.Value.NetValue, result.Value.NetValue);
            Assert.Equal(source.Value.GrossValue, result.Value.GrossValue);
            Assert.Equal(source.Value.TaxPercentValue, result.Value.TaxPercentValue);
            Assert.Equal(source.Parent, result.Parent);
            Assert.Equal(source.BookDate, result.BookDate);
            Assert.Equal(source.Category, result.Category);
            Assert.Equal(source.Tags, result.Tags);
            Assert.Equal(source.Title, result.Title);

            //parent
            Assert.Equal(parent.Id, result.Parent.Id);
            Assert.Equal(parent.Title, result.Parent.Title);
            Assert.Equal(parent.Note, result.Parent.Note);
            Assert.Equal(parent.BookDate, result.Parent.BookDate);
            Assert.Equal(parent.UserStock, result.Parent.UserStock);
            Assert.Equal(parent.ExternalStock, result.Parent.ExternalStock);
            Assert.Equal(parent.IsPropertyChangedEnabled, result.Parent.IsPropertyChangedEnabled);
            Assert.Equal(parent.Type, result.Parent.Type);
            Assert.Equal(parent.Positions.Count, result.Parent.Positions.Count);
            Assert.True(result.Parent.IsValid);
        }
    }
}
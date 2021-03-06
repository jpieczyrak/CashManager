﻿using System;
using System.Linq;

using CashManager.Configuration.Mapping;
using CashManager.Data.Extensions;
using CashManager.Model;

using Xunit;

namespace CashManager.Tests.Data.Data.Transactions
{
    public class TransactionCopyTests
    {
        [Fact]
        public void Copy_Null_Null()
        {
            //given

            //when
            var result = Transaction.Copy(null);

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
            var position = new Position
            {
                Value = new PaymentValue(10, 10, 0),
                Category = new Model.Category { Name = "cat" },
                Title = "pos",
                Tags = new [] { new Tag { Name = "tag" } }
            };
            var source = new Transaction
            {
                Title = "Title",
                Notes = new TrulyObservableCollection<Note> { new Note("Note") },
                BookDate = DateTime.Today,
                UserStock = user,
                ExternalStock = external,
                Type = type,
                Positions = new TrulyObservableCollection<Position>(new[] { position })
            };
            var storedFileInfo = new StoredFileInfo("file", source.Id);
            source.StoredFiles.Add(storedFileInfo);

            var expected = new Transaction($"{source.Id}{DateTime.Now}".GenerateGuid())
            {
                Title = "Title",
                Notes = new TrulyObservableCollection<Note> { new Note("Note") },
                BookDate = DateTime.Today,
                UserStock = user,
                ExternalStock = external,
                Type = type,
                Positions = new TrulyObservableCollection<Position>(new[] { position })
            };
            expected.StoredFiles.Add(storedFileInfo);
            MapperConfiguration.Configure();

            //when
            var result = Transaction.Copy(source);

            //then
            Assert.NotNull(result);
            Assert.NotEqual(source.Id, result.Id);
            Assert.Equal(expected.Title, result.Title);
            Assert.Equal(expected.Notes.Select(x => x.Value), result.Notes.Select(x => x.Value));
            Assert.Equal(expected.BookDate, result.BookDate);
            Assert.Equal(expected.UserStock, result.UserStock);
            Assert.Equal(expected.ExternalStock, result.ExternalStock);
            Assert.Equal(expected.IsPropertyChangedEnabled, result.IsPropertyChangedEnabled);
            Assert.Equal(expected.Type, result.Type);
            Assert.Equal(expected.Positions.Count, result.Positions.Count);
            Assert.True(result.IsValid);
        }
    }
}
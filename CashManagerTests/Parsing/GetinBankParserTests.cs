using System;
using System.Collections.Generic;
using System.Linq;


using Logic.LogicObjectsProviders;
using Logic.Mapping;
using Logic.Model;
using Logic.Parsing;
using Logic.TransactionManagement.TransactionElements;

using NUnit.Framework;

namespace CashManagerTests.Parsing
{
    [TestFixture]
    public class GetinBankParserTests
    {
        [Test]
        public void ParseTest()
        {
            MapperConfiguration.Configure();
            var parser = new GetinBankParser();

            var source = StockProvider.GetStock("Getin");
            var expected = new Transaction(eTransactionType.Buy, new DateTime(2016, 9, 6), "[Sierpień] Czynsz + media",
                "JĘDRZEJ PIECZYRAK: PRZELEW WYCHODZĄCY (PLN)", 
                DateTime.Today, DateTime.Today,
                new List<Subtransaction>
                {
                    new Subtransaction("[Sierpień] Czynsz + media", 684.62d)
                }, source, StockProvider.Default);

            string input = @"    06.09.2016 – PRZELEW WYCHODZĄCY
JĘDRZEJ PIECZYRAK – [Sierpień] Czynsz + media

-684,62 PLN";

            //when
            var output = parser.Parse(input, source);
            var parsed = output.FirstOrDefault();

            //then
            Assert.NotNull(parsed);
            Assert.AreEqual(expected.Date, parsed.Date);
            Assert.AreEqual(expected.Title, parsed.Title);
            Assert.AreEqual(expected.Note, parsed.Note);
            Assert.AreEqual(expected.Source, parsed.Source);
            Assert.AreEqual(expected.Target, parsed.Target);
            
            Assert.AreEqual(expected.Subtransactions.First().Name, parsed.Subtransactions.First().Name);
            Assert.AreEqual(expected.Subtransactions.First().Value, parsed.Subtransactions.First().Value);

            Assert.AreEqual(expected.Value, parsed.Value);
        }
    }
}
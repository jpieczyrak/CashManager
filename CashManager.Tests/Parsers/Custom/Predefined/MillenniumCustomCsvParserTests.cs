using System;
using System.Collections.Generic;

using CashManager.Data.DTO;
using CashManager.Data.Extensions;
using CashManager.Logic.Parsers.Custom.Predefined;

using Xunit;

namespace CashManager.Tests.Parsers.Custom.Predefined
{
    public class MillenniumCustomCsvParserTests : BaseParserTests
    {
        #region inputs

        private const string NONE_EMPTY_MILLENNIUM_INPUT = "Numer rachunku/karty,Data transakcji,Data rozliczenia,Rodzaj transakcji,Na konto/Z konta,Odbiorca/Zleceniodawca,Opis,Obciążenia,Uznania,Saldo,Waluta\r\n\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-25\",\"2019-01-25\",\"TRANSAKCJA KARTĄ PŁATNICZĄ\",\"\",\"\",\"JMP S.A. BIEDRONKA 3577GLIWICE 19/01/23\",\"-73.03\",\"\",\"2144.30\",\"\"\r\n\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-25\",\"2019-01-25\",\"TRANSAKCJA KARTĄ PŁATNICZĄ\",\"\",\"\",\"MDJ SP. Z O.O.         GLIWICE 19/01/23\",\"-8.00\",\"\",\"2217.33\",\"\"\r\n\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-22\",\"2019-01-22\",\"TRANSAKCJA KARTĄ PŁATNICZĄ\",\"\",\"\",\"MDJ SP. Z O.O.         GLIWICE 19/01/19\",\"-9.13\",\"\",\"2225.33\",\"\"\r\n\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-21\",\"2019-01-21\",\"TRANSAKCJA KARTĄ PŁATNICZĄ\",\"\",\"\",\"JMP S.A. BIEDRONKA 3577GLIWICE 19/01/18\",\"-79.27\",\"\",\"2234.46\",\"\"\r\n\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-18\",\"2019-01-18\",\"TRANSAKCJA KARTĄ PŁATNICZĄ\",\"\",\"\",\"JMP S.A. BIEDRONKA 3577GLIWICE 19/01/16\",\"-69.82\",\"\",\"2313.73\",\"\"\r\n\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-16\",\"2019-01-16\",\"TRANSAKCJA KARTĄ PŁATNICZĄ\",\"\",\"\",\"MDJ SP. Z O.O.         GLIWICE 19/01/14\",\"-7.55\",\"\",\"2383.55\",\"\"\r\n\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-15\",\"2019-01-15\",\"PRZELEW PRZYCHODZĄCY\",\"28 14 6011 8120 2501 5950 1500 02\",\"osoba adres\",\"my income\",\"\",\"647.85\",\"2391.10\",\"\"\r\n\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-14\",\"2019-01-14\",\"TRANSAKCJA KARTĄ PŁATNICZĄ\",\"\",\"\",\"JMP S.A. BIEDRONKA 3577GLIWICE 19/01/11\",\"-75.71\",\"\",\"1743.25\",\"\"\r\n\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-11\",\"2019-01-11\",\"TRANSAKCJA KARTĄ PŁATNICZĄ\",\"\",\"\",\"MDJ SP. Z O.O.         GLIWICE 19/01/09\",\"-8.25\",\"\",\"1818.96\",\"\"\r\n\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-02\",\"2019-01-02\",\"STAŁE ZLECENIE ZEWNĘTRZNE\",\"28 14 6011 8120 2501 5950 1500 02\",\"Jędrzej Pieczyrak\",\"Zwrot za internet\",\"-49.00\",\"\",\"419.57\",\"\"\r\n\"PL12 1160 2202 0000 0987 1234 1234\",\"2018-11-19\",\"2018-11-19\",\"PRZELEW DO INNEGO BANKU\",\"28 14 6011 8120 2501 5950 1500 02\",\"Jędrzej Pieczyrak\",\"Zwrot za lampy\",\"-225.00\",\"\",\"2651.31\",\"\"";

        #endregion

        [Fact]
        public void Parse_EmptyInput_Empty()
        {
            //given
            var parser = new CustomCsvParserFactory().Create(PredefinedCsvParsers.Millennium);
            var input = string.Empty;

            //when
            var result = parser.Parse(input, null, null, null, null);

            //then
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_NoneEmptyInput_NonEmpty()
        {
            //given
            var parser = new CustomCsvParserFactory().Create(PredefinedCsvParsers.Millennium);
            var input = NONE_EMPTY_MILLENNIUM_INPUT;
            var stock = new Stock();

            //when
            var result = parser.Parse(input, stock, null, null, null);

            //then
            Assert.NotEmpty(result);
            Assert.Equal(11, result.Length);
        }

        [Fact]
        public void Parse_SingleIncomeTransaction_Matching()
        {
            //given
            var defaultIncome = new TransactionType { Name = "in", Income = true };
            var defaultOutcome = new TransactionType { Name = "out", Outcome = true };
            Stock[] stocks =
            {
                new Stock { Name = "fake one", IsUserStock = true },
                new Stock { Name = "Millennium", IsUserStock = true },
                new Stock { Name = "external" }
            };
            var parser = new CustomCsvParserFactory(stocks).Create(PredefinedCsvParsers.Millennium);
            var input = "\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-14\",\"2019-01-15\",\"PRZELEW PRZYCHODZĄCY\",\"28 14 6011 8120 2501 5950 1500 02\",\"osoba adres\",\"my income\",\"\",\"647.85\",\"2391.10\",\"\"";

            var guid = input.Replace(";", string.Empty).GenerateGuid();
            var transaction = new Transaction(guid)
            {
                Title = "my income",
                Note = "PRZELEW PRZYCHODZĄCY",
                Positions = new List<Position> { new Position("my income", 647.85m) },
                BookDate = new DateTime(2019, 01, 15),
                TransactionSourceCreationDate = new DateTime(2019, 01, 14),
                UserStock = stocks[1],
                ExternalStock = stocks[2],
                Type = defaultIncome
            };

            //when
            var result = parser.Parse(input, stocks[1], stocks[2], defaultOutcome, defaultIncome);

            //then
            Assert.Single(result);
            ValidateTransaction(result[0], transaction);
            Assert.Equal(2391.10m, parser.Balances[stocks[1]].Value);
        }

        [Fact]
        public void Parse_SingleOutcomeTransaction_Matching()
        {
            //given
            var defaultIncome = new TransactionType { Name = "in", Income = true };
            var defaultOutcome = new TransactionType { Name = "out", Outcome = true };
            Stock[] stocks =
            {
                new Stock { Name = "fake one", IsUserStock = true },
                new Stock { Name = "Millennium", IsUserStock = true },
                new Stock { Name = "external" }
            };
            var parser = new CustomCsvParserFactory(stocks).Create(PredefinedCsvParsers.Millennium);
            var input = "\"PL12 1160 2202 0000 0987 1234 1234\",\"2019-01-22\",\"2019-01-22\",\"TRANSAKCJA KARTĄ PŁATNICZĄ\",\"\",\"\",\"MDJ SP. Z O.O.         GLIWICE 19/01/19\",\"-9.13\",\"\",\"2225.33\",\"\"";
            var defaultUserStock = stocks[1];
            var guid = input.Replace(";", string.Empty).GenerateGuid();
            var transaction = new Transaction(guid)
            {
                Title = "MDJ SP. Z O.O.         GLIWICE 19/01/19",
                Note = "TRANSAKCJA KARTĄ PŁATNICZĄ",
                Positions = new List<Position> { new Position("MDJ SP. Z O.O.         GLIWICE 19/01/19", 9.13m) },
                BookDate = new DateTime(2019, 01, 22),
                TransactionSourceCreationDate = new DateTime(2019, 01, 22),
                UserStock = stocks[1],
                ExternalStock = stocks[2],
                Type = defaultOutcome
            };

            //when
            var result = parser.Parse(input, defaultUserStock, stocks[2], defaultOutcome, defaultIncome);

            //then
            Assert.Single(result);
            ValidateTransaction(result[0], transaction);
            Assert.Equal(2225.33m, parser.Balances[stocks[1]].Value); //the matching one has been updated
        }
    }
}
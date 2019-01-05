using System;
using System.Collections.Generic;

using CashManager.Data.DTO;
using CashManager.Data.Extensions;
using CashManager.Logic.Parsers.Custom.Predefined;

using Xunit;

namespace CashManager.Tests.Parsers.Custom.Predefined
{
    public class IngCustomCsvParserTests : BaseParserTests
    {
        #region inputs

        private const string NONE_EMPTY_ING_INPUT =
            "\"Lista transakcji\";;;;;\"ING Bank Śląski S.A. ul. Sokolska 34, 40-086 Katowice www.ingbank.pl\";;;;;;;;;;;;;;;\r\n\"Dokument nr 0032347_221218\";\r\n\"Wygenerowany dnia: 2018-12-22, 13:58\";;;;;;;;;;;;;;;;;;;;\r\n\r\n\"Dane Użytkownika:\";\r\n\"xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm\";\r\n\r\n\"Wybrane rachunki:\";\r\n\"KONTO Direct (PLN)\";;\"11 1111 1111 1111 1111 1111 1111\";\r\n\"Dream Saver (PLN)\";;\"11 1111 1111 1111 1111 1111 1111\";\r\n\"Otwarte Konto Oszczędnościowe (PLN)\";;\"11 1111 1111 1111 1111 1111 1111\";\r\n\"Smart Saver (PLN)\";;\"11 1111 1111 1111 1111 1111 1111\";\r\n\"K@rta wirtualna ING VISA (PLN)\";;\"11 1111 1111 1111 1111 1111 1111\";\r\n\r\n\"Zastosowane kryteria wyboru\";;;;;\"Podsumowanie\";;\r\n\r\n\"Zakres dat:\";\"2018-06-22 - 2018-12-22\";\"Typy transakcji:\";\"wszystkie\";;\"Liczba transakcji:\";713;\r\n\r\n\"Ukryto transakcje Smart Saver\";;;;;\"Suma uznań (341):\";22222,71;PLN;;;;;;;;;;;;;\r\n\r\n;;;;;\"Suma obciążeń (372):\";333333,26;PLN;;;;;;;;;;;;;\r\n\r\n\"Data transakcji\";\"Data księgowania\";\"Dane kontrahenta\";\"Tytuł\";\"Nr rachunku\";\"Nazwa banku\";\"Szczegóły\";\"Nr transakcji\";\"Kwota transakcji (waluta rachunku)\";\"Waluta\";\"Kwota blokady/zwolnienie blokady\";\"Waluta\";\"Kwota płatności w walucie\";\"Waluta\";\"Konto\";\"Saldo po transakcji\";\"Waluta\";;;;\r\n\"2018-12-21\";;\" ZABKA Z2135 K1 GLIWICE PL \";\" Płatność kartą 21.12.2018 Nr karty 4246xx9261\";\'\';\"\";\"\";;;;-3,99;PLN;;;\"KONTO Direct\";2138,02;PLN;;;;\r\n\"2018-12-21\";\"2018-12-21\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" przelew Smart Saver Płatność kartą 19.12.2018 Nr karty 4246xx9261 Kwota: 165,97 PLN\";\'69105013311000009144064921 \';\"ING Bank Śląski S.A.\";\" \";\'201835564008056902\';4,03;PLN;;;;;\"Smart Saver\";1677,46;PLN;;;;\r\n\"2018-12-20\";\"2018-12-22\";\" PaylaneLegimiabonament Gdansk PL \";\" Płatność kartą 20.12.2018 Nr karty 4779xx7113\";\'1915031/19730 \';\"\";\"TR.KART -39.99  \";\'201835697304522092\';-39,99;PLN;;;;;\"K@rta wirtualna ING VISA\";105,22;PLN;;;;\r\n\"2018-12-20\";\"2018-12-20\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" przelew Smart Saver Płatność kartą 18.12.2018 Nr karty 4246xx9261 Kwota: 39,98 PLN\";\'69105013311000009144064921 \';\"ING Bank Śląski S.A.\";\" \";\'201835464008229006\';0,02;PLN;;;;;\"Smart Saver\";1673,43;PLN;;;;\r\n\"2018-12-20\";\"2018-12-20\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" przelew Smart Saver Płatność kartą 18.12.2018 Nr karty 4246xx9261 Kwota: 24,28 PLN\";\'69105013311000009144064921 \';\"ING Bank Śląski S.A.\";\" \";\'201835464008179591\';5,72;PLN;;;;;\"Smart Saver\";1673,41;PLN;;;;\r\n\"2018-12-20\";\"2018-12-20\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" przelew Smart Saver Płatność kartą 18.12.2018 Nr karty 4246xx9261 Kwota: 21,98 PLN\";\'69105013311000009144064921 \';\"ING Bank Śląski S.A.\";\" \";\'201835464008068067\';8,02;PLN;;;;;\"Smart Saver\";1667,69;PLN;;;;\r\n\"2018-12-20\";\"2018-12-20\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" Na legimi\";\'69105013311000009144064921 \';\"ING Bank Śląski S.A.\";\"ST.ZLEC  \";\'201835497208742045\';39,99;PLN;;;;;\"K@rta wirtualna ING VISA\";145,21;PLN;;;;\r\n\"2018-12-20\";\"2018-12-20\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" Na legimi\";\'59105013311000009707881869 \';\"ING Bank Śląski S.A.\";\"ST.ZLEC  \";\'201835497208742045\';-39,99;PLN;;;;;\"KONTO Direct\";2159,80;PLN;;;;";
        
        #endregion

        [Fact]
        public void Parse_EmptyInput_Empty()
        {
            //given
            var parser = new CustomCsvParserFactory().Create(PredefinedCsvParsers.Ing);
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
            var parser = new CustomCsvParserFactory().Create(PredefinedCsvParsers.Ing);
            var input = NONE_EMPTY_ING_INPUT;

            //when
            var result = parser.Parse(input, null, null, null, null);

            //then
            Assert.NotEmpty(result);
            Assert.Equal(7, result.Length);
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
                new Stock { Name = "K@rta wirtualna ING VISA", IsUserStock = true }, 
                new Stock { Name = "external" }, 
            };
            var parser = new CustomCsvParserFactory(stocks).Create(PredefinedCsvParsers.Ing);
            var input = "\"2018-12-20\";\"2018-12-20\";\" imie nazw, ulica 1/2, 11-222 miasto \";\" Na legimi\";\'123123123123 \';\"ING Bank Śląski S.A.\";\"zlecenie stałe\";\'201835497208742225\';39,99;PLN;;;;;\"K@rta wirtualna ING VISA\";145,21;PLN;;;;";

            var guid = input.Replace(";", string.Empty).GenerateGuid();
            var transaction = new Transaction(guid)
            {
                Title = "Na legimi",
                Note = "imie nazw, ulica 1/2, 11-222 miasto",
                Positions = new List<Position> { new Position("zlecenie stałe", 39.99m) },
                BookDate = new DateTime(2018, 12, 20),
                TransactionSourceCreationDate = new DateTime(2018, 12, 20),
                UserStock = stocks[1],
                ExternalStock = stocks[2],
                Type = defaultIncome
            };

            //when
            var result = parser.Parse(input, stocks[0], stocks[2], defaultOutcome, defaultIncome);

            //then
            Assert.Single(result);
            ValidateTransaction(result[0], transaction);
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
                new Stock { Name = "K@rta wirtualna ING VISA", IsUserStock = true }, 
                new Stock { Name = "external" }, 
            };
            var parser = new CustomCsvParserFactory(stocks).Create(PredefinedCsvParsers.Ing);
            var input = "\"2018-12-20\";\"2018-12-22\";\" imie nazw, ulica 1/2, 11-222 miasto \";\" Na legimi\";\'123123123123 \';\"ING Bank Śląski S.A.\";\"zlecenie stałe\";\'201835497208742225\';-39,99;PLN;;;;;\"K@rta wirtualna ING VISA\";145,21;PLN;;;;";

            var guid = input.Replace(";", string.Empty).GenerateGuid();
            var transaction = new Transaction(guid)
            {
                Title = "Na legimi",
                Note = "imie nazw, ulica 1/2, 11-222 miasto",
                Positions = new List<Position> { new Position("zlecenie stałe", 39.99m) },
                BookDate = new DateTime(2018, 12, 22),
                TransactionSourceCreationDate = new DateTime(2018, 12, 20),
                UserStock = stocks[1],
                ExternalStock = stocks[2],
                Type = defaultOutcome
            };

            //when
            var result = parser.Parse(input, stocks[0], stocks[2], defaultOutcome, defaultIncome);

            //then
            Assert.Single(result);
            ValidateTransaction(result[0], transaction);
        }

        [Fact]
        public void Parse_SingleOutcomeTransactionWithoutMatchingStock_Matching()
        {
            //given
            var defaultIncome = new TransactionType { Name = "in", Income = true };
            var defaultOutcome = new TransactionType { Name = "out", Outcome = true };
            Stock[] stocks = 
            {
                new Stock { Name = "fake one", IsUserStock = true }, 
                new Stock { Name = "none matching", IsUserStock = true }, 
                new Stock { Name = "external" }, 
            };
            var parser = new CustomCsvParserFactory(stocks).Create(PredefinedCsvParsers.Ing);
            var input = "\"2018-12-20\";\"2018-12-22\";\" imie nazw, ulica 1/2, 11-222 miasto \";\" Na legimi\";\'123123123123 \';\"ING Bank Śląski S.A.\";\"zlecenie stałe\";\'201835497208742225\';-39,99;PLN;;;;;\"K@rta wirtualna ING VISA\";145,21;PLN;;;;";

            var defaultUserStock = stocks[0];
            var guid = input.Replace(";", string.Empty).GenerateGuid();
            var transaction = new Transaction(guid)
            {
                Title = "Na legimi",
                Note = "imie nazw, ulica 1/2, 11-222 miasto",
                Positions = new List<Position> { new Position("zlecenie stałe", 39.99m) },
                BookDate = new DateTime(2018, 12, 22),
                TransactionSourceCreationDate = new DateTime(2018, 12, 20),
                UserStock = defaultUserStock,
                ExternalStock = stocks[2],
                Type = defaultOutcome
            };

            //when
            var result = parser.Parse(input, defaultUserStock, stocks[2], defaultOutcome, defaultIncome);

            //then
            Assert.Single(result);
            ValidateTransaction(result[0], transaction);
        }
    }
}
using CashManager.Logic.Parsers.Custom;

using Xunit;

namespace CashManager.Tests.Parsers
{
    public class CustomCsvParserTests : BaseParserTests
    {
        private const string NONE_EMPTY_ING_INPUT =
            "\"Lista transakcji\";;;;;\"ING Bank Śląski S.A. ul. Sokolska 34, 40-086 Katowice www.ingbank.pl\";;;;;;;;;;;;;;;\r\n\"Dokument nr 0032347_221218\";\r\n\"Wygenerowany dnia: 2018-12-22, 13:58\";;;;;;;;;;;;;;;;;;;;\r\n\r\n\"Dane Użytkownika:\";\r\n\"xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm\";\r\n\r\n\"Wybrane rachunki:\";\r\n\"KONTO Direct (PLN)\";;\"11 1111 1111 1111 1111 1111 1111\";\r\n\"Dream Saver (PLN)\";;\"11 1111 1111 1111 1111 1111 1111\";\r\n\"Otwarte Konto Oszczędnościowe (PLN)\";;\"11 1111 1111 1111 1111 1111 1111\";\r\n\"Smart Saver (PLN)\";;\"11 1111 1111 1111 1111 1111 1111\";\r\n\"K@rta wirtualna ING VISA (PLN)\";;\"11 1111 1111 1111 1111 1111 1111\";\r\n\r\n\"Zastosowane kryteria wyboru\";;;;;\"Podsumowanie\";;\r\n\r\n\"Zakres dat:\";\"2018-06-22 - 2018-12-22\";\"Typy transakcji:\";\"wszystkie\";;\"Liczba transakcji:\";713;\r\n\r\n\"Ukryto transakcje Smart Saver\";;;;;\"Suma uznań (341):\";22222,71;PLN;;;;;;;;;;;;;\r\n\r\n;;;;;\"Suma obciążeń (372):\";333333,26;PLN;;;;;;;;;;;;;\r\n\r\n\"Data transakcji\";\"Data księgowania\";\"Dane kontrahenta\";\"Tytuł\";\"Nr rachunku\";\"Nazwa banku\";\"Szczegóły\";\"Nr transakcji\";\"Kwota transakcji (waluta rachunku)\";\"Waluta\";\"Kwota blokady/zwolnienie blokady\";\"Waluta\";\"Kwota płatności w walucie\";\"Waluta\";\"Konto\";\"Saldo po transakcji\";\"Waluta\";;;;\r\n\"2018-12-21\";;\" ZABKA Z2135 K1 GLIWICE PL \";\" Płatność kartą 21.12.2018 Nr karty 4246xx9261\";\'\';\"\";\"\";;;;-3,99;PLN;;;\"KONTO Direct\";2138,02;PLN;;;;\r\n\"2018-12-21\";\"2018-12-21\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" przelew Smart Saver Płatność kartą 19.12.2018 Nr karty 4246xx9261 Kwota: 165,97 PLN\";\'69105013311000009144064921 \';\"ING Bank Śląski S.A.\";\" \";\'201835564008056902\';4,03;PLN;;;;;\"Smart Saver\";1677,46;PLN;;;;\r\n\"2018-12-20\";\"2018-12-22\";\" PaylaneLegimiabonament Gdansk PL \";\" Płatność kartą 20.12.2018 Nr karty 4779xx7113\";\'1915031/19730 \';\"\";\"TR.KART -39.99  \";\'201835697304522092\';-39,99;PLN;;;;;\"K@rta wirtualna ING VISA\";105,22;PLN;;;;\r\n\"2018-12-20\";\"2018-12-20\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" przelew Smart Saver Płatność kartą 18.12.2018 Nr karty 4246xx9261 Kwota: 39,98 PLN\";\'69105013311000009144064921 \';\"ING Bank Śląski S.A.\";\" \";\'201835464008229006\';0,02;PLN;;;;;\"Smart Saver\";1673,43;PLN;;;;\r\n\"2018-12-20\";\"2018-12-20\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" przelew Smart Saver Płatność kartą 18.12.2018 Nr karty 4246xx9261 Kwota: 24,28 PLN\";\'69105013311000009144064921 \';\"ING Bank Śląski S.A.\";\" \";\'201835464008179591\';5,72;PLN;;;;;\"Smart Saver\";1673,41;PLN;;;;\r\n\"2018-12-20\";\"2018-12-20\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" przelew Smart Saver Płatność kartą 18.12.2018 Nr karty 4246xx9261 Kwota: 21,98 PLN\";\'69105013311000009144064921 \';\"ING Bank Śląski S.A.\";\" \";\'201835464008068067\';8,02;PLN;;;;;\"Smart Saver\";1667,69;PLN;;;;\r\n\"2018-12-20\";\"2018-12-20\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" Na legimi\";\'69105013311000009144064921 \';\"ING Bank Śląski S.A.\";\"ST.ZLEC  \";\'201835497208742045\';39,99;PLN;;;;;\"K@rta wirtualna ING VISA\";145,21;PLN;;;;\r\n\"2018-12-20\";\"2018-12-20\";\" xxxx-yyyyy zzzzz, aaaaaaaaaa 1/22 12-122 mmmmmmmm \";\" Na legimi\";\'59105013311000009707881869 \';\"ING Bank Śląski S.A.\";\"ST.ZLEC  \";\'201835497208742045\';-39,99;PLN;;;;;\"KONTO Direct\";2159,80;PLN;;;;";

        [Fact]
        public void Parse_NoneRulesEmptyInput_Empty()
        {
            //given
            var rules = new Rule[0];
            var parser = new CustomCsvParser(rules);
            var input = "";

            //when
            var result = parser.Parse(input, null, null, null, null);

            //then
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_SomeRulesEmptyInput_Empty()
        {
            //given
            var rules = new[] { new Rule() };
            var parser = new CustomCsvParser(rules);
            var input = "";

            //when
            var result = parser.Parse(input, null, null, null, null);

            //then
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_NoRulesNoneEmptyInput_Empty()
        {
            //given
            var rules = new Rule[0];
            var parser = new CustomCsvParser(rules);
            var input = NONE_EMPTY_ING_INPUT;

            //when
            var result = parser.Parse(input, null, null, null, null);

            //then
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_SomeRulesNoneEmptyInput_Empty()
        {
            //given
            var rules = new[]
            {
                new Rule
                {
                    Property = TransactionField.Title,
                    Column = 4,
                    IsOptional = false
                }
            };
            var parser = new CustomCsvParser(rules);
            var input = NONE_EMPTY_ING_INPUT;

            //when
            var result = parser.Parse(input, null, null, null, null);

            //then
            Assert.NotEmpty(result);
            Assert.Equal(8 + 2, result.Length);
        }
    }
}
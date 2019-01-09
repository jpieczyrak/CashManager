using System;

using CashManager.Data.DTO;
using CashManager.Logic.DefaultData.InputParsers;

namespace CashManager.Logic.DefaultData
{
    public class DefaultDataProvider : IDataProvider
    {
        protected TransactionType _workType;
        protected TransactionType _buyType;
        protected TransactionType _transferInType;
        protected TransactionType _transferOutType;
        protected TransactionType _giftsType;
        protected Tag[] _tags;

        public DefaultDataProvider()
        {
            GetCategories();
            GetTransactionTypes();
            GetTags();
        }

        public virtual Transaction[] GetTransactions(Stock[] stocks, Category[] categories, TransactionType[] types, Tag[] tags)
        {
            return new Transaction[] { };
        }

        public Category[] GetCategories()
        {
            return new CategoryParser().Parse(
                "Dom\r\n.Meble\r\n.Oświetlenie\r\n.Ogród\r\n.Remont\r\n.Kredyt\r\n.Sprzątanie\r\n.Zakup nieruchomości\r\n.Inne\r\nDzieci\r\n.Opieka\r\n.Ubrania\r\n.Szkoła\r\n.Zabawki\r\n.Zajęcia dodatkowe\r\n.Inne\r\nEdukacja\r\n.Podręczniki\r\n.Opłaty\r\n.Inne\r\nInne\r\nJedzenie\r\n.Fastfood\r\n.Restauracje\r\n.Sklepy\r\n.Herbata\r\n.Inne\r\nOsobiste\r\n.Fryzjer\r\n.Ośrodki sportowe\r\n.Środki trwałe\r\n.Drobnostki\r\n.Inne\r\nUbezpieczenia\r\n.Inwestycje\r\n.Lokaty\r\n.Inne\r\nOpłaty\r\n.Mandaty\r\n.Usługiprawne\r\n.Ratypożyczek\r\n.Inne\r\n.Podatki\r\n..ZUS\r\n..PIT\r\n..VAT\r\n..Inne\r\nPrezenty\r\n.Rodzina\r\n.Znajomi\r\n.Uroczystości\r\n.Dobroczynność\r\n.Inne\r\nRachunki\r\n.Czynsz\r\n.Gaz\r\n.Prąd\r\n.Telefon\r\n.Tv\r\n.Woda\r\n.Wywóz śmieci\r\n.Ochrona\r\n.Inne\r\nRozrywka\r\n.Bilety\r\n.Gry\r\n.Kino\r\n.Teatr\r\n.Książki\r\n.Loterie\r\n.Muzyka\r\n.Życienocne\r\n.Papierosy\r\n.Subskrypcje\r\n.Inne\r\nAuto\r\n.Parking\r\n.Opłatydrogowe\r\n.Leasing\r\n.Kredyt\r\n.Serwis\r\n.Myjnia\r\n.Części\r\n.Ubezpieczenie\r\n.Zakup\r\n.Inne\r\n.Paliwo\r\n..Benzyna\r\n..Ropa\r\n..Gaz\r\n..Prąd\r\n..Inne\r\nWakacje\r\n.Jedzenie\r\n.Noclegi\r\n.Przelot\r\n.Wycieczki zorganizowane\r\n.Wynajem pojazdów\r\n.Transport\r\n.Inne\r\nUsługi\r\nZdrowie\r\n.Leki\r\n.Optyk\r\n.Wizyty lekarskie\r\n.Dentyści\r\n.Ubezpieczenie zdrowotne\r\n.Inne\r\nZwierzęta\r\n.Karma\r\n.Zabawki\r\n.Weterynarz\r\n.Leki\r\n.Inne");
        }

        public virtual Stock[] GetStocks()
        {
            return new[]
            {
                new Stock { Name = "User1", IsUserStock = true, Balance = new Balance(DateTime.MinValue, 0) },
                new Stock { Name = "Wallet", IsUserStock = true, Balance = new Balance(DateTime.MinValue, 0) },
                new Stock { Name = "Ex1" },
            };
        }

        public TransactionType[] GetTransactionTypes()
        {
            _workType = new TransactionType { Income = true, Name = "Work", IsDefault = true };
            _buyType = new TransactionType { Outcome = true, Name = "Buy", IsDefault = true };
            _transferInType = new TransactionType { Name = "Transfer in", Income = true };
            _transferOutType = new TransactionType { Name = "Transfer out", Outcome = true };
            _giftsType = new TransactionType { Income = true, Name = "Gifts" };
            return new[]
            {
                _workType,
                _buyType,
                _transferInType,
                _transferOutType,
                _giftsType
            };
        }

        public Tag[] GetTags()
        {
            _tags = new[]
            {
                new Tag { Name = "tag 1" },
                new Tag { Name = "tag 2" },
                new Tag { Name = "tag 3" },
            };
            return _tags;
        }
    }
}
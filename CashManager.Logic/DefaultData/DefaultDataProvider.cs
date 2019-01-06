using System;

using CashManager.Data.DTO;
using CashManager.Logic.DefaultData.Builders;

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
            return new CategoryBuilder()
                   .AddTopCategory("Dom")
                   .AddChildrenCategoryAndGoUp("Meble")
                   .AddChildrenCategoryAndGoUp("Oświetlenie")
                   .AddChildrenCategoryAndGoUp("Ogród")
                   .AddChildrenCategoryAndGoUp("Remont")
                   .AddChildrenCategoryAndGoUp("Kredyt")
                   .AddChildrenCategoryAndGoUp("Sprzątanie")
                   .AddChildrenCategoryAndGoUp("Zakup nieruchomości")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Dzieci")
                   .AddChildrenCategoryAndGoUp("Opieka")
                   .AddChildrenCategoryAndGoUp("Ubrania")
                   .AddChildrenCategoryAndGoUp("Szkoła")
                   .AddChildrenCategoryAndGoUp("Zabawki")
                   .AddChildrenCategoryAndGoUp("Zajęcia dodatkowe")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Edukacja")
                   .AddChildrenCategoryAndGoUp("Podręczniki")
                   .AddChildrenCategoryAndGoUp("Opłaty")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Inne")
                   .AddTopCategory("Jedzenie")
                   .AddChildrenCategoryAndGoUp("Fast food")
                   .AddChildrenCategoryAndGoUp("Restauracje")
                   .AddChildrenCategoryAndGoUp("Sklepy")
                   .AddChildrenCategoryAndGoUp("Herbata")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Osobiste")
                   .AddChildrenCategoryAndGoUp("Fryzjer")
                   .AddChildrenCategoryAndGoUp("Ośrodki sportowe")
                   .AddChildrenCategoryAndGoUp("Środki trwałe")
                   .AddChildrenCategoryAndGoUp("Drobnostki")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Ubezpieczenia")
                   .AddChildrenCategoryAndGoUp("Inwestycje")
                   .AddChildrenCategoryAndGoUp("Lokaty")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Opłaty")
                   .AddChildrenCategoryAndGoUp("Mandaty")
                   .AddChildrenCategoryAndGoUp("Usługi prawne")
                   .AddChildrenCategoryAndGoUp("Raty pożyczek")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddChildrenCategory("Podatki")
                   .AddChildrenCategoryAndGoUp("ZUS")
                   .AddChildrenCategoryAndGoUp("PIT")
                   .AddChildrenCategoryAndGoUp("VAT")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Prezenty")
                   .AddChildrenCategoryAndGoUp("Rodzina")
                   .AddChildrenCategoryAndGoUp("Znajomi")
                   .AddChildrenCategoryAndGoUp("Uroczystości")
                   .AddChildrenCategoryAndGoUp("Dobroczynność")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Rachunki")
                   .AddChildrenCategoryAndGoUp("Czynsz")
                   .AddChildrenCategoryAndGoUp("Gaz")
                   .AddChildrenCategoryAndGoUp("Prąd")
                   .AddChildrenCategoryAndGoUp("Telefon")
                   .AddChildrenCategoryAndGoUp("Tv")
                   .AddChildrenCategoryAndGoUp("Woda")
                   .AddChildrenCategoryAndGoUp("Wywóz śmieci")
                   .AddChildrenCategoryAndGoUp("Ochrona")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Rozrywka")
                   .AddChildrenCategoryAndGoUp("Bilety")
                   .AddChildrenCategoryAndGoUp("Gry")
                   .AddChildrenCategoryAndGoUp("Kino")
                   .AddChildrenCategoryAndGoUp("Teatr")
                   .AddChildrenCategoryAndGoUp("Książki")
                   .AddChildrenCategoryAndGoUp("Loterie")
                   .AddChildrenCategoryAndGoUp("Muzyka")
                   .AddChildrenCategoryAndGoUp("Życie nocne")
                   .AddChildrenCategoryAndGoUp("Papierosy")
                   .AddChildrenCategoryAndGoUp("Subskrypcje")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Auto")
                   .AddChildrenCategoryAndGoUp("Parking")
                   .AddChildrenCategoryAndGoUp("Opłaty drogowe")
                   .AddChildrenCategoryAndGoUp("Leasing")
                   .AddChildrenCategoryAndGoUp("Kredyt")
                   .AddChildrenCategoryAndGoUp("Serwis")
                   .AddChildrenCategoryAndGoUp("Myjnia")
                   .AddChildrenCategoryAndGoUp("Części")
                   .AddChildrenCategoryAndGoUp("Ubezpieczenie")
                   .AddChildrenCategoryAndGoUp("Zakup")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddChildrenCategory("Paliwo")
                   .AddChildrenCategoryAndGoUp("Benzyna")
                   .AddChildrenCategoryAndGoUp("Ropa")
                   .AddChildrenCategoryAndGoUp("Gaz")
                   .AddChildrenCategoryAndGoUp("Prąd")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Wakacje")
                   .AddChildrenCategoryAndGoUp("Jedzenie")
                   .AddChildrenCategoryAndGoUp("Noclegi")
                   .AddChildrenCategoryAndGoUp("Przelot")
                   .AddChildrenCategoryAndGoUp("Wycieczki zorganizowane")
                   .AddChildrenCategoryAndGoUp("Wynajem pojazdów")
                   .AddChildrenCategoryAndGoUp("Transport")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Usługi")
                   .AddTopCategory("Zdrowie")
                   .AddChildrenCategoryAndGoUp("Leki")
                   .AddChildrenCategoryAndGoUp("Optyk")
                   .AddChildrenCategoryAndGoUp("Wizyty lekarskie")
                   .AddChildrenCategoryAndGoUp("Dentyści")
                   .AddChildrenCategoryAndGoUp("Ubezpieczenie zdrowotne")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .AddTopCategory("Zwierzęta")
                   .AddChildrenCategoryAndGoUp("Karma")
                   .AddChildrenCategoryAndGoUp("Zabawki")
                   .AddChildrenCategoryAndGoUp("Weterynarz")
                   .AddChildrenCategoryAndGoUp("Leki")
                   .AddChildrenCategoryAndGoUp("Inne")
                   .Build();
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
            _tags = new Tag[]
            {
                new Tag { Name = "tag 1" },
                new Tag { Name = "tag 2" },
                new Tag { Name = "tag 3" },
            };
            return _tags;
        }
    }
}
using System;

using CashManager.Data.DTO;

namespace CashManager.Logic.DefaultData
{
    public class DefaultDataProvider : IDataProvider
    {
        protected Category _root;
        protected Category _home;
        protected Category _fun;
        protected Category _fun_pc;
        protected Category _fun_books;
        protected Category _fun_games;
        protected Category _fun_games_strategy;
        protected Category _fun_games_fps;
        protected Category _home_cleaning;
        protected Category _home_food;
        protected Category _home_food_base;
        protected Category _home_food_chocolates;
        protected Category _home_food_tea;
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
            _root = new Category { Name = "Root" };
            _home = new Category { Name = "Home", Parent = _root };
            _fun = new Category { Name = "Fun", Parent = _root };
            _fun_pc = new Category { Name = "PC", Parent = _fun };
            _fun_books = new Category { Name = "Books", Parent = _fun };
            _fun_games = new Category { Name = "Games", Parent = _fun };
            _fun_games_strategy = new Category { Name = "Strategy", Parent = _fun_games };
            _fun_games_fps = new Category { Name = "FPS", Parent = _fun_games };
            _home_cleaning = new Category { Name = "Cleaning", Parent = _home };
            _home_food = new Category { Name = "Food", Parent = _home };
            _home_food_base = new Category { Name = "Base food", Parent = _home_food };
            _home_food_chocolates = new Category { Name = "Chocolates", Parent = _home_food };
            _home_food_tea = new Category { Name = "Tea", Parent = _home_food };
            var dtoCategories = new[]
            {
                _root,
                _home,
                _fun,
                _fun_pc,
                _fun_books,
                _fun_games,
                _fun_games_strategy,
                _fun_games_fps,
                _home_cleaning,
                _home_food,
                _home_food_base,
                _home_food_chocolates,
                _home_food_tea
            };

            return dtoCategories;
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
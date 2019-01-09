using AutoMapper;

using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Logic.Balances;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using Category = CashManager_MVVM.Model.Category;
using Stock = CashManager_MVVM.Model.Stock;
using Tag = CashManager_MVVM.Model.Tag;

namespace CashManager_MVVM.Configuration.Mapping
{
    public static class MapperConfiguration
    {
        private static readonly object _lock = new object();
        private static bool _isInitialized;

        public static void Configure()
        {
            lock (_lock)
            {
                if (!_isInitialized)
                {
                    Mapper.Initialize(config =>
                    {
                        config.CreateMap<Category, CashManager.Data.DTO.Category>();
                        config.CreateMap<CashManager.Data.DTO.Category, Category>();

                        config.CreateMap<Balance, CashManager.Data.DTO.Balance>();
                        config.CreateMap<CashManager.Data.DTO.Balance, Balance>();

                        config.CreateMap<Stock, CashManager.Data.DTO.Stock>();
                        config.CreateMap<CashManager.Data.DTO.Stock, Stock>()
                              .AfterMap((dto, model) => model.IsPropertyChangedEnabled = true);

                        config.CreateMap<Tag, CashManager.Data.DTO.Tag>();
                        config.CreateMap<CashManager.Data.DTO.Tag, Tag>();

                        config.CreateMap<PaymentValue, CashManager.Data.DTO.PaymentValue>();
                        config.CreateMap<CashManager.Data.DTO.PaymentValue, PaymentValue>();

                        config.CreateMap<TransactionType, CashManager.Data.DTO.TransactionType>();
                        config.CreateMap<CashManager.Data.DTO.TransactionType, TransactionType>();

                        config.CreateMap<StoredFileInfo, CashManager.Data.DTO.StoredFileInfo>();
                        config.CreateMap<CashManager.Data.DTO.StoredFileInfo, StoredFileInfo>();

                        config.CreateMap<Position, CashManager.Data.DTO.Position>()
                              .ConstructUsing(x => new CashManager.Data.DTO.Position());
                        config.CreateMap<CashManager.Data.DTO.Position, Position>()
                              .BeforeMap((dto, model) => model.IsPropertyChangedEnabled = false)
                              .AfterMap((dto, model) => model.IsPropertyChangedEnabled = true);

                        config.CreateMap<Transaction, CashManager.Data.DTO.Transaction>();
                        config.CreateMap<CashManager.Data.DTO.Transaction, Transaction>()
                              .BeforeMap((dto, model) => model.IsPropertyChangedEnabled = false)
                              .AfterMap((dto, model) =>
                              {
                                  foreach (var position in model.Positions)
                                  {
                                      position.IsPropertyChangedEnabled = false;
                                      position.Parent = model;
                                      position.IsPropertyChangedEnabled = true;
                                  }
                                  model.IsPropertyChangedEnabled = true;
                              });

                        config.CreateMap<SearchState, CashManager.Data.ViewModelState.SearchState>();
                        config.CreateMap<CashManager.Data.ViewModelState.SearchState, SearchState>();

                        config.CreateMap<DateFrame, CashManager.Data.ViewModelState.Selectors.DateFrame>();
                        config.CreateMap<CashManager.Data.ViewModelState.Selectors.DateFrame, DateFrame>();

                        config.CreateMap<RangeSelector, CashManager.Data.ViewModelState.Selectors.RangeSelector>();
                        config.CreateMap<CashManager.Data.ViewModelState.Selectors.RangeSelector, RangeSelector>();

                        config.CreateMap<TextSelector, CashManager.Data.ViewModelState.Selectors.TextSelector>();
                        config.CreateMap<CashManager.Data.ViewModelState.Selectors.TextSelector, TextSelector>();

                        config.CreateMap<MultiPicker, CashManager.Data.ViewModelState.Selectors.MultiPicker>();
                        config.CreateMap<CashManager.Data.ViewModelState.Selectors.MultiPicker, MultiPicker>();

                        config.CreateMap<CustomBalance, CashManager.Data.ViewModelState.Balances.CustomBalance>();
                        config.CreateMap<CashManager.Data.ViewModelState.Balances.CustomBalance, CustomBalance>();
                    });

                    _isInitialized = true;
                }
            }
        }
    }
}
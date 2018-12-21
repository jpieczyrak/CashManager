using AutoMapper;

using CashManager_MVVM.Model;

using Category = CashManager_MVVM.Model.Category;
using Stock = CashManager_MVVM.Model.Stock;
using Tag = CashManager_MVVM.Model.Tag;

namespace CashManager_MVVM.Configuration.Mapping
{
    public class MapperConfiguration
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
                        config.CreateMap<CashManager.Data.DTO.Stock, Stock>();

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
                                  foreach (var position in model.Positions) position.Parent = model;
                                  model.IsPropertyChangedEnabled = true;
                              });
                    });
                    _isInitialized = true;
                }
            }
        }
    }
}
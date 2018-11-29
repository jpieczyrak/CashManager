using AutoMapper;

using CashManager_MVVM.Model;

using Category = CashManager_MVVM.Model.Category;
using Stock = CashManager_MVVM.Model.Stock;
using Tag = CashManager_MVVM.Model.Tag;

namespace CashManager_MVVM.Mapping
{
    public class MapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Category, CashManager.Data.DTO.Category>();
                config.CreateMap<CashManager.Data.DTO.Category, Category>();

                config.CreateMap<Stock, CashManager.Data.DTO.Stock>();
                config.CreateMap<CashManager.Data.DTO.Stock, Stock>();

                config.CreateMap<Tag, CashManager.Data.DTO.Tag>();
                config.CreateMap<CashManager.Data.DTO.Tag, Tag>();

                config.CreateMap<PaymentValue, CashManager.Data.DTO.PaymentValue>();
                config.CreateMap<CashManager.Data.DTO.PaymentValue, PaymentValue>();

                config.CreateMap<Position, CashManager.Data.DTO.Position>().ConstructUsing(x => new CashManager.Data.DTO.Position());
                config.CreateMap<CashManager.Data.DTO.Position, Position>();

                config.CreateMap<Transaction, CashManager.Data.DTO.Transaction>();
                config.CreateMap<CashManager.Data.DTO.Transaction, Transaction>();
            });
        }
    }
}
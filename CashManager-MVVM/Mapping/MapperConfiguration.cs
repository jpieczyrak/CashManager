using AutoMapper;

using CashManager_MVVM.Model;

using Logic.LogicObjectsProviders;

using Category = CashManager_MVVM.Model.Category;
using Stock = CashManager_MVVM.Model.Stock;
using Subtransaction = CashManager_MVVM.Model.Subtransaction;
using Tag = CashManager_MVVM.Model.Tag;
using Transaction = Logic.Model.Transaction;

namespace CashManager_MVVM.Mapping
{
    public class MapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Category, Logic.DTO.Category>();
                config.CreateMap<Logic.DTO.Category, Category>();

                config.CreateMap<Stock, Logic.DTO.Stock>();
                config.CreateMap<Logic.DTO.Stock, Stock>();

                config.CreateMap<Transaction, Logic.DTO.Transaction>();
                config.CreateMap<Logic.DTO.Transaction, Transaction>();

                config.CreateMap<Model.Transaction, Logic.DTO.Transaction>();
                config.CreateMap<Logic.DTO.Transaction, Model.Transaction>();

                config.CreateMap<Tag, Logic.DTO.Tag>();
                config.CreateMap<Logic.DTO.Tag, Tag>();
                
                config.CreateMap<Subtransaction, Logic.DTO.Subtransaction>();
                config.CreateMap<Logic.DTO.Subtransaction, Subtransaction>();

                config.CreateMap<PaymentValue, Logic.DTO.PaymentValue>();
                config.CreateMap<Logic.DTO.PaymentValue, PaymentValue>();
            });
        }
    }
}
using AutoMapper;

using Logic.Model;

namespace Logic.Mapping
{
    public class MapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Category, DTO.Category>();
                config.CreateMap<DTO.Category, Category>();

                config.CreateMap<Transaction, DTO.Transaction>();
                config.CreateMap<DTO.Transaction, Transaction>();

                config.CreateMap<Subtransaction, DTO.Subtransaction>();
                config.CreateMap<DTO.Subtransaction, Subtransaction>();

                config.CreateMap<TransactionPartPayment, DTO.TransactionPartPayment>();
                config.CreateMap<DTO.TransactionPartPayment, TransactionPartPayment>();
            });
        }
    }
}
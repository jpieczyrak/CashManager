using AutoMapper;

using Logic.LogicObjectsProviders;
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

                config.CreateMap<Stock, DTO.Stock>();
                config.CreateMap<DTO.Stock, Stock>();

                config.CreateMap<Transaction, DTO.Transaction>();
                config.CreateMap<DTO.Transaction, Transaction>();

                config.CreateMap<Subtransaction, DTO.Subtransaction>()
                      .ForMember(dest => dest.CategoryId,
                          ex => ex.MapFrom(sub => sub.Category.Id));
                config.CreateMap<DTO.Subtransaction, Subtransaction>()
                      .ForMember(dest => dest.Category,
                          ex => ex.MapFrom(
                              sub => CategoryProvider.GetById(sub.CategoryId)
                                    ));

                config.CreateMap<Payment, DTO.Payment>();
                config.CreateMap<DTO.Payment, Payment>();
            });
        }
    }
}
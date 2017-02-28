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
                config.CreateMap<DTO.Category, Category>();
                config.CreateMap<Category, DTO.Category>();

                config.CreateMap<Transaction, DTO.Transaction>();
                config.CreateMap<DTO.Transaction, Transaction>();
            });
        }
    }
}
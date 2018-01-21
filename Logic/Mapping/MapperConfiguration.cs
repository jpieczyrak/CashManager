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

            });
        }
    }
}
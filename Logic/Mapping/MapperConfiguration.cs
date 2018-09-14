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

				config.CreateMap<Stock, DTO.Stock>();
				config.CreateMap<DTO.Stock, Stock>();

				config.CreateMap<Transaction, DTO.Transaction>();
				config.CreateMap<DTO.Transaction, Transaction>();

				config.CreateMap<Transaction, DTO.Transaction>();
				config.CreateMap<DTO.Transaction, Transaction>();

				config.CreateMap<Tag, DTO.Tag>();
				config.CreateMap<DTO.Tag, Tag>();
			});
		}
	}
}

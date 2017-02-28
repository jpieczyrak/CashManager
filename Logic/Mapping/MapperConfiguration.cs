using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Logic.DTO;

namespace Logic.Mapping
{
    public class MapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Category, Model.Category>();
                cfg.CreateMap<Model.Category, Category>();
            });
        }
    }
}

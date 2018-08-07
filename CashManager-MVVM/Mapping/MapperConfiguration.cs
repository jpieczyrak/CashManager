﻿using AutoMapper;

using CashManager_MVVM.Model;

using Category = CashManager_MVVM.Model.Category;
using Stock = CashManager_MVVM.Model.Stock;
using Subtransaction = CashManager_MVVM.Model.Subtransaction;
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

                config.CreateMap<Transaction, CashManager.Data.DTO.Transaction>();
                config.CreateMap<CashManager.Data.DTO.Transaction, Transaction>();

                config.CreateMap<Transaction, CashManager.Data.DTO.Transaction>();
                config.CreateMap<CashManager.Data.DTO.Transaction, Transaction>();

                config.CreateMap<Tag, CashManager.Data.DTO.Tag>();
                config.CreateMap<CashManager.Data.DTO.Tag, Tag>();
                
                config.CreateMap<Subtransaction, CashManager.Data.DTO.Subtransaction>();
                config.CreateMap<CashManager.Data.DTO.Subtransaction, Subtransaction>();

                config.CreateMap<PaymentValue, CashManager.Data.DTO.PaymentValue>();
                config.CreateMap<CashManager.Data.DTO.PaymentValue, PaymentValue>();
            });
        }
    }
}
using AutoMapper;
using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Logic.Balances;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;
using System;
using System.Collections.Generic;
using Category = CashManager_MVVM.Model.Category;
using Stock = CashManager_MVVM.Model.Stock;
using Tag = CashManager_MVVM.Model.Tag;

namespace CashManager_MVVM.Configuration.Mapping
{
    public static class MapperConfiguration
    {
        private static readonly object _lock = new object();
        private static bool _isInitialized;

        public static void Configure()
        {
            lock (_lock)
            {
                if (!_isInitialized)
                {
                    var tags = new Dictionary<Guid, Tag>();
                    var stocks = new Dictionary<Guid, Stock>();
                    var categories = new Dictionary<Guid, Category>();
                    var types = new Dictionary<Guid, TransactionType>();
                    Mapper.Initialize(config =>
                    {
                        config.CreateMap<Category, CashManager.Data.DTO.Category>();
                        config.CreateMap<CashManager.Data.DTO.Category, ExpandableCategory>();
                        config.CreateMap<Category, ExpandableCategory>();
                        config.CreateMap<ExpandableCategory, Category>()
                              .ConstructUsing((dto, context) =>
                              {
                                  if (dto == null) return null;
                                  if (dto.Name != null && categories.TryGetValue(dto.Id, out var output)) return output;

                                  var category = context.Options.CreateInstance<Category>();
                                  if (dto.Name != null) categories[dto.Id] = category;
                                  return category;
                              });
                        config.CreateMap<CashManager.Data.DTO.Category, Category>()
                              .ConstructUsing((dto, context) =>
                              {
                                  if (dto == null) return null;
                                  if (dto.Name != null && categories.TryGetValue(dto.Id, out var output)) return output;

                                  var category = context.Options.CreateInstance<Category>();
                                  if (dto.Name != null) categories[dto.Id] = category;
                                  return category;
                              });

                        config.CreateMap<Balance, CashManager.Data.DTO.Balance>();
                        config.CreateMap<CashManager.Data.DTO.Balance, Balance>()
                              .AfterMap((dto, model) => model.IsPropertyChangedEnabled = true);

                        config.CreateMap<Stock, CashManager.Data.DTO.Stock>()
                              .AfterMap((model, dto) =>
                              {
                                  stocks[model.Id] = model;
                              });
                        config.CreateMap<CashManager.Data.DTO.Stock, Stock>()
                              .BeforeMap((dto, model) => model.IsPropertyChangedEnabled = false)
                              .ConstructUsing((dto, context) =>
                              {
                                  if (dto == null) return null;
                                  if (stocks.TryGetValue(dto.Id, out var output)) return output;
                                  return context.ConfigurationProvider.ServiceCtor(typeof(Stock)) as Stock;
                              })
                              .AfterMap((dto, model) =>
                              {
                                  stocks[dto.Id] = model;
                                  model.IsPropertyChangedEnabled = true;
                              });

                        config.CreateMap<Tag, CashManager.Data.DTO.Tag>();
                        config.CreateMap<CashManager.Data.DTO.Tag, Tag>()
                              .ConstructUsing((dto, context) =>
                              {
                                  if (dto == null) return null;
                                  if (tags.TryGetValue(dto.Id, out var output)) return output;
                                  return context.ConfigurationProvider.ServiceCtor(typeof(Tag)) as Tag;
                              })
                              .AfterMap((dto, model) =>
                              {
                                  tags[dto.Id] = model;
                              });

                        config.CreateMap<PaymentValue, CashManager.Data.DTO.PaymentValue>();
                        config.CreateMap<CashManager.Data.DTO.PaymentValue, PaymentValue>();

                        config.CreateMap<TransactionType, CashManager.Data.DTO.TransactionType>();
                        config.CreateMap<CashManager.Data.DTO.TransactionType, TransactionType>()
                              .ConstructUsing((dto, context) =>
                              {
                                  if (dto == null) return null;
                                  if (types.TryGetValue(dto.Id, out var output)) return output;
                                  return context.ConfigurationProvider.ServiceCtor(typeof(TransactionType)) as TransactionType;
                              })
                              .AfterMap((dto, model) =>
                              {
                                  types[dto.Id] = model;
                              });

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
                                  foreach (var position in model.Positions)
                                  {
                                      position.IsPropertyChangedEnabled = false;
                                      position.Parent = model;

                                      if (position.Category != null && position.Category.Name == null)
                                          if (categories.TryGetValue(position.Category.Id, out var output))
                                              position.Category = output;

                                      position.IsPropertyChangedEnabled = true;
                                  }
                                  model.IsPropertyChangedEnabled = true;
                              });

                        config.CreateMap<SearchState, CashManager.Data.ViewModelState.SearchState>();
                        config.CreateMap<CashManager.Data.ViewModelState.SearchState, SearchState>();

                        config.CreateMap<DateFrameSelector, CashManager.Data.ViewModelState.Selectors.DateFrameSelector>();
                        config.CreateMap<CashManager.Data.ViewModelState.Selectors.DateFrameSelector, DateFrameSelector>();

                        config.CreateMap<RangeSelector, CashManager.Data.ViewModelState.Selectors.RangeSelector>();
                        config.CreateMap<CashManager.Data.ViewModelState.Selectors.RangeSelector, RangeSelector>();

                        config.CreateMap<TextSelector, CashManager.Data.ViewModelState.Selectors.TextSelector>();
                        config.CreateMap<CashManager.Data.ViewModelState.Selectors.TextSelector, TextSelector>();

                        config.CreateMap<MultiPicker, CashManager.Data.ViewModelState.Selectors.MultiPicker>();
                        config.CreateMap<CashManager.Data.ViewModelState.Selectors.MultiPicker, MultiPicker>();

                        config.CreateMap<CustomBalance, CashManager.Data.ViewModelState.Balances.CustomBalance>();
                        config.CreateMap<CashManager.Data.ViewModelState.Balances.CustomBalance, CustomBalance>();
                    });

                    _isInitialized = true;
                }
            }
        }
    }
}
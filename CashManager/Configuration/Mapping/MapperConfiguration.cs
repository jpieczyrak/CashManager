using System;
using System.Collections.Generic;

using AutoMapper;

using CashManager.Data.ViewModelState;
using CashManager.Features.MassReplacer;
using CashManager.Logic.Balances;
using CashManager.Logic.Parsers.Custom;
using CashManager.Model;
using CashManager.Model.Selectors;
using CashManager.Model.Setters;

using Rule = CashManager.Model.Parsers.Rule;
using SearchState = CashManager.Features.Search.SearchState;

namespace CashManager.Configuration.Mapping
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
                        config.CreateMap<Category, Data.DTO.Category>();
                        config.CreateMap<Data.DTO.Category, ExpandableCategory>();
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
                        config.CreateMap<Data.DTO.Category, Category>()
                              .ConstructUsing((dto, context) =>
                              {
                                  if (dto == null) return null;
                                  if (dto.Name != null && categories.TryGetValue(dto.Id, out var output)) return output;

                                  var category = context.Options.CreateInstance<Category>();
                                  if (dto.Name != null) categories[dto.Id] = category;
                                  return category;
                              });

                        config.CreateMap<Balance, Data.DTO.Balance>();
                        config.CreateMap<Data.DTO.Balance, Balance>()
                              .BeforeMap((dto, model) => model.IsPropertyChangedEnabled = false)
                              .AfterMap((dto, model) => model.IsPropertyChangedEnabled = true);

                        config.CreateMap<Stock, Data.DTO.Stock>()
                              .AfterMap((model, dto) =>
                              {
                                  stocks[model.Id] = model;
                              });
                        config.CreateMap<Data.DTO.Stock, Stock>()
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

                        config.CreateMap<Tag, Data.DTO.Tag>();
                        config.CreateMap<Data.DTO.Tag, Tag>()
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

                        config.CreateMap<PaymentValue, Data.DTO.PaymentValue>();
                        config.CreateMap<Data.DTO.PaymentValue, PaymentValue>();

                        config.CreateMap<TransactionType, Data.DTO.TransactionType>();
                        config.CreateMap<Data.DTO.TransactionType, TransactionType>()
                              .BeforeMap((dto, model) => model.IsPropertyChangedEnabled = false)
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

                        config.CreateMap<StoredFileInfo, Data.DTO.StoredFileInfo>();
                        config.CreateMap<Data.DTO.StoredFileInfo, StoredFileInfo>();

                        config.CreateMap<Position, Data.DTO.Position>()
                              .ConstructUsing(x => new Data.DTO.Position());
                        config.CreateMap<Data.DTO.Position, Position>()
                              .BeforeMap((dto, model) => model.IsPropertyChangedEnabled = false)
                              .AfterMap((dto, model) => model.IsPropertyChangedEnabled = true);

                        config.CreateMap<Transaction, Data.DTO.Transaction>();
                        config.CreateMap<Data.DTO.Transaction, Transaction>()
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

                        config.CreateMap<SearchState, Data.ViewModelState.SearchState>();
                        config.CreateMap<Data.ViewModelState.SearchState, SearchState>();

                        config.CreateMap<DateFrameSelector, Data.ViewModelState.Selectors.DateFrameSelector>();
                        config.CreateMap<Data.ViewModelState.Selectors.DateFrameSelector, DateFrameSelector>();

                        config.CreateMap<RangeSelector, Data.ViewModelState.Selectors.RangeSelector>();
                        config.CreateMap<Data.ViewModelState.Selectors.RangeSelector, RangeSelector>();

                        config.CreateMap<TextSelector, Data.ViewModelState.Selectors.TextSelector>();
                        config.CreateMap<Data.ViewModelState.Selectors.TextSelector, TextSelector>();

                        config.CreateMap<MultiPicker, Data.ViewModelState.Selectors.MultiPicker>();
                        config.CreateMap<Data.ViewModelState.Selectors.MultiPicker, MultiPicker>();

                        config.CreateMap<CustomBalance, Data.ViewModelState.Balances.CustomBalance>();
                        config.CreateMap<Data.ViewModelState.Balances.CustomBalance, CustomBalance>();

                        config.CreateMap<Rule, Logic.Parsers.Custom.Rule>().ReverseMap();
                        config.CreateMap<Rule, Data.ViewModelState.Parsers.Rule>().ReverseMap();

                        config.CreateMap<Model.Parsers.CustomCsvParser, Data.ViewModelState.Parsers.CustomCsvParser>().ReverseMap();
                        config.CreateMap<CustomCsvParser, Data.ViewModelState.Parsers.CustomCsvParser>().ReverseMap();
                        config.CreateMap<CustomCsvParser, Model.Parsers.CustomCsvParser>().ReverseMap();

                        config.CreateMap<MassReplacerState, ReplacerState>().ReverseMap();
                        config.CreateMap<TextSetter, Data.ViewModelState.Setters.TextSetter>().ReverseMap();
                        config.CreateMap<MultiSetter, Data.ViewModelState.Setters.MultiSetter>().ReverseMap();
                        config.CreateMap<SingleSetter, Data.ViewModelState.Setters.SingleSetter>().ReverseMap();
                        config.CreateMap<DateSetter, Data.ViewModelState.Setters.DateSetter>().ReverseMap();
                    });

                    _isInitialized = true;
                }
            }
        }
    }
}
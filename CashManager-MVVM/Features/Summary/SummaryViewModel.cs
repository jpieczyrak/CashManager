using System;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Stocks;

using CashManager_MVVM.CommonData;
using CashManager_MVVM.Logic.Calculators;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager_MVVM.Features.Summary
{
    public class SummaryViewModel : ViewModelBase, IUpdateable
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly TransactionsProvider _provider;
        private readonly TransactionBalanceCalculator _calculator;
        private DataPoint[] _balanceSource;
        private PlotModel _balanceModel;

        public DataPoint[] BalanceSource
        {
            get => _balanceSource;
            set => Set(ref _balanceSource, value);
        }

        public PlotModel BalanceModel
        {
            get => _balanceModel;
            set => Set(ref _balanceModel, value);
        }

        public SummaryViewModel(IQueryDispatcher queryDispatcher, TransactionsProvider provider, TransactionBalanceCalculator calculator)
        {
            _queryDispatcher = queryDispatcher;
            _provider = provider;
            _calculator = calculator;
            BalanceModel = new PlotModel { IsLegendVisible = false };
            Update();
        }

        public void Update()
        {
            var stocks = _queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()).Where(x => x.IsUserStock);
            if (!stocks.Any()) return;
            var dateFilter = new DateFrame(DateFrameType.BookDate);

            BalanceModel.Series.Clear();
            BalanceModel.Axes.Clear();

            var values = _calculator.CalculateBalance(_provider.AllTransactions, Mapper.Map<Stock[]>(stocks), dateFilter,
                                        x => new DateTime(x.BookDate.Year, x.BookDate.Month, 1))
                                    .OrderBy(x => x.BookDate).ToArray();
            if (values.Any())
            {
                BalanceModel.Series.Add(new ColumnSeries
                {
                    ItemsSource = values,
                    ValueField = nameof(TransactionBalance.Value),
                });
                BalanceModel.Axes.Add(new CategoryAxis
                {
                    Position = AxisPosition.Bottom,
                    ItemsSource = values.Select(x => x.BookDate.ToString("MM.yyyy")).ToArray(),
                });
            }

            BalanceModel.InvalidatePlot(true);
            BalanceModel.ResetAllAxes();
        }
    }
}
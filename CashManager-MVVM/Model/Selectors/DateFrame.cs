﻿using System;

namespace CashManager_MVVM.Model.Selectors
{
    public class DateFrame : BaseSelector
    {
        private DateTime _from;
        private DateTime _to;

        public DateTime From
        {
            get => _from;
            set => Set(nameof(From), ref _from, value);
        }

        public DateTime To
        {
            get => _to;
            set => Set(nameof(To), ref _to, value);
        }

        public DateFrame(string description)
        {
            Description = description;

            var today = DateTime.Today;
            _from = new DateTime(today.Year, today.Month, 1);
            _to = _from.AddMonths(1).AddDays(-1);
        }
    }
}
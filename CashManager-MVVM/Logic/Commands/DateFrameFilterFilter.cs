﻿using System;
using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Model.Selectors;

namespace CashManager_MVVM.Logic.Commands
{
    public class DateFrameFilterFilter : IFilter<Transaction>, IFilter<Position>
    {
        private readonly DateFrame _dateFrame;
        private readonly Func<IBookable, DateTime> _selector;

        private DateFrameFilterFilter(DateFrame dateFrame, Func<IBookable, DateTime> selector)
        {
            _dateFrame = dateFrame;
            _selector = selector;
        }

        public IEnumerable<Position> Execute(IEnumerable<Position> elements) => Filter(elements).OfType<Position>();

        public IEnumerable<Transaction> Execute(IEnumerable<Transaction> elements) => Filter(elements).OfType<Transaction>();

        public bool CanExecute() => _dateFrame.IsChecked;

        public IEnumerable<IBookable> Filter(IEnumerable<IBookable> elements)
        {
            return elements.Where(x => _selector(x) >= _dateFrame.From && _selector(x) <= _dateFrame.To);
        }

        public static DateFrameFilterFilter Create(DateFrame dateFrame)
        {
            switch (dateFrame.Type)
            {
                case DateFrameType.BookDate:
                    return new DateFrameFilterFilter(dateFrame, x => x.BookDate);
                case DateFrameType.CreationDate:
                    return new DateFrameFilterFilter(dateFrame, x => x.InstanceCreationDate);
                case DateFrameType.EditDate:
                    return new DateFrameFilterFilter(dateFrame, x => x.LastEditDate);
            }

            return null;
        }
    }
}
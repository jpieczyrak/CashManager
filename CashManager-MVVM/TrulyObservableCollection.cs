using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CashManager_MVVM
{
    public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        private bool _stopNotifications;

        public TrulyObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        public TrulyObservableCollection(IEnumerable<T> items) : this()
        {
            int startIndex = Count;
            var newItems = items is List<T> list ? list : new List<T>(items);
            foreach (var item in newItems) Items.Add(item);

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, startIndex));
        }

        public int AddRange(IEnumerable<T> items)
        {
            var changedItems = items is List<T> list ? list : new List<T>(items);
            _stopNotifications = true;
            foreach (var item in changedItems) Add(item);
            _stopNotifications = false;

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            return Count;
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Object item in e.NewItems)
                    ((INotifyPropertyChanged) item).PropertyChanged += ItemPropertyChanged;
            if (e.OldItems != null)
                foreach (Object item in e.OldItems)
                    ((INotifyPropertyChanged) item).PropertyChanged -= ItemPropertyChanged;
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T) sender));
            OnCollectionChanged(args);
        }

        #region Override

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_stopNotifications) base.OnCollectionChanged(e);
        }

        #endregion
    }
}
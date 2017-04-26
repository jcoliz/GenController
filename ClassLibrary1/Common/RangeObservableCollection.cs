using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Common
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        protected bool _suppressNotification = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;

            foreach (T item in list)
            {
                Add(item);
            }
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void InsertRange(int index, IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;

            int i = index;
            foreach (T item in list)
            {
                Insert(i++, item);
            }
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

    }
}

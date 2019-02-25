namespace System.Windows.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    public class WpfObservableRangeCollection<T> : ObservableRangeCollection<T>
    {
        private DeferredEventsCollection _deferredEvents;

        public WpfObservableRangeCollection() { }

        public WpfObservableRangeCollection(IEnumerable<T> collection) : base(collection) { }

        public WpfObservableRangeCollection(List<T> list) : base(list) { }


        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this ObservableCollection will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        /// <remarks>
        /// When overriding this method, either call its base implementation
        /// or call <see cref="BlockReentrancy"/> to guard against reentrant collection changes.
        /// </remarks>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            ICollection<NotifyCollectionChangedEventArgs> _deferredEvents
                = (ICollection<NotifyCollectionChangedEventArgs>)typeof(ObservableRangeCollection<T>).GetField("_deferredEvents", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
            if (_deferredEvents != null)
            {
                _deferredEvents.Add(e);
                return;
            }

            foreach (NotifyCollectionChangedEventHandler handler in GetHandlers())
            {
                if (IsRange(e) && handler.Target is CollectionView cv)
                {
                    cv.Refresh();
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        protected override IDisposable DeferEvents() => new DeferredEventsCollection(this);

        private bool IsRange(NotifyCollectionChangedEventArgs e) => e.NewItems?.Count > 1 || e.OldItems?.Count > 1;

        private IEnumerable<NotifyCollectionChangedEventHandler> GetHandlers()
        {
            FieldInfo info = typeof(ObservableCollection<T>).GetField(nameof(CollectionChanged), BindingFlags.Instance | BindingFlags.NonPublic);
            MulticastDelegate @event = (MulticastDelegate)info.GetValue(this);
            return @event?.GetInvocationList().Cast<NotifyCollectionChangedEventHandler>().Distinct() ?? Enumerable.Empty<NotifyCollectionChangedEventHandler>();
        }

        private class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable
        {
            private readonly WpfObservableRangeCollection<T> _collection;

            public DeferredEventsCollection(WpfObservableRangeCollection<T> collection)
            {
                Debug.Assert(collection != null);
                Debug.Assert(collection._deferredEvents == null);
                _collection = collection;
                _collection._deferredEvents = this;
            }

            public void Dispose()
            {
                _collection._deferredEvents = null;
                ILookup<bool, NotifyCollectionChangedEventHandler> handlers = _collection.GetHandlers().ToLookup(h => h.Target is CollectionView);

                foreach (NotifyCollectionChangedEventHandler handler in handlers[false])
                {
                    foreach (NotifyCollectionChangedEventArgs e in this)
                    {
                        handler(_collection, e);
                    }
                }

                foreach (CollectionView cv in handlers[true].Select(h => h.Target)
                                                            .Cast<CollectionView>()
                                                            .Distinct())
                {
                    cv.Refresh();
                }
            }
        }
    }
}
#pragma warning disable CA1010 // Collections should implement generic interface
namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class CollectionSynchronizerTests : INotifyPropertyChanged, INotifyCollectionChanged, IEnumerable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [Test]
        public void Initialize()
        {
            var source = new[] { 1, 2, 3 };
            var synchronizer = new CollectionSynchronizer<int>(source);
            CollectionAssert.AreEqual(source, synchronizer);
        }

        [Test]
        public void RefreshNulls()
        {
            var source = new[] { 1, 2, 3 };
            var synchronizer = new CollectionSynchronizer<int>(new int[0]);
            synchronizer.Refresh(source);
            CollectionAssert.AreEqual(source, synchronizer);
        }

        [Test]
        public void RefreshSignals()
        {
            var source = new ObservableCollection<int>();
            var synchronizer = new CollectionSynchronizer<int>(source);
            using (var expected = source.SubscribeAll())
            {
                using (var actual = this.SubscribeAll())
                {
                    source.Add(1);
                    synchronizer.Refresh(source, null, this.OnPropertyChanged, this.OnCollectionChanged);
                    CollectionAssert.AreEqual(source, synchronizer);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }
    }
}

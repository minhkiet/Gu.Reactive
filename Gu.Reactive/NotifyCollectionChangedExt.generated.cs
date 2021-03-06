﻿namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;

    using System.Reactive;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Factory methods for creating observables from notifying collections.
    /// </summary>
    public static partial class NotifyCollectionChangedExt
    {
        /// <summary>
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The sopurce item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this ObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ObserveItemPropertyChanged<ObservableCollection<TItem>, TItem, TProperty>(source, property, signalInitial);
        }

        /// <summary>
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The sopurce item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        public static IObservable<PropertyChangedEventArgs> ObserveItemPropertyChangedSlim<TItem, TProperty>(
            this ObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ObserveItemPropertyChangedSlim<ObservableCollection<TItem>, TItem, TProperty>(source, property, signalInitial);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<ObservableCollection<TItem>>>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ItemPropertyChanged<ObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<ObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ItemPropertyChanged<ObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChangedSlim(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<PropertyChangedEventArgs> ItemPropertyChangedSlim<TItem, TProperty>(
             this IObservable<ObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ItemPropertyChangedSlim<ObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The sopurce item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this ReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ObserveItemPropertyChanged<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property, signalInitial);
        }

        /// <summary>
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The sopurce item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        public static IObservable<PropertyChangedEventArgs> ObserveItemPropertyChangedSlim<TItem, TProperty>(
            this ReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ObserveItemPropertyChangedSlim<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property, signalInitial);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<ReadOnlyObservableCollection<TItem>>>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ItemPropertyChanged<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<ReadOnlyObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ItemPropertyChanged<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChangedSlim(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<PropertyChangedEventArgs> ItemPropertyChangedSlim<TItem, TProperty>(
             this IObservable<ReadOnlyObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ItemPropertyChangedSlim<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The sopurce item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this IReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ObserveItemPropertyChanged<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property, signalInitial);
        }

        /// <summary>
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The sopurce item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        public static IObservable<PropertyChangedEventArgs> ObserveItemPropertyChangedSlim<TItem, TProperty>(
            this IReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ObserveItemPropertyChangedSlim<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property, signalInitial);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<IReadOnlyObservableCollection<TItem>>>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ItemPropertyChanged<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<IReadOnlyObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ItemPropertyChanged<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChangedSlim(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<PropertyChangedEventArgs> ItemPropertyChangedSlim<TItem, TProperty>(
             this IObservable<IReadOnlyObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return ItemPropertyChangedSlim<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }
    }
}
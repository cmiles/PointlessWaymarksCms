﻿using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;

namespace PointlessWaymarksCmsWpfControls.Utility
{
    public class ItemsControlScrollToEndOnNewItemBehaviour : Behavior<ItemsControl>
    {
        private INotifyCollectionChanged _cachedItemsSource;

        public static readonly DependencyProperty ItemsControlScrollViewerProperty =
            DependencyProperty.Register("ItemsControlScrollViewer", typeof(ScrollViewer),
                typeof(ItemsControlScrollToEndOnNewItemBehaviour), new PropertyMetadata(default(ScrollViewer)));

        public ScrollViewer ItemsControlScrollViewer
        {
            get => (ScrollViewer) GetValue(ItemsControlScrollViewerProperty);
            set => SetValue(ItemsControlScrollViewerProperty, value);
        }

        private void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            UpdateItemsSource();
        }

        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                try
                {
                    Application.Current?.Dispatcher?.BeginInvoke(
                        (Action) (() => { ItemsControlScrollViewer.ScrollToEnd(); }), DispatcherPriority.DataBind);
                }
                catch (Exception ex)
                {
                }
        }

        private void ListBoxItemsSourceChanged(object? sender, EventArgs e)
        {
            UpdateItemsSource();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            UpdateItemsSource();

            AssociatedObject.Loaded += AssociatedObjectLoaded;

            var itemsSourcePropertyDescriptor = TypeDescriptor.GetProperties(AssociatedObject)["ItemsSource"];
            itemsSourcePropertyDescriptor.AddValueChanged(AssociatedObject, ListBoxItemsSourceChanged);
        }

        private void UpdateItemsSource()
        {
            if (_cachedItemsSource != null)
            {
                _cachedItemsSource.CollectionChanged -= ItemsSourceCollectionChanged;
                var itemsSourcePropertyDescriptor = TypeDescriptor.GetProperties(AssociatedObject)["ItemsSource"];
                itemsSourcePropertyDescriptor.RemoveValueChanged(AssociatedObject, ListBoxItemsSourceChanged);
            }

            if (AssociatedObject.ItemsSource is INotifyCollectionChanged sourceCollection)
            {
                _cachedItemsSource = sourceCollection;
                if (_cachedItemsSource == null) return;
                _cachedItemsSource.CollectionChanged += ItemsSourceCollectionChanged;
                var itemsSourcePropertyDescriptor = TypeDescriptor.GetProperties(AssociatedObject)["ItemsSource"];
                itemsSourcePropertyDescriptor.AddValueChanged(AssociatedObject, ListBoxItemsSourceChanged);
            }
        }
    }
}
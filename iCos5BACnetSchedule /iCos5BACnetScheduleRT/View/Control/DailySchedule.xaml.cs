using iCos5.BACnet.Zenon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace iCos5BACnetScheduleRT.View.Control
{
  /// <summary>
  /// DailySchedule.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class DailySchedule : UserControl
  {
    public int SelectedIndex
    {
      get { return (int)GetValue(SelectedIndexProperty); }
      set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexProperty =
      DependencyProperty.Register("SelectedIndex", typeof(int), typeof(DailySchedule),
        new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedIndexChanged)));

    private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DailySchedule me = (DailySchedule)d;

      if (me == null)
      {
        return;
      }

      if (!(me.ItemsSource is ObservableCollection<BooleanScheduleItemPair>))
      {
        return;
      }

      me.setSelectedIndex((int)e.NewValue);
    }

    public IEnumerable ItemsSource
    {
      get { return (IEnumerable)GetValue(ItemsSourceProperty); }
      set
      {
        if (value == null)
        {
          ClearValue(ItemsSourceProperty);
        }
        else
        {
          SetValue(ItemsSourceProperty, value);
        }
      }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DailySchedule),
          new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DailySchedule me = (DailySchedule)d;

      if (me == null)
      {
        return;
      }

      if (!(e.NewValue is ObservableCollection<BooleanScheduleItemPair>))
      {
        return;
      }

      if (e.OldValue is INotifyCollectionChanged)
      {
        INotifyCollectionChanged oldValueINotifyCollectionChanged = (INotifyCollectionChanged)e.OldValue;

        if (oldValueINotifyCollectionChanged != null)
        {
          oldValueINotifyCollectionChanged.CollectionChanged -= me.itemINotifyCollectionChanged_CollectionChanged;
        }
      }
      else if (e.OldValue is INotifyCollectionChanged[])
      {
        foreach (INotifyCollectionChanged notifyCollectionChanged in (INotifyCollectionChanged[])e.OldValue)
        {
          notifyCollectionChanged.CollectionChanged -= me.itemINotifyCollectionChanged_CollectionChanged;
        }
      }

      INotifyCollectionChanged newValueINotifyCollectionChanged = (INotifyCollectionChanged)e.NewValue;

      if (newValueINotifyCollectionChanged != null)
      {
        me.SelectedIndex = -1;
        me.refresh();
        newValueINotifyCollectionChanged.CollectionChanged += me.itemINotifyCollectionChanged_CollectionChanged;
      }

      GC.Collect();
    }

    private List<ScheduleElement> _scheduleElements = new List<ScheduleElement>();

    public DailySchedule()
    {
      InitializeComponent();
    }

    private void itemINotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      refresh();
    }

    private void refresh()
    {
      ObservableCollection<BooleanScheduleItemPair> itemPairs = (ObservableCollection<BooleanScheduleItemPair>)ItemsSource;

      MainContainerGrid.Children.Clear();

      foreach (ScheduleElement scheduleElement in _scheduleElements)
      {
        ((BooleanScheduleItemPair)scheduleElement.DataContext).IsSelectedEvent -= itemPair_IsSelectedEvent;
      }

      _scheduleElements.Clear();

      foreach (BooleanScheduleItemPair itemPair in itemPairs)
      {
        ScheduleElement scheduleElement = new ScheduleElement()
        {
          DataContext = itemPair,
        };

        itemPair.Index = _scheduleElements.Count;
        itemPair.IsSelectedEvent += itemPair_IsSelectedEvent;

        MainContainerGrid.Children.Add(scheduleElement);
        _scheduleElements.Add(scheduleElement);
      }

      GC.Collect();
    }

    private void itemPair_IsSelectedEvent(object sender, EventArgs e)
    {
      SelectedIndex = ((BooleanScheduleItemPair)sender).Index;
    }

    private void setSelectedIndex(int selectedIndex)
    {
      foreach (ScheduleElement scheduleElement in _scheduleElements)
      {
        if (scheduleElement.IsSelected)
        {
          scheduleElement.IsSelected = false;
        }
      }

      if (selectedIndex >= 0 && selectedIndex < _scheduleElements.Count)
      {
        _scheduleElements[selectedIndex].IsSelected = true;
      }
    }
  }
}

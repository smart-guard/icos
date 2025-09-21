using iCos5.BACnet.Zenon;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace iCos5BACnetScheduleRT.View
{
  public partial class ViewWeeklySchedule : UserControl
  {
    public int SelectedIndexMonday
    {
      get { return (int)GetValue(SelectedIndexMondayProperty); }
      set { SetValue(SelectedIndexMondayProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexMondayProperty =
      DependencyProperty.Register("SelectedIndexMonday", typeof(int), typeof(ViewWeeklySchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexMondayChanged)));

    private static void OnSelectedIndexMondayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewWeeklySchedule me = (ViewWeeklySchedule)d;

      if (me.SelectedIndexMonday != -1)
      {
        me.setSelectedColumn(0);
      }
    }

    public int SelectedIndexTuesday
    {
      get { return (int)GetValue(SelectedIndexTuesdayProperty); }
      set { SetValue(SelectedIndexTuesdayProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexTuesdayProperty =
      DependencyProperty.Register("SelectedIndexTuesday", typeof(int), typeof(ViewWeeklySchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexTuesdayChanged)));

    private static void OnSelectedIndexTuesdayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewWeeklySchedule me = (ViewWeeklySchedule)d;

      if (me.SelectedIndexTuesday != -1)
      {
        me.setSelectedColumn(1);
      }
    }

    public int SelectedIndexWednesday
    {
      get { return (int)GetValue(SelectedIndexWednesdayProperty); }
      set { SetValue(SelectedIndexWednesdayProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexWednesdayProperty =
      DependencyProperty.Register("SelectedIndexWednesday", typeof(int), typeof(ViewWeeklySchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexWednesdayChanged)));

    private static void OnSelectedIndexWednesdayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewWeeklySchedule me = (ViewWeeklySchedule)d;

      if (me.SelectedIndexWednesday != -1)
      {
        me.setSelectedColumn(2);
      }
    }

    public int SelectedIndexThursday
    {
      get { return (int)GetValue(SelectedIndexThursdayProperty); }
      set { SetValue(SelectedIndexThursdayProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexThursdayProperty =
      DependencyProperty.Register("SelectedIndexThursday", typeof(int), typeof(ViewWeeklySchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexThursdayChanged)));

    private static void OnSelectedIndexThursdayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewWeeklySchedule me = (ViewWeeklySchedule)d;

      if (me.SelectedIndexThursday != -1)
      {
        me.setSelectedColumn(3);
      }
    }

    public int SelectedIndexFriday
    {
      get { return (int)GetValue(SelectedIndexFridayProperty); }
      set { SetValue(SelectedIndexFridayProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexFridayProperty =
      DependencyProperty.Register("SelectedIndexFriday", typeof(int), typeof(ViewWeeklySchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexFridayChanged)));

    private static void OnSelectedIndexFridayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewWeeklySchedule me = (ViewWeeklySchedule)d;

      if (me.SelectedIndexFriday != -1)
      {
        me.setSelectedColumn(4);
      }
    }

    public int SelectedIndexSaturday
    {
      get { return (int)GetValue(SelectedIndexSaturdayProperty); }
      set { SetValue(SelectedIndexSaturdayProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexSaturdayProperty =
      DependencyProperty.Register("SelectedIndexSaturday", typeof(int), typeof(ViewWeeklySchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexSaturdayChanged)));

    private static void OnSelectedIndexSaturdayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewWeeklySchedule me = (ViewWeeklySchedule)d;

      if (me.SelectedIndexSaturday != -1)
      {
        me.setSelectedColumn(5);
      }
    }

    public int SelectedIndexSunday
    {
      get { return (int)GetValue(SelectedIndexSundayProperty); }
      set { SetValue(SelectedIndexSundayProperty, value); }
    }

    public static readonly DependencyProperty SelectedIndexSundayProperty =
      DependencyProperty.Register("SelectedIndexSunday", typeof(int), typeof(ViewWeeklySchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexSundayChanged)));

    private static void OnSelectedIndexSundayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewWeeklySchedule me = (ViewWeeklySchedule)d;

      if (me.SelectedIndexSunday != -1)
      {
        me.setSelectedColumn(6);
      }
    }

    private void setSelectedColumn(int columnIndex)
    {
      if (columnIndex >= 0 && columnIndex < 7)
      {
        if (DailySchedulesGrid.CurrentCell.Column != null &&
            DailySchedulesGrid.CurrentCell.Column.DisplayIndex == columnIndex)
        {
          foreach (DataGridCellInfo cell in DailySchedulesGrid.SelectedCells)
          {
            if (cell.Column.DisplayIndex != columnIndex)
            {
              DailySchedulesGrid.SelectedCells.Remove(cell);
            }
          }
        }
        else
        {
          DailySchedulesGrid.CurrentCell = new DataGridCellInfo(DailySchedulesGrid.Items[0], DailySchedulesGrid.Columns[columnIndex]);
          DailySchedulesGrid.SelectedCells.Clear();
          DailySchedulesGrid.SelectedCells.Add(DailySchedulesGrid.CurrentCell);
        }
      }
    }

    private void DailySchedulesGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (!(DataContext is WeeklySchedule))
      {
        return;
      }

      try
      {
        WeeklySchedule weeklySchedule = (WeeklySchedule)DataContext;
        DailySchedulesGrid.ItemsSource = new IBACnetScheduleSet[] { weeklySchedule.ScheduleSet };

        if (weeklySchedule.ScheduleSet == null)
        {
          return;
        }

        WeeklyScheduleSet weeklyScheduleSet = (WeeklyScheduleSet)weeklySchedule.ScheduleSet;
        DailyScheduleMonday.ItemsSource = weeklyScheduleSet.ScheduleItemPairs[0];
        DailyScheduleTuesday.ItemsSource = weeklyScheduleSet.ScheduleItemPairs[1];
        DailyScheduleWednesday.ItemsSource = weeklyScheduleSet.ScheduleItemPairs[2];
        DailyScheduleThursday.ItemsSource = weeklyScheduleSet.ScheduleItemPairs[3];
        DailyScheduleFriday.ItemsSource = weeklyScheduleSet.ScheduleItemPairs[4];
        DailyScheduleSaturday.ItemsSource = weeklyScheduleSet.ScheduleItemPairs[5];
        DailyScheduleSunday.ItemsSource = weeklyScheduleSet.ScheduleItemPairs[6];
      }
      catch (Exception ex)
      {
        loggingError("DailySchedulesGrid_DataContextChanged", ex.ToString());
      }
    }

    private void DailySchedulesGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
      if (DataContext == null)
      {
        return;
      }

      try
      {
        foreach (DataGridCellInfo cell in e.RemovedCells)
        {
          resetSelectedIndex(cell.Column.DisplayIndex);
        }

        if (DailySchedulesGrid.SelectedCells.Count != 1)
        {
          DailyWeekDay.Text = string.Empty;
          DailyScheduleListGrid.ItemsSource = null;

          if (DailySchedulesGrid.SelectedCells.Count == 7)
          {
            resetSelectedIndex(DailySchedulesGrid.CurrentCell.Column.DisplayIndex);
          }

          return;
        }

        DataGridColumn column = DailySchedulesGrid.SelectedCells[0].Column;
        string currentWeekDay = column.Header.ToString();

        if (currentWeekDay.Equals(DailyWeekDay.Text))
        {
          return;
        }

        DailyWeekDay.Text = currentWeekDay;
        DailyScheduleListGrid.ItemsSource = ((WeeklySchedule)DataContext).ScheduleSet.ScheduleItemPairs[column.DisplayIndex];
        setBindingSelectedIndex(column.DisplayIndex);
      }
      catch (Exception ex)
      {
        loggingError("DailySchedulesGrid_SelectedCellsChanged", ex.ToString());
      }
    }

    private void resetSelectedIndex(int columnIndex)
    {
      switch (columnIndex)
      {
        case 0:
          SelectedIndexMonday = -1;
          break;
        case 1:
          SelectedIndexTuesday = -1;
          break;
        case 2:
          SelectedIndexWednesday = -1;
          break;
        case 3:
          SelectedIndexThursday = -1;
          break;
        case 4:
          SelectedIndexFriday = -1;
          break;
        case 5:
          SelectedIndexSaturday = -1;
          break;
        case 6:
          SelectedIndexSunday = -1;
          break;
      }
    }

    private void setBindingSelectedIndex(int columnIndex)
    {
      Binding selectedIndexBinding;

      switch (columnIndex)
      {
        case 0:
        default:
          selectedIndexBinding = new Binding("SelectedIndexMonday");
          break;
        case 1:
          selectedIndexBinding = new Binding("SelectedIndexTuesday");
          break;
        case 2:
          selectedIndexBinding = new Binding("SelectedIndexWednesday");
          break;
        case 3:
          selectedIndexBinding = new Binding("SelectedIndexThursday");
          break;
        case 4:
          selectedIndexBinding = new Binding("SelectedIndexFriday");
          break;
        case 5:
          selectedIndexBinding = new Binding("SelectedIndexSaturday");
          break;
        case 6:
          selectedIndexBinding = new Binding("SelectedIndexSunday");
          break;
      }

      selectedIndexBinding.Source = thisControl;
      selectedIndexBinding.Mode = BindingMode.TwoWay;
      BindingOperations.SetBinding(thisControl, SelectedListIndexProperty, selectedIndexBinding);
    }

    private void DailySchedulesGridColumnHeader_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        DailySchedulesGrid.Focus();
        DataGridColumnHeader columnHeader = (DataGridColumnHeader)sender;

        if (columnHeader != null)
        {
          DailySchedulesGrid.CurrentCell = new DataGridCellInfo(DailySchedulesGrid.Items[0], columnHeader.Column);
          DailySchedulesGrid.SelectedCells.Clear();
          DailySchedulesGrid.SelectedCells.Add(DailySchedulesGrid.CurrentCell);
        }
      }
      catch (Exception ex)
      {
        loggingError("DailySchedulesGridColumnHeader_Click", ex.ToString());
      }
    }
  }
}

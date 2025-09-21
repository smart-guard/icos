using iCos5.BACnet.Zenon;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace iCos5BACnetScheduleRT.View
{
  public partial class ViewExceptionSchedule : UserControl
  {
    public int SelectedListIndex
    {
      get { return (int)GetValue(SelectedListIndexProperty); }
      set { SetValue(SelectedListIndexProperty, value); }
    }

    public static readonly DependencyProperty SelectedListIndexProperty =
      DependencyProperty.Register("SelectedListIndex", typeof(int), typeof(ViewExceptionSchedule), new FrameworkPropertyMetadata(-1));

    private void StartTimePicker_SelectedDateTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
    {
      if (DailyScheduleListGrid.SelectedCells.Count != 2 || e.OldValue == null || e.OldValue.Equals(e.NewValue))
      {
        return;
      }

      try
      {
        ObservableCollection<BooleanScheduleItemPair> items = (ObservableCollection<BooleanScheduleItemPair>)DailyScheduleListGrid.ItemsSource;
        int itemRow = DailyScheduleListGrid.SelectedIndex;
        DateTime newTime = Convert.ToDateTime(e.NewValue);
        newTime = new DateTime(1900, 1, 1, newTime.Hour, newTime.Minute, newTime.Second);

        if (itemRow > 0 && items[itemRow - 1].StopItem.Time.Subtract(ScheduleItem.NullDateTime).TotalSeconds == 0)
        {
          MessageBox.Show($"이전항목의 종료시간이 설정되지 않았습니다.{Environment.NewLine}이전 줄의 종료시간을 먼저 설정하십시오.");
          return;
        }

        if (items[itemRow].StopItem.Time.Subtract(ScheduleItem.NullDateTime).TotalSeconds == 0)
        {
          MessageBox.Show($"선택항목의 종료시간이 설정되지 않았습니다.{Environment.NewLine}같은 줄의 종료시간을 먼저 설정하십시오.");
          return;
        }

        TimePicker timePicker = (TimePicker)sender;
        DateTime fromTime = itemRow == 0 ? new DateTime(1900, 1, 1, 0, 0, 0) : items[itemRow - 1].StopItem.Time.AddSeconds(1);
        DateTime toTime = items[itemRow].StopItem.Time.AddSeconds(1 - _config.SchedulePairLimitInterval * 60);

        if (newTime.Subtract(fromTime).TotalSeconds < 0)
        {
          timePicker.SelectedDateTime = fromTime;
          MessageBox.Show($"시작시간 범위를 벗어났습니다.{Environment.NewLine}{fromTime.ToString("HH:mm:ss")} 보다 느린 시간으로 설정하십시오.");
        }

        if (newTime.Subtract(toTime).TotalSeconds > 0)
        {
          timePicker.SelectedDateTime = toTime;
          MessageBox.Show($"시작시간 범위를 벗어났습니다.{Environment.NewLine}{toTime.ToString("HH:mm:ss")} 보다 빠른 시간으로 설정하십시오.");
        }
      }
      catch (Exception ex)
      {
        loggingError("StartTimePicker_SelectedDateTimeChanged", ex.ToString());
      }
    }

    private void StopTimePicker_SelectedDateTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
    {
      if (DailyScheduleListGrid.SelectedCells.Count != 2 || e.OldValue == null || e.OldValue.Equals(e.NewValue))
      {
        return;
      }

      try
      {
        ObservableCollection<BooleanScheduleItemPair> items = (ObservableCollection<BooleanScheduleItemPair>)DailyScheduleListGrid.ItemsSource;
        int itemRow = DailyScheduleListGrid.SelectedIndex;
        DateTime newTime = Convert.ToDateTime(e.NewValue);
        newTime = new DateTime(1900, 1, 1, newTime.Hour, newTime.Minute, newTime.Second);

        if (items[itemRow].StartItem.Time.Subtract(ScheduleItem.NullDateTime).TotalSeconds == 0)
        {
          MessageBox.Show($"선택항목의 시작시간이 설정되지 않았습니다.{Environment.NewLine}같은 줄의 시작시간을 먼저 설정하십시오.");
          return;
        }

        if (itemRow < items.Count - 1 && items[itemRow + 1].StartItem.Time.Subtract(ScheduleItem.NullDateTime).TotalSeconds == 0)
        {
          MessageBox.Show($"다음항목의 시작시간이 설정되지 않았습니다.{Environment.NewLine}다음 줄의 시작시간을 먼저 설정하십시오.");
          return;
        }

        TimePicker timePicker = (TimePicker)sender;
        DateTime fromTime = items[itemRow].StartItem.Time.AddSeconds(_config.SchedulePairLimitInterval * 60 - 1);
        DateTime toTime = itemRow == items.Count - 1 ? new DateTime(1900, 1, 1, 23, 59, 59) : items[itemRow + 1].StartItem.Time.AddSeconds(-1);

        if (newTime.Subtract(fromTime).TotalSeconds < 0)
        {
          timePicker.SelectedDateTime = fromTime;
          MessageBox.Show($"종료시간 범위를 벗어났습니다.{Environment.NewLine}{fromTime.ToString("HH:mm:ss")} 보다 느린 시간으로 설정하십시오.");
        }

        if (newTime.Subtract(toTime).TotalSeconds > 0)
        {
          timePicker.SelectedDateTime = toTime;
          MessageBox.Show($"종료시간 범위를 벗어났습니다.{Environment.NewLine}{toTime.ToString("HH:mm:ss")} 보다 빠른 시간으로 설정하십시오.");
        }
      }
      catch (Exception ex)
      {
        loggingError("StopTimePicker_SelectedDateTimeChanged", ex.ToString());
      }
    }

    private void DailySchedulePreviousAdd_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        ObservableCollection<BooleanScheduleItemPair> itemPair = (ObservableCollection<BooleanScheduleItemPair>)DailyScheduleListGrid.ItemsSource;
        int currentIndex = DailyScheduleListGrid.SelectedIndex;

        if (currentIndex < 0 || currentIndex >= itemPair.Count)
        {
          MessageBox.Show($"잘못된 인덱스 번호입니다.[{currentIndex}]");
          return;
        }

        if (itemPair[currentIndex].StartItem.Time.Subtract(ScheduleItem.NullDateTime).TotalSeconds == 0)
        {
          MessageBox.Show($"선택한 항목의 시작시간이 설정되지 않았습니다.");
          return;
        }

        DateTime newStartTime = currentIndex == 0 ? new DateTime(1900, 1, 1, 0, 0, 0) : itemPair[currentIndex - 1].StopItem.Time.AddSeconds(1);
        DateTime newStopTime = itemPair[currentIndex].StartItem.Time.AddSeconds(-1);

        if (newStopTime.Subtract(newStartTime).TotalSeconds >= _config.SchedulePairLimitInterval * 60 - 1)
        {
          ObservableCollection<BooleanScheduleItemPair> tempItemsSource = (ObservableCollection<BooleanScheduleItemPair>)DailyScheduleListGrid.ItemsSource;
          DailyScheduleListGrid.ItemsSource = null;
          itemPair.Insert(currentIndex, new BooleanScheduleItemPair(newStartTime, newStopTime));
          DailyScheduleListGrid.ItemsSource = tempItemsSource;
          DailyScheduleListGrid.SelectedIndex = currentIndex;
          GC.Collect();
        }
        else
        {
          MessageBox.Show($"시간 간격이 작아 항목을 추가 할 수 없습니다.{Environment.NewLine}최소 {_config.SchedulePairLimitInterval * 60 - 1}초 이상의 간격이 필요합니다.");
        }
      }
      catch (Exception ex)
      {
        loggingError("DailySchedulePreviousAdd_Click", ex.ToString());
      }
    }

    private void DailyScheduleNextAdd_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        ObservableCollection<BooleanScheduleItemPair> itemPair = (ObservableCollection<BooleanScheduleItemPair>)DailyScheduleListGrid.ItemsSource;
        int currentIndex = DailyScheduleListGrid.SelectedIndex;

        if (currentIndex < 0 || currentIndex >= itemPair.Count)
        {
          if (itemPair.Count == 0)
          {
            ObservableCollection<BooleanScheduleItemPair> tempItemsSource = (ObservableCollection<BooleanScheduleItemPair>)DailyScheduleListGrid.ItemsSource;
            DailyScheduleListGrid.ItemsSource = null;
            itemPair.Insert(currentIndex + 1, new BooleanScheduleItemPair(new DateTime(1900, 1, 1, 0, 0, 0), new DateTime(1900, 1, 1, 23, 59, 59)));
            DailyScheduleListGrid.ItemsSource = tempItemsSource;
            DailyScheduleListGrid.SelectedIndex = currentIndex + 1;
            GC.Collect();
            return;
          }
          else
          {
            MessageBox.Show($"잘못된 인덱스 번호입니다.[{currentIndex}]");
            return;
          }
        }

        if (itemPair[currentIndex].StopItem.Time.Subtract(ScheduleItem.NullDateTime).TotalSeconds == 0)
        {
          MessageBox.Show($"선택한 항목의 종료시간이 설정되지 않았습니다.");
          return;
        }

        DateTime newStartTime = itemPair[currentIndex].StopItem.Time.AddSeconds(1);
        DateTime newStopTime = currentIndex == itemPair.Count - 1 ? new DateTime(1900, 1, 1, 23, 59, 59) : itemPair[currentIndex + 1].StartItem.Time.AddSeconds(-1);

        if (newStopTime.Subtract(newStartTime).TotalSeconds >= _config.SchedulePairLimitInterval * 60 - 1)
        {
          ObservableCollection<BooleanScheduleItemPair> tempItemsSource = (ObservableCollection<BooleanScheduleItemPair>)DailyScheduleListGrid.ItemsSource;
          DailyScheduleListGrid.ItemsSource = null;
          itemPair.Insert(currentIndex + 1, new BooleanScheduleItemPair(newStartTime, newStopTime));
          DailyScheduleListGrid.ItemsSource = tempItemsSource;
          DailyScheduleListGrid.SelectedIndex = currentIndex + 1;
          GC.Collect();
        }
        else
        {
          MessageBox.Show($"시간간격이 작아 항목을 추가할 수 없습니다.{Environment.NewLine}최소 {_config.SchedulePairLimitInterval * 60 - 1}초 이상의 간격이 필요합니다.");
        }
      }
      catch (Exception ex)
      {
        loggingError("DailyScheduleNextAdd_Click", ex.ToString());
      }
    }

    private void DailyScheduleRemove_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        ObservableCollection<BooleanScheduleItemPair> itemPair = (ObservableCollection<BooleanScheduleItemPair>)DailyScheduleListGrid.ItemsSource;
        int currentIndex = DailyScheduleListGrid.SelectedIndex;

        if (currentIndex < 0 || currentIndex >= itemPair.Count)
        {
          MessageBox.Show($"잘못된 인덱스 번호입니다.[{currentIndex}]");
          return;
        }

        if (MessageBox.Show("선택한 항목을 삭제하시겠습니까?", "스케줄 항목 삭제", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          ObservableCollection<BooleanScheduleItemPair> tempItemsSource = (ObservableCollection<BooleanScheduleItemPair>)DailyScheduleListGrid.ItemsSource;
          DailyScheduleListGrid.ItemsSource = null;
          itemPair.RemoveAt(currentIndex);
          DailyScheduleListGrid.ItemsSource = tempItemsSource;
          DailyScheduleListGrid.SelectedIndex = -1;
          GC.Collect();
        }
      }
      catch (Exception ex)
      {
        loggingError("DailyScheduleRemove_Click", ex.ToString());
      }
    }

    private void DailyScheudleApply_Click(object sender, RoutedEventArgs e)
    {
      if (!(DataContext is ExceptionSchedule))
      {
        return;
      }

      try
      {
        IList<DataGridCellInfo> selectedCells = DailySchedulesGrid.SelectedCells;
        int currentIndex = -1;

        if (selectedCells.Count > 0)
        {
          currentIndex = selectedCells[0].Column.DisplayIndex;
        }

        if (MessageBox.Show($"스케줄을 저장하시겠습니까?", "예외스케줄 저장", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          writeValue(((ExceptionSchedule)DataContext).GetCode(), currentIndex);
        }
      }
      catch (Exception ex)
      {
        loggingError("DailyScheudleApply_Click", ex.ToString());
      }
    }
  }
}

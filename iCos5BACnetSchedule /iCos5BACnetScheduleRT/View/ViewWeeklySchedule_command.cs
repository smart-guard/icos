using iCos5.BACnet.Zenon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace iCos5BACnetScheduleRT.View
{
  public partial class ViewWeeklySchedule : UserControl
  {
    private void CommandBinding_Copy(object sender, ExecutedRoutedEventArgs e)
    {
      try
      {
        if (!(DataContext is WeeklySchedule))
        {
          return;
        }

        IList<DataGridCellInfo> selectedCells = DailySchedulesGrid.SelectedCells;

        if (selectedCells.Count == 0)
        {
          return;
        }

        List<ObservableCollection<BooleanScheduleItemPair>> schedules = new List<ObservableCollection<BooleanScheduleItemPair>>();
        ObservableCollection<BooleanScheduleItemPair>[] itemPairs = ((WeeklyScheduleSet)DailySchedulesGrid.Items[0]).ScheduleItemPairs;

        foreach (DataGridCellInfo cellInfo in selectedCells)
        {
          schedules.Add(itemPairs[cellInfo.Column.DisplayIndex]);
        }

        DailyCopySchedule duplicate = new DailyCopySchedule(schedules.ToArray());
        Clipboard.SetDataObject(duplicate.GetCode(), true);
      }
      catch (Exception ex)
      {
        loggingError("CommandBinding_Copy", ex.ToString());
      }
    }

    private void CommandBinding_Paste(object sender, ExecutedRoutedEventArgs e)
    {
      try
      {
        if (!(DataContext is WeeklySchedule))
        {
          return;
        }

        IList<DataGridCellInfo> selectedCells = DailySchedulesGrid.SelectedCells;

        if (selectedCells.Count == 0)
        {
          return;
        }

        DataObject retrievedData = (DataObject)Clipboard.GetDataObject();

        if (retrievedData.GetDataPresent(DataFormats.Text))
        {
          int currentPos = selectedCells[0].Column.DisplayIndex;
          DailyCopySchedule duplicate = new DailyCopySchedule((string)retrievedData.GetData(DataFormats.Text));
          ObservableCollection<BooleanScheduleItemPair>[] targetItems = ((WeeklyScheduleSet)selectedCells[0].Item).ScheduleItemPairs;

          foreach (ObservableCollection<BooleanScheduleItemPair> itemPairs in duplicate.ScheduleSet.ScheduleItemPairs)
          {
            targetItems[currentPos].Clear();

            foreach (BooleanScheduleItemPair itemPair in itemPairs)
            {
              targetItems[currentPos].Add(itemPair);
            }

            currentPos++;

            if (currentPos > DailySchedulesGrid.Columns.Count - 1)
            {
              break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        loggingError("CommandBinding_Paste", ex.ToString());
      }
    }

    private void CommandBinding_Delete(object sender, ExecutedRoutedEventArgs e)
    {
      try
      {
        IList<DataGridCellInfo> selectedCells = DailySchedulesGrid.SelectedCells;

        if (selectedCells.Count == 0)
        {
          return;
        }

        if (MessageBox.Show($"선택한 스케줄을 삭제하시겠습니까?", "스케줄 삭제", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
        {
          return;
        }

        WeeklySchedule weeklySchedule = (WeeklySchedule)DataContext;

        foreach (DataGridCellInfo cell in selectedCells)
        {
          weeklySchedule.ScheduleSet.ScheduleItemPairs[cell.Column.DisplayIndex].Clear();
        }
      }
      catch (Exception ex)
      {
        loggingError("CommandBinding_Delete", ex.ToString());
      }
    }
  }
}

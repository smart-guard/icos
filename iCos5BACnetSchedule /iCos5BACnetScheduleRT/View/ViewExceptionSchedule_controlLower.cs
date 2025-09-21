using iCos5.BACnet.Zenon;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace iCos5BACnetScheduleRT.View
{
  public partial class ViewExceptionSchedule : UserControl
  {
    public int SelectedIndexColumn1
    {
      get { return (int)GetValue(SelectedIndexColumn1Property); }
      set { SetValue(SelectedIndexColumn1Property, value); }
    }

    public static readonly DependencyProperty SelectedIndexColumn1Property =
      DependencyProperty.Register("SelectedIndexColumn1", typeof(int), typeof(ViewExceptionSchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexColumn1Changed)));

    private static void OnSelectedIndexColumn1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewExceptionSchedule me = (ViewExceptionSchedule)d;

      if (me.SelectedIndexColumn1 != -1)
      {
        me.setSelectedColumn(0);
      }
    }

    public int SelectedIndexColumn2
    {
      get { return (int)GetValue(SelectedIndexColumn2Property); }
      set { SetValue(SelectedIndexColumn2Property, value); }
    }

    public static readonly DependencyProperty SelectedIndexColumn2Property =
      DependencyProperty.Register("SelectedIndexColumn2", typeof(int), typeof(ViewExceptionSchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexColumn2Changed)));

    private static void OnSelectedIndexColumn2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewExceptionSchedule me = (ViewExceptionSchedule)d;

      if (me.SelectedIndexColumn2 != -1)
      {
        me.setSelectedColumn(1);
      }
    }

    public int SelectedIndexColumn3
    {
      get { return (int)GetValue(SelectedIndexColumn3Property); }
      set { SetValue(SelectedIndexColumn3Property, value); }
    }

    public static readonly DependencyProperty SelectedIndexColumn3Property =
      DependencyProperty.Register("SelectedIndexColumn3", typeof(int), typeof(ViewExceptionSchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexColumn3Changed)));

    private static void OnSelectedIndexColumn3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewExceptionSchedule me = (ViewExceptionSchedule)d;

      if (me.SelectedIndexColumn3 != -1)
      {
        me.setSelectedColumn(2);
      }
    }

    public int SelectedIndexColumn4
    {
      get { return (int)GetValue(SelectedIndexColumn4Property); }
      set { SetValue(SelectedIndexColumn4Property, value); }
    }

    public static readonly DependencyProperty SelectedIndexColumn4Property =
      DependencyProperty.Register("SelectedIndexColumn4", typeof(int), typeof(ViewExceptionSchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexColumn4Changed)));

    private static void OnSelectedIndexColumn4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewExceptionSchedule me = (ViewExceptionSchedule)d;

      if (me.SelectedIndexColumn4 != -1)
      {
        me.setSelectedColumn(3);
      }
    }

    public int SelectedIndexColumn5
    {
      get { return (int)GetValue(SelectedIndexColumn5Property); }
      set { SetValue(SelectedIndexColumn5Property, value); }
    }

    public static readonly DependencyProperty SelectedIndexColumn5Property =
      DependencyProperty.Register("SelectedIndexColumn5", typeof(int), typeof(ViewExceptionSchedule),
        new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexColumn5Changed)));

    private static void OnSelectedIndexColumn5Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewExceptionSchedule me = (ViewExceptionSchedule)d;

      if (me.SelectedIndexColumn5 != -1)
      {
        me.setSelectedColumn(4);
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
      if (!(DataContext is ExceptionSchedule))
      {
        return;
      }

      try
      {
        ExceptionSchedule exceptionSchedule = (ExceptionSchedule)DataContext;
        DailySchedulesGrid.ItemsSource = new IBACnetScheduleSet[] { exceptionSchedule.ScheduleSet };

        if (exceptionSchedule.ScheduleSet == null)
        {
          return;
        }

        ExceptionScheduleSet exceptionScheduleSet = (ExceptionScheduleSet)exceptionSchedule.ScheduleSet;
        DailySchedule1.ItemsSource = exceptionScheduleSet.ScheduleItemPairs[0];
        DailySchedule2.ItemsSource = exceptionScheduleSet.ScheduleItemPairs[1];
        DailySchedule3.ItemsSource = exceptionScheduleSet.ScheduleItemPairs[2];
        DailySchedule4.ItemsSource = exceptionScheduleSet.ScheduleItemPairs[3];
        DailySchedule5.ItemsSource = exceptionScheduleSet.ScheduleItemPairs[4];
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
          ExceptionIndex.Text = string.Empty;
          DailyScheduleListGrid.ItemsSource = null;

          if (DailySchedulesGrid.SelectedCells.Count == 5)
          {
            resetSelectedIndex(DailySchedulesGrid.CurrentCell.Column.DisplayIndex);
          }

          return;
        }

        DataGridColumn column = DailySchedulesGrid.SelectedCells[0].Column;
        string currentColumnNumber = column.Header.ToString();

        if (currentColumnNumber.Equals(ExceptionIndex.Text))
        {
          return;
        }

        ExceptionIndex.Text = currentColumnNumber;
        ExceptionSchedule exceptionSchedule = (ExceptionSchedule)DataContext;
        DailyScheduleListGrid.ItemsSource = exceptionSchedule.ScheduleSet.ScheduleItemPairs[column.DisplayIndex];
        ScheduleCalendarEntry.DataContext = exceptionSchedule.ExceptionScheduleItems[column.DisplayIndex].Calendar;
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
          SelectedIndexColumn1 = -1;
          break;
        case 1:
          SelectedIndexColumn2 = -1;
          break;
        case 2:
          SelectedIndexColumn3 = -1;
          break;
        case 3:
          SelectedIndexColumn4 = -1;
          break;
        case 4:
          SelectedIndexColumn5 = -1;
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
          selectedIndexBinding = new Binding("SelectedIndexColumn1");
          break;
        case 1:
          selectedIndexBinding = new Binding("SelectedIndexColumn2");
          break;
        case 2:
          selectedIndexBinding = new Binding("SelectedIndexColumn3");
          break;
        case 3:
          selectedIndexBinding = new Binding("SelectedIndexColumn4");
          break;
        case 4:
          selectedIndexBinding = new Binding("SelectedIndexColumn5");
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

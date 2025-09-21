using iCos5.BACnet.MVVM;
using System;
using System.Collections.ObjectModel;

namespace iCos5.BACnet.Zenon
{
  public class DailyScheduleItem : NotifyBase
  {
    private int _selectedIndex = -1;
    public int SelectedIndex
    { 
      get
      {
        return _selectedIndex;
      }
      set
      {
        _selectedIndex = value;
        OnPropertyChanged(nameof(SelectedIndex));
        OnSelectedIndexEvent();
      }
    }

    private ObservableCollection<BooleanScheduleItemPair> _scheduleItemPairs;
    public ObservableCollection<BooleanScheduleItemPair> ScheduleItemPairs
    {
      get
      {
        return _scheduleItemPairs;
      }
      set
      {
        _scheduleItemPairs = value;
        OnPropertyChanged(nameof(ScheduleItemPairs));
      }
    }

    public event EventHandler SelectedIndexEvent;

    public void OnSelectedIndexEvent()
    {
      SelectedIndexEvent?.Invoke(this, EventArgs.Empty);
    }

    public DailyScheduleItem(ObservableCollection<BooleanScheduleItemPair> booleanScheduleItemPairs)
    {
      ScheduleItemPairs = booleanScheduleItemPairs;
    }
  }
}

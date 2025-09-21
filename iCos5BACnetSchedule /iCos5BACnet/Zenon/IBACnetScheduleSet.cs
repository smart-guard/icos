using System.Collections.ObjectModel;

namespace iCos5.BACnet.Zenon
{
  public interface IBACnetScheduleSet
  {
    ObservableCollection<BooleanScheduleItemPair>[] ScheduleItemPairs { get; set; }
  }
}
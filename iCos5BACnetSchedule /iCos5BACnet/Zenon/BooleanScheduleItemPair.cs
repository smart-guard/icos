using iCos5.BACnet.MVVM;
using System;
using System.Collections.Generic;

namespace iCos5.BACnet.Zenon
{
  public class BooleanScheduleItemPair : NotifyBase
  {
    private ScheduleItem _startItem;
    public ScheduleItem StartItem 
    { 
      get
      {
        return _startItem;
      }
      set
      {
        _startItem = value;
        OnPropertyChanged(nameof(StartItem));
      }
    }

    private ScheduleItem _stopItem;
    public ScheduleItem StopItem 
    { 
      get
      {
        return _stopItem;
      }
      set
      {
        _stopItem = value;
        OnPropertyChanged(nameof(StopItem));
      }
    }

    public event EventHandler IsSelectedEvent;

    public int Index { get; set; } = -1;

    public void OnIsSelectedEvent()
    {
      IsSelectedEvent?.Invoke(this, EventArgs.Empty);
    }

    public BooleanScheduleItemPair(DateTime starttime, DateTime stoptime)
    {
      StartItem = new ScheduleItem(Convert.ToDateTime(starttime), BACnetTagType.Enum, 0x01);
      StopItem = new ScheduleItem(Convert.ToDateTime(stoptime), BACnetTagType.Enum, 0x00);
    }

    public BooleanScheduleItemPair(ScheduleItem startItem, ScheduleItem stopItem)
    {
      StartItem = startItem;
      StopItem = stopItem;
    }

    public BooleanScheduleItemPair(ScheduleItem scheduleItem, bool value)
    {
      if (value)
      {
        StartItem = scheduleItem;
        StopItem = new ScheduleItem(ScheduleItem.NullDateTime, BACnetTagType.Enum, 0x00);
      }
      else
      {
        StartItem = new ScheduleItem(ScheduleItem.NullDateTime, BACnetTagType.Enum, 0x01);
        StopItem = scheduleItem;
      }
    }

    public void SetStartTime(DateTime dateTime)
    {
      StartItem.Time = dateTime;
    }

    public void SetStopTime(DateTime dateTime)
    {
      StopItem.Time = dateTime;
    }

    public bool IsValidStartTime()
    {
      return StartItem.Time.Subtract(ScheduleItem.NullDateTime).TotalSeconds > 0;
    }

    public bool IsValidStopTime()
    {
      return StopItem.Time.Subtract(ScheduleItem.NullDateTime).TotalSeconds > 0;
    }

    public List<BACnetTagNode> GetBACnetTagNodes()
    {
      List<BACnetTagNode> tagNodes = new List<BACnetTagNode>();

      if (IsValidStartTime())
      {
        tagNodes.AddRange(StartItem.GetBACnetTagNodes());
      }

      if (IsValidStopTime())
      {
        tagNodes.AddRange(StopItem.GetBACnetTagNodes());
      }

      return tagNodes;
    }
  }
}

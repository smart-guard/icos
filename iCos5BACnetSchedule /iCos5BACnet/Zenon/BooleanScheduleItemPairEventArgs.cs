using System;

namespace iCos5.BACnet.Zenon
{
  public class BooleanScheduleItemPairEventArgs : EventArgs
  {
    public BooleanScheduleItemPair ScheduleItemPair { get; set; }

    public BooleanScheduleItemPairEventArgs(BooleanScheduleItemPair booleanScheduleItemPair)
    {
      ScheduleItemPair = booleanScheduleItemPair;
    }
  }
}

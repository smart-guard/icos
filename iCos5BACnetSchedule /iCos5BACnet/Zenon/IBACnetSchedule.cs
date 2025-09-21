namespace iCos5.BACnet.Zenon
{
  public interface IBACnetSchedule
  {
    IBACnetScheduleSet ScheduleSet { get; }

    byte[] GetBytes();
    string GetCode();
    void ClearScheduleSet();
  }
}
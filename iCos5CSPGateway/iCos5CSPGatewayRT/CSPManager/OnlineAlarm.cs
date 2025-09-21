using Scada.AddIn.Contracts.AlarmMessageList;
using System;

namespace iCos5CSPGatewayRT.Manager
{
  public enum OnlineAlarmReasonType
  {
    Received = 0,
    Cleared = 1,
  }

  public class OnlineAlarm
  {
    public OnlineAlarmReasonType ReasonType { get; }
    public string VariableName { get; }
    public object Value { get; }
    public DateTime ReceivedTime { get; }
    public DateTime ClearedTime { get; }
    public int Status { get; }
    public string LimitText { get; }

    public OnlineAlarm(IAlarmEntry alarmEntry, OnlineAlarmReasonType reasonType)
    {
      ReasonType = reasonType;
      VariableName = alarmEntry.VariableName;
      Value = alarmEntry.Value;
      ReceivedTime = alarmEntry.ReceivedTime;
      ClearedTime = alarmEntry.ClearedTime;
      Status = alarmEntry.State;
      LimitText = alarmEntry.Text;
    }
  }
}

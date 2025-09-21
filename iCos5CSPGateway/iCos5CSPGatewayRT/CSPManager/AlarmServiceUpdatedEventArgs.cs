using System;

namespace iCos5CSPGatewayRT.Manager
{
  public class AlarmServiceUpdatedEventArgs : EventArgs
  {
    public OnlineAlarm[] OnlineAlarms { get; }

    public AlarmServiceUpdatedEventArgs(OnlineAlarm[] onlineAlarms)
    {
      OnlineAlarms = onlineAlarms;
    }
  }
}

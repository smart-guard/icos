using System;

namespace iCos5CSPGatewayRT.Manager
{
  public class OnlineValue
  {
    public object Value { get; set; } = 0;
    public DateTime LastUpdateTime { get; set; }
    public ulong StatusValue { get; set; } = 0;
  }
}

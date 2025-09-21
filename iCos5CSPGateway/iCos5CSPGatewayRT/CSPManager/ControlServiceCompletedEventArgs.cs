using iCos5.CSPGateway.CSPMessage;
using iCos5.CSPGateway.DB;
using System;

namespace iCos5CSPGatewayRT.Manager
{
  public class ControlServiceCompletedEventArgs : EventArgs
  {
    public ControlResponseMessage Message { get; } = null;
    public ControlResponseScheme Scheme { get; } = null;

    public ControlServiceCompletedEventArgs(ControlResponseMessage message)
    {
      Message = message;
    }

    public ControlServiceCompletedEventArgs(ControlResponseScheme scheme)
    {
      Scheme = scheme;
    }
  }
}

using System;

namespace iCos5CSPGatewayRT.Manager
{
  public class ControlBooleanEventArgs : EventArgs
  {
    public bool Value { get; }
    public string Message { get; } = "";

    public ControlBooleanEventArgs(bool value)
    {
      Value = value;
    }

    public ControlBooleanEventArgs(bool value, string message)
    {
      Value = value;
      Message = message;
    }
  }

  public class ControlListItemEventArgs : EventArgs
  {
    public ControlListItem Value { get; }
    public string Message { get; } = "";

    public ControlListItemEventArgs(ControlListItem value)
    {
      Value = value;
    }

    public ControlListItemEventArgs(ControlListItem value, string message)
    {
      Value = value;
      Message = message;
    }
  }

  public class ControlIntegerEventArgs : EventArgs
  {
    public int Value { get; }
    public string Message { get; } = "";

    public ControlIntegerEventArgs(int value)
    {
      Value = value;
    }

    public ControlIntegerEventArgs(int value, string message)
    {
      Value = value;
      Message = message;
    }
  }
}

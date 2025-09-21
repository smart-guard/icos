using iCos5.BACnet.MVVM;

namespace iCos5.BACnet.Zenon
{
  public class ScheduleVariable : NotifyBase
  {
    private uint _instanceID = 0;
    public uint InstanceID
    {
      get
      {
        return _instanceID;
      }
      set
      {
        _instanceID = value;
        OnPropertyChanged(nameof(InstanceID));
      }
    }

    private string _weekly = "";
    public string Weekly
    {
      get
      {
        return _weekly;
      }
      set
      {
        _weekly = value;
        OnPropertyChanged(nameof(Weekly));
      }
    }

    private string _exception = "";
    public string Exception
    {
      get
      {
        return _exception;
      }
      set
      {
        _exception = value;
        OnPropertyChanged(nameof(Exception));
      }
    }

    private string _calendar = "";
    public string Calendar
    {
      get
      {
        return _calendar;
      }
      set
      {
        _calendar = value;
        OnPropertyChanged(nameof(Calendar));
      }
    }

    private string _functionName = "";
    public string FunctionName
    {
      get
      {
        return _functionName;
      }
      set
      {
        _functionName = value;
        OnPropertyChanged(nameof(FunctionName));
      }
    }

    private string _targetObjectName = "";
    public string TargetObjectName
    {
      get
      {
        return _targetObjectName;
      }
      set
      {
        _targetObjectName = value;
        OnPropertyChanged(nameof(TargetObjectName));
      }
    }
  }
}

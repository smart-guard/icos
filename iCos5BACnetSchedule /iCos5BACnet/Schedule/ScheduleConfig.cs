using iCos5.BACnet.MVVM;
using iCos5.BACnet.Zenon;
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;

namespace iCos5.BACnet.Schedule
{
  public class ScheduleConfig : NotifyBase
  {
    public static class Constants
    {
      public static readonly string RootName = "HDCLabs";
      public static readonly string SolutionName = "BACnetSchedule";
      public static readonly string SolutionVersion = "1.0";
      public static readonly string DialogueParameterVariable = "BACnetScheduleDialogParam";
      public static readonly string DialogueParameterOnlineContainer = "BACnetDialogueParameterOnlineContainer";
      public static readonly string WeeklyScheduleOnlineContainer = "BACnetWeeklyScheduleOnlineContainer";
      public static readonly string ExceptionScheduleOnlineContainer = "BACnetExceptionScheduleOnlineContainer";
      public static readonly string CalendarOnlineContainer = "BACnetCalendarOnlineContainer";
    }

    private bool _isActiveExceptionTab = true;
    public bool IsActiveExceptionTab
    {
      get
      {
        return _isActiveExceptionTab;
      }
      set
      {
        _isActiveExceptionTab = value;
        OnPropertyChanged(nameof(IsActiveExceptionTab));
      }
    }

    private bool _isActiveCalendarTab = false;
    public bool IsActiveCalendarTab
    {
      get
      {
        return _isActiveCalendarTab;
      }
      set
      {
        _isActiveCalendarTab = value;
        OnPropertyChanged(nameof(IsActiveCalendarTab));
      }
    }

    private int _defaultExceptionPriority = 16;
    public int DefaultExceptionPriority
    {
      get
      {
        return _defaultExceptionPriority;
      }
      set
      {
        _defaultExceptionPriority = value;
        OnPropertyChanged(nameof(DefaultExceptionPriority));
      }
    }

    private int _schedulePairLimitInterval = 10;
    public int SchedulePairLimitInterval
    {
      get
      {
        return _schedulePairLimitInterval;
      }
      set
      {
        _schedulePairLimitInterval = value;
        OnPropertyChanged(nameof(SchedulePairLimitInterval));
      }
    }

    private bool _enableCEL = false;

    public bool EnableCEL
    {
      get { return _enableCEL; }
      set
      {
        _enableCEL = value;
        OnPropertyChanged("EnableCEL");
      }
    }

    private int _celClassPvID = -1;

    public int CELClassPvID
    {
      get { return _celClassPvID; }
      set
      {
        _celClassPvID = value;
        OnPropertyChanged("CELClassPvID");
      }
    }

    private string _celGroupName = "None";

    public string CELGroupName
    {
      get { return _celGroupName; }
      set
      {
        _celGroupName = value;
        OnPropertyChanged("CELGroupName");
      }
    }

    private int _celGroupPvID = -1;

    public int CELGroupPvID
    {
      get { return _celGroupPvID; }
      set
      {
        _celGroupPvID = value;
        OnPropertyChanged("CELGroupPvID");
      }
    }

    public ObservableCollection<BACnetScheduleDriver> BACnetDriverDevices { get; set; } = new ObservableCollection<BACnetScheduleDriver>();

    [ScriptIgnore]
    public ObservableCollection<string> CELGroups { get; set; } = new ObservableCollection<string>();

    public void Reset()
    {
      IsActiveExceptionTab = true;
      IsActiveCalendarTab = false;
      DefaultExceptionPriority = 13;
      SchedulePairLimitInterval = 10;
      EnableCEL = false;
      CELClassPvID = -1;
      CELGroupName = "None";
      CELGroupPvID = -1;
      BACnetDriverDevices.Clear();
    }

    public ScheduleVariable GetScheduleVariable(DialogueParameter dialogueParameter)
    {
      foreach (BACnetScheduleDriver scheduleDrv in BACnetDriverDevices)
      {
        if (scheduleDrv.DriverIdentification.Equals(dialogueParameter.DriverIdentification))
        {
          foreach (BACnetScheduleDevice scheduleDev in scheduleDrv.BACnetDevices)
          {
            if (scheduleDev.DeviceName.Equals(dialogueParameter.DeviceName))
            {
              foreach (ScheduleVariable scheduleVar in scheduleDev.ScheduleVariables)
              {
                if (scheduleVar.InstanceID == dialogueParameter.ScheduleID)
                {
                  return scheduleVar;
                }
              }
            }
          }
        }
      }

      return null;
    }
  }
}

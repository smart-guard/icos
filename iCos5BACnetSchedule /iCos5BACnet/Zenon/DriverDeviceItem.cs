using iCos5.BACnet.MVVM;
using System.Collections.ObjectModel;

namespace iCos5.BACnet.Zenon
{
  public class DriverDeviceItem : NotifyBase
  {
    private string _driverName;
    public string DriverName
    {
      get
      {
        return _driverName;
      }
      set
      {
        _driverName = value;
        OnPropertyChanged(nameof(DriverName));
      }
    }

    private string _deviceName = "Device_LAB_1";
    public string DeviceName
    {
      get
      {
        return _deviceName;
      }
      set
      {
        _deviceName = value;
        OnPropertyChanged(nameof(DeviceName));
      }
    }

    public ObservableCollection<ScheduleVariable> BACnetScheduleVariables { get; set; } = new ObservableCollection<ScheduleVariable>()
    {
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 1.weekly-schedule", Exception = "Device_LAB_1.Schedule 1.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 2.weekly-schedule", Exception = "Device_LAB_1.Schedule 2.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 3.weekly-schedule", Exception = "Device_LAB_1.Schedule 3.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 4.weekly-schedule", Exception = "Device_LAB_1.Schedule 4.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 5.weekly-schedule", Exception = "Device_LAB_1.Schedule 5.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 6.weekly-schedule", Exception = "Device_LAB_1.Schedule 6.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 7.weekly-schedule", Exception = "Device_LAB_1.Schedule 7.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 8.weekly-schedule", Exception = "Device_LAB_1.Schedule 8.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 9.weekly-schedule", Exception = "Device_LAB_1.Schedule 9.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 10.weekly-schedule", Exception = "Device_LAB_1.Schedule 10.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 11.weekly-schedule", Exception = "Device_LAB_1.Schedule 11.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 12.weekly-schedule", Exception = "Device_LAB_1.Schedule 12.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 13.weekly-schedule", Exception = "Device_LAB_1.Schedule 13.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 14.weekly-schedule", Exception = "Device_LAB_1.Schedule 14.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 15.weekly-schedule", Exception = "Device_LAB_1.Schedule 15.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 16.weekly-schedule", Exception = "Device_LAB_1.Schedule 16.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 17.weekly-schedule", Exception = "Device_LAB_1.Schedule 17.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 18.weekly-schedule", Exception = "Device_LAB_1.Schedule 18.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 19.weekly-schedule", Exception = "Device_LAB_1.Schedule 19.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 20.weekly-schedule", Exception = "Device_LAB_1.Schedule 20.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 21.weekly-schedule", Exception = "Device_LAB_1.Schedule 21.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 22.weekly-schedule", Exception = "Device_LAB_1.Schedule 22.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 23.weekly-schedule", Exception = "Device_LAB_1.Schedule 23.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 24.weekly-schedule", Exception = "Device_LAB_1.Schedule 24.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 25.weekly-schedule", Exception = "Device_LAB_1.Schedule 25.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 26.weekly-schedule", Exception = "Device_LAB_1.Schedule 26.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 27.weekly-schedule", Exception = "Device_LAB_1.Schedule 27.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 28.weekly-schedule", Exception = "Device_LAB_1.Schedule 28.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 29.weekly-schedule", Exception = "Device_LAB_1.Schedule 29.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 30.weekly-schedule", Exception = "Device_LAB_1.Schedule 30.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 31.weekly-schedule", Exception = "Device_LAB_1.Schedule 31.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
      new ScheduleVariable() { Weekly = "Device_LAB_1.Schedule 32.weekly-schedule", Exception = "Device_LAB_1.Schedule 32.exception-schedule", Calendar = "Device_LAB_1.Calendar 1.date-list" },
    };

    public DriverDeviceItem(string driverName, string deviceName)
    {
      DriverName = driverName;
      DeviceName = deviceName;
    }
  }
}

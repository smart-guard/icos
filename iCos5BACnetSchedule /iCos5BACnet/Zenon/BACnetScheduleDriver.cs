using System.Collections.ObjectModel;
using System.Linq;

namespace iCos5.BACnet.Zenon
{
  public class BACnetScheduleDriver
  {
    public string DriverIdentification { get; set; } = "";
    public string ConnectionName { get; set; } = "";
    public string DeviceServer1Name { get; set; } = "";
    public int DeviceServer1ID { get; set; } = 0;
    public string DeviceServer2Name { get; set; } = "";
    public int DeviceServer2ID { get; set; } = 0;

    public ObservableCollection<BACnetScheduleDevice> BACnetDevices { get; set; } = new ObservableCollection<BACnetScheduleDevice>();

    public BACnetScheduleDevice GetBACnetDevice(string deviceName)
    {
      foreach (BACnetScheduleDevice device in BACnetDevices)
      {
        if (device.DeviceName.Equals(deviceName))
        {
          return device;
        }
      }

      return null;
    }

    public BACnetScheduleDevice GetBACnetDevice(uint netAddr)
    {
      foreach (BACnetScheduleDevice device in BACnetDevices)
      {
        if (device.ProcessId == netAddr)
        {
          return device;
        }
      }

      return null;
    }

    public void Sort()
    {
      BACnetDevices = new ObservableCollection<BACnetScheduleDevice>(BACnetDevices.OrderBy(x => x.ProcessId));

      foreach (BACnetScheduleDevice scheduleDevice in BACnetDevices)
      {
        scheduleDevice.Sort();
      }
    }
  }
}

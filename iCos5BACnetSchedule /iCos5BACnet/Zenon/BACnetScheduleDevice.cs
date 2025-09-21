using System.Collections.ObjectModel;
using System.Linq;

namespace iCos5.BACnet.Zenon
{
  public class BACnetScheduleDevice
  {
    public string DeviceName { get; set; } = "";
    public string DeviceAddress { get; set; } = "";
    public uint ProcessId { get; set; } = 0;

    public ObservableCollection<ScheduleVariable> ScheduleVariables { get; set; } = new ObservableCollection<ScheduleVariable>();

    public ScheduleVariable GetScheduleVariable(uint instanceID, string objectName)
    {
      foreach (ScheduleVariable scheduleVariable in ScheduleVariables)
      {
        if (scheduleVariable.InstanceID == instanceID)
        {
          return scheduleVariable;
        }
      }

      ScheduleVariable newNode = new ScheduleVariable();
      newNode.InstanceID = instanceID;
      newNode.FunctionName = $"BACnetSchedule-{objectName}";
      newNode.TargetObjectName = objectName;
      ScheduleVariables.Add(newNode);
      return newNode;
    }

    public void Sort()
    {
      ScheduleVariables = new ObservableCollection<ScheduleVariable>(ScheduleVariables.OrderBy(x => x.InstanceID));
    }
  }
}

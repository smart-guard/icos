namespace iCos5.BACnet.Schedule
{
  public class DialogueParameter
  {
    public string DriverIdentification { get; set; }
    public string DeviceName { get; set; }
    public string ObjectName { get; set; }
    public int ScheduleID { get; set; }

    public DialogueParameter(string param)
    {
      string[] strParams = param.Split('/');

      if (strParams.Length != 4)
      {
        return;
      }

      int index;

      if (int.TryParse(strParams[3], out index))
      {
        DriverIdentification = strParams[0];
        DeviceName = strParams[1];
        ObjectName = strParams[2];
        ScheduleID = index;
      }
      else
      {
        return;
      }
    }

    public override string ToString()
    {
      return $"{DriverIdentification}/{DeviceName}/{ObjectName}/{ScheduleID}";
    }
  }
}

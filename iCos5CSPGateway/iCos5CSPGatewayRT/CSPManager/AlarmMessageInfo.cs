using iCos5.CSPGateway.CSPMessage;
using System.Collections.Generic;

namespace iCos5CSPGatewayRT.Manager
{
  public class AlarmMessageInfo
  {
    public int BuildingID { get; set; } = 0;
    public string ValueType { get; set; } = "";
    public string TagName { get; set; } = "";
    public List<double> MinimumLimits { get; set; } = new List<double>();
    public List<double> MaximumLimits { get; set; } = new List<double>();

    public AlarmMessage GetCSPMessage()
    {
      return new AlarmMessage()
      {
        bd = BuildingID,
        ty = ValueType,
        nm = TagName,
        mi = MinimumLimits,
        mx = MaximumLimits
      };
    }

    public void ApplyLimitValue(ref AlarmMessage alarmMessage)
    {
      if (MaximumLimits.Count > 0 && alarmMessage.vl >= MaximumLimits[0])
      {
        for (int i = MaximumLimits.Count - 1; i > 0; i--)
        {
          if (alarmMessage.vl >= MaximumLimits[i])
          {
            alarmMessage.xl = MaximumLimits[i].ToString();
            alarmMessage.il = "-";
            return;
          }
        }

        alarmMessage.xl = MaximumLimits[0].ToString();
        alarmMessage.il = "-";
      }
      else if (MinimumLimits.Count > 0 && alarmMessage.vl <= MinimumLimits[MinimumLimits.Count - 1])
      {
        for (int i = 0; i < MinimumLimits.Count - 1; i++)
        {
          if (alarmMessage.vl <= MinimumLimits[i])
          {
            alarmMessage.xl = "-";
            alarmMessage.il = MinimumLimits[i].ToString();
            return;
          }
        }

        alarmMessage.xl = "-";
        alarmMessage.il = MinimumLimits[MinimumLimits.Count - 1].ToString();
      }
      else
      {
        alarmMessage.xl = "-";
        alarmMessage.il = "-";
      }
    }
  }
}

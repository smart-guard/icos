using System;
using System.Web.Script.Serialization;

namespace iCos5.CSPGateway.CSPMessage
{
  public class ControlMessage
  {
    /// <summary>
    /// Building ID
    /// 8 length integer
    /// </summary>
    public int bd { get; set; } = 0;

    public ControlMessage(int buildingID)
    {
      bd = buildingID;
    }
  }

  public class ControlRequestMessage
  {
    /// <summary>
    /// Sequnce Number
    /// 8 length integer
    /// </summary>
    public int seq { get; set; } = 0;

    /// <summary>
    /// Building ID
    /// 8 length integer
    /// </summary>
    public int bd { get; set; } = 0;

    /// <summary>
    /// Tag Name
    /// [AHU_#].[SF_#].[P_#]![S].[M]
    /// </summary>
    public string nm { get; set; } = "";

    /// <summary>
    /// Value
    /// casting to string
    /// </summary>
    public string vl { get; set; } = "";

    /// <summary>
    /// Control Request Datetime
    /// Datetime Format : yyyy-MM-dd HH:mm:ss.fff
    /// </summary>
    public string tm { get; set; } = "1900-01-01 00:00:00.000";

    /// <summary>
    /// Description of request's cause
    /// </summary>
    public string des { get; set; } = "";

    [ScriptIgnore]
    public double Value 
    { 
      get 
      {
        try
        {
          return Convert.ToDouble(vl);
        }
        catch
        {
          return 0;
        }
      }
    }

    [ScriptIgnore]
    public decimal DecimalValue
    {
      get
      {
        try
        {
          return Convert.ToDecimal(vl);
        }
        catch
        {
          return 0;
        }
      }
    }
  }
}

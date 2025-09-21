using System.Collections.Generic;

namespace iCos5.CSPGateway.CSPMessage
{
  public class AlarmMessage
  {
    /// <summary>
    /// Building ID
    /// 8 length integer
    /// </summary>
    public int bd { get; set; } = 0;

    /// <summary>
    /// Tag Type 
    /// bln:boolean, num:numeric, str:string
    /// </summary>
    public string ty { get; set; } = "num";

    /// <summary>
    /// Tag Name
    /// [AHU_#].[SF_#].[P_#]![S].[M]
    /// </summary>
    public string nm { get; set; } = "";

    /// <summary>
    /// Value
    /// casting to double
    /// </summary>
    public double vl { get; set; } = 0;

    /// <summary>
    /// Limit Value if Minimum limit event
    /// </summary>
    public string il { get; set; } = "";

    /// <summary>
    /// Limit Value if Maximum limit event
    /// </summary>
    public string xl { get; set; } = "";

    /// <summary>
    /// Minimum Limit List
    /// </summary>
    public List<double> mi { get; set; } = new List<double>();

    /// <summary>
    /// Maximum Limit List
    /// </summary>
    public List<double> mx { get; set; } = new List<double>();

    /// <summary>
    /// Alarm Generation Datetime
    /// Datetime Format : yyyy-MM-dd HH:mm:ss.fff
    /// </summary>
    public string tm { get; set; } = "1900-01-01 00:00:00.000";

    /// <summary>
    /// Communication Status
    /// 0:abnormal, 1:normal
    /// </summary>
    public int st { get; set; } = 1;

    /// <summary>
    /// Alarm Status
    /// 0:clear, 1:occur
    /// </summary>
    public int al { get; set; } = 1;

    /// <summary>
    /// Description encoded in UTF-8 due to competition in Korea
    /// </summary>
    public string des { get; set; } = "";
  }
}

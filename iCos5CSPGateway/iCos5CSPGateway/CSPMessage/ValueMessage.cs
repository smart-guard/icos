namespace iCos5.CSPGateway.CSPMessage
{
  public class ValueMessage
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
    /// casting to string
    /// </summary>
    public string vl { get; set; } = "";

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
  }
}

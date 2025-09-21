using System;

namespace iCos5.CSPGateway.DB
{
  public class ControlRequestScheme
  {
    /// <summary>
    /// Sequence Number 
    /// </summary>
    public string seq { get; set; } = string.Empty;

    /// <summary>
    /// Building ID
    /// </summary>
    public string bldg_id { get; set; } = string.Empty;

    /// <summary>
    /// Tag Name
    /// </summary>
    public string itfc_id { get; set; } = string.Empty;

    /// <summary>
    /// Parameter ID(Key)
    /// </summary>
    public string ctrl_prmt_id { get; set; } = string.Empty;

    /// <summary>
    /// Description
    /// </summary>
    public string ctrl_prmt_nm { get; set; } = string.Empty;

    /// <summary>
    /// Value
    /// </summary>
    public string ctrl_val { get; set; } = string.Empty;

    /// <summary>
    /// Active DateTime(Key)
    /// </summary>
    public string act_tm { get; set; } = string.Empty;

    /// <summary>
    /// Request DateTime
    /// </summary>
    public DateTime RequestDateTime { get; set; }

    public int SequenceNumber
    {
      get
      {
        try
        {
          return Convert.ToInt32(seq);
        }
        catch
        {
          return 0;
        }
      }
    }

    public double Value
    {
      get
      {
        try
        {
          return Convert.ToDouble(ctrl_val);
        }
        catch
        {
          return 0;
        }
      }
    }

    public decimal DecimalValue
    {
      get
      {
        try
        {
          return Convert.ToDecimal(ctrl_val);
        }
        catch
        {
          return 0;
        }
      }
    }

    public ControlRequestScheme()
    {
      RequestDateTime = DateTime.Now;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace iCos5.CSPGateway.CSPMessage
{
  public enum StatusCode
  {
    OK = 200,
    SKIP = 100,
    INVALID_BUILDING_ID = 101,
    INVALID_TAG_ID = 102,
    INVALID_SEQ_NUMBER = 103,
    INVALID_MESSAGE = 104,
  }

  public class ReturnBody
  {
    public string result { get; set; }
  }

  public class CtrlReturnBody : ReturnBody
  {
    public List<ControlRequestMessage> data { get; set; }
  }

  public class ReturnMessage
  {
    /// <summary>
    /// Result code of call REST API
    /// 8 length integer, 200 is ok, else is fail
    /// </summary>
    public int statusCode { get; set; }

    /// <summary>
    /// Result body message of call normal REST API
    /// ReturnBody including result
    /// </summary>
    public ReturnBody body { get; set; }

    [ScriptIgnore]
    public string Status 
    { 
      get
      {
        try
        {
          return Enum.GetName(typeof(StatusCode), statusCode);
        }
        catch
        {
          return "Void";
        }
      }
    }
  }

  public class ControlReturnMessage
  {
    /// <summary>
    /// Result code of call REST API
    /// 8 length integer
    /// </summary>
    public int statusCode { get; set; }

    /// <summary>
    /// Result body message of call Control Request REST API
    /// CtrlReturnBody including result and request message
    /// </summary>
    public CtrlReturnBody body { get; set; }
  }
}

using iCos5.CSPGateway.CSPMessage;

namespace iCos5CSPGatewayRT.Manager
{
  public class ValueMessageInfo
  {
    public int BuildingID { get; set; } = 0;
    public string ValueType { get; set; } = "";
    public string TagName { get; set; } = "";

    public ValueMessage GetCSPMessage()
    {
      return new ValueMessage()
      {
        bd = BuildingID,
        ty = ValueType,
        nm = TagName,
      };
    }
  }
}

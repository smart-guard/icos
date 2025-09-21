using iCos5.CSPGateway.CSPMessage;

namespace iCos5CSPGatewayRT.Manager
{
  public class ValueMessageInfoStringID
  {
    public string BuildingID { get; set; } = string.Empty;
    public string ValueType { get; set; } = "";
    public string TagName { get; set; } = "";

    public ValueMessageStringID GetCSPMessage()
    {
      return new ValueMessageStringID()
      {
        bd = BuildingID,
        ty = ValueType,
        nm = TagName,
      };
    }
  }
}

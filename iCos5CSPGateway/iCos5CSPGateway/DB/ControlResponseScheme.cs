using iCos5.CSPGateway.CSPMessage;

namespace iCos5.CSPGateway.DB
{
  public class ControlResponseScheme
  {
    private static readonly string _codeOk = "SUCCESS_OK";                    // 제어 성공
    private static readonly string _codeReject = "SUCCESS_REJECT";            // 운영자 거절
    private static readonly string _codeMissing = "SUCCESS_MISSING";          // 운영자 부재중
    private static readonly string _codeDeactivation = "FAIL_DEACTIVATION";   // 제어 사용 안함
    private static readonly string _codeTagError = "FAIL_TAG_ERROR";          // 관제점 없음
    private static readonly string _codeDevError = "FAIL_DEVICE_COM_ERROR";   // 장치 통신 불량
    private static readonly string _codeTimeOut = "FAIL_TIME_OUT";            // 장치 타임아웃
    private static readonly string _codeCSPError = "FAIL_CSP_COMM_ERROR";     // 클라우드 통신 불량
    private static readonly string _codeCommonError = "FAIL_COMMON_ERROR";    // 제어 오류

    /// <summary>
    /// Sequnce Number
    /// </summary>
    public int SequnceNumber { get; set; } = 0;

    /// <summary>
    /// Control Operation Datetime
    /// DateTime Format : yyyy-MM-dd HH:mm:ss.fff
    /// </summary>
    public string UpdateTime { get; set; } = "1900-01-01 00:00:00.000";

    /// <summary>
    /// Parameter ID
    /// </summary>
    public string ParameterID { get; set; } = string.Empty;

    /// <summary>
    /// Active Datetime
    /// DateTime Format : yyyy-MM-dd HH:mm:ss.fff
    /// </summary>
    public string ActiveTime { get; set; } = "1900-01-01 00:00:00.000";

    /// <summary>
    /// Result Code
    /// string
    /// </summary>
    public string Message { get; set; } = _codeOk;

    public ControlResponseScheme(ControlRequestScheme requestScheme)
    {
      SequnceNumber = requestScheme.SequenceNumber;
      ParameterID = requestScheme.ctrl_prmt_id;
      ActiveTime = requestScheme.act_tm;
    }

    public ControlResponseScheme(ControlRequestScheme requestScheme, ControlResponseCode controlResponseCode)
    {
      SequnceNumber = requestScheme.SequenceNumber;
      ParameterID = requestScheme.ctrl_prmt_id;
      ActiveTime = requestScheme.act_tm;
      SetStringCode(controlResponseCode);
    }

    public void SetStringCode(ControlResponseCode responseCode)
    {
      switch (responseCode)
      {
        case ControlResponseCode.Ok:
        default:
          Message = _codeOk;
          break;
        case ControlResponseCode.Reject:
          Message = _codeReject;
          break;
        case ControlResponseCode.Missing:
          Message = _codeMissing;
          break;
        case ControlResponseCode.Deactivation:
          Message = _codeDeactivation;
          break;
        case ControlResponseCode.TagError:
          Message = _codeTagError;
          break;
        case ControlResponseCode.DeviceError:
          Message = _codeDevError;
          break;
        case ControlResponseCode.TimeOut:
          Message = _codeTimeOut;
          break;
        case ControlResponseCode.CSPError:
          Message = _codeCSPError;
          break;
        case ControlResponseCode.CommonError:
          Message = _codeCommonError;
          break;
      }
    }
  }
}

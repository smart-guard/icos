using System;

namespace iCos5.CSPGateway.CSPMessage
{
  public enum ControlResponseCode
  {
    Ok,
    Reject,
    Missing,
    Deactivation,
    TagError,
    DeviceError,
    TimeOut,
    CSPError,
    CommonError
  }

  public class ControlResponseMessage
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
    /// 8 length integer
    /// </summary>
    public int seq { get; set; } = 0;

    /// <summary>
    /// Control Operation Datetime
    /// Datetime Format : yyyy-MM-dd HH:mm:ss.fff
    /// </summary>
    public string tm { get; set; } = "1900-01-01 00:00:00.000";

    /// <summary>
    /// Result Code
    /// string
    /// </summary>
    public string code { get; set; } = _codeOk;

    public ControlResponseMessage(int sequenceNumber)
    {
      seq = sequenceNumber;
    }

    public ControlResponseMessage(int sequenceNumber, ControlResponseCode controlResponseCode)
    {
      seq = sequenceNumber;
      SetStringCode(controlResponseCode);
    }

    public void SetDateTimeNow(bool isLocalTime)
    {
      tm = isLocalTime ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") 
                       : DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
    }

    public void SetStringCode(ControlResponseCode responseCode)
    {
      switch (responseCode)
      {
        case ControlResponseCode.Ok:
        default:
          code = _codeOk;
          break;
        case ControlResponseCode.Reject:
          code = _codeReject;
          break;
        case ControlResponseCode.Missing:
          code = _codeMissing;
          break;
        case ControlResponseCode.Deactivation:
          code = _codeDeactivation;
          break;
        case ControlResponseCode.TagError:
          code = _codeTagError;
          break;
        case ControlResponseCode.DeviceError:
          code = _codeDevError;
          break;
        case ControlResponseCode.TimeOut:
          code = _codeTimeOut;
          break;
        case ControlResponseCode.CSPError:
          code = _codeCSPError;
          break;
        case ControlResponseCode.CommonError:
          code = _codeCommonError;
          break;
      }
    }
  }
}

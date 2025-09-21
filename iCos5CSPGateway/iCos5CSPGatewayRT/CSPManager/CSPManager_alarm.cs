using iCos5.CSPGateway.CSPMessage;
using PASoft.Common;
using PASoft.Common.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace iCos5CSPGatewayRT.Manager
{
  public partial class CSPManager : Disposable
  {
    private OnlineAlarmService _onlineAlarmService;
    private Dictionary<string, AlarmMessageInfo> _dicAlarmMessageInfo = new Dictionary<string, AlarmMessageInfo>();
    private string _alarmS3Path = "icos/alarm/";
    private string _alarmDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "iCos5", "alarm");
    private CancellationTokenSource _cancelOnlineUploadAlarm = new CancellationTokenSource();

    private void initCSPAlarm()
    {
      try
      {
        if (_config.ActiveInsite)
        {
          _alarmS3Path = _config.ConnectionInfo.AlarmBucketFolder;
          _alarmDirPath = Path.Combine(_config.TempPath, "alarm");

          if (!Directory.Exists(_alarmDirPath))
          {
            Directory.CreateDirectory(_alarmDirPath);
          }
        }

        _onlineAlarmService = new OnlineAlarmService(_zenonProject,
                                                     onlineAlarm_Updated,
                                                     _config.AlarmRetryInterval * 1000,
                                                     retryAlarmTimer_Elapsed);

        if (_config.IsAlarmRetry)
        {
          logging(logLevel.Info, $" - Cloud Gateway Alarm Initalization : Alarm Variables/{_dicAlarmMessageInfo.Count}, Alarm and File RetryInterval/{_config.AlarmRetryInterval}sec");
        }
        else
        {
          logging(logLevel.Info, $" - Cloud Gateway Alarm Initalization : Alarm Variables/{_dicAlarmMessageInfo.Count}, Only File RetryInterval/{_config.AlarmRetryInterval}sec");
        }
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $" - Cloud Gateway Alarm Initalization : {ex}");
      }
    }

    private int callAPIAlarm(List<AlarmMessage> alarmMessages)
    {
      if (_config.CSPDebugMode)
      {
        return 1;
      }

      int failNumber = 0;

      foreach (AlarmMessage alarmMessage in alarmMessages)
      {
        try
        {
          string result = _serviceAPI.InvokeAlarm(Json.GetString(alarmMessage));
          ReturnMessage returnMessage = (ReturnMessage)Json.LoadString(result, typeof(ReturnMessage));

          if (returnMessage.statusCode == (int)StatusCode.OK)
          {
            string alScript = alarmMessage.al == 0 ? "해제" : "발생";
            logging(logLevel.Info, $"Success API Alarm[Building({alarmMessage.bd})] : {alarmMessage.nm}/기준값({alarmMessage.vl})/{alarmMessage.tm}/{alScript}/{alarmMessage.des}");
          }
          else
          {
            logging(logLevel.Warn, $"Failure API Alarm[Building({alarmMessage.bd})] : {alarmMessage.nm}/기준값({alarmMessage.vl})/{alarmMessage.tm}/statusCode[{returnMessage.Status}({returnMessage.statusCode})]");
            failNumber++;
          }
        }
        catch (WebException ex)
        {
          logging(logLevel.Error, $"Failure API Alarm[Building({alarmMessage.bd})] : {alarmMessage.nm}/{alarmMessage.tm} : {ex}");
          failNumber++;
        }
      }

      return failNumber;
    }

    private bool callS3Alarm(string alarmFilePath, int alarmMessageCount)
    {
      if (_config.CSPDebugMode)
      {
        return false;
      }

      string[] pathNodes = alarmFilePath.Split('\\');
      
      if (pathNodes.Length < 4)
      {
        return false;
      }

      string buildingID = pathNodes[pathNodes.Length - 3];
      string dayFolder = pathNodes[pathNodes.Length - 2];
      string fileName = pathNodes[pathNodes.Length - 1];

      if (_serviceS3.IsConnected)
      {
        int failCount = 0;

        while (failCount < _config.RetransmissionCount)
        {
          if (_serviceS3.UploadFile($"{_alarmS3Path}/{buildingID}/{dayFolder}/{fileName}", alarmFilePath))
          {
            detailLogging(logLevel.Info, $"Success S3 Alarm[Building({buildingID})] : {fileName} Alarms File({alarmMessageCount} Alarms)");
            return true;
          }
          else
          {
            Thread.Sleep(_config.RetransmissionInterval);
            failCount++;
          }
        }

        logging(logLevel.Warn, $"Failure S3 Alarm[Building({buildingID})] : {fileName} Alarms File({alarmMessageCount} Alarms)[Upload Failure]");
        return false;
      }
      else
      {
        logging(logLevel.Warn, $"Failure S3 Alarm[Building({buildingID})] : {alarmMessageCount} Alarms[Disconnected]");
        return false;
      }
    }

    private void onlineAlarm_Updated(object sender, AlarmServiceUpdatedEventArgs e)
    {
      try
      {
        List<OnlineAlarm> onlineAlarms = new List<OnlineAlarm>(e.OnlineAlarms);

        if (_config.ActiveInsite)
        {
          if (_config.IsSingleBuilding)
          {
            Task.Run(() => { taskAlarmSingle(onlineAlarms); }, _cancelOnlineUploadAlarm.Token);
          }
          else
          {
            Task.Run(() => { taskAlarmMultiple(onlineAlarms); }, _cancelOnlineUploadAlarm.Token);
          }
        }
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[onlineAlarm_Updated] : {ex}");
      }
    }

    private void taskAlarmSingle(List<OnlineAlarm> onlineAlarms)
    {
      try
      {
        List<AlarmMessage> alarmMessages = new List<AlarmMessage>();
        DateTime ignoreTime = DateTime.Now.AddMinutes(_config.AlarmIgnoreMinutes * (-1));

        foreach (OnlineAlarm onlineAlarm in onlineAlarms)
        {
          if (_dicAlarmMessageInfo.ContainsKey(onlineAlarm.VariableName))
          {
            AlarmMessage alarmMessage = _dicAlarmMessageInfo[onlineAlarm.VariableName].GetCSPMessage();
            alarmMessage.vl = onlineAlarm.Value is double ? Convert.ToDouble(onlineAlarm.Value) : double.MaxValue;
            _dicAlarmMessageInfo[onlineAlarm.VariableName].ApplyLimitValue(ref alarmMessage);

            switch (onlineAlarm.ReasonType)
            {
              case OnlineAlarmReasonType.Received:
                if (onlineAlarm.ReceivedTime < ignoreTime)
                {
                  continue;
                }

                alarmMessage.tm = _config.IsLocalTime ?
                                  onlineAlarm.ReceivedTime.ToString("yyyy-MM-dd HH:mm:ss.fff") :
                                  onlineAlarm.ReceivedTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
                alarmMessage.al = 1;
                break;
              case OnlineAlarmReasonType.Cleared:
                if (onlineAlarm.ClearedTime < ignoreTime)
                {
                  continue;
                }

                alarmMessage.tm = _config.IsLocalTime ?
                                  onlineAlarm.ClearedTime.ToString("yyyy-MM-dd HH:mm:ss.fff") :
                                  onlineAlarm.ClearedTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
                alarmMessage.al = 0;
                break;
              default:
                continue;
            }

            try
            {
              alarmMessage.st = (onlineAlarm.Status & 0x40000) == 0 ? 1 : 0;
            }
            catch
            {
              alarmMessage.st = 0;
            }

            alarmMessage.des = onlineAlarm.LimitText;
            alarmMessages.Add(alarmMessage);
          }
        }

        if (alarmMessages.Count > 0)
        {
          DateTime dtNow = DateTime.Now;
          string alarmFile = $"{dtNow:yyyyMMddHHmmss}.json";
          string alarmPath = Path.Combine(_alarmDirPath, _config.BuildingID.ToString(), $"{dtNow:yyyyMMdd}", alarmFile);
          Json.SaveFile(alarmMessages, alarmPath);

          int failAPIAlarms = callAPIAlarm(alarmMessages);
          bool isOkS3Alarm = callS3Alarm(alarmPath, alarmMessages.Count);

          if (failAPIAlarms == 0 && isOkS3Alarm)
          {
            File.Delete(alarmPath);
            logging(logLevel.Info, $"Success AlarmMessage Upload[Building({_config.BuildingID})] : {alarmMessages.Count} Alarms");
          }
          else
          {
            logging(logLevel.Warn, $"Failure AlarmMessage Upload[Building({_config.BuildingID})] : [{failAPIAlarms}] API Failure, [{isOkS3Alarm}] S3 result");
          }
        }
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[taskAlarmSingle] : {ex}");
      }
    }

    private void taskAlarmMultiple(List<OnlineAlarm> onlineAlarms)
    {
      try
      {
        Dictionary<int, List<AlarmMessage>> dicAlarmMessages = new Dictionary<int, List<AlarmMessage>>();
        DateTime ignoreTime = DateTime.Now.AddMinutes(_config.AlarmIgnoreMinutes * (-1));

        foreach (OnlineAlarm onlineAlarm in onlineAlarms)
        {
          if (_dicAlarmMessageInfo.ContainsKey(onlineAlarm.VariableName))
          {
            AlarmMessage alarmMessage = _dicAlarmMessageInfo[onlineAlarm.VariableName].GetCSPMessage();
            alarmMessage.vl = onlineAlarm.Value is double ? Convert.ToDouble(onlineAlarm.Value) : double.MaxValue;
            _dicAlarmMessageInfo[onlineAlarm.VariableName].ApplyLimitValue(ref alarmMessage);

            switch (onlineAlarm.ReasonType)
            {
              case OnlineAlarmReasonType.Received:
                if (onlineAlarm.ReceivedTime < ignoreTime)
                {
                  continue;
                }

                alarmMessage.tm = _config.IsLocalTime ?
                                  onlineAlarm.ReceivedTime.ToString("yyyy-MM-dd HH:mm:ss.fff") :
                                  onlineAlarm.ReceivedTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
                alarmMessage.al = 1;
                break;
              case OnlineAlarmReasonType.Cleared:
                if (onlineAlarm.ClearedTime < ignoreTime)
                {
                  continue;
                }

                alarmMessage.tm = _config.IsLocalTime ?
                                  onlineAlarm.ClearedTime.ToString("yyyy-MM-dd HH:mm:ss.fff") :
                                  onlineAlarm.ClearedTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
                alarmMessage.al = 0;
                break;
              default:
                continue;
            }

            try
            {
              alarmMessage.st = (onlineAlarm.Status & 0x40000) == 0 ? 1 : 0;
            }
            catch
            {
              alarmMessage.st = 0;
            }

            alarmMessage.des = onlineAlarm.LimitText;

            if (!dicAlarmMessages.ContainsKey(alarmMessage.bd))
            {
              dicAlarmMessages.Add(alarmMessage.bd, new List<AlarmMessage>());
            }

            dicAlarmMessages[alarmMessage.bd].Add(alarmMessage);
          }
        }

        foreach (KeyValuePair<int, List<AlarmMessage>> keypair in dicAlarmMessages)
        {
          int buildingID = keypair.Key;
          List<AlarmMessage> alarmMessages = keypair.Value;

          if (alarmMessages.Count > 0)
          {
            DateTime dtNow = DateTime.Now;
            string alarmFile = $"{dtNow:yyyyMMddHHmmss}.json";
            string alarmPath = Path.Combine(_alarmDirPath, buildingID.ToString(), $"{dtNow:yyyyMMdd}", alarmFile);
            Json.SaveFile(alarmMessages, alarmPath);

            int failAPIAlarms = callAPIAlarm(alarmMessages);
            bool isOkS3Alarm = callS3Alarm(alarmPath, alarmMessages.Count);

            if (failAPIAlarms == 0 && isOkS3Alarm)
            {
              File.Delete(alarmPath);
              logging(logLevel.Info, $"Success AlarmMessage Upload[Building({buildingID})] : {alarmMessages.Count} Alarms");
            }
            else
            {
              logging(logLevel.Warn, $"Failure AlarmMessage Upload[Building({buildingID})] : [{failAPIAlarms}] API Failure, [{isOkS3Alarm}] S3 result");
            }
          }
        }
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[taskAlarmMultiple] : {ex}");
      }
    }

    private void retryAlarmTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      try
      {
        if (_isS3Uploading)
        {
          detailLogging(logLevel.Warn, $"Skip AlarmMessage Upload.");
        }
        else
        {
          _isS3Uploading = true;
          alarmFileUpload(_alarmDirPath);
          removeEmptyDirectory(_alarmDirPath);
          _isS3Uploading = false;
        }

        GC.Collect();
      }
      catch (Exception ex)
      {
        _isS3Uploading = false;
        logging(logLevel.Error, $"[retryAlarmtimer_Elapsed] : {ex}");
      }
    }

    private void alarmFileUpload(string directoryPath)
    {
      foreach (string childPath in Directory.GetDirectories(directoryPath))
      {
        alarmFileUpload(childPath);
      }

      foreach (string filePath in Directory.GetFiles(directoryPath))
      {
        List<AlarmMessage> alarmMessages = (List<AlarmMessage>)Json.LoadFile(filePath, typeof(List<AlarmMessage>));
        int failAPIAlarms = 0;

        if (_config.IsAlarmRetry)
        {
          failAPIAlarms = callAPIAlarm(alarmMessages);
        }

        bool isOkS3Alarm = callS3Alarm(filePath, alarmMessages.Count);

        if (failAPIAlarms == 0 && isOkS3Alarm)
        {
          File.Delete(filePath);
          detailLogging(logLevel.Info, $"Delete AlarmMessage File : {filePath}");
        }
        else
        {
          logging(logLevel.Warn, $"Failure AlarmMessage Upload : [{failAPIAlarms}] API Failure, [{isOkS3Alarm}] S3 result({filePath})");
        }

        Thread.Sleep(_config.SequenceInterval);
      }
    }
  }
}

using iCos5.CSPGateway.CSPMessage;
using PASoft.Common;
using PASoft.Common.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace iCos5CSPGatewayRT.Manager
{
  public partial class CSPManager : Disposable
  {
    private Scheduler _scheduler;
    private Dictionary<string, OnlineVariableService> _onlineVariableServices = new Dictionary<string, OnlineVariableService>();
    private Dictionary<string, OnlineValue> _dicOnlineValues = new Dictionary<string, OnlineValue>();
    private Dictionary<string, ValueMessageInfo> _dicValueMessageInfo = new Dictionary<string, ValueMessageInfo>();
    private string _valueS3Path = "icos/todo/";
    private string _valueDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "iCos5", "todo");
    private CancellationTokenSource _cancelOnlineUpdateValue = new CancellationTokenSource();
    private CancellationTokenSource _cancelUploadGateway = new CancellationTokenSource();

    private void initCSPValue()
    {
      try
      {
        disposeItems.Add(_scheduler);

        if (_config.ActiveInsite)
        {
          _valueS3Path = _config.ConnectionInfo.ValueBucketFolder;
          _valueDirPath = Path.Combine(_config.TempPath, "todo");

          if (!Directory.Exists(_valueDirPath))
          {
            Directory.CreateDirectory(_valueDirPath);
          }
        }

        if (_config.ActiveInbase)
        {
          _valueS3PathInbase = _config.ConnectionInfoInbase.ValueBucketFolder;
          _valueDirPathInbase = Path.Combine(_config.TempPath, "todo-inbase");

          if (!Directory.Exists(_valueDirPathInbase))
          {
            Directory.CreateDirectory(_valueDirPathInbase);
          }
        }

        if (_config.ActiveBEMS)
        {
          _valueSMBPathBEMS = _config.ConnectionInfoBEMS.StorePath.Replace('/', '\\').Trim(new char[] { ' ', '\\' });
          _valueSMBPathBEMS = Path.Combine(_valueSMBPathBEMS, "todo");
          _valueDirPathBEMS = Path.Combine(_config.TempPath, "todo-bems");

          if (!Directory.Exists(_valueDirPathBEMS))
          {
            Directory.CreateDirectory(_valueDirPathBEMS);
          }
        }

        foreach (KeyValuePair<string, OnlineVariableService> keyValue in _onlineVariableServices)
        {
          keyValue.Value.Initialization();
        }

        TimeSpan tsPeriod = TimeSpan.FromMinutes(_config.PeriodInterval);
        TimeSpan tsOffset = TimeSpan.FromMinutes(_config.PeriodOffset);

        _scheduler = new Scheduler(schedulerCallback, Scheduler.TickType.EveryHour, tsPeriod, tsOffset);

        logging(logLevel.Info, $" - Cloud Gateway Value Initalization : Period/{_config.PeriodInterval} Seconds, Offset/{_config.PeriodOffset}, Online Groups/{_onlineVariableServices.Count}, Variables/{_dicValueMessageInfo.Count}");

        if (_config.InitialUpdate)
        {
          Task.Run(() => { onlineUpdateValue(); }, _cancelOnlineUpdateValue.Token);
        }
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $" - Cloud Gateway Value Initalization : {ex}");
      }
    }

    private bool callS3Value(string valueFilePath)
    {
      if (_config.CSPDebugMode)
      {
        return false;
      }

      string[] pathNodes = valueFilePath.Split('\\');

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
          if (_serviceS3.UploadFile($"{_valueS3Path}/{buildingID}/{dayFolder}/{fileName}", valueFilePath))
          {
            detailLogging(logLevel.Info, $"Success S3 Value[Building({buildingID})] : {fileName} Value File");
            return true;
          }
          else
          {
            Thread.Sleep(_config.RetransmissionInterval);
            failCount++;
          }
        }

        logging(logLevel.Warn, $"Failure S3 Value[Building({buildingID})] : {fileName} Value File[Upload Failure]");
        return false;
      }
      else
      {
        logging(logLevel.Warn, $"Failure S3 Value[Building({buildingID})] : {fileName} Value File[Disconnected]");
        return false;
      }
    }

    private void schedulerCallback()
    {
      Task.Run(() => { onlineUpdateValue(); }, _cancelOnlineUpdateValue.Token);
      Task.Run(() => { uploadGateway(); }, _cancelUploadGateway.Token);
      GC.Collect();
    }

    private void onlineUpdateValue()
    {
      try
      {
        foreach (KeyValuePair<string, OnlineVariableService> keyValue in _onlineVariableServices)
        {
          OnlineVariableService service = keyValue.Value;

          if (!service.IsRunning)
          {
            service.Update();
          }
          else 
          {
            logging(logLevel.Warn, $"Skip Value Update of ({keyValue.Key}) OnlineContainer.");
          }
        }

        logging(logLevel.Info, $"Start threads of update value.");
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[onlineValueLoading] : {ex}");
      }
    }

    private void uploadGateway()
    {
      if (_config.ActiveInsite)
      {
        makeValueMessage();
      }

      if (_config.ActiveInbase)
      {
        makeValueMessageInbase();
      }

      if (_config.ActiveBEMS)
      {
        makeValueMessageBEMS();
      }

      if (_config.ActiveInsite)
      {
        sendValueMessage();
      }

      if (_config.ActiveInbase)
      {
        sendValueMessageInbase();
      }

      if (_config.ActiveBEMS)
      {
        sendValueMessageBEMS();
      }
    }

    private void makeValueMessage()
    {
      try
      {
        List<ValueMessage> valueMessages = new List<ValueMessage>();
        int numberOfFiles = 0;

        foreach (KeyValuePair<string, OnlineValue> keyValue in _dicOnlineValues)
        {
          OnlineValue onlineValue = keyValue.Value;
          ValueMessage valueMessage = _dicValueMessageInfo[keyValue.Key].GetCSPMessage();

          if (onlineValue.LastUpdateTime.Year < 1900)
          {
            detailLogging(logLevel.Info, $"Invalid UpdateTime => {valueMessage.nm}/{onlineValue.Value}/{onlineValue.LastUpdateTime}/{onlineValue.StatusValue}");
            continue;
          }

          valueMessage.vl = valueMessage.ty.Equals("str") ? onlineValue.Value.ToString() : Convert.ToDouble(onlineValue.Value).ToString();
          valueMessage.tm = onlineValue.LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
          valueMessage.st = (onlineValue.StatusValue & 0x40000) == 0 ? 1 : 0;

          valueMessages.Add(valueMessage);

          if (valueMessages.Count >= _config.MaximumCount)
          {
            saveMessage(valueMessages, numberOfFiles);
            valueMessages.Clear();
            numberOfFiles++;
          }
        }

        if (valueMessages.Count > 0)
        {
          saveMessage(valueMessages, numberOfFiles);
          valueMessages.Clear();
          numberOfFiles++;
        }

        detailLogging(logLevel.Info, $"Completed save ({numberOfFiles}) json value files.");
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[makeValueMessage] : {ex}");
      }
    }

    private void saveMessage(List<ValueMessage> valueMessages, int numberOfFiles)
    {
      DateTime dtNow = DateTime.Now;
      string valueFile = $"{dtNow:yyyyMMddHHmmss}-{numberOfFiles}.json";
      string valuePath = Path.Combine(_valueDirPath, _config.BuildingID.ToString(), $"{dtNow:yyyyMMdd}", valueFile);
      Json.SaveFile(valueMessages, valuePath);
      detailLogging(logLevel.Info, $"Save {valueFile} => {valueMessages.Count} Variables");
    }

    private void sendValueMessage()
    {
      try
      {
        if (_isS3Uploading)
        {
          detailLogging(logLevel.Warn, $"Skip ValueMessage Upload.");
          return;
        }

        _isS3Uploading = true;
        valueFileUpload(_valueDirPath);
        removeEmptyDirectory(_valueDirPath);
        _isS3Uploading = false;
      }
      catch (Exception ex)
      {
        _isS3Uploading = false;
        logging(logLevel.Error, $"[sendValueMessage] : {ex}");
      }
    }

    private void valueFileUpload(string directoryPath)
    {
      foreach (string childPath in Directory.GetDirectories(directoryPath))
      {
        valueFileUpload(childPath);
      }

      foreach (string filePath in Directory.GetFiles(directoryPath))
      {
        bool isOkS3Value = callS3Value(filePath);

        if (isOkS3Value)
        {
          File.Delete(filePath);
          detailLogging(logLevel.Info, $"Delete ValueMessage File : {filePath}");
        }
        else
        {
          logging(logLevel.Warn, $"Failure ValueMessage Upload : [{isOkS3Value}] S3 result({filePath})");
        }

        Thread.Sleep(_config.SequenceInterval);
      }
    }
  }
}

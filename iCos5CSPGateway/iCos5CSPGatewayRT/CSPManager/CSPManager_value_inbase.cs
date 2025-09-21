using iCos5.CSPGateway.CSPMessage;
using PASoft.Common;
using PASoft.Common.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace iCos5CSPGatewayRT.Manager
{
  public partial class CSPManager : Disposable
  {
    private Dictionary<string, ValueMessageInfoStringID> _dicValueMessageInfoInbase = new Dictionary<string, ValueMessageInfoStringID>();
    private string _valueS3PathInbase = "icos/todo-inbase/";
    private string _valueDirPathInbase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "iCos5", "todo-inbase");

    private bool callS3ValueInbase(string valueFilePath)
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

      if (_serviceS3Inbase.IsConnected)
      {
        int failCount = 0;

        while (failCount < _config.RetransmissionCount)
        {
          if (_serviceS3Inbase.UploadFile($"{_valueS3PathInbase}/{buildingID}/{dayFolder}/{fileName}", valueFilePath))
          {
            detailLogging(logLevel.Info, $"Success S3 Value[Building({buildingID})] : (Inbase) {fileName} Value File");
            return true;
          }
          else
          {
            Thread.Sleep(_config.RetransmissionInterval);
            failCount++;
          }
        }

        logging(logLevel.Warn, $"Failure S3 Value[Building({buildingID})] : (Inbase) {fileName} Value File[Upload Failure]");
        return false;
      }
      else
      {
        logging(logLevel.Warn, $"Failure S3 Value[Building({buildingID})] : (Inbase) {fileName} Value File[Disconnected]");
        return false;
      }
    }

    private void makeValueMessageInbase()
    {
      try
      {
        List<ValueMessageStringID> valueMessages = new List<ValueMessageStringID>();
        int numberOfFiles = 0;

        foreach (KeyValuePair<string, OnlineValue> keyValue in _dicOnlineValues)
        {
          OnlineValue onlineValue = keyValue.Value;
          ValueMessageStringID valueMessage = _dicValueMessageInfoInbase[keyValue.Key].GetCSPMessage();

          if (onlineValue.LastUpdateTime.Year < 1900)
          {
            detailLogging(logLevel.Info, $"Invalid UpdateTime (Inbase) => {valueMessage.nm}/{onlineValue.Value}/{onlineValue.LastUpdateTime}/{onlineValue.StatusValue}");
            continue;
          }

          valueMessage.vl = valueMessage.ty.Equals("str") ? onlineValue.Value.ToString() : Convert.ToDouble(onlineValue.Value).ToString();
          valueMessage.tm = onlineValue.LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
          valueMessage.st = (onlineValue.StatusValue & 0x40000) == 0 ? 1 : 0;

          valueMessages.Add(valueMessage);

          if (valueMessages.Count >= _config.MaximumCount)
          {
            saveMessageInbase(valueMessages, numberOfFiles);
            valueMessages.Clear();
            numberOfFiles++;
          }
        }

        if (valueMessages.Count > 0)
        {
          saveMessageInbase(valueMessages, numberOfFiles);
          valueMessages.Clear();
          numberOfFiles++;
        }

        detailLogging(logLevel.Info, $"Completed save ({numberOfFiles}) json value files.(Inbase)");
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[makeValueMessage] : (Inbase) {ex}");
      }
    }

    private void saveMessageInbase(List<ValueMessageStringID> valueMessages, int numberOfFiles)
    {
      DateTime dtNow = DateTime.Now;
      string valueFile = $"{dtNow:yyyyMMddHHmmss}-{numberOfFiles}.json";
      string valuePath = Path.Combine(_valueDirPathInbase, _config.BuildingIDInbase.ToString(), $"{dtNow:yyyyMMdd}", valueFile);
      Json.SaveFile(valueMessages, valuePath);
      detailLogging(logLevel.Info, $"Save {valueFile} (Inbase) => {valueMessages.Count} Variables");
    }

    private void sendValueMessageInbase()
    {
      try
      {
        if (_isS3UploadingInbase)
        {
          detailLogging(logLevel.Warn, $"Skip ValueMessage Upload. (Inbase)");
          return;
        }

        _isS3UploadingInbase = true;
        valueFileUploadInbase(_valueDirPathInbase);
        removeEmptyDirectory(_valueDirPathInbase);
        _isS3UploadingInbase = false;
      }
      catch (Exception ex)
      {
        _isS3UploadingInbase = false;
        logging(logLevel.Error, $"[sendValueMessage] : (Inbase) {ex}");
      }
    }

    private void valueFileUploadInbase(string directoryPath)
    {
      foreach (string childPath in Directory.GetDirectories(directoryPath))
      {
        valueFileUploadInbase(childPath);
      }

      foreach (string filePath in Directory.GetFiles(directoryPath))
      {
        bool isOkS3Value = callS3ValueInbase(filePath);

        if (isOkS3Value)
        {
          File.Delete(filePath);
          detailLogging(logLevel.Info, $"Delete ValueMessage File : (Inbase) {filePath}");
        }
        else
        {
          logging(logLevel.Warn, $"Failure ValueMessage Upload : (Inbase) [{isOkS3Value}] S3 result({filePath})");
        }

        Thread.Sleep(_config.SequenceInterval);
      }
    }
  }
}

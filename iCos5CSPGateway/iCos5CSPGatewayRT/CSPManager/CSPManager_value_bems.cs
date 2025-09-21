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
    private Dictionary<string, ValueMessageInfoStringID> _dicValueMessageInfoBEMS = new Dictionary<string, ValueMessageInfoStringID>();
    private string _valueSMBPathBEMS = @"BEMS\todo-bems";
    private string _valueDirPathBEMS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "iCos5", "todo-bems");

    private bool callSMBValue(string valueFilePath)
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

      if (_smbClient.IsConnected)
      {
        string targetPath = _smbClient.GetTargetPath(Path.Combine(_valueSMBPathBEMS, buildingID, dayFolder, fileName));
        string directoryName = Path.GetDirectoryName(targetPath);

        if (!Directory.Exists(directoryName))
        {
          Directory.CreateDirectory(directoryName);
        }

        try
        {
          File.Copy(valueFilePath, targetPath);
          detailLogging(logLevel.Info, $"Success SMB Value[Building({buildingID})] : (BEMS) {fileName} Value File");
          return true;
        }
        catch (Exception ex)
        {
          logging(logLevel.Error, $"[callSMBValue] : (BEMS) {ex}");
          return false;
        }
      }
      else
      {
        logging(logLevel.Warn, $"Failure SMB Value[Building({buildingID})] : (BEMS) {fileName} Value File[Disconnected]");
        return false;
      }
    }

    private void makeValueMessageBEMS()
    {
      try
      {
        List<ValueMessageStringID> valueMessages = new List<ValueMessageStringID>();
        int numberOfFiles = 0;

        foreach (KeyValuePair<string, OnlineValue> keyValue in _dicOnlineValues)
        {
          OnlineValue onlineValue = keyValue.Value;
          ValueMessageStringID valueMessage = _dicValueMessageInfoBEMS[keyValue.Key].GetCSPMessage();

          if (onlineValue.LastUpdateTime.Year < 1900)
          {
            detailLogging(logLevel.Info, $"Invalid UpdateTime (BEMS) => {valueMessage.nm}/{onlineValue.Value}/{onlineValue.LastUpdateTime}/{onlineValue.StatusValue}");
            continue;
          }

          valueMessage.vl = valueMessage.ty.Equals("str") ? onlineValue.Value.ToString() : Convert.ToDouble(onlineValue.Value).ToString();
          valueMessage.tm = onlineValue.LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
          valueMessage.st = (onlineValue.StatusValue & 0x40000) == 0 ? 1 : 0;

          valueMessages.Add(valueMessage);

          if (valueMessages.Count >= _config.MaximumCount)
          {
            saveMessageBEMS(valueMessages, numberOfFiles);
            valueMessages.Clear();
            numberOfFiles++;
          }
        }

        if (valueMessages.Count > 0)
        {
          saveMessageBEMS(valueMessages, numberOfFiles);
          valueMessages.Clear();
          numberOfFiles++;
        }

        detailLogging(logLevel.Info, $"Completed save ({numberOfFiles}) json value files.(BEMS)");
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[makeValueMessage] : (BEMS) {ex}");
      }
    }

    private void saveMessageBEMS(List<ValueMessageStringID> valueMessages, int numberOfFiles)
    {
      DateTime dtNow = DateTime.Now;
      string valueFile = $"{dtNow:yyyyMMddHHmmss}-{numberOfFiles}.json";
      string valuePath = Path.Combine(_valueDirPathBEMS, _config.BuildingIDBEMS.ToString(), $"{dtNow:yyyyMMdd}", valueFile);
      Json.SaveFile(valueMessages, valuePath);
      detailLogging(logLevel.Info, $"Save {valueFile} (BEMS) => {valueMessages.Count} Variables");
    }

    private void sendValueMessageBEMS()
    {
      try
      {
        if (_isSMBUploadingBEMS)
        {
          detailLogging(logLevel.Warn, $"Skip ValueMessage Upload. (BEMS)");
          return;
        }

        _isSMBUploadingBEMS = true;
        valueFileUploadBEMS(_valueDirPathBEMS);
        removeEmptyDirectory(_valueDirPathBEMS);
        _isSMBUploadingBEMS = false;
      }
      catch (Exception ex)
      {
        _isSMBUploadingBEMS = false;
        logging(logLevel.Error, $"[sendValueMessage] : (BEMS) {ex}");
      }
    }

    private void valueFileUploadBEMS(string directoryPath)
    {
      foreach (string childPath in Directory.GetDirectories(directoryPath))
      {
        valueFileUploadBEMS(childPath);
      }

      foreach (string filePath in Directory.GetFiles(directoryPath))
      {
        bool isOkSMBValue = callSMBValue(filePath);

        if (isOkSMBValue)
        {
          File.Delete(filePath);
          detailLogging(logLevel.Info, $"Delete ValueMessage File : (BEMS) {filePath}");
        }
        else
        {
          logging(logLevel.Warn, $"Failure ValueMessage Upload : (BEMS) [{isOkSMBValue}] SMB result({filePath})");
        }

        Thread.Sleep(_config.SequenceInterval);
      }
    }
  }
}

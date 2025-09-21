using iCos5.CSPGateway;
using iCos5CSPGateway.AWS;
using PASoft.Common;
using PASoft.Common.Encryption;
using PASoft.Common.SMB;
using PASoft.Zenon.Addins;
using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.Variable;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace iCos5CSPGatewayRT.Manager
{
  public partial class CSPManager : Disposable
  {
    private enum logLevel
    {
      Trace,
      Debug,
      Info,
      Warn,
      Error,
      Fatal
    }

    private IProject _zenonProject;
    private GatewayConfig _config;
    private ServiceS3 _serviceS3;
    private ServiceS3 _serviceS3Inbase;
    private ServiceAPI _serviceAPI;
    private SMBClient _smbClient;
    private CelLogging _celLogging;
    private bool _isS3Uploading = false;
    private bool _isS3UploadingInbase = false;
    private bool _isSMBUploadingBEMS = false;
    private CancellationTokenSource _cancelInit = new CancellationTokenSource();

    public CSPManager(IProject zenonProject, GatewayConfig config, CelLogging celLogging)
    {
      try
      {
        _zenonProject = zenonProject;
        _config = config;
        _celLogging = celLogging;

        if (_config == null)
        {
          logging(logLevel.Error, $" >>> Load Config : Config is null!!");
        }
        else
        {
          logging(logLevel.Info, $" >>> Load Config : ok");

          if (_config.ActiveInsite || _config.ActiveInbase || _config.ActiveBEMS)
          {
            Task.Run(() => {
              initCSPCommon();
              initCSPValue();

              if (_config.ActiveInsite)
              {
                initCSPAlarm();
              }

              if (_config.ActiveInsite || _config.ActiveBEMS)
              {
                initCSPControl();
              }
              else
              {
                disposeItems.Add(_controlTimer);
              }

              if (_config.ActiveInsite)
              {
                logging(logLevel.Info, $" >>> Insite Service : Active");
              }
              else
              {
                logging(logLevel.Warn, $" >>> Insite Service : Inactive");
              }

              if (_config.ActiveInbase)
              {
                logging(logLevel.Info, $" >>> Inbase Service : Active");
              }
              else
              {
                logging(logLevel.Warn, $" >>> Inbase Service : Inactive");
              }

              if (_config.ActiveBEMS)
              {
                logging(logLevel.Info, $" >>> BEMS Service : Active");
              }
              else
              {
                logging(logLevel.Warn, $" >>> BEMS Service : Inactive");
              }
            }, _cancelInit.Token);
          }
          else
          {
            logging(logLevel.Warn, $" >>> Insite Service : Inactive");
            logging(logLevel.Warn, $" >>> Inbase Service : Inactive");
            logging(logLevel.Warn, $" >>> BEMS Service : Inactive");
          }
        }
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $" >>> Load Config : {ex}");
      }
    }

    private void initCSPCommon()
    {
      try
      {
        // Initialize AWS service connection
        if (_config.ActiveInsite)
        {
          _serviceS3 = new ServiceS3(AESCryptor.Decoding256(_config.ConnectionInfo.AccessKeyID),
                                     AESCryptor.Decoding256(_config.ConnectionInfo.SecretAccessKey),
                                     _config.ConnectionInfo.S3ServiceUrl,
                                     _config.ConnectionInfo.BucketName);
          _serviceAPI = new ServiceAPI(_config.ConnectionInfo.AlarmEventApiUrl,
                                       _config.ConnectionInfo.RemoteControlApiUrl,
                                       AESCryptor.Decoding256(_config.ConnectionInfo.AwsApiKey));
        }

        if (_config.ActiveInbase)
        {
          _serviceS3Inbase = new ServiceS3(AESCryptor.Decoding256(_config.ConnectionInfoInbase.AccessKeyID),
                                           AESCryptor.Decoding256(_config.ConnectionInfoInbase.SecretAccessKey),
                                           _config.ConnectionInfoInbase.S3ServiceUrl,
                                           _config.ConnectionInfoInbase.BucketName);
        }

        // Initialize SMB service connection
        if (_config.ActiveBEMS)
        {
          _smbClient = new SMBClient(_config.ConnectionInfoBEMS.HostName,
                                     _config.ConnectionInfoBEMS.UserName,
                                     AESCryptor.Decoding256(_config.ConnectionInfoBEMS.Password));
        }

        _config.TempPath = _config.TempPath.Replace('/', '\\');

        Regex cspNameRegex = !_config.ActiveInbase && _config.ActiveInsite && !_config.IsSingleBuilding ?
                             new Regex(@"^(\d+?)\.([\w_.]+?)\:(\w+?)\.(\w+?)$") :
                             new Regex(@"^([\w_.]+?)\:(\w+?)\.(\w+?)$");

        foreach (IVariable variable in _zenonProject.VariableCollection)
        {
          // Check Array Tag Symbol
          if (!variable.DataType.IsSimpleDataType)
          {
            continue;
          }

          // Check CSP Naming Rule
          if (!_config.EnableAllTagSelection && !cspNameRegex.IsMatch(variable.Name))
          {
            continue;
          }

          // Initialize Value Dictionary
          string valueContainerName = $"{GatewayConfig.Constants.CSPOnlineContainer}_{variable.NetAddress}_{variable.Driver.Identification}";

          if (_zenonProject.OnlineVariableContainerCollection[valueContainerName] == null)
          {
            if (variable.Driver.Name.Equals(GatewayConfig.Constants.BACnetDriverName))
            {
              _onlineVariableServices.Add(valueContainerName, new OnlineVariableService(_zenonProject,
                                                                                        valueContainerName,
                                                                                        _config.IsLocalTime,
                                                                                        100,
                                                                                        20,
                                                                                        _config.UseDriverUpdateTime,
                                                                                        ref _dicOnlineValues,
                                                                                        ref _dicAlarmMessageInfo));
            }
            else
            {
              _onlineVariableServices.Add(valueContainerName, new OnlineVariableService(_zenonProject,
                                                                                        valueContainerName,
                                                                                        _config.IsLocalTime,
                                                                                        100,
                                                                                        100,
                                                                                        _config.UseDriverUpdateTime,
                                                                                        ref _dicOnlineValues,
                                                                                        ref _dicAlarmMessageInfo));
            }
          }

          _onlineVariableServices[valueContainerName].AddVariable(variable);

          string cloudValueType = getCloudValueType(variable.IecType);

          // Insite Part
          if (_config.ActiveInsite)
          {
            ValueMessageInfo valueMessageInfo = new ValueMessageInfo();

            if (_config.IsSingleBuilding)
            {
              try
              {
                valueMessageInfo.BuildingID = _config.BuildingID;
              }
              catch { }

              valueMessageInfo.TagName = variable.Name;
            }
            else
            {
              valueMessageInfo.BuildingID = 0;
              valueMessageInfo.TagName = variable.Name;

              for (int i = 0; i < variable.Name.Length; i++)
              {
                if (variable.Name[i].Equals(GatewayConfig.Constants.MultiBuildingDelimiter))
                {
                  try
                  {
                    valueMessageInfo.BuildingID = Convert.ToInt32(variable.Name.Substring(0, i));

                    int nextIndex = i + 1;
                    valueMessageInfo.TagName = variable.Name.Substring(nextIndex, variable.Name.Length - nextIndex);
                  }
                  catch { }

                  break;
                }
              }
            }

            valueMessageInfo.ValueType = cloudValueType;
            _dicValueMessageInfo.Add(variable.Name, valueMessageInfo);

            // Initialize Alarm Dictionary
            if (variable.LimitValueCollection.Count > 0)
            {
              AlarmMessageInfo alarmMessageInfo = new AlarmMessageInfo()
              {
                BuildingID = valueMessageInfo.BuildingID,
                TagName = valueMessageInfo.TagName,
                ValueType = valueMessageInfo.ValueType,
              };

              foreach (ILimitValue limit in variable.LimitValueCollection)
              {
                if (limit.IsMaximum())
                {
                  alarmMessageInfo.MaximumLimits.Add(limit.Value);
                }
                else if (limit.IsMinimum())
                {
                  alarmMessageInfo.MinimumLimits.Add(limit.Value);
                }
              }

              alarmMessageInfo.MaximumLimits.Sort();
              alarmMessageInfo.MinimumLimits.Sort();

              _dicAlarmMessageInfo.Add(variable.Name, alarmMessageInfo);
            }

            // Initialize Control Dictionary
            if (!_config.IsSingleBuilding)
            {
              if (!_buildingIDs.Contains(valueMessageInfo.BuildingID))
              {
                _buildingIDs.Add(valueMessageInfo.BuildingID);
              }
            }
          }

          // Inbase Part
          if (_config.ActiveInbase)
          {
            ValueMessageInfoStringID valueMessageInfoInbase = new ValueMessageInfoStringID();
            valueMessageInfoInbase.BuildingID = _config.BuildingIDInbase;
            valueMessageInfoInbase.TagName = variable.Name;
            valueMessageInfoInbase.ValueType = cloudValueType;
            _dicValueMessageInfoInbase.Add(variable.Name, valueMessageInfoInbase);
          }

          // BEMS Part
          if (_config.ActiveBEMS)
          {
            ValueMessageInfoStringID valueMessageInfoBEMS = new ValueMessageInfoStringID();
            valueMessageInfoBEMS.BuildingID = _config.BuildingIDBEMS;
            valueMessageInfoBEMS.TagName = variable.Name;
            valueMessageInfoBEMS.ValueType = cloudValueType;
            _dicValueMessageInfoBEMS.Add(variable.Name, valueMessageInfoBEMS);
          }
        }

        logging(logLevel.Info, $" - {GatewayConfig.Constants.SolutionNewName} Common Initalization");
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $" - {GatewayConfig.Constants.SolutionNewName} Common Initalization : {ex}");
      }
    }

    private string getCloudValueType(IecType iecType)
    {
      switch (iecType)
      {
        case IecType.Bool:
          return "bln";
        case IecType.SInt:
        case IecType.Int:
        case IecType.DInt:
        case IecType.LInt:
        case IecType.UsInt:
        case IecType.UInt:
        case IecType.UdInt:
        case IecType.UlInt:
        case IecType.Real:
        case IecType.LReal:
        case IecType.Byte:
        case IecType.Word:
        case IecType.DWord:
        case IecType.LWord:
        case IecType.FloatLong:
        case IecType.FloatIeee:
        default:
          return "num";
        case IecType.String:
        case IecType.WString:
          return "str";
        case IecType.Time:
        case IecType.Date:
        case IecType.Tod:
        case IecType.DateAndTime:
        case IecType.ArchiveDataType:
        case IecType.VoidArray:
        case IecType.TopologicalWord4:
        case IecType.TopologicalLineWord4:
          return "null";
      }
    }

    public void BeforeFree()
    {
      try
      {
        // Stop Initialization
        _cancelInit.Cancel();

        // Terminate Value
        _scheduler.TickEnable = false;
        _cancelOnlineUpdateValue.Cancel();
        _cancelUploadGateway.Cancel();
        _onlineVariableServices.Clear();
        _onlineVariableServices = null;
        _dicOnlineValues.Clear();
        _dicOnlineValues = null;
        _dicValueMessageInfo.Clear();
        _dicValueMessageInfo = null;
        _dicValueMessageInfoInbase.Clear();
        _dicValueMessageInfoInbase = null;
        _dicValueMessageInfoBEMS.Clear();
        _dicValueMessageInfoBEMS = null;

        if (_config.ActiveInsite)
        {
          // Terminate Alarm
          _cancelOnlineUploadAlarm.Cancel();
          _onlineAlarmService.BeforeFree();
          _dicAlarmMessageInfo.Clear();
          _dicAlarmMessageInfo = null;
        }

        if (_config.ActiveInsite || _config.ActiveBEMS)
        {
          // Terminate Control
          _controlTimer.Stop();
          _buildingIDs.Clear();
          _buildingIDs = null;
          _dicOnlineControlServices.Clear();
          _dicOnlineControlServices = null;
          _dicOnlineControlServicesBems.Clear();
          _dicOnlineControlServicesBems = null;
        }

        logging(logLevel.Info, $" >>> Finish {GatewayConfig.Constants.SolutionNewName} Addins : {_zenonProject.Name}");
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $" >>> Finish {GatewayConfig.Constants.SolutionNewName} Addins : {ex}");
      }
    }

    private void logging(logLevel level, string message)
    {
      switch (level)
      {
        case logLevel.Trace:
          _celLogging.Trace(message);
          break;
        case logLevel.Debug:
          _celLogging.Debug(message);
          break;
        case logLevel.Info:
          _celLogging.Info(message);
          break;
        case logLevel.Warn:
          _celLogging.Warn(message);
          break;
        case logLevel.Error:
          _celLogging.Error(message);
          break;
        case logLevel.Fatal:
          _celLogging.Fatal(message);
          break;
      }
    }

    private void detailLogging(logLevel level, string message)
    {
      if (_config.DetailLogging)
      {
        logging(level, message);
      }
    }

    private void removeEmptyDirectory(string directoryPath)
    {
      foreach (string directory in Directory.GetDirectories(directoryPath))
      {
        removeEmptyDirectory(directory);

        if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
        {
          Directory.Delete(directory, false);
        }
      }
    }
  }
}

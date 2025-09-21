using iCos5.CSPGateway;
using iCos5.CSPGateway.CSPMessage;
using PASoft.Common;
using PASoft.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;

namespace iCos5CSPGatewayRT.Manager
{
  public partial class CSPManager : Disposable
  {
    public event EventHandler<ControlBooleanEventArgs> ShowWindowTriggered;
    public event EventHandler<ControlListItemEventArgs> AddControlListItemTriggered;
    public event EventHandler<ControlIntegerEventArgs> RemoveControlListItemTriggered;

    private List<int> _buildingIDs = new List<int>();
    private Timer _controlTimer;
    private Dictionary<int, OnlineControlService> _dicOnlineControlServices = new Dictionary<int, OnlineControlService>();

    public bool ConfirmOnlineControlService(int sequenceNumber)
    {
      if (_dicOnlineControlServices.ContainsKey(sequenceNumber))
      {
        _dicOnlineControlServices[sequenceNumber].ConfirmSetValue();
        return true;
      }
      else
      {
        return false;
      }
    }

    public bool RejectOnlineControlService(int sequenceNumber)
    {
      if (_dicOnlineControlServices.ContainsKey(sequenceNumber))
      {
        _dicOnlineControlServices[sequenceNumber].RejectSetValue();
        return true;
      }
      else
      {
        return false;
      }
    }

    private void initCSPControl()
    {
      try
      {
        disposeItems.Add(_controlTimer);

        if (_config.ActiveInsite || _config.ActiveBEMS)
        {
          _controlTimer = _config.IsUseControl ? new Timer(_config.ControlUpdatePeriod * 1000) : new Timer(_config.ControlFlushPeriod * 60 * 1000);

          if (_config.ActiveInsite)
          {
            _controlTimer.Elapsed += controlTimer_Elapsed;
          }

          if (_config.ActiveBEMS)
          {
            _controlTimer.Elapsed += controlTimerBEMS_Elapsed;
          }

          _controlTimer.Start();

          if (_config.IsMannedControl)
          {
            logging(logLevel.Info, $" - Cloud Gateway Control Initalization : Activate/{_config.IsUseControl}, Polling Interval/{_config.ControlUpdatePeriod}sec, Timeout/{_config.ControlTimeout}sec, Hold On Time/{_config.HoldOnTime}min");
          }
          else
          {
            logging(logLevel.Info, $" - Cloud Gateway Control Initalization : Activate/{_config.IsUseControl}, Polling Interval/{_config.ControlUpdatePeriod}sec, Timeout/{_config.ControlTimeout}sec");
          }
        }
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $" - Cloud Gateway Control Initalization : {ex}");
      }
    }

    private int callAPIGetControl(ControlMessage controlMessage)
    {
      if (_config.CSPDebugMode)
      {
        return 0;
      }

      try
      {
        string result = _serviceAPI.InvokeGetControl(Json.GetString(controlMessage));
        ControlReturnMessage returnMessage = (ControlReturnMessage)Json.LoadString(result, typeof(ControlReturnMessage));

        if (returnMessage.statusCode == (int)StatusCode.OK)
        {
          int newRequestCount = 0;

          foreach (ControlRequestMessage controlRequestMessage in returnMessage.body.data)
          {
            controlRequestMessage.bd = controlMessage.bd;

            if (addOnlineControlService(controlRequestMessage))
            {
              newRequestCount++;
            }
          }

          if (_config.DetailLogging)
          {
            logging(logLevel.Info, $"Success API GetControl[Building({controlMessage.bd})] : {newRequestCount} adds / {returnMessage.body.data.Count} requests");
          }
          else
          {
            if (returnMessage.body.data.Count > 0)
            {
              logging(logLevel.Info, $"Success API GetControl[Building({controlMessage.bd})] : {newRequestCount} adds / {returnMessage.body.data.Count} requests");
            }
          }

          return newRequestCount;
        }
        else
        {
          logging(logLevel.Warn, $"Failure API GetControl[Building({controlMessage.bd})] : statusCode[{returnMessage.statusCode}]");
          return 0;
        }
      }
      catch (WebException ex)
      {
        logging(logLevel.Error, $"Failure API GetControl[Building({controlMessage.bd})] : {ex}");
        return 0;
      }
    }

    private bool addOnlineControlService(ControlRequestMessage controlRequestMessage)
    {
      try
      {
        string controlContainerName = $"{GatewayConfig.Constants.CSPOnlineContainer}_control_{controlRequestMessage.seq}";
        string variableName = _config.IsSingleBuilding ? 
                              controlRequestMessage.nm : 
                              $"{controlRequestMessage.bd}{GatewayConfig.Constants.MultiBuildingDelimiter}{controlRequestMessage.nm}";

        if (_dicOnlineControlServices.ContainsKey(controlRequestMessage.seq))
        {
          return false;
        }

        OnlineControlService onlineControlService;

        if (_zenonProject.VariableCollection[variableName] == null)
        {
          onlineControlService = new OnlineControlService(controlRequestMessage,
                                                          _config.IsLocalTime, 
                                                          ControlResponseCode.TagError);
        }
        else if (DateTime.Now.Subtract(TimeSpan.FromMinutes(_config.HoldOnTime)) > Convert.ToDateTime(controlRequestMessage.tm))
        {
          onlineControlService = new OnlineControlService(controlRequestMessage,
                                                          _config.IsLocalTime,
                                                          ControlResponseCode.TimeOut);
        }
        else
        {
          if (_config.IsUseControl)
          {
            if (_config.IsMannedControl)
            {
              onlineControlService = new OnlineControlService(controlRequestMessage,
                                                              _zenonProject,
                                                              controlContainerName,
                                                              variableName,
                                                              _config.IsLocalTime,
                                                              _config.ControlTimeout * 1000,
                                                              _config.HoldOnTime * 60 * 1000);
            }
            else
            {
              onlineControlService = new OnlineControlService(controlRequestMessage,
                                                              _zenonProject,
                                                              controlContainerName,
                                                              variableName,
                                                              _config.IsLocalTime,
                                                              _config.ControlTimeout * 1000);
            }
          }
          else
          {
            onlineControlService = new OnlineControlService(controlRequestMessage,
                                                            _config.IsLocalTime, 
                                                            ControlResponseCode.Deactivation);
          }
        }

        onlineControlService.OnCompleted += onlineControlService_OnCompleted;
        _dicOnlineControlServices.Add(controlRequestMessage.seq, onlineControlService);
        onAddControlListItemTriggered(new ControlListItem(controlRequestMessage, _config.HoldOnTime, _config.ControlTimeout));
        return true;
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[addOnlineControlService][Building({controlRequestMessage.bd})][Tag({controlRequestMessage.nm})] : {ex}");
        return false;
      }
    }

    private bool callAPISetControl(ControlResponseMessage controlResponseMessage)
    {
      if (_config.CSPDebugMode)
      {
        return false;
      }

      try
      {
        string result = _serviceAPI.InvokeSetControl(Json.GetString(controlResponseMessage));
        ReturnMessage returnMessage = (ReturnMessage)Json.LoadString(result, typeof(ReturnMessage));

        if (returnMessage.statusCode == (int)StatusCode.OK)
        {
          logging(logLevel.Info, $"Success API SetControl[Sequence({controlResponseMessage.seq})] : {controlResponseMessage.code}");
          return true;
        }
        else
        {
          logging(logLevel.Warn, $"Failure API SetControl[Sequence({controlResponseMessage.seq})] : statusCode[{returnMessage.Status}({returnMessage.statusCode})]");
          return false;
        }
      }
      catch (WebException ex)
      {
        logging(logLevel.Error, $"Failure API SetControl[Sequence({controlResponseMessage.seq})] : {ex}");
        return false;
      }
    }

    private void controlTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      if (_config.IsSingleBuilding)
      {
        try
        {
          int reqCount = callAPIGetControl(new ControlMessage(_config.BuildingID));

          if (_config.IsMannedControl && reqCount > 0)
          {
            onShowWindowTriggered(true);
          }
        }
        catch (Exception ex)
        {
          logging(logLevel.Error, $"[controlTimer_Elapsed] : {ex}");
        }
      }
      else
      {
        foreach (int buildingID in _buildingIDs)
        {
          try
          {
            int reqCount = callAPIGetControl(new ControlMessage(buildingID));

            if (_config.IsMannedControl && reqCount > 0)
            {
              onShowWindowTriggered(true);
            }
          }
          catch (Exception ex)
          {
            logging(logLevel.Error, $"[controlTimer_Elapsed] : {ex}");
          }
        }
      }
    }

    private void onlineControlService_OnCompleted(object sender, ControlServiceCompletedEventArgs e)
    {
      try
      {
        if (callAPISetControl(e.Message))
        {
          _dicOnlineControlServices[e.Message.seq] = null;
          _dicOnlineControlServices.Remove(e.Message.seq);
          onRemoveControlListItemTriggered(e.Message.seq);
          GC.Collect();
        }
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[onlineControlService_OnCompleted] : {ex}");
      }
    }

    private void onShowWindowTriggered(bool value)
    {
      ShowWindowTriggered?.Invoke(new object(), new ControlBooleanEventArgs(value));
    }

    private void onAddControlListItemTriggered(ControlListItem value)
    {
      AddControlListItemTriggered?.Invoke(new object(), new ControlListItemEventArgs(value));
    }

    private void onRemoveControlListItemTriggered(int value)
    {
      RemoveControlListItemTriggered?.Invoke(new object(), new ControlIntegerEventArgs(value));
    }
  }
}

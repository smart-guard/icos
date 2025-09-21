using iCos5.CSPGateway;
using iCos5.CSPGateway.CSPMessage;
using iCos5.CSPGateway.DB;
using PASoft.Common;
using PASoft.DB;
using PASoft.DB.Postgres;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Timers;

namespace iCos5CSPGatewayRT.Manager
{
  public partial class CSPManager : Disposable
  {
    private Dictionary<int, OnlineControlService_bems> _dicOnlineControlServicesBems = new Dictionary<int, OnlineControlService_bems>();

    public bool ConfirmOnlineControlServiceBems(int sequenceNumber)
    {
      if (_dicOnlineControlServicesBems.ContainsKey(sequenceNumber))
      {
        _dicOnlineControlServicesBems[sequenceNumber].ConfirmSetValue();
        return true;
      }
      else
      {
        return false;
      }
    }

    public bool RejectOnlineControlServiceBems(int sequenceNumber)
    {
      if (_dicOnlineControlServicesBems.ContainsKey(sequenceNumber))
      {
        _dicOnlineControlServicesBems[sequenceNumber].RejectSetValue();
        return true;
      }
      else
      {
        return false;
      }
    }

    private int callAPIGetControl_bems(string buildingId)
    {
      if (_config.CSPDebugMode)
      {
        return 0;
      }

      try
      {
        NpgsqlConfig dbConfig = _config.ConnectionInfoBEMSdb;
        DBResult dbResult = PostgresClient.ExecuteReader(dbConfig.ConnectionString, dbConfig.RequestProcedure);

        if (dbResult.Result.Equals("OK"))
        {
          DataTable dt = (DataTable)dbResult.Return;
          DataRow[] controls = dt.Select($"bldg_id='{buildingId}'");
          int newRequestCount = 0;

          foreach (DataRow control in controls)
          {
            ControlRequestScheme controlRequestScheme = new ControlRequestScheme
            {
              seq = control["seq"].ToString(),
              bldg_id = buildingId,
              itfc_id = control["itfc_id"].ToString(),
              ctrl_prmt_id = control["ctrl_prmt_id"].ToString(),
              ctrl_prmt_nm = control["ctrl_prmt_nm"].ToString(),
              ctrl_val = control["ctrl_val"].ToString(),
              act_tm = ((DateTime)control["act_tm"]).ToString("yyyy-MM-dd HH:mm:ss")
            };

            if (addOnlineControlService_bems(controlRequestScheme))
            {
              newRequestCount++;
            }
          }

          if (_config.DetailLogging)
          {
            logging(logLevel.Info, $"Success API GetControl[Building({buildingId})] : (bems) {newRequestCount} adds / {dt.Rows.Count} requests");
          }
          else
          {
            if (controls.Length > 0)
            {
              logging(logLevel.Info, $"Success API GetControl[Building({buildingId})] : (bems) {newRequestCount} adds / {dt.Rows.Count} requests");
            }
          }

          return newRequestCount;
        }
        else
        {
          logging(logLevel.Warn, $"Failure API GetControl_bems[Building({buildingId})] : statusCode[{dbResult.Result}]");
          return 0;
        }
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"Failure API GetControl_bems[Building({buildingId})] : {ex}");
        return 0;
      }
    }

    private bool addOnlineControlService_bems(ControlRequestScheme controlRequestScheme)
    {
      try
      {
        string controlContainerName = $"{GatewayConfig.Constants.CSPOnlineContainer}_control_{controlRequestScheme.ctrl_prmt_id}_{controlRequestScheme.act_tm}";
        string variableName = controlRequestScheme.itfc_id;

        if (_dicOnlineControlServicesBems.ContainsKey(controlRequestScheme.SequenceNumber))
        {
          return false;
        }

        OnlineControlService_bems onlineControlService;

        if (_zenonProject.VariableCollection[variableName] == null)
        {
          onlineControlService = new OnlineControlService_bems(controlRequestScheme,
                                                               _config.IsLocalTime,
                                                               ControlResponseCode.TagError);
        }
        else if (DateTime.Now.Subtract(TimeSpan.FromMinutes(_config.HoldOnTime)) > Convert.ToDateTime(controlRequestScheme.RequestDateTime))
        {
          onlineControlService = new OnlineControlService_bems(controlRequestScheme,
                                                               _config.IsLocalTime,
                                                               ControlResponseCode.TimeOut);
        }
        else
        {
          if (_config.IsUseControl)
          {
            if (_config.IsMannedControl)
            {
              onlineControlService = new OnlineControlService_bems(controlRequestScheme,
                                                                   _zenonProject,
                                                                   controlContainerName,
                                                                   variableName,
                                                                   _config.IsLocalTime,
                                                                   _config.ControlTimeout * 1000,
                                                                   _config.HoldOnTime * 60 * 1000);
            }
            else
            {
              onlineControlService = new OnlineControlService_bems(controlRequestScheme,
                                                                   _zenonProject,
                                                                   controlContainerName,
                                                                   variableName,
                                                                   _config.IsLocalTime,
                                                                   _config.ControlTimeout * 1000);
            }
          }
          else
          {
            onlineControlService = new OnlineControlService_bems(controlRequestScheme,
                                                                 _config.IsLocalTime,
                                                                 ControlResponseCode.Deactivation);
          }
        }

        onlineControlService.OnCompleted += onlineControlServiceBems_OnCompleted;
        _dicOnlineControlServicesBems.Add(controlRequestScheme.SequenceNumber, onlineControlService);
        onAddControlListItemTriggered(new ControlListItem(controlRequestScheme, _config.HoldOnTime, _config.ControlTimeout));
        return true;
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[addOnlineControlService][Building({controlRequestScheme.bldg_id})][Tag({controlRequestScheme.itfc_id})] : (bems) {ex}");
        return false;
      }
    }

    private bool callAPISetControl_bems(ControlResponseScheme controlResponseScheme)
    {
      if (_config.CSPDebugMode)
      {
        return false;
      }

      try
      {
        NpgsqlConfig dbConfig = _config.ConnectionInfoBEMSdb;
        string resultQuery = dbConfig.ReplyProcedure.Replace("'ID'", $"'{controlResponseScheme.ParameterID}'")
                                                    .Replace("'TIME'", $"'{controlResponseScheme.ActiveTime}'")
                                                    .Replace("'MESSAGE'", $"'{controlResponseScheme.Message}'");
        DBResult dbResult = PostgresClient.ExecuteNonQuery(dbConfig.ConnectionString, resultQuery);

        if (dbResult.Result.Equals("OK"))
        {
          logging(logLevel.Info, $"Success API SetControl[Sequence({controlResponseScheme.SequnceNumber})] : (bems) {controlResponseScheme.Message}");
          return true;
        }
        else
        {
          logging(logLevel.Warn, $"Failure API SetControl[Sequence({controlResponseScheme.SequnceNumber})] : (bems) statusCode[{dbResult.Result}]");
          return false;
        }
      }
      catch (WebException ex)
      {
        logging(logLevel.Error, $"Failure API SetControl[Sequence({controlResponseScheme.SequnceNumber})] : (bems) {ex}");
        return false;
      }
    }

    private void controlTimerBEMS_Elapsed(object sender, ElapsedEventArgs e)
    {
      try
      {
        int reqCount = callAPIGetControl_bems(_config.BuildingIDBEMS);

        if (_config.IsMannedControl && reqCount > 0)
        {
          onShowWindowTriggered(true);
        }
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[controlTimer_Elapsed] : (BEMS) {ex}");
      }
    }

    private void onlineControlServiceBems_OnCompleted(object sender, ControlServiceCompletedEventArgs e)
    {
      try
      {
        if (callAPISetControl_bems(e.Scheme))
        {
          _dicOnlineControlServicesBems[e.Scheme.SequnceNumber] = null;
          _dicOnlineControlServicesBems.Remove(e.Scheme.SequnceNumber);
          onRemoveControlListItemTriggered(e.Scheme.SequnceNumber);
          GC.Collect();
        }
      }
      catch (Exception ex)
      {
        logging(logLevel.Error, $"[onlineControlService_OnCompleted] : (bems) {ex}");
      }
    }
  }
}

using iCos5.CSPGateway.CSPMessage;
using iCos5.CSPGateway.DB;
using iCos5.CSPGateway.MVVM;
using System;
using System.Timers;

namespace iCos5CSPGatewayRT.Manager
{
  public class ControlListItem : NotifyBase
  {
    private int _sequenceNumber = 0;
    public int SequenceNumber
    {
      get { return _sequenceNumber; }
      set
      {
        _sequenceNumber = value;
        OnPropertyChanged("SequenceNumber");
      }
    }

    private object _buildingID = 0;
    public object BuildingID
    {
      get { return _buildingID; }
      set
      {
        _buildingID = value;
        OnPropertyChanged("BuildingID");
      }
    }

    private string _variableName = "";
    public string VariableName
    {
      get { return _variableName; }
      set
      {
        _variableName = value;
        OnPropertyChanged("VariableName");
      }
    }

    private string _controlValue = "";
    public string ControlValue
    {
      get { return _controlValue; }
      set
      {
        _controlValue = value;
        OnPropertyChanged("ControlValue");
      }
    }

    private string _requestTime = "";
    public string RequestTime
    {
      get { return _requestTime; }
      set
      {
        _requestTime = value;
        OnPropertyChanged("RequestTime");
      }
    }

    private string _confirmWaitTime;
    public string ConfirmWaitTime
    {
      get { return _confirmWaitTime; }
      set
      {
        _confirmWaitTime = value;
        OnPropertyChanged("ConfirmWaitTime");
      }
    }

    private string _controlWaitTime;
    public string ControlWaitTime
    {
      get { return _controlWaitTime; }
      set
      {
        _controlWaitTime = value;
        OnPropertyChanged("ControlWaitTime");
      }
    }

    private string _description = "";
    public string Description
    {
      get { return _description; }
      set
      {
        _description = value;
        OnPropertyChanged("Description");
      }
    }

    public bool IsBEMSControl = false;

    private Timer _confirmWaitTimer;
    private Timer _controlWaitTimer;
    private TimeSpan _confirmWaitTimeSpan;
    private TimeSpan _controlWaitTimeSpan;

    public ControlListItem(ControlRequestMessage controlRequestMessage, int confirmWaitMinutes, int controlWaitSeconds)
    {
      SequenceNumber = controlRequestMessage.seq;
      BuildingID = controlRequestMessage.bd;
      VariableName = controlRequestMessage.nm;
      ControlValue = controlRequestMessage.vl;
      RequestTime = controlRequestMessage.tm;
      Description = controlRequestMessage.des;
      IsBEMSControl = false;

      _confirmWaitTimeSpan = TimeSpan.FromMinutes(confirmWaitMinutes);
      ConfirmWaitTime = _confirmWaitTimeSpan.ToString();
      _confirmWaitTimer = new Timer(1000);
      _confirmWaitTimer.Elapsed += confirmWaitTimer_Elapsed;
      _confirmWaitTimer.Start();

      _controlWaitTimeSpan = TimeSpan.FromSeconds(controlWaitSeconds);
      ControlWaitTime = _controlWaitTimeSpan.ToString();
      _controlWaitTimer = new Timer(1000);
      _controlWaitTimer.Elapsed += controlWaitTimer_Elapsed;
    }

    public ControlListItem(ControlRequestScheme controlRequestScheme, int confirmWaitMinutes, int controlWaitSeconds)
    {
      SequenceNumber = controlRequestScheme.SequenceNumber;
      BuildingID = controlRequestScheme.bldg_id;
      VariableName = controlRequestScheme.itfc_id;
      ControlValue = controlRequestScheme.ctrl_val;
      RequestTime = controlRequestScheme.act_tm;
      Description = controlRequestScheme.ctrl_prmt_nm;
      IsBEMSControl = true;

      _confirmWaitTimeSpan = TimeSpan.FromMinutes(confirmWaitMinutes);
      ConfirmWaitTime = _confirmWaitTimeSpan.ToString();
      _confirmWaitTimer = new Timer(1000);
      _confirmWaitTimer.Elapsed += confirmWaitTimer_Elapsed;
      _confirmWaitTimer.Start();

      _controlWaitTimeSpan = TimeSpan.FromSeconds(controlWaitSeconds);
      ControlWaitTime = _controlWaitTimeSpan.ToString();
      _controlWaitTimer = new Timer(1000);
      _controlWaitTimer.Elapsed += controlWaitTimer_Elapsed;
    }

    public bool StartControlWaitTimer()
    {
      if (_controlWaitTimer.Enabled)
      {
        return false;
      }
      else
      {
        _controlWaitTimer.Start();
        return true;
      }
    }

    private void confirmWaitTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      _confirmWaitTimeSpan = _confirmWaitTimeSpan.Subtract(TimeSpan.FromSeconds(1));
      ConfirmWaitTime = _confirmWaitTimeSpan.ToString();

      if (_confirmWaitTimeSpan.TotalSeconds == 0)
      {
        _confirmWaitTimer.Stop();
      }
    }

    private void controlWaitTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      _controlWaitTimeSpan = _controlWaitTimeSpan.Subtract(TimeSpan.FromSeconds(1));
      ControlWaitTime = _controlWaitTimeSpan.ToString();

      if (_controlWaitTimeSpan.TotalSeconds == 0)
      {
        _controlWaitTimer.Stop();
      }
    }
  }
}

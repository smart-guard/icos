using iCos5.CSPGateway.CSPMessage;
using iCos5.CSPGateway.DB;
using PASoft.Zenon.Addins.Extension;
using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.Variable;
using System;
using System.Timers;

namespace iCos5CSPGatewayRT.Manager
{
  public class OnlineControlService_bems
  {
    public ControlRequestScheme RequestScheme { get; }
    public ControlResponseScheme ResponseScheme { get; private set; }
    public ControlServiceState State { get; private set; } = ControlServiceState.Ready;

    public event EventHandler<ControlServiceCompletedEventArgs> OnCompleted;

    private IProject _zenonProject;
    private IOnlineVariableContainer _onlineContainer;
    private IVariable _variable;
    private bool _isLocalTime;
    private Timer _timeoutTimer;
    private Timer _holdOnTimer;
    private System.Windows.Forms.NotifyIcon _notifyIcon;

    public OnlineControlService_bems(ControlRequestScheme requestScheme,
                                     IProject zenonProject,
                                     string onlineContainerName,
                                     string variableName,
                                     bool isLocalTime,
                                     int timeoutMilliSeconds,
                                     int holdOnMilliSeconds)
    {
      RequestScheme = requestScheme;
      ResponseScheme = new ControlResponseScheme(requestScheme);

      _zenonProject = zenonProject;
      _isLocalTime = isLocalTime;

      if (_zenonProject.OnlineVariableContainerCollection[onlineContainerName] != null)
      {
        _zenonProject.OnlineVariableContainerCollection.Delete(onlineContainerName);
      }

      _onlineContainer = zenonProject.OnlineVariableContainerCollection.Create(onlineContainerName);
      _onlineContainer.AddVariable(variableName);
      _onlineContainer.Changed += onlineContainer_Changed;

      _variable = _zenonProject.VariableCollection[variableName];

      _timeoutTimer = new Timer(timeoutMilliSeconds);
      _timeoutTimer.Elapsed += timeoutTimer_Elapsed;
      _timeoutTimer.AutoReset = false;

      _holdOnTimer = new Timer(holdOnMilliSeconds);
      _holdOnTimer.Elapsed += holdOnTimer_Elapsed;
      _holdOnTimer.AutoReset = false;
      _holdOnTimer.Start();

      State = ControlServiceState.HoldOn;
      initNotifyIcon();
    }

    public OnlineControlService_bems(ControlRequestScheme requestScheme,
                                     IProject zenonProject,
                                     string onlineContainerName,
                                     string variableName,
                                     bool isLocalTime,
                                     int timeoutMilliSeconds)
    {
      RequestScheme = requestScheme;
      ResponseScheme = new ControlResponseScheme(requestScheme);

      _zenonProject = zenonProject;
      _isLocalTime = isLocalTime;

      if (_zenonProject.OnlineVariableContainerCollection[onlineContainerName] != null)
      {
        _zenonProject.OnlineVariableContainerCollection.Delete(onlineContainerName);
      }

      _onlineContainer = zenonProject.OnlineVariableContainerCollection.Create(onlineContainerName);
      _onlineContainer.AddVariable(variableName);
      _onlineContainer.Changed += onlineContainer_Changed;

      _variable = _zenonProject.VariableCollection[variableName];

      _timeoutTimer = new Timer(timeoutMilliSeconds);
      _timeoutTimer.Elapsed += timeoutTimer_Elapsed;
      _timeoutTimer.AutoReset = false;

      _onlineContainer.Activate();
      _variable.SetValue(0, RequestScheme.Value);
      _timeoutTimer.Start();

      State = ControlServiceState.Control;
      initNotifyIcon();
    }

    public OnlineControlService_bems(ControlRequestScheme requestScheme,
                                     bool isLocalTime, 
                                     ControlResponseCode controlResponseCode)
    {
      RequestScheme = requestScheme;
      ResponseScheme = new ControlResponseScheme(requestScheme, controlResponseCode);

      _isLocalTime = isLocalTime;

      _holdOnTimer = new Timer(1000);
      _holdOnTimer.Elapsed += holdOnSkipTimer_Elapsed;
      _holdOnTimer.AutoReset = false;
      _holdOnTimer.Start();

      State = ControlServiceState.Skip;
      initNotifyIcon();
    }

    ~OnlineControlService_bems()
    {
      _notifyIcon.Visible = false;

      if (_timeoutTimer != null)
      {
        _timeoutTimer.Stop();
        _timeoutTimer.Elapsed -= timeoutTimer_Elapsed;
        _timeoutTimer.Dispose();
        _timeoutTimer = null;
      }

      if (_holdOnTimer != null)
      {
        _holdOnTimer.Stop();
        _holdOnTimer.Elapsed -= holdOnTimer_Elapsed;
        _holdOnTimer.Elapsed -= holdOnSkipTimer_Elapsed;
        _holdOnTimer.Dispose();
        _holdOnTimer = null;
      }

      if (_onlineContainer != null)
      {
        _onlineContainer.Deactivate();
        _onlineContainer.Changed -= onlineContainer_Changed;
        _zenonProject.OnlineVariableContainerCollection.Delete(_onlineContainer.Name);
        _onlineContainer = null;
      }
    }

    public void ConfirmSetValue()
    {
      _holdOnTimer.Stop();
      _onlineContainer.Activate();
      _variable.SetValue(0, RequestScheme.Value);
      _timeoutTimer.Start();
      State = ControlServiceState.Control;
    }

    public void RejectSetValue()
    {
      _holdOnTimer.Stop();
      State = ControlServiceState.Reject;
      ResponseScheme.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
      ResponseScheme.SetStringCode(ControlResponseCode.Reject);
      onCompleted();
    }

    private void onlineContainer_Changed(object sender, ChangedEventArgs e)
    {
      if (RequestScheme.DecimalValue == Convert.ToDecimal(e.Variable.GetValue(0)))
      {
        _timeoutTimer.Stop();
        _onlineContainer.Deactivate();
        DateTime lastUpdateTime = e.Variable.Get_LastUpdateTimeWithMilliSeconds(_isLocalTime);

        if (lastUpdateTime < new DateTime(2000, 1, 1))
        {
          lastUpdateTime = _isLocalTime ? DateTime.Now : DateTime.Now.ToUniversalTime();
        }

        ResponseScheme.UpdateTime = lastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        onCompleted();
      }
    }

    private void timeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      _timeoutTimer.Stop();
      _onlineContainer.Deactivate();

      if ((_variable.Get_StatusValue() & 0x40000) == 0)
      {
        ResponseScheme.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        ResponseScheme.SetStringCode(ControlResponseCode.TimeOut);
      }
      else
      {
        ResponseScheme.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        ResponseScheme.SetStringCode(ControlResponseCode.DeviceError);
      }

      onCompleted();
    }

    private void holdOnTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      _holdOnTimer.Stop();
      ResponseScheme.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
      ResponseScheme.SetStringCode(ControlResponseCode.Missing);
      onCompleted();
    }

    private void holdOnSkipTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      _holdOnTimer.Stop();
      ResponseScheme.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
      onCompleted();
    }

    private void onCompleted()
    {
      State = ControlServiceState.Response;
      OnCompleted?.Invoke(this, new ControlServiceCompletedEventArgs(ResponseScheme));

      switch (ResponseScheme.Message)
      {
        case "SUCCESS_OK":
          _notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
          break;
        case "SUCCESS_REJECT":
        case "SUCCESS_MISSING":
        case "FAIL_DEACTIVATION":
          _notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
          break;
        default:
          _notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Error;
          break;
      }

      _notifyIcon.BalloonTipTitle = $"{RequestScheme.itfc_id} 제어";
      _notifyIcon.BalloonTipText = $"제어값:{RequestScheme.ctrl_val}{Environment.NewLine}제어시간:{ResponseScheme.UpdateTime}{Environment.NewLine}제어결과코드:{ResponseScheme.Message}";
      _notifyIcon.ShowBalloonTip(200);
    }

    private void initNotifyIcon()
    {
      _notifyIcon = new System.Windows.Forms.NotifyIcon();
      _notifyIcon.Icon = RTResources.HDCLabs_H_L;
      string msgText = $"관제점:{RequestScheme.itfc_id}{Environment.NewLine}제어값:{RequestScheme.ctrl_val}{Environment.NewLine}요청시간:{RequestScheme.act_tm}";
      _notifyIcon.Text = msgText.Length > 63 ? msgText.Substring(0, 63) : msgText;
      _notifyIcon.Visible = true;
    }
  }
}

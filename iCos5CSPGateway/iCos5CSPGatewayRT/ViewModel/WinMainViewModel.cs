using iCos5.CSPGateway;
using iCos5.CSPGateway.MVVM;
using iCos5CSPGatewayRT.Manager;
using PASoft.UI.WPF;

namespace iCos5CSPGatewayRT.ViewModel
{
  public class WinMainViewModel : NotifyBase
  {
    private bool _showWinMainWithSound = false;
    public bool ShowWinMainWithSound
    {
      get { return _showWinMainWithSound; }
      set
      {
        _showWinMainWithSound = value;
        OnPropertyChanged("ShowWinMainWithSound");
      }
    }

    private ControlListItem _addControlListItem = null;
    public ControlListItem AddControlListItem
    {
      get { return _addControlListItem; }
      set
      {
        _addControlListItem = value;
        OnPropertyChanged("AddControlListItem");
      }
    }

    private int _removeControlListSequenceNumber = -1;
    public int RemoveControlListSequenceNumber
    {
      get { return _removeControlListSequenceNumber; }
      set
      {
        _removeControlListSequenceNumber = value;
        OnPropertyChanged("RemoveControlListSequenceNumber");
      }
    }

    public GatewayConfig Config { get; set; }
    public ObservableCollectionEx<ControlListItem> ControlList { get; set; }

    private CSPManager _cspManager;

    public WinMainViewModel(GatewayConfig gatewayConfig, CSPManager cspManager)
    {
      Config = gatewayConfig;
      ControlList = new ObservableCollectionEx<ControlListItem>();

      _cspManager = cspManager;
      _cspManager.ShowWindowTriggered += cspManager_ShowWindowTriggered;
      _cspManager.AddControlListItemTriggered += cspManager_AddControlListItemTriggered;
      _cspManager.RemoveControlListItemTriggered += cspManager_RemoveControlListItemTriggered;
    }

    public bool CSPManagerControlConfirm(ControlListItem controlListItem)
    {
      if (controlListItem.IsBEMSControl)
      {
        return _cspManager.ConfirmOnlineControlServiceBems(controlListItem.SequenceNumber);
      }
      else
      {
        return _cspManager.ConfirmOnlineControlService(controlListItem.SequenceNumber);
      }
    }

    public bool CSPManagerControlReject(ControlListItem controlListItem)
    {
      if (controlListItem.IsBEMSControl)
      {
        return _cspManager.RejectOnlineControlServiceBems(controlListItem.SequenceNumber);
      }
      else
      {
        return _cspManager.RejectOnlineControlService(controlListItem.SequenceNumber);
      }
    }

    private void cspManager_ShowWindowTriggered(object sender, ControlBooleanEventArgs e)
    {
      ShowWinMainWithSound = e.Value;
    }

    private void cspManager_AddControlListItemTriggered(object sender, ControlListItemEventArgs e)
    {
      AddControlListItem = e.Value;
    }

    private void cspManager_RemoveControlListItemTriggered(object sender, ControlIntegerEventArgs e)
    {
      RemoveControlListSequenceNumber = e.Value;
    }
  }
}

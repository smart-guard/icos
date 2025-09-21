using iCos5.CSPGateway;
using iCos5.CSPGateway.MVVM;
using iCos5CSPGatewayED.View;
using PASoft.Common.Serialization;
using Scada.AddIn.Contracts;
using System.IO;

namespace iCos5CSPGatewayED.ViewModel
{
  public class WinMainViewModel : NotifyBase
  {
    public bool IsLock { get; set; } = true;
    public GatewayConfig Config { get; set; }

    public ViewCloud CloudView { get; set; }
    public ViewCommon CommonView { get; set; }
    public ViewValue ValueView { get; set; }
    public ViewAlarm AlarmView { get; set; }
    public ViewControl ControlView { get; set; }

    public WinMainViewModel(IProject zenonProject, string configPath)
    {
      Config = File.Exists(configPath) ? (GatewayConfig)Json.LoadFile(configPath, typeof(GatewayConfig), FileShare.Read) : new GatewayConfig();

      CloudView = new ViewCloud() { DataContext = Config };
      CommonView = new ViewCommon(zenonProject) { DataContext = Config };
      ValueView = new ViewValue(zenonProject) { DataContext = Config };
      AlarmView = new ViewAlarm() { DataContext = Config };
      ControlView = new ViewControl() { DataContext = Config };
    }
  }
}
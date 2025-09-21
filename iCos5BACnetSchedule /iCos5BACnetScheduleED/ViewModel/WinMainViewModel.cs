using iCos5.BACnet.MVVM;
using iCos5.BACnet.Schedule;
using iCos5BACnetScheduleED.View;
using PASoft.Common.Serialization;
using Scada.AddIn.Contracts;
using System.IO;

namespace iCos5BACnetScheduleED.ViewModel
{
  public class WinMainViewModel : NotifyBase
  {
    public bool IsLock { get; set; } = true;
    public ScheduleConfig Config { get; set; }

    public ViewConfig ConfigView { get; set; }

    public WinMainViewModel(WinMain winMain, IProject zenonProject, string configPath)
    {
      Config = File.Exists(configPath) ? (ScheduleConfig)Json.LoadFile(configPath, typeof(ScheduleConfig), FileShare.Read) : new ScheduleConfig();

      ConfigView = new ViewConfig(winMain, zenonProject) { DataContext = Config };
    }
  }
}
using iCos5.BACnet.Schedule;
using iCos5BACnetScheduleRT.ViewModel;
using PASoft.Zenon.Addins;
using Scada.AddIn.Contracts.Variable;
using System.Windows;
using System.Windows.Controls;

namespace iCos5BACnetScheduleRT.View
{
  /// <summary>
  /// ViewExceptionSchedule.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class ViewExceptionSchedule : UserControl
  {
    private WinMainViewModel _winMainViewModel;
    private ScheduleConfig _config;
    private CelLogging _celLogging;

    public ViewExceptionSchedule(WinMainViewModel winMainViewModel, ScheduleConfig scheduleConfig, IOnlineVariableContainer onlineContainer, CelLogging celLogging)
    {
      InitializeComponent();

      _winMainViewModel = winMainViewModel;
      _config = scheduleConfig;
      _onlineContainer = onlineContainer;
      _celLogging = celLogging;
    }

    public void SetVariable(IVariable variable)
    {
      _variable = variable;
    }

    private void loggingError(string code, string message)
    {
      if (_celLogging.Enable)
      {
        _celLogging.Error($"[ViewExceptionSchedule][{code}]{message}");
      }
      else
      {
        MessageBox.Show($"[ViewExceptionSchedule][{code}]{message}");
      }
    }
  }
}

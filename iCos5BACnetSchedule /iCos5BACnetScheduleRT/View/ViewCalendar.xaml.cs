using iCos5.BACnet.Schedule;
using PASoft.Zenon.Addins;
using Scada.AddIn.Contracts.Variable;
using System.Windows;
using System.Windows.Controls;

namespace iCos5BACnetScheduleRT.View
{
  /// <summary>
  /// ViewCalendar.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class ViewCalendar : UserControl
  {
    private ScheduleConfig _config;
    private CelLogging _celLogging;

    public ViewCalendar(ScheduleConfig scheduleConfig, IOnlineVariableContainer onlineContainer, IVariable variable, CelLogging celLogging)
    {
      InitializeComponent();

      _config = scheduleConfig;
      //_onlineContainer = onlineContainer;
      //_variable = variable;
      _celLogging = celLogging;
    }

    private void loggingError(string code, string message)
    {
      if (_celLogging.Enable)
      {
        _celLogging.Error($"[ViewCalendar][{code}]{message}");
      }
      else
      {
        MessageBox.Show($"[ViewCalendar][{code}]{message}");
      }
    }
  }
}

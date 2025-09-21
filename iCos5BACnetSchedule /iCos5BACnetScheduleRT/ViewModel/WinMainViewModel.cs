using iCos5.BACnet.MVVM;
using iCos5.BACnet.Schedule;
using iCos5.BACnet.Zenon;
using iCos5BACnetScheduleRT.View;
using PASoft.Zenon.Addins;
using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.Variable;
using System.Windows;

namespace iCos5BACnetScheduleRT.ViewModel
{
  public class WinMainViewModel : NotifyBase
  {
    private string _scheduleDescription = "";
    public string ScheduleDescription
    {
      get
      {
        return _scheduleDescription;
      }
      set
      {
        _scheduleDescription = value;
        OnPropertyChanged(nameof(ScheduleDescription));
      }
    }

    private ViewWeeklySchedule _weeklyScheduleView;
    public ViewWeeklySchedule WeeklyScheduleView
    {
      get
      {
        return _weeklyScheduleView;
      }
      set
      {
        _weeklyScheduleView = value;
        OnPropertyChanged(nameof(WeeklyScheduleView));
      }
    }

    private ViewExceptionSchedule _exceptionScheduleView;
    public ViewExceptionSchedule ExceptionScheduleView
    {
      get
      {
        return _exceptionScheduleView;
      }
      set
      {
        _exceptionScheduleView = value;
        OnPropertyChanged(nameof(ExceptionScheduleView));
      }
    }

    private bool _isOnlineVariableError = false;
    public bool IsOnlineVariableError
    {
      get
      {
        return _isOnlineVariableError;
      }
      set
      {
        _isOnlineVariableError = value;
        OnPropertyChanged("IsOnlineVariableError");
      }
    }

    private ScheduleConfig _scheduleConfig;
    private IProject _zenonProject;
    private CelLogging _celLogging;

    public WinMainViewModel(ScheduleConfig scheduleConfig,
                            IProject zenonProject,
                            IOnlineVariableContainer weeklyOnlineContainer,
                            IOnlineVariableContainer exceptionOnlineContainer,
                            CelLogging celLogging)
    {
      _scheduleConfig = scheduleConfig;
      _zenonProject = zenonProject;
      _celLogging = celLogging;

      WeeklyScheduleView = new ViewWeeklySchedule(this, _scheduleConfig, weeklyOnlineContainer, _celLogging);

      if (_scheduleConfig.IsActiveExceptionTab)
      {
        ExceptionScheduleView = new ViewExceptionSchedule(this, _scheduleConfig, exceptionOnlineContainer, _celLogging);
      }
    }

    internal void ClearScheduleItems()
    {
      _weeklyScheduleView.ClearValue();
      _exceptionScheduleView.ClearValue();
    }

    public void SetParameter(DialogueParameter dialogueParameter)
    {
      ScheduleDescription = dialogueParameter.ToString();
      ScheduleVariable scheduleVariable = _scheduleConfig.GetScheduleVariable(dialogueParameter);

      if (scheduleVariable == null)
      {
        MessageBox.Show($"[{dialogueParameter}] 스케줄 변수가 등록되지 않았습니다.", ScheduleConfig.Constants.SolutionName);
        return;
      }

      if (_zenonProject.VariableCollection[scheduleVariable.Weekly] != null)
      {
        WeeklyScheduleView.SetVariable(_zenonProject.VariableCollection[scheduleVariable.Weekly]);
      }

      if (_scheduleConfig.IsActiveExceptionTab && _zenonProject.VariableCollection[scheduleVariable.Exception] != null)
      {
        ExceptionScheduleView.SetVariable(_zenonProject.VariableCollection[scheduleVariable.Exception]);
      }
    }
  }
}
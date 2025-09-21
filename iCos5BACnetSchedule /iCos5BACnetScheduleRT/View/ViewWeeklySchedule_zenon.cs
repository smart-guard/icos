using iCos5.BACnet.Zenon;
using PASoft.Zenon.Addins.Extension;
using Scada.AddIn.Contracts.Variable;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace iCos5BACnetScheduleRT.View
{
  public partial class ViewWeeklySchedule : UserControl
  {
    public bool IsProgress
    {
      get { return (bool)GetValue(IsProgressProperty); }
      set { SetValue(IsProgressProperty, value); }
    }

    public static readonly DependencyProperty IsProgressProperty =
      DependencyProperty.Register("IsProgress", typeof(bool), typeof(ViewWeeklySchedule),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    private void setIsProgress()
    {
      IsProgress = true;
    }

    private void resetIsProgress()
    {
      IsProgress = false;
    }

    private IOnlineVariableContainer _onlineContainer;
    private IVariable _variable;
    private int _prevIndex = 0;

    public void ClearValue()
    {
      ((WeeklySchedule)DataContext)?.ClearScheduleSet();
    }

    public void LoadedValue()
    {
      if (_variable == null)
      {
        return;
      }

      updateValue();
    }

    private void updateValue()
    {
      setIsProgress();
      _onlineContainer.Deactivate();
      _onlineContainer.AddVariable(_variable.Name);
      _onlineContainer.Changed += _onlineContainer_Changed;

      if (_onlineContainer.Count > 0)
      {
        _onlineContainer.Activate();
      }
    }

    private void writeValue(string code, int prevIndex)
    {
      _variable.SetValue(0, code);
      _prevIndex = prevIndex;
      _celLogging.Info($"[{_variable.Name}]Update Weekly Schedule");
      updateValue();
    }

    private void updateInvoke(string code, bool isInvalid)
    {
      if (Dispatcher.CheckAccess())
      {
        DataContext = new WeeklySchedule(code);
        DailySchedulesGrid.CurrentCell = new DataGridCellInfo(DailySchedulesGrid.Items[0], DailySchedulesGrid.Columns[_prevIndex]);
        DailySchedulesGrid.SelectedCells.Add(DailySchedulesGrid.CurrentCell);
        _prevIndex = 0;

        resetIsProgress();

        if (isInvalid)
        {
          if (!_winMainViewModel.IsOnlineVariableError)
          {
            _winMainViewModel.IsOnlineVariableError = true;
          }
        }
      }
      else
      {
        Dispatcher.BeginInvoke(new Action<string, bool>(updateInvoke), code, isInvalid);
      }
    }

    private void _onlineContainer_Changed(object sender, ChangedEventArgs e)
    {
      try
      {
        if (e.Variable.Equals(_variable))
        {
          string code = e.Variable.GetValue(0).ToString();
          bool isInvalid = (e.Variable.Get_StatusValue() & 0x40000) != 0;
          unloadValue();

          Task.Run(() => { updateInvoke(code, isInvalid); });
        }
      }
      catch (Exception ex)
      {
        loggingError("_onlineContainer_Changed", ex.ToString());
        resetIsProgress();
      }
    }

    private void unloadValue()
    {
      _onlineContainer.Deactivate();
      _onlineContainer.Changed -= _onlineContainer_Changed;
      _onlineContainer.Remove(_variable.Name);
    }
  }
}

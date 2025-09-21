using iCos5.BACnet.Schedule;
using iCos5BACnetScheduleRT.View;
using PASoft.Common.Serialization;
using PASoft.Zenon;
using PASoft.Zenon.Addins;
using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.Variable;
using System;
using System.IO;
using System.Windows;

namespace iCos5BACnetScheduleRT
{
  /// <summary>
  /// Description of Project Service Extension.
  /// </summary>
  [AddInExtension("BACnet Schedule Viewer", "BACnet Schedule viewer", DefaultStartMode = DefaultStartupModes.Auto)]
  public class ProjectServiceExtension : IProjectServiceExtension
  {
    private IProject _zenonProject;
    private IVariable _dialogParam;
    private IOnlineVariableContainer _dialogParamOnline;
    private CelLogging _celLogging;
    private ScheduleConfig _config;
    private WinMain _winMain;

    public void Start(IProject context, IBehavior behavior)
    {
      try
      {
        _zenonProject = context;

        string wizardDir = Path.Combine(ZenonPath.DirRuntimeOthers(_zenonProject, ZenonType.Runtime), ScheduleConfig.Constants.RootName);
        string configPath = Path.Combine(wizardDir, $"{ScheduleConfig.Constants.SolutionName}_config.json");

        if (File.Exists(configPath))
        {
          _config = (ScheduleConfig)Json.LoadFile(configPath, typeof(ScheduleConfig));
        }

        _celLogging = new CelLogging(_zenonProject, "", _config.CELGroupPvID, _config.CELClassPvID);
        _celLogging.Enable = _config.EnableCEL;

        if (_zenonProject.OnlineVariableContainerCollection[ScheduleConfig.Constants.DialogueParameterOnlineContainer] != null)
        {
          _zenonProject.OnlineVariableContainerCollection.Delete(ScheduleConfig.Constants.DialogueParameterOnlineContainer);
        }

        if (_zenonProject.OnlineVariableContainerCollection[ScheduleConfig.Constants.WeeklyScheduleOnlineContainer] != null)
        {
          _zenonProject.OnlineVariableContainerCollection.Delete(ScheduleConfig.Constants.WeeklyScheduleOnlineContainer);
        }

        if (_zenonProject.OnlineVariableContainerCollection[ScheduleConfig.Constants.ExceptionScheduleOnlineContainer] != null)
        {
          _zenonProject.OnlineVariableContainerCollection.Delete(ScheduleConfig.Constants.ExceptionScheduleOnlineContainer);
        }

        _dialogParamOnline = _zenonProject.OnlineVariableContainerCollection.Create(ScheduleConfig.Constants.DialogueParameterOnlineContainer);

        _winMain = new WinMain(_config, 
                               _zenonProject,
                               _zenonProject.OnlineVariableContainerCollection.Create(ScheduleConfig.Constants.WeeklyScheduleOnlineContainer),
                               _zenonProject.OnlineVariableContainerCollection.Create(ScheduleConfig.Constants.ExceptionScheduleOnlineContainer), 
                               _celLogging);

        _dialogParam = _zenonProject.VariableCollection[ScheduleConfig.Constants.DialogueParameterVariable];
        _dialogParamOnline.AddVariable(_dialogParam.Name);
        _dialogParamOnline.Changed += _dialogParamOnline_Changed;

        _dialogParamOnline.Activate();
      }
      catch (Exception ex)
      {
        if (_celLogging.Enable)
        {
          _celLogging.Error($"[Start]{ex}");
        }
        else
        {
          MessageBox.Show($"[Start]{ex}");
        }
      }
    }

    public void Stop()
    {
      try
      {
        if (_winMain != null)
        {
          _winMain.Close();
          _winMain = null;
          GC.Collect();
        }

        _dialogParamOnline.Deactivate();
        _dialogParamOnline.Changed -= _dialogParamOnline_Changed;
        _zenonProject.OnlineVariableContainerCollection.Delete(ScheduleConfig.Constants.DialogueParameterOnlineContainer);
        _zenonProject.OnlineVariableContainerCollection.Delete(ScheduleConfig.Constants.WeeklyScheduleOnlineContainer);
        _zenonProject.OnlineVariableContainerCollection.Delete(ScheduleConfig.Constants.ExceptionScheduleOnlineContainer);
      }
      catch (Exception ex)
      {
        if (_celLogging.Enable)
        {
          _celLogging.Error($"[Stop]{ex}");
        }
        else
        {
          MessageBox.Show($"[Stop]{ex}");
        }
      }
    }

    private void _dialogParamOnline_Changed(object sender, ChangedEventArgs e)
    {
      string strParam = e.Variable.GetValue(0).ToString();

      if (strParam.Equals(""))
      {
        return;
      }

      try
      {
        _winMain.ShowDialog(new DialogueParameter(strParam));
      }
      catch (Exception ex)
      {
        if (_celLogging.Enable)
        {
          _celLogging.Error($"[_dialogParamOnline_Changed]{ex}");
        }
        else
        {
          MessageBox.Show($"[_dialogParamOnline_Changed]{ex}");
        }
      }

      e.Variable.SetValue(0, "");
    }
  }
}
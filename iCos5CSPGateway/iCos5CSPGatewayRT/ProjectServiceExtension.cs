using iCos5.CSPGateway;
using iCos5CSPGatewayRT.Manager;
using iCos5CSPGatewayRT.View;
using PASoft.Common.Serialization;
using PASoft.Zenon;
using PASoft.Zenon.Addins;
using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.Variable;
using System;
using System.IO;
using System.Windows;

namespace iCos5CSPGatewayRT
{
  /// <summary>
  /// Description of Project Service Extension.
  /// </summary>
  [AddInExtension("Cloud Gateway", "Cloud Gateway Service of AWS Communication", DefaultStartMode = DefaultStartupModes.Auto)]
  public class ProjectServiceExtension : IProjectServiceExtension
  {
    private IProject _zenonProject;
    private IVariable _dialogParam;
    private IOnlineVariableContainer _dialogParamOnline;
    private GatewayConfig _config;
    private CelLogging _celLogging;
    private WinMain _winMain;
    private CSPManager _cspManager;

    public void Start(IProject context, IBehavior behavior)
    {
      try
      {
        _zenonProject = context;

        string wizardDir = Path.Combine(ZenonPath.DirRuntimeOthers(_zenonProject, ZenonType.Runtime), GatewayConfig.Constants.RootName);
        string configPath = Path.Combine(wizardDir, $"{GatewayConfig.Constants.SolutionName}_config.json");

        if (File.Exists(configPath))
        {
          _config = (GatewayConfig)Json.LoadFile(configPath, typeof(GatewayConfig));
        }

        _celLogging = new CelLogging(_zenonProject, "", _config.CELGroupPvID, _config.CELClassPvID);
        _celLogging.Enable = _config.EnableCEL;

        if (_zenonProject.OnlineVariableContainerCollection[GatewayConfig.Constants.DialogueOnlineContainer] != null)
        {
          _zenonProject.OnlineVariableContainerCollection.Delete(GatewayConfig.Constants.DialogueOnlineContainer);
        }

        _dialogParamOnline = _zenonProject.OnlineVariableContainerCollection.Create(GatewayConfig.Constants.DialogueOnlineContainer);

        _cspManager = new CSPManager(_zenonProject, _config, _celLogging);
        _winMain = new WinMain(_config, _zenonProject, _celLogging, _cspManager);

        _dialogParam = _zenonProject.VariableCollection[GatewayConfig.Constants.DialogueVariable];
        _dialogParamOnline.AddVariable(_dialogParam.Name);
        _dialogParamOnline.Changed += dialogParamOnline_Changed;

        _dialogParamOnline.Activate();

        if (_celLogging.Enable)
        {
          _celLogging.Info($"[Start] Ready Cloud Manager.");
        }
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
          _winMain.CloseWinMain();
        }

        _dialogParamOnline.Deactivate();
        _dialogParamOnline.Changed -= dialogParamOnline_Changed;
        _zenonProject.OnlineVariableContainerCollection.Delete(GatewayConfig.Constants.DialogueOnlineContainer);

        _cspManager.BeforeFree();
        _cspManager.Dispose();
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

    private void dialogParamOnline_Changed(object sender, ChangedEventArgs e)
    {
      string strParam = e.Variable.GetValue(0).ToString();

      if (!strParam.Equals(GatewayConfig.Constants.DialogueFunctionParam))
      {
        return;
      }

      try
      {
        _winMain.ShowDialogue(false);
      }
      catch (Exception ex)
      {
        if (_celLogging.Enable)
        {
          _celLogging.Error($"[dialogParamOnline_Changed]{ex}");
        }
        else
        {
          MessageBox.Show($"[dialogParamOnline_Changed]{ex}");
        }
      }

      e.Variable.SetValue(0, "");
    }
  }
}
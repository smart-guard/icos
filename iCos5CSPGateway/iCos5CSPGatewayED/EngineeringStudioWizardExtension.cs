using iCos5.CSPGateway;
using iCos5CSPGatewayED.View;
using PASoft.Zenon.Addins;
using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.Function;
using Scada.AddIn.Contracts.Variable;
using System;
using System.IO;
using System.Windows;

namespace iCos5CSPGatewayED
{
  /// <summary>
  /// Engineering Studio Wizard Extension with WPF
  /// </summary>
  [AddInExtension("Cloud Gateway Wizard", "This wizard is configuration of HDCLabs cloud interface", "HDC Labs")]
  public class EngineeringStudioWizardExtension : IEditorWizardExtension
  {
    #region IEditorWizardExtension implementation
    /// <summary>
    /// Method which is executed on starting the SCADA Engineering Studio Wizard
    /// </summary>
    /// <param name="context">SCADA Engineering Studio application object</param>
    /// <param name="behavior">For future use</param>
    public void Run(IEditorApplication context, IBehavior behavior)
    {
      try
      {
        if (context.Workspace.ActiveProject == null)
        {
          MessageBox.Show($"활성화된 프로젝트가 없습니다.{Environment.NewLine}워크스페이스에서 프로젝트를 활성화 시키십시오!", GatewayConfig.Constants.SolutionNewName);
          return;
        }

        IProject zenonProject = context.Workspace.ActiveProject;
        string addinsDir = ZenonPath.DirEditorAddin(zenonProject);
        string rtAddinsName = $"{GatewayConfig.Constants.RootName}{GatewayConfig.Constants.SolutionName}RT.scadaAddIn";

        if (!Directory.Exists(addinsDir))
        {
          Directory.CreateDirectory(addinsDir);
        }

        if (!File.Exists(Path.Combine(addinsDir, rtAddinsName)))
        {
          if (MessageBox.Show($"{GatewayConfig.Constants.SolutionNewName}가 설치되지 않았습니다.{Environment.NewLine}새로 설치하시겠습니까?", GatewayConfig.Constants.SolutionNewName, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
          {
            File.WriteAllBytes(Path.Combine(addinsDir, rtAddinsName), EDResources.RuntimeAddins);

            if (zenonProject.VariableCollection[GatewayConfig.Constants.DialogueVariable] == null)
            {
              IVariable dialogueParameterVariable = zenonProject.VariableCollection.Create(GatewayConfig.Constants.DialogueVariable,
                                                                                           getZenonDriver(zenonProject, "Intern"),
                                                                                           ChannelType.SystemDriverVariable,
                                                                                           zenonProject.DataTypeCollection["STRING"]);
              dialogueParameterVariable.StringLength = 65535;
            }

            IFunction function = zenonProject.FunctionCollection[GatewayConfig.Constants.DialogueFunction];

            if (function == null)
            {
              function = zenonProject.FunctionCollection.Create(GatewayConfig.Constants.DialogueFunction, FunctionType.WriteSetValue);
            }
            else if (function.Type != FunctionType.WriteSetValue)
            {
              zenonProject.FunctionCollection.Delete(GatewayConfig.Constants.DialogueFunction);
              function = zenonProject.FunctionCollection.Create(GatewayConfig.Constants.DialogueFunction, FunctionType.WriteSetValue);
            }

            function.SetDynamicProperty("SetValue.Variable", GatewayConfig.Constants.DialogueVariable);
            function.SetDynamicProperty("SetValue.StrValue", GatewayConfig.Constants.DialogueFunctionParam);
            function.SetDynamicProperty("SetValue.IsDirect", true);
          }
          else
          {
            return;
          }
        }

        string wizardDir = Path.Combine(ZenonPath.DirEditorOthers(zenonProject), GatewayConfig.Constants.RootName);

        if (!Directory.Exists(wizardDir))
        {
          Directory.CreateDirectory(wizardDir);
        }

        WinSplash splashWin = new WinSplash();
        splashWin.Show();

        WinMain mainWin = new WinMain(splashWin, zenonProject,
                                      Path.Combine(addinsDir, rtAddinsName),
                                      Path.Combine(wizardDir, $"{GatewayConfig.Constants.SolutionName}_config.json"));
        mainWin.ShowDialog();
      }
      catch (Exception ex)
      {
        MessageBox.Show($"[Run]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }

    private IDriver getZenonDriver(IProject zenonProject, string drvName)
    {
      foreach (IDriver driver in zenonProject.DriverCollection)
      {
        if (driver.Name.Contains(drvName))
        {
          return driver;
        }
      }

      return null;
    }
    #endregion
  }
}
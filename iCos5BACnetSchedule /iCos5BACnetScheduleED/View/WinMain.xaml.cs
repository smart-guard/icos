using iCos5.BACnet.Schedule;
using iCos5BACnetScheduleED.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PASoft.Common.Serialization;
using PASoft.Zenon.Addins.Extension;
using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.Variable;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace iCos5BACnetScheduleED.View
{
  /// <summary>
  /// WinMain.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class WinMain : MetroWindow
  {
    private WinSplash _splash;
    private IProject _zenonProject;
    private string _addinPath;
    private string _configPath;
    private ScheduleConfig _config;

    public WinMain(WinSplash splash, IProject zenonProject, string addinPath, string configPath)
    {
      InitializeComponent();

      _splash = splash;
      _zenonProject = zenonProject;
      _addinPath = addinPath;
      _configPath = configPath;

      Title = $"{ScheduleConfig.Constants.RootName} BACnet Schedule Ver {ScheduleConfig.Constants.SolutionVersion} - {_zenonProject.Name}";
      DataContext = new WinMainViewModel(this, _zenonProject, _configPath);
      _config = ((WinMainViewModel)DataContext).Config;
    }

    private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
    {
      _splash.Close();
      _zenonProject.Parent.Parent.BringWindowToTop();
      HamburgerMenuControl.SelectedIndex = 0;

      Binding showProgressDialogueBinding = new Binding("ConfigView.IsProgress");
      showProgressDialogueBinding.Source = DataContext;
      showProgressDialogueBinding.Mode = BindingMode.OneWay;
      BindingOperations.SetBinding(thisWindows, ShowProgressDialogueProperty, showProgressDialogueBinding);
    }

    private async void HamburgerMenuControl_ItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
    {
      try
      {
        if (e.IsItemOptions)
        {
          int preIndex = HamburgerMenuControl.SelectedIndex;

          switch ((string)((HamburgerMenuItem)e.InvokedItem).ToolTip)
          {
            case "Reinstall":
              MessageDialogResult resultReinstall = await this.ShowMessageAsync("패키지 재설치",
                                                                                $"{ScheduleConfig.Constants.RootName} {ScheduleConfig.Constants.SolutionName} 모듈을 다시 설치 하시겠습니까?",
                                                                                MessageDialogStyle.AffirmativeAndNegative,
                                                                                new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Theme });

              if (resultReinstall == MessageDialogResult.Affirmative)
              {
                File.WriteAllBytes(_addinPath, EDResources.RuntimeAddins);

                if (_zenonProject.VariableCollection[ScheduleConfig.Constants.DialogueParameterVariable] == null)
                {
                  IVariable dialogueParameterVariable = _zenonProject.VariableCollection.Create(ScheduleConfig.Constants.DialogueParameterVariable,
                                                                                                getZenonDriver(_zenonProject, "Intern"),
                                                                                                ChannelType.SystemDriverVariable,
                                                                                                _zenonProject.DataTypeCollection["STRING"]);
                  dialogueParameterVariable.StringLength = 65535;
                }
              }

              break;
            case "Reset":
              MessageDialogResult resultInit = await this.ShowMessageAsync("설정파일초기화",
                                                                           $"{ScheduleConfig.Constants.RootName} {ScheduleConfig.Constants.SolutionName} 설정을 초기화 하시겠습니까?",
                                                                           MessageDialogStyle.AffirmativeAndNegative,
                                                                           new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Theme });

              if (resultInit == MessageDialogResult.Affirmative)
              {
                _config.Reset();
              }

              break;
            case "Save":
              MessageDialogResult resultSave = await this.ShowMessageAsync("설정파일저장",
                                                                           $"{ScheduleConfig.Constants.RootName} {ScheduleConfig.Constants.SolutionName} 설정을 저장하시겠습니까?",
                                                                           MessageDialogStyle.AffirmativeAndNegative,
                                                                           new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Accented });

              if (resultSave == MessageDialogResult.Affirmative)
              {
                _config.CELGroupPvID = _zenonProject.AlarmGroupsCollection().GetpvIdByName(_config.CELGroupName);
                Json.SaveFormattedFile(_config, _configPath);
              }

              break;
            case "About":
            default:
              await this.ShowMessageAsync($"{ScheduleConfig.Constants.RootName} {ScheduleConfig.Constants.SolutionName} Ver {ScheduleConfig.Constants.SolutionVersion}",
                                          " © 2023 HDC Labs All rights reserved.",
                                          MessageDialogStyle.Affirmative,
                                          new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
              break;
          }

          HamburgerMenuControl.SelectedIndex = preIndex;
        }
        else
        {
          HamburgerMenuControl.Content = e.InvokedItem;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"[HamburgerMenuControl_ItemInvoked]{ex}", ScheduleConfig.Constants.SolutionName);
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

    private void TextEdit_Click(object sender, RoutedEventArgs e)
    {
      if (File.Exists(_configPath))
        Process.Start(@"C:\Windows\notepad.exe", _configPath);
      else
        MessageBox.Show($@"설정 파일이 없습니다. 저장 후 사용하십시오.{_configPath}");
    }

    private void LockUnlock_Click(object sender, RoutedEventArgs e)
    {
      WinMainViewModel viewModel = (WinMainViewModel)DataContext;
      bool isVisible = viewModel.IsLock;
      viewModel.IsLock = !viewModel.IsLock;
      string resouceName = viewModel.IsLock ? "lock_fillW" : "unlock_fillW";
      LockUnlockImage.Source = (ImageSource)Resources[resouceName];

      ((HamburgerMenuItemCollection)HamburgerMenuControl.OptionsItemsSource)[0].IsVisible = isVisible;
      ((HamburgerMenuItemCollection)HamburgerMenuControl.OptionsItemsSource)[1].IsVisible = isVisible;

      viewModel.ConfigView.IsLock = viewModel.IsLock;
    }
  }
}

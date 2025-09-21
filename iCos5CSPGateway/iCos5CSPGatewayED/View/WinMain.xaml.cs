using iCos5.CSPGateway;
using iCos5CSPGatewayED.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PASoft.Common.Serialization;
using PASoft.Zenon.Addins.Extension;
using Scada.AddIn.Contracts;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace iCos5CSPGatewayED.View
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
    private GatewayConfig _config;

    public WinMain(WinSplash splash, IProject zenonProject, string addinPath, string configPath)
    {
      InitializeComponent();

      _splash = splash;
      _zenonProject = zenonProject;
      _addinPath = addinPath;
      _configPath = configPath;

      Title = $"{GatewayConfig.Constants.RootName} {GatewayConfig.Constants.SolutionNewName} Ver {GatewayConfig.Constants.SolutionVersion} - {_zenonProject.Name}";
      DataContext = new WinMainViewModel(_zenonProject, _configPath);
      _config = ((WinMainViewModel)DataContext).Config;
    }

    private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
    {
      _splash.Close();
      _zenonProject.Parent.Parent.BringWindowToTop();
      HamburgerMenuControl.SelectedIndex = 0;
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
                                                                                $"{GatewayConfig.Constants.RootName} {GatewayConfig.Constants.SolutionNewName} 모듈을 다시 설치 하시겠습니까?",
                                                                                MessageDialogStyle.AffirmativeAndNegative,
                                                                                new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Theme });

              if (resultReinstall == MessageDialogResult.Affirmative)
              {
                File.WriteAllBytes(_addinPath, EDResources.RuntimeAddins);
              }

              break;
            case "Reset":
              MessageDialogResult resultInit = await this.ShowMessageAsync("설정파일초기화",
                                                                           $"{GatewayConfig.Constants.RootName} {GatewayConfig.Constants.SolutionNewName} 설정을 초기화 하시겠습니까?",
                                                                           MessageDialogStyle.AffirmativeAndNegative,
                                                                           new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Theme });

              if (resultInit == MessageDialogResult.Affirmative)
              {
                _config.Reset();
              }

              break;
            case "Save":
              MessageDialogResult resultSave = await this.ShowMessageAsync("설정파일저장",
                                                                           $"{GatewayConfig.Constants.RootName} {GatewayConfig.Constants.SolutionNewName} 모듈을 다시 설치 하시겠습니까?까?",
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
              MessageDialogResult result = await this.ShowMessageAsync($"{GatewayConfig.Constants.RootName} {GatewayConfig.Constants.SolutionNewName} Ver {GatewayConfig.Constants.SolutionVersion}",
                                                                       " © 2023 HDC Labs All rights reserved.",
                                                                       MessageDialogStyle.AffirmativeAndNegative,
                                                                       new MetroDialogSettings { AffirmativeButtonText = "닫기",
                                                                                                 NegativeButtonText = "릴리즈노트",
                                                                                                 ColorScheme = MetroDialogColorScheme.Inverted });

              if (result == MessageDialogResult.Negative)
              {

                WinReleaseNotes win = new WinReleaseNotes(EDResources.ReleaseNotes);
                win.ShowDialog();
              }

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
        MessageBox.Show($"[HamburgerMenuControl_ItemInvoked]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
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

      ViewCloud viewCloud = ((HamburgerMenuItemCollection)HamburgerMenuControl.ItemsSource)[0].Tag as ViewCloud;
      viewCloud.IsLock = viewModel.IsLock;

      ((HamburgerMenuItemCollection)HamburgerMenuControl.ItemsSource)[1].IsVisible = isVisible;
      ((HamburgerMenuItemCollection)HamburgerMenuControl.ItemsSource)[2].IsVisible = isVisible;
      ((HamburgerMenuItemCollection)HamburgerMenuControl.ItemsSource)[3].IsVisible = isVisible;
      ((HamburgerMenuItemCollection)HamburgerMenuControl.ItemsSource)[4].IsVisible = isVisible;

      ((HamburgerMenuItemCollection)HamburgerMenuControl.OptionsItemsSource)[0].IsVisible = isVisible;
      ((HamburgerMenuItemCollection)HamburgerMenuControl.OptionsItemsSource)[1].IsVisible = isVisible;

      if (viewModel.IsLock)
      {
        HamburgerMenuControl.SelectedIndex = 0;
      }
    }
  }
}

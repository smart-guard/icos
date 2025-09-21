using iCos5.BACnet.Schedule;
using iCos5BACnetScheduleRT.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PASoft.Zenon.Addins;
using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.Variable;
using System;
using System.Windows;
using System.Windows.Data;

namespace iCos5BACnetScheduleRT.View
{
  /// <summary>
  /// WinMain.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class WinMain : MetroWindow
  {
    private ScheduleConfig _scheduleConfig;
    private CelLogging _celLogging;
    private bool _skipLoadedValue = false;

    public WinMain(ScheduleConfig scheduleConfig,
                   IProject zenonProject,
                   IOnlineVariableContainer weeklyOnlineContainer,
                   IOnlineVariableContainer exceptionOnlineContainer,
                   CelLogging celLogging)
    {
      InitializeComponent();

      _scheduleConfig = scheduleConfig;
      _celLogging = celLogging;

      DataContext = new WinMainViewModel(_scheduleConfig,
                                         zenonProject,
                                         weeklyOnlineContainer,
                                         exceptionOnlineContainer,
                                         _celLogging);
    }

    public void ShowDialog(DialogueParameter dialogueParameter)
    {
      Title = $"Device ID : {dialogueParameter.DeviceName} - Object Name : {dialogueParameter.ObjectName}";
      WinMainViewModel viewModel = (WinMainViewModel)DataContext;
      viewModel.ClearScheduleItems();
      viewModel.SetParameter(dialogueParameter);
      Show();
      Activate();

      switch (HamburgerMenuControl.SelectedIndex)
      {
        case 0:
          viewModel.WeeklyScheduleView.LoadedValue();
          break;
        default:
          HamburgerMenuControl.SelectedIndex = 0;
          break;
      }
    }

    private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        HamburgerMenuControl.SelectedIndex = 0;

        ((HamburgerMenuItemBase)HamburgerMenuControl.Items[1]).IsVisible = _scheduleConfig.IsActiveExceptionTab;

        Binding showOnlineVariableErrorDialogueBinding = new Binding("IsOnlineVariableError");
        showOnlineVariableErrorDialogueBinding.Source = DataContext;
        showOnlineVariableErrorDialogueBinding.Mode = BindingMode.OneWay;
        BindingOperations.SetBinding(thisWindows, ShowOnlineVariableErrorDialogueProperty, showOnlineVariableErrorDialogueBinding);
      }
      catch (Exception ex)
      {
        loggingError("MetroWindow_Loaded", ex.ToString());
      }
    }

    private async void HamburgerMenuControl_ItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
    {
      try
      {
        if (_skipLoadedValue)
        {
          _skipLoadedValue = false;
          return;
        }

        if (e.IsItemOptions &&
            ((string)((HamburgerMenuItem)e.InvokedItem).ToolTip).Equals("About"))
        {
          int preIndex = HamburgerMenuControl.SelectedIndex;
          await this.ShowMessageAsync($"{ScheduleConfig.Constants.RootName} {ScheduleConfig.Constants.SolutionName} Ver {ScheduleConfig.Constants.SolutionVersion}",
                                      " © 2023 HDC Labs All rights reserved.",
                                      MessageDialogStyle.Affirmative,
                                      new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
          _skipLoadedValue = true;
          HamburgerMenuControl.SelectedIndex = preIndex;
        }
        else
        {
          HamburgerMenuControl.Content = e.InvokedItem;
          HamburgerMenuItemBase hamburgerMenuItem = (HamburgerMenuItemBase)HamburgerMenuControl.Content;

          switch (HamburgerMenuControl.SelectedIndex)
          {
            case 0:
              ViewWeeklySchedule viewWeeklySchedule = (ViewWeeklySchedule)hamburgerMenuItem?.Tag;

              if (viewWeeklySchedule != null)
              {
                bindingShowProgressDialogue("WeeklyScheduleView.IsProgress", "주간 스케줄 설정 로딩 중입니다.");
                viewWeeklySchedule.LoadedValue();
              }

              break;
            case 1:
              ViewExceptionSchedule viewExceptionSchedule = (ViewExceptionSchedule)hamburgerMenuItem?.Tag;

              if (viewExceptionSchedule != null)
              {
                bindingShowProgressDialogue("ExceptionScheduleView.IsProgress", "예외 스케줄 설정 로딩 중입니다.");
                viewExceptionSchedule.LoadedValue();
              }

              break;
            case 2:
            default:
              break;
          }
        }
      }
      catch (Exception ex)
      {
        loggingError("HamburgerMenuControl_ItemInvoked", ex.ToString());
      }
    }

    private void bindingShowProgressDialogue(string bindingPath, string message)
    {
      SetProgressDialogMessage(message);

      Binding showProgressDialogueBinding = new Binding(bindingPath);
      showProgressDialogueBinding.Source = DataContext;
      showProgressDialogueBinding.Mode = BindingMode.OneWay;
      BindingOperations.SetBinding(thisWindows, ShowProgressDialogueProperty, showProgressDialogueBinding);
    }

    private void loggingError(string code, string message)
    {
      if (_celLogging.Enable)
      {
        _celLogging.Error($"[MainView][{code}]{message}");
      }
      else
      {
        MessageBox.Show($"[MainView][{code}]{message}");
      }
    }
  }
}

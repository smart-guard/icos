using iCos5BACnetScheduleRT.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace iCos5BACnetScheduleRT.View
{
  public partial class WinMain : MetroWindow
  {
    public bool ShowProgressDialogue
    {
      get { return (bool)GetValue(ShowProgressDialogueProperty); }
      set { SetValue(ShowProgressDialogueProperty, value); }
    }

    public static readonly DependencyProperty ShowProgressDialogueProperty =
      DependencyProperty.Register("ShowProgressDialogue", typeof(bool), typeof(WinMain),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnShowProgressDialogueChanged)));

    private static void OnShowProgressDialogueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      WinMain me = (WinMain)d;

      if ((bool)e.NewValue)
      {
        me.showProgressDialog();
      }
      else
      {
        me.closeProgressDialog();
      }
    }

    private bool _closeProgressDialog = false;
    private string _progressDialogTitle = "BACnet 스케줄";
    private string _progressDialogMessage = "작업중입니다.";

    public void SetProgressDialogTitle(string title)
    {
      _progressDialogTitle = title;
    }

    public void SetProgressDialogMessage(string message)
    {
      _progressDialogMessage = message;
    }

    private async void showProgressDialog()
    {
      MetroDialogSettings dlgSettings = new MetroDialogSettings()
      {
        AnimateShow = true,
        AnimateHide = true
      };

      ProgressDialogController progressDialog = await this.ShowProgressAsync(_progressDialogTitle, _progressDialogMessage, settings: dlgSettings);
      progressDialog.SetIndeterminate();

      if (await Task.Run(() => waitCloseProgressDialog()))
      {
        await progressDialog.CloseAsync();
        _closeProgressDialog = false;
        GC.Collect();
      }
    }

    private bool waitCloseProgressDialog()
    {
      while (!_closeProgressDialog)
      {
        Thread.Sleep(100);
      }

      return _closeProgressDialog;
    }

    private void closeProgressDialog()
    {
      _closeProgressDialog = true;
    }

    public bool ShowOnlineVariableErrorDialogue
    {
      get { return (bool)GetValue(ShowOnlineVariableErrorDialogueProperty); }
      set { SetValue(ShowOnlineVariableErrorDialogueProperty, value); }
    }

    public static readonly DependencyProperty ShowOnlineVariableErrorDialogueProperty =
      DependencyProperty.Register("ShowOnlineVariableErrorDialogue", typeof(bool), typeof(WinMain),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnShowOnlineVariableErrorDialogueChanged)));

    private static void OnShowOnlineVariableErrorDialogueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      WinMain me = (WinMain)d;

      if ((bool)e.NewValue)
      {
        me.showOnlineVariableErrorDialog();
      }
    }

    private async void showOnlineVariableErrorDialog()
    {
      await this.ShowMessageAsync("스케줄 로딩중...",
                                  "관제점 통신 불량!!",
                                  MessageDialogStyle.Affirmative,
                                  new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Accented });

      WinMainViewModel viewModel = (WinMainViewModel)DataContext;
      viewModel.IsOnlineVariableError = false;
    }
  }
}

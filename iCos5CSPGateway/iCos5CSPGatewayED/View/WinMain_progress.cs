using iCos5.CSPGateway;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace iCos5CSPGatewayED.View
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
        me.showProgressDialog();
      else
        me.closeProgressDialog();
    }

    private bool _closeProgressDialog = false;
    private string _progressDialogTitle = GatewayConfig.Constants.SolutionNewName;
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
  }
}

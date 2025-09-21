using iCos5.CSPGateway;
using iCos5CSPGatewayRT.Manager;
using iCos5CSPGatewayRT.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PASoft.UI.WPF;
using PASoft.Zenon.Addins;
using Scada.AddIn.Contracts;
using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace iCos5CSPGatewayRT.View
{
  public partial class WinMain : MetroWindow
  {
    public WinMainViewModel ViewModel 
    {
      get { return (WinMainViewModel)DataContext; } 
    }

    public bool ShowWinMain
    {
      get { return (bool)GetValue(ShowWinMainProperty); }
      set { SetValue(ShowWinMainProperty, value); }
    }

    public static readonly DependencyProperty ShowWinMainProperty =
      DependencyProperty.Register("ShowWinMain", typeof(bool), typeof(WinMain),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnShowWinMainChanged)));

    private static void OnShowWinMainChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      WinMain me = (WinMain)d;

      if (me == null)
      {
        return;
      }

      if (e.NewValue is bool value)
      {
        if (value)
        {
          me.ShowDialogue(true);
          me.ShowWinMain = false;
        }
      }
    }

    public ControlListItem AddControlList
    {
      get { return (ControlListItem)GetValue(AddControlListProperty); }
      set { SetValue(AddControlListProperty, value); }
    }

    public static readonly DependencyProperty AddControlListProperty =
      DependencyProperty.Register("AddControlList", typeof(ControlListItem), typeof(WinMain),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnAddControlListChanged)));

    private static void OnAddControlListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      WinMain me = (WinMain)d;

      if (me == null)
      {
        return;
      }

      if (e.NewValue is ControlListItem value)
      {
        me.ViewModel.ControlList.Add(value);
        me.AddControlList = null;
      }
    }

    public int RemoveControlList
    {
      get { return (int)GetValue(RemoveControlListProperty); }
      set { SetValue(RemoveControlListProperty, value); }
    }

    public static readonly DependencyProperty RemoveControlListProperty =
      DependencyProperty.Register("RemoveControlList", typeof(int), typeof(WinMain),
        new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnRemoveControlListChanged)));

    private static void OnRemoveControlListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      WinMain me = (WinMain)d;

      if (me == null)
      {
        return;
      }

      if (e.NewValue is int value)
      {
        me.StopBell();

        ObservableCollectionEx<ControlListItem> controlList = me.ViewModel.ControlList;

        foreach (ControlListItem controlListItem in controlList)
        {
          if (controlListItem.SequenceNumber.Equals(value))
          {
            controlList.Remove(controlListItem);
            break;
          }
        }
      }
    }

    private IProject _zenonProject;
    private GatewayConfig _config;
    private SoundPlayer _soundPlayer = new SoundPlayer();

    public WinMain(GatewayConfig config, IProject zenonProject, CelLogging celLogging, CSPManager cspManager)
    {
      InitializeComponent();

      _zenonProject = zenonProject;
      _config = config;

      Title = $"{GatewayConfig.Constants.RootName} {GatewayConfig.Constants.SolutionNewName} Ver {GatewayConfig.Constants.SolutionVersion} - {_zenonProject.Name}";
      DataContext = new WinMainViewModel(_config, cspManager);

      Binding showWinMainBinding = new Binding("ShowWinMainWithSound");
      showWinMainBinding.Source = DataContext;
      showWinMainBinding.Mode = BindingMode.TwoWay;
      BindingOperations.SetBinding(thisWindows, ShowWinMainProperty, showWinMainBinding);

      Binding addControlListBinding = new Binding("AddControlListItem");
      addControlListBinding.Source = DataContext;
      addControlListBinding.Mode = BindingMode.TwoWay;
      BindingOperations.SetBinding(thisWindows, AddControlListProperty, addControlListBinding);

      Binding removeControlListBinding = new Binding("RemoveControlListSequenceNumber");
      removeControlListBinding.Source = DataContext;
      removeControlListBinding.Mode = BindingMode.TwoWay;
      BindingOperations.SetBinding(thisWindows, RemoveControlListProperty, removeControlListBinding);
    }

    public void ShowDialogue(bool isSound)
    {
      switch (Visibility)
      {
        case Visibility.Visible:
          Activate();
          break;
        case Visibility.Hidden:
          Visibility = Visibility.Visible;
          break;
        case Visibility.Collapsed:
          Show();
          break;
      }

      if (isSound)
      {
        switch (_config.BellPlayType)
        {
          case BellPlayTypes.Continuous:
            playBell(true);
            break;
          case BellPlayTypes.Once:
            playBell(false);
            break;
          case BellPlayTypes.None:
            break;
        }
      }
    }

    public void StopBell()
    {
      _soundPlayer.Stop();
    }

    private void playBell(bool isLoop)
    {
      _soundPlayer.Stop();

      switch (_config.BellSound)
      {
        case BellSounds.Bell00:
          _soundPlayer.Stream = iCos5CSPGateway.Resource.Bell00;
          break;
        case BellSounds.Bell01:
          _soundPlayer.Stream = iCos5CSPGateway.Resource.Bell01;
          break;
        case BellSounds.Bell02:
          _soundPlayer.Stream = iCos5CSPGateway.Resource.Bell02;
          break;
      }

      if (isLoop)
      {
        _soundPlayer.PlayLooping();
      }
      else
      {
        _soundPlayer.Play();
      }
    }

    public void CloseWinMain()
    {
      _soundPlayer.Stop();
      IsCanClosing = true;
      Close();
    }

    private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
    {

    }

    private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        ControlListItem controlListItem = (ControlListItem)((Button)sender).CommandParameter;

        if (ViewModel.CSPManagerControlConfirm(controlListItem))
        {
          await this.ShowMessageAsync($"{GatewayConfig.Constants.RootName} {GatewayConfig.Constants.SolutionNewName} Ver {GatewayConfig.Constants.SolutionVersion}",
                                      $"운영자 제어 실행 확인{Environment.NewLine}관제점 : [{controlListItem.SequenceNumber}]{controlListItem.VariableName}{Environment.NewLine}제어값 : {controlListItem.ControlValue}",
                                      MessageDialogStyle.Affirmative,
                                      new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Accented });
        }
        else
        {
          await this.ShowMessageAsync($"{GatewayConfig.Constants.RootName} {GatewayConfig.Constants.SolutionNewName} Ver {GatewayConfig.Constants.SolutionVersion}",
                                      $"제어 항목이 없습니다.{Environment.NewLine}관제점 : [{controlListItem.SequenceNumber}]{controlListItem.VariableName}{Environment.NewLine}제어값 : {controlListItem.ControlValue}",
                                      MessageDialogStyle.Affirmative,
                                      new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Theme });
          _soundPlayer.Stop();
          ViewModel.ControlList.Remove(controlListItem);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"[ConfirmButton_Click]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }

    private async void RejectButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        ControlListItem controlListItem = (ControlListItem)((Button)sender).CommandParameter;

        if (ViewModel.CSPManagerControlReject(controlListItem))
        {
          await this.ShowMessageAsync($"{GatewayConfig.Constants.RootName} {GatewayConfig.Constants.SolutionNewName} Ver {GatewayConfig.Constants.SolutionVersion}",
                                      $"운영자 제어 실행 거절{Environment.NewLine}관제점 : [{controlListItem.SequenceNumber}]{controlListItem.VariableName}{Environment.NewLine}제어값 : {controlListItem.ControlValue}",
                                      MessageDialogStyle.Affirmative,
                                      new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
        }
        else
        {
          await this.ShowMessageAsync($"{GatewayConfig.Constants.RootName} {GatewayConfig.Constants.SolutionNewName} Ver {GatewayConfig.Constants.SolutionVersion}",
                                      $"제어 항목이 없습니다.{Environment.NewLine}관제점 : [{controlListItem.SequenceNumber}]{controlListItem.VariableName}{Environment.NewLine}제어값 : {controlListItem.ControlValue}",
                                      MessageDialogStyle.Affirmative,
                                      new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Theme });
          _soundPlayer.Stop();
          ViewModel.ControlList.Remove(controlListItem);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"[RejectButton_Click]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }
  }
}
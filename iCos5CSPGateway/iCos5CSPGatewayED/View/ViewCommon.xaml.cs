using iCos5.CSPGateway;
using MahApps.Metro.Controls;
using PASoft.Zenon.Addins.Extension;
using Scada.AddIn.Contracts;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace iCos5CSPGatewayED.View
{
  public partial class ViewCommon : System.Windows.Controls.UserControl
  {
    public bool IsExpandCEL
    {
      get { return (bool)GetValue(IsExpandCELProperty); }
      set { SetValue(IsExpandCELProperty, value); }
    }

    public static readonly DependencyProperty IsExpandCELProperty =
      DependencyProperty.Register("IsExpandCEL", typeof(bool), typeof(ViewCommon),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsExpandCELChanged)));

    private static void OnIsExpandCELChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewCommon me = (ViewCommon)d;

      if ((bool)e.NewValue)
        ((Storyboard)me.FindResource("ExpandStoryCEL")).Begin(me);
      else
        ((Storyboard)me.FindResource("CollapseStoryCEL")).Begin(me);
    }

    public GatewayConfig GatewayConfig { get => DataContext as GatewayConfig; }

    private IProject _zenonProject;

    public ViewCommon(IProject zenonProject)
    {
      InitializeComponent();

      _zenonProject = zenonProject;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      UpdateCELGroups(_zenonProject.AlarmGroupsCollection());
    }

    public void UpdateCELGroups(AlarmClassObjectCollection alarmClassObjects)
    {
      string oldText = GatewayConfig.CELGroupName;
      GatewayConfig.CELGroupName = "";
      GatewayConfig.CELGroups.Clear();
      GatewayConfig.CELGroups.Add("None");

      foreach (AlarmClassObject item in alarmClassObjects)
      {
        GatewayConfig.CELGroups.Add(item.Name);
      }

      GatewayConfig.CELGroupName = oldText;
    }

    private void ExpandCEL_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      IsExpandCEL = !IsExpandCEL;
    }

    private void EnableCEL_Toggled(object sender, RoutedEventArgs e)
    {
      try
      {
        if (GatewayConfig == null)
        {
          return;
        }

        if (((ToggleSwitch)sender).IsOn)
        {
          AlarmClassObjectCollection alarmClasses = _zenonProject.AlarmClassesCollection();
          GatewayConfig.CELClassPvID = alarmClasses.GetpvIdByName(GatewayConfig.Constants.RootName);

          if (GatewayConfig.CELClassPvID < 0)
          {
            alarmClasses.Add(GatewayConfig.Constants.RootName);
            GatewayConfig.CELClassPvID = alarmClasses.GetpvIdByName(GatewayConfig.Constants.RootName);
          }

          if (GatewayConfig.CELGroupPvID < 0)
          {
            GatewayConfig.CELGroupName = GatewayConfig.Constants.SolutionName;
            AlarmClassObjectCollection alarmGroups = _zenonProject.AlarmGroupsCollection();
            int solutionNamePvID = alarmGroups.GetpvIdByName(GatewayConfig.Constants.SolutionName);

            if (solutionNamePvID < 0)
            {
              alarmGroups.Add(GatewayConfig.Constants.SolutionName);
              GatewayConfig.CELGroupPvID = alarmGroups.GetpvIdByName(GatewayConfig.Constants.SolutionName);
            }
            else
            {
              GatewayConfig.CELGroupPvID = solutionNamePvID;
            }
          }
        }
        else
        {
          GatewayConfig.CELClassPvID = -1;
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"[ExpandCEL_MouseLeftButtonDown]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }

    private void AddCELGroup_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        WinTextInput win = new WinTextInput("CEL 그룹 생성", "CEL 그룹");

        if (win.ShowDialog() == true)
        {
          AlarmClassObjectCollection alarmClassObjects = _zenonProject.AlarmGroupsCollection();

          if (alarmClassObjects.GetIndexByName(win.InputText.Text) >= 0)
          {
            System.Windows.MessageBox.Show("같은 이름의 그룹이 있습니다!!", "CEL 그룹", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
          }
          else
          {
            if (alarmClassObjects.Add(win.InputText.Text))
            {
              GatewayConfig.CELGroupName = win.InputText.Text;
              UpdateCELGroups(alarmClassObjects);
            }
            else
            {
              System.Windows.MessageBox.Show("그룹 추가과정에 오류가 있습니다!!", "CEL 그룹", MessageBoxButton.OK, MessageBoxImage.Error);
              return;
            }
          }
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"[AddCELGroup_Click]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }

    private void TempFolderDialog_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        FolderBrowserDialog openFolderDlg = new FolderBrowserDialog();
        openFolderDlg.RootFolder = Environment.SpecialFolder.MyComputer;

        try
        {
          openFolderDlg.SelectedPath = Directory.Exists(FolderPath.Text) ? FolderPath.Text : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        catch
        {
          openFolderDlg.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        if (openFolderDlg.ShowDialog() == DialogResult.OK)
        {
          FolderPath.Text = openFolderDlg.SelectedPath;
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"[TempFolderDialog_Click]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }
  }
}
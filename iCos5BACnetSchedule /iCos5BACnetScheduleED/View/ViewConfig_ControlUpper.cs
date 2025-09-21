using iCos5.BACnet.Schedule;
using MahApps.Metro.Controls;
using PASoft.Zenon.Addins.Extension;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace iCos5BACnetScheduleED.View
{
  public partial class ViewConfig : UserControl
  {
    public bool IsExpandCEL
    {
      get { return (bool)GetValue(IsExpandCELProperty); }
      set { SetValue(IsExpandCELProperty, value); }
    }

    public static readonly DependencyProperty IsExpandCELProperty =
      DependencyProperty.Register("IsExpandCEL", typeof(bool), typeof(ViewConfig),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsExpandCELChanged)));

    private static void OnIsExpandCELChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewConfig me = (ViewConfig)d;

      if ((bool)e.NewValue)
        ((Storyboard)me.FindResource("ExpandStoryCEL")).Begin(me);
      else
        ((Storyboard)me.FindResource("CollapseStoryCEL")).Begin(me);
    }

    private void ExpandCEL_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      IsExpandCEL = !IsExpandCEL;
    }

    private void EnableCEL_Toggled(object sender, RoutedEventArgs e)
    {
      try
      {
        ScheduleConfig config = (ScheduleConfig)DataContext;

        if (config == null)
        {
          return;
        }

        if (((ToggleSwitch)sender).IsOn)
        {
          AlarmClassObjectCollection alarmObjects = _zenonProject.AlarmClassesCollection();
          config.CELClassPvID = alarmObjects.GetpvIdByName(ScheduleConfig.Constants.RootName);

          if (config.CELClassPvID < 0)
          {
            alarmObjects.Add(ScheduleConfig.Constants.RootName);
            config.CELClassPvID = alarmObjects.GetpvIdByName(ScheduleConfig.Constants.RootName);
          }

          if (config.CELGroupPvID < 0)
          {
            config.CELGroupName = ScheduleConfig.Constants.SolutionName;
            AlarmClassObjectCollection alarmGroups = _zenonProject.AlarmGroupsCollection();
            int solutionNamePvID = alarmGroups.GetpvIdByName(ScheduleConfig.Constants.SolutionName);

            if (solutionNamePvID < 0)
            {
              alarmGroups.Add(ScheduleConfig.Constants.SolutionName);
              config.CELGroupPvID = alarmGroups.GetpvIdByName(ScheduleConfig.Constants.SolutionName);
            }
            else
            {
              config.CELGroupPvID = solutionNamePvID;
            }
          }
        }
        else
        {
          config.CELClassPvID = -1;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"[ExpandCEL_MouseLeftButtonDown]{ex}", ScheduleConfig.Constants.SolutionName);
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
            MessageBox.Show("같은 이름의 그룹이 있습니다!!", "CEL 그룹", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
          }
          else
          {
            if (alarmClassObjects.Add(win.InputText.Text))
            {
              ((ScheduleConfig)DataContext).CELGroupName = win.InputText.Text;
              updateCELGroups(alarmClassObjects);
            }
            else
            {
              MessageBox.Show("그룹 추가과정에 오류가 있습니다!!", "CEL 그룹", MessageBoxButton.OK, MessageBoxImage.Error);
              return;
            }
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"[AddCELGroup_Click]{ex}", ScheduleConfig.Constants.SolutionName);
      }
    }

    private void updateCELGroups(AlarmClassObjectCollection alarmClassObjects)
    {
      ScheduleConfig config = (ScheduleConfig)DataContext;
      string oldText = config.CELGroupName;
      config.CELGroupName = "";
      config.CELGroups.Clear();
      config.CELGroups.Add("None");

      foreach (AlarmClassObject item in alarmClassObjects)
      {
        config.CELGroups.Add(item.Name);
      }

      config.CELGroupName = oldText;
    }
  }
}

using PASoft.Zenon.Addins.Extension;
using Scada.AddIn.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace iCos5BACnetScheduleED.View
{
  public partial class ViewConfig : UserControl
  {
    public bool IsLock
    {
      get { return (bool)GetValue(IsLockProperty); }
      set { SetValue(IsLockProperty, value); }
    }

    public static readonly DependencyProperty IsLockProperty =
      DependencyProperty.Register("IsLock", typeof(bool), typeof(ViewConfig),
        new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsLockChanged)));

    private static void OnIsLockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewConfig me = (ViewConfig)d;

      if ((bool)e.NewValue)
      {
        me.IsExpandCEL = false;
        ((Storyboard)me.FindResource("CollapseLockUnlock")).Begin(me);
      }
      else
      {
        ((Storyboard)me.FindResource("ExpandLockUnlock")).Begin(me);
      }
    }

    private WinMain _parentWinMain;
    private IProject _zenonProject;

    public ViewConfig(WinMain winMain, IProject zenonProject)
    {
      InitializeComponent();

      _parentWinMain = winMain;
      _zenonProject = zenonProject;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      updateCELGroups(_zenonProject.AlarmGroupsCollection());
    }
  }
}

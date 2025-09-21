using iCos5.BACnet.Zenon;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace iCos5BACnetScheduleRT.View.Control
{
  /// <summary>
  /// ScheduleControl.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class ScheduleElement : UserControl
  {
    public bool IsSelected
    {
      get { return (bool)GetValue(IsSelectedProperty); }
      set { SetValue(IsSelectedProperty, value); }
    }

    public static readonly DependencyProperty IsSelectedProperty =
      DependencyProperty.Register("IsSelected", typeof(bool), typeof(ScheduleElement),
        new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ScheduleElement me = (ScheduleElement)d;

      if (me.IsSelected)
      {
        ((Storyboard)me.FindResource("BlinkSelectedArrow")).Begin(me, true);
      }
      else
      {
        ((Storyboard)me.FindResource("BlinkSelectedArrow")).Stop(me);
      }
    }

    public ScheduleElement()
    {
      InitializeComponent();
    }

    private void NormalBorder_MouseMove(object sender, MouseEventArgs e)
    {
      NormalTooltip.Placement = PlacementMode.Relative;
      NormalTooltip.HorizontalOffset = e.GetPosition((IInputElement)sender).X;
      NormalTooltip.VerticalOffset = e.GetPosition((IInputElement)sender).Y - 20;
    }

    private void AbnormalBorder_MouseMove(object sender, MouseEventArgs e)
    {
      AbnormalTooltip.Placement = PlacementMode.Relative;
      AbnormalTooltip.HorizontalOffset = e.GetPosition((IInputElement)sender).X;
      AbnormalTooltip.VerticalOffset = e.GetPosition((IInputElement)sender).Y - 20;
    }

    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
      ((BooleanScheduleItemPair)DataContext).OnIsSelectedEvent();
    }
  }
}

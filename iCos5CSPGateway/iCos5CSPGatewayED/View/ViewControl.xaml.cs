using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace iCos5CSPGatewayED.View
{
  public partial class ViewControl : UserControl
  {
    public bool IsExpandMannedType
    {
      get { return (bool)GetValue(IsExpandMannedTypeProperty); }
      set { SetValue(IsExpandMannedTypeProperty, value); }
    }

    public static readonly DependencyProperty IsExpandMannedTypeProperty =
      DependencyProperty.Register("IsExpandMannedType", typeof(bool), typeof(ViewControl),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsExpandMannedTypeChanged)));

    private static void OnIsExpandMannedTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewControl me = (ViewControl)d;

      if ((bool)e.NewValue)
        ((Storyboard)me.FindResource("ExpandStoryMannedType")).Begin(me);
      else
        ((Storyboard)me.FindResource("CollapseStoryMannedType")).Begin(me);
    }

    private SoundPlayer _soundPlayer = new SoundPlayer();

    public ViewControl()
    {
      InitializeComponent();
    }

    private void ExpandMannedType_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      IsExpandMannedType = !IsExpandMannedType;
    }

    private void ActiveSwitch_Toggled(object sender, RoutedEventArgs e)
    {
      if (!ActiveSwitch.IsOn)
      {
        IsExpandMannedType = false;
      }
    }

    private void PlayBellSound_Click(object sender, RoutedEventArgs e)
    {
      _soundPlayer.Stop();

      switch (BellSoundItem.SelectedIndex)
      {
        case 0:
          _soundPlayer.Stream = iCos5CSPGateway.Resource.Bell00;
          break;
        case 1:
          _soundPlayer.Stream = iCos5CSPGateway.Resource.Bell01;
          break;
        case 2:
          _soundPlayer.Stream = iCos5CSPGateway.Resource.Bell02;
          break;
      }

      _soundPlayer.Play();
    }
  }
}

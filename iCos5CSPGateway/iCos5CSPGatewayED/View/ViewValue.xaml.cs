using iCos5.CSPGateway;
using Scada.AddIn.Contracts;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace iCos5CSPGatewayED.View
{
  public partial class ViewValue : UserControl
  {
    public bool IsExpandPeriod
    {
      get { return (bool)GetValue(IsExpandPeriodProperty); }
      set { SetValue(IsExpandPeriodProperty, value); }
    }

    public static readonly DependencyProperty IsExpandPeriodProperty =
      DependencyProperty.Register("IsExpandPeriod", typeof(bool), typeof(ViewValue),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsExpandPeriodChanged)));

    private static void OnIsExpandPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewValue me = (ViewValue)d;

      if ((bool)e.NewValue)
        ((Storyboard)me.FindResource("ExpandStoryPeriod")).Begin(me);
      else
        ((Storyboard)me.FindResource("CollapseStoryPeriod")).Begin(me);
    }

    public ViewValue(IProject zenonProject)
    {
      InitializeComponent();
    }

    private void ExpandPeriod_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      IsExpandPeriod = !IsExpandPeriod;
    }

    private void PeriodTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
      try
      {
        getClockImage();
      }
      catch (Exception ex)
      {
        MessageBox.Show($"[PeriodTime_ValueChanged]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }

    private void OffsetTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
      try
      {
        getClockImage();
      }
      catch (Exception ex)
      {
        MessageBox.Show($"[OffsetTime_ValueChanged]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }

    private void getClockImage()
    {
      int period;
      int seqMin;

      try
      {
        period = (int)PeriodTime.Value;
        seqMin = (int)OffsetTime.Value;
        seqMin = seqMin > period ? period : seqMin;
      }
      catch
      {
        return;
      }

      DrawingGroup drawings = new DrawingGroup();
      drawings.Children.Add(new GeometryDrawing(Brushes.Transparent,
                                                new Pen(Brushes.Transparent, 12.5),
                                                (Geometry)Resources["AnalogClockAnimation2_still_frameGeometry2"]));

      Pen pen = new Pen(Brushes.Lime, 25);
      pen.StartLineCap = PenLineCap.Round;
      pen.EndLineCap = PenLineCap.Round;

      while (seqMin < 60)
      {
        int seqNum = seqMin == 0 ? 62 : seqMin + 2;

        drawings.Children.Add(new GeometryDrawing(Brushes.Transparent,
                                                  pen,
                                                  (Geometry)Resources[$"AnalogClockAnimation2_still_frameGeometry{seqNum}"]));

        seqMin += period;
      }

      SequenceMarked.Source = new DrawingImage(drawings);
    }
  }
}

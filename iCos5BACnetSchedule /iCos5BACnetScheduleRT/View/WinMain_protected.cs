using MahApps.Metro.Controls;
using System.ComponentModel;

namespace iCos5BACnetScheduleRT.View
{
  public partial class WinMain : MetroWindow
  {
    public bool IsCanClosing { get; set; } = false;

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);

      if (IsCanClosing == false)
      {
        e.Cancel = true;
        Hide();
      }
    }
  }
}

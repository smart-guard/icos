using MahApps.Metro.Controls;
using System.ComponentModel;

namespace iCos5CSPGatewayRT.View
{
  public partial class WinMain : MetroWindow
  {
    public bool IsCanClosing { get; set; } = false;

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);

      if (!IsCanClosing)
      {
        e.Cancel = true;
        Hide();
      }
    }
  }
}

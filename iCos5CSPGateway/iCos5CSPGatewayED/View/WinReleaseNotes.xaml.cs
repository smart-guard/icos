using System.Windows;

namespace iCos5CSPGatewayED.View
{
  public partial class WinReleaseNotes : Window
  {
    public WinReleaseNotes(string message)
    {
      InitializeComponent();
      textBoxMessage.Text = message;
    }

    private void ButtonOK_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }
  }
}

using System.Windows;

namespace iCos5CSPGatewayED.View
{
  /// <summary>
  /// WinTextInput.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class WinTextInput : Window
  {
    public WinTextInput(string title, string inputName)
    {
      InitializeComponent();
      
      Title = title;
      InputName.Content = inputName;
      InputText.Focus();
    }

    private void ButtonOK_Click(object sender, RoutedEventArgs e)
    {
      if (InputText.Text.Equals(string.Empty))
      {
        MessageBox.Show($"{InputName.Content} is empty!!");
      }
      else
      {
        DialogResult = true;
        Close();
      }
    }

    private void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }
  }
}

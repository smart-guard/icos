using iCos5.CSPGateway;
using iCos5.CSPGateway.AWS;
using iCos5.CSPGateway.DB;
using iCos5.CSPGateway.SMB;
using iCos5CSPGateway.AWS;
using MahApps.Metro.Controls;
using PASoft.Common.Encryption;
using PASoft.Common.Serialization;
using PASoft.Common.SMB;
using PASoft.DB.Postgres;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace iCos5CSPGatewayED.View
{
  public partial class ViewCloud : System.Windows.Controls.UserControl
  {
    public bool IsLock
    {
      get { return (bool)GetValue(IsLockProperty); }
      set { SetValue(IsLockProperty, value); }
    }

    public static readonly DependencyProperty IsLockProperty =
      DependencyProperty.Register("IsLock", typeof(bool), typeof(ViewCloud),
        new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsLockChanged)));

    private static void OnIsLockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewCloud me = (ViewCloud)d;
      bool isEnabled = !(bool)e.NewValue;
      me.borderBEMSRequestProcedure.IsEnabled = isEnabled;
      me.borderBEMSReplyProcedure.IsEnabled = isEnabled;
    }

    public bool IsExpandAWS
    {
      get { return (bool)GetValue(IsExpandAWSProperty); }
      set { SetValue(IsExpandAWSProperty, value); }
    }

    public static readonly DependencyProperty IsExpandAWSProperty =
      DependencyProperty.Register("IsExpandAWS", typeof(bool), typeof(ViewCloud),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsExpandAWSChanged)));

    private static void OnIsExpandAWSChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewCloud me = (ViewCloud)d;

      if ((bool)e.NewValue)
        ((Storyboard)me.FindResource("ExpandStoryAWS")).Begin(me);
      else
        ((Storyboard)me.FindResource("CollapseStoryAWS")).Begin(me);
    }

    public bool IsExpandBuilding
    {
      get { return (bool)GetValue(IsExpandBuildingProperty); }
      set { SetValue(IsExpandBuildingProperty, value); }
    }

    public static readonly DependencyProperty IsExpandBuildingProperty =
      DependencyProperty.Register("IsExpandBuilding", typeof(bool), typeof(ViewCloud),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsExpandBuildingChanged)));

    private static void OnIsExpandBuildingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewCloud me = (ViewCloud)d;

      if ((bool)e.NewValue)
        ((Storyboard)me.FindResource("ExpandStoryBuilding")).Begin(me);
      else
        ((Storyboard)me.FindResource("CollapseStoryBuilding")).Begin(me);
    }

    public bool IsExpandAWSInbase
    {
      get { return (bool)GetValue(IsExpandAWSInbaseProperty); }
      set { SetValue(IsExpandAWSInbaseProperty, value); }
    }

    public static readonly DependencyProperty IsExpandAWSInbaseProperty =
      DependencyProperty.Register("IsExpandAWSInbase", typeof(bool), typeof(ViewCloud),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsExpandAWSInbaseChanged)));

    private static void OnIsExpandAWSInbaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewCloud me = (ViewCloud)d;

      if ((bool)e.NewValue)
        ((Storyboard)me.FindResource("ExpandStoryAWSInbase")).Begin(me);
      else
        ((Storyboard)me.FindResource("CollapseStoryAWSInbase")).Begin(me);
    }

    public bool IsExpandBEMS
    {
      get { return (bool)GetValue(IsExpandBEMSProperty); }
      set { SetValue(IsExpandBEMSProperty, value); }
    }

    public static readonly DependencyProperty IsExpandBEMSProperty =
      DependencyProperty.Register("IsExpandBEMS", typeof(bool), typeof(ViewCloud),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsExpandBEMSChanged)));

    private static void OnIsExpandBEMSChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewCloud me = (ViewCloud)d;

      if ((bool)e.NewValue)
        ((Storyboard)me.FindResource("ExpandStoryBEMS")).Begin(me);
      else
        ((Storyboard)me.FindResource("CollapseStoryBEMS")).Begin(me);
    }

    public bool IsExpandBEMSdb
    {
      get { return (bool)GetValue(IsExpandBEMSdbProperty); }
      set { SetValue(IsExpandBEMSdbProperty, value); }
    }

    public static readonly DependencyProperty IsExpandBEMSdbProperty =
      DependencyProperty.Register("IsExpandBEMSdb", typeof(bool), typeof(ViewCloud),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsExpandBEMSdbChanged)));

    private static void OnIsExpandBEMSdbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ViewCloud me = (ViewCloud)d;

      if ((bool)e.NewValue)
        ((Storyboard)me.FindResource("ExpandStoryBEMSdb")).Begin(me);
      else
        ((Storyboard)me.FindResource("CollapseStoryBEMSdb")).Begin(me);
    }

    public GatewayConfig GatewayConfig { get => DataContext as GatewayConfig; }

    public ViewCloud()
    {
      InitializeComponent();
    }

    private void thisControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      passwordBEMS.Password = AESCryptor.Decoding256(GatewayConfig.ConnectionInfoBEMS.Password);
      passwordBEMSdb.Password = AESCryptor.Decoding256(GatewayConfig.ConnectionInfoBEMSdb.Password);
    }

    private void ExpandConfig_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;

      if (element.Tag is string strTag)
      {
        if (strTag.Equals("Inbase"))
        {
          IsExpandAWS = false;
          IsExpandBuilding = false;
          IsExpandAWSInbase = !IsExpandAWSInbase;
          IsExpandBEMS = false;
          IsExpandBEMSdb = false;
        }
        else if (strTag.Equals("BEMS"))
        {
          IsExpandAWS = false;
          IsExpandBuilding = false;
          IsExpandAWSInbase = false;
          IsExpandBEMS = !IsExpandBEMS;
        }
        else if (strTag.Equals("BEMSdb"))
        {
          IsExpandAWS = false;
          IsExpandBuilding = false;
          IsExpandAWSInbase = false;
          IsExpandBEMSdb = !IsExpandBEMSdb;
        }
      }
      else
      {
        IsExpandAWS = !IsExpandAWS;
        IsExpandBuilding = false;
        IsExpandAWSInbase = false;
        IsExpandBEMS = false;
        IsExpandBEMSdb = false;
      }
    }

    private string _awsFileLastPath = "";

    private void AWSFileDialog_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        string dirname;
        string filename;

        try
        {
          dirname = Path.GetDirectoryName(_awsFileLastPath);
          filename = Path.GetFileName(_awsFileLastPath);
        }
        catch
        {
          dirname = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
          filename = "";
        }

        OpenFileDialog openFileDlg = new OpenFileDialog()
        {
          Title = "AWS 접속 파일",
          InitialDirectory = dirname,
          FileName = filename,
          DefaultExt = ".aws",
          Filter = "AWS Connection File (.aws)|*.aws"
        };

        if (openFileDlg.ShowDialog() == DialogResult.OK)
        {
          _awsFileLastPath = openFileDlg.FileName;

          FrameworkElement element = (FrameworkElement)sender;

          if (element.Tag is string strTag && strTag.Equals("Inbase"))
          {
            GatewayConfig.ConnectionInfoInbase = (AWSConfig)Json.LoadFile(openFileDlg.FileName, typeof(AWSConfig), FileShare.Read);
          }
          else
          {
            GatewayConfig.ConnectionInfo = (AWSConfig)Json.LoadFile(openFileDlg.FileName, typeof(AWSConfig), FileShare.Read);
          }
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"[AWSFileDialog_Click]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }

    private void AWSConnectionTest_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        FrameworkElement element = (FrameworkElement)sender;
        AWSConfig connectionInfo;

        if (element.Tag is string strTag && strTag.Equals("Inbase"))
        {
          connectionInfo = GatewayConfig.ConnectionInfoInbase;
        }
        else
        {
          connectionInfo = GatewayConfig.ConnectionInfo;

          string awsApiKey = AESCryptor.Decoding256(connectionInfo.AwsApiKey);

          if (awsApiKey.Equals(AESCryptor.NULL_CRYPTOR) ||
              connectionInfo.AlarmEventApiUrl.Equals(string.Empty) ||
              connectionInfo.RemoteControlApiUrl.Equals(string.Empty))
          {
            System.Windows.MessageBox.Show("AWS 접속정보(Lambda API)가 등록 되지 않았습니다.", "AWS Connection Test", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
          }
        }

        string accessKeyID = AESCryptor.Decoding256(connectionInfo.AccessKeyID);
        string secretAccessKey = AESCryptor.Decoding256(connectionInfo.SecretAccessKey);

        if (accessKeyID.Equals(AESCryptor.NULL_CRYPTOR) ||
            secretAccessKey.Equals(AESCryptor.NULL_CRYPTOR) ||
            connectionInfo.S3ServiceUrl.Equals(string.Empty))
        {
          System.Windows.MessageBox.Show("AWS 접속정보(S3)가 등록 되지 않았습니다.", "AWS Connection Test", MessageBoxButton.OK, MessageBoxImage.Warning);
          return;
        }

        if (ServiceS3.CheckConnection(AESCryptor.Decoding256(connectionInfo.AccessKeyID),
                                      AESCryptor.Decoding256(connectionInfo.SecretAccessKey),
                                      connectionInfo.S3ServiceUrl,
                                      connectionInfo.BucketName))
        {
          System.Windows.MessageBox.Show("접속 테스트 성공", "AWS Connection Test", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
          System.Windows.MessageBox.Show("접속 테스트 실패!!", "AWS Connection Test", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"[AWSConnectionTest_Click]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }

    private void ExpandBuilding_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      IsExpandAWS = false;
      IsExpandBuilding = !IsExpandBuilding;
      IsExpandAWSInbase = false;
    }

    private void ActiveSwitch_Toggled(object sender, RoutedEventArgs e)
    {
      ToggleSwitch toggleSwitch = (ToggleSwitch)sender;

      if (!toggleSwitch.IsOn)
      {
        if (toggleSwitch.Tag is string strTag)
        {
          if (strTag.Equals("Inbase"))
          {
            IsExpandAWSInbase = false;
          }
          else if (strTag.Equals("BEMS"))
          {
            IsExpandBEMS = false;
          }
        }
        else
        {
          IsExpandAWS = false;
          IsExpandBuilding = false;
        }
      }
    }

    private void SMBConnectionTest_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        SMBConfig smbConfig = GatewayConfig.ConnectionInfoBEMS;

        if (smbConfig.HostName.Equals(string.Empty) || smbConfig.StorePath.Equals(string.Empty))
        {
          System.Windows.MessageBox.Show("BEMS 원격 저장소가 등록 되지 않았습니다.", "SMB Connection Test", MessageBoxButton.OK, MessageBoxImage.Warning);
          return;
        }

        SMBClient smbClient = new SMBClient(smbConfig.HostName, smbConfig.UserName, AESCryptor.Decoding256(smbConfig.Password));

        if (smbClient.CheckConnection())
        {
          System.Windows.MessageBox.Show("접속 테스트 성공", "SMB Connection Test", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
          System.Windows.MessageBox.Show("접속 테스트 실패!!", "SMB Connection Test", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"[BEMSConnectionTest_Click]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }

    private void passwordBEMS_PasswordChanged(object sender, RoutedEventArgs e)
    {
      GatewayConfig.ConnectionInfoBEMS.Password = AESCryptor.Encoding256(passwordBEMS.Password);
    }

    private void DBConnectionTest_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        NpgsqlConfig dbConfig = GatewayConfig.ConnectionInfoBEMSdb;

        if (dbConfig.HostName.Equals(string.Empty) || dbConfig.Database.Equals(string.Empty))
        {
          System.Windows.MessageBox.Show("BEMS 제어 데이터베이스가 등록 되지 않았습니다.", "DB Connection Test", MessageBoxButton.OK, MessageBoxImage.Warning);
          return;
        }

        if (PostgresClient.ConnectionTest(dbConfig.ConnectionString).Result == "OK")
        {
          System.Windows.MessageBox.Show("연결 테스트 성공", "DB Connection Test", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
          System.Windows.MessageBox.Show("연결 테스트 실패!!", "DB Connection Test", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"[DBConnectionTest_Click]{ex}", GatewayConfig.Constants.SolutionNewName);
      }
    }

    private void passwordBEMSdb_PasswordChanged(object sender, RoutedEventArgs e)
    {
      GatewayConfig.ConnectionInfoBEMSdb.Password = AESCryptor.Encoding256(passwordBEMSdb.Password);
    }
  }
}
using iCos5.CSPGateway.AWS;
using iCos5.CSPGateway.DB;
using iCos5.CSPGateway.SMB;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace iCos5CSPGatewayED.View.Converter
{
  public class PasswordString : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is string))
        return "";

      string password = "";

      for (int i = 0; i < ((string)value).Length; i++)
      {
        password += "*";
      }

      return password;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class BooleanInverter : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is bool flag && !flag;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class BoolToVisibility : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = value is bool ? (bool)value : false;
      return flag ? Visibility.Visible : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class BoolToOpacity : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is bool flag && flag ? 1 : 0.6;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class NBoolToVisibility : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = value is bool ? (bool)value : false;
      return flag ? Visibility.Hidden : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class ConnectionInfoString : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (parameter is string serviceType)
      {
        if (serviceType.Equals("Insite"))
        {
          if (value is AWSConfig awsConfig)
          {
            if (!awsConfig.S3ServiceUrl.Equals(string.Empty) &&
                !awsConfig.BucketName.Equals(string.Empty) &&
                !awsConfig.ValueBucketFolder.Equals(string.Empty) &&
                !awsConfig.AlarmBucketFolder.Equals(string.Empty) &&
                !awsConfig.AccessKeyID.Equals(string.Empty) &&
                !awsConfig.SecretAccessKey.Equals(string.Empty) &&
                !awsConfig.AlarmEventApiUrl.Equals(string.Empty) &&
                !awsConfig.RemoteControlApiUrl.Equals(string.Empty) &&
                !awsConfig.AwsApiKey.Equals(string.Empty))
            {
              return "등록";
            }
          }
        }
        else if (serviceType.Equals("Inbase"))
        {
          if (value is AWSConfig awsConfig)
          {
            if (!awsConfig.S3ServiceUrl.Equals(string.Empty) &&
                !awsConfig.BucketName.Equals(string.Empty) &&
                !awsConfig.ValueBucketFolder.Equals(string.Empty) &&
                !awsConfig.AccessKeyID.Equals(string.Empty) &&
                !awsConfig.SecretAccessKey.Equals(string.Empty))
            {
              return "등록";
            }
          }
        }
        else if (serviceType.Equals("BEMS"))
        {
          if (value is SMBConfig smbConfig)
          {
            if (!smbConfig.HostName.Equals(string.Empty) &&
                !smbConfig.StorePath.Equals(string.Empty))
            {
              return "등록";
            }
          }
        }
        else if (serviceType.Equals("BEMSdb"))
        {
          if (value is NpgsqlConfig dbConfig)
          {
            if (!dbConfig.HostName.Equals(string.Empty) &&
                !dbConfig.Database.Equals(string.Empty))
            {
              return "등록";
            }
          }
        }

        return "미등록";
      }

      return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class CELEnableString : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = value is bool ? (bool)value : false;
      return flag ? "활성" : "비활성";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class MannedTypeString : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = value is bool ? (bool)value : false;
      return flag ? "유인 관제" : "무인 관제";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}

using iCos5.BACnet.Zenon;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace iCos5BACnetScheduleRT.View.Converter
{
  public class StartTimePos : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is DateTime))
        return new GridLength(0, GridUnitType.Star);

      DateTime startTime = (DateTime)value;

      if (startTime.Subtract(ScheduleItem.NullDateTime).TotalSeconds > 0)
      {
        double startRatio = (startTime.Hour * 3600 + startTime.Minute * 60 + startTime.Second) / 86400f;
        return new GridLength(startRatio, GridUnitType.Star);
      }
      else
      {
        return new GridLength(0, GridUnitType.Star);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class StopTimePos : MarkupExtension, IMultiValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length != 2)
        return new GridLength(1, GridUnitType.Star);

      double startRatio = 0;
      double stopRatio = 1;
      double heightRatio = 0;

      if (values[0] is DateTime)
      {
        DateTime startTime = (DateTime)values[0];

        if (startTime.Subtract(ScheduleItem.NullDateTime).TotalSeconds > 0)
        {
          startRatio = (startTime.Hour * 3600 + startTime.Minute * 60 + startTime.Second) / 86400f;
        }
      }

      if (values[1] is DateTime)
      {
        DateTime stopTime = (DateTime)values[1];

        if (stopTime.Subtract(ScheduleItem.NullDateTime).TotalSeconds > 0)
        {
          stopRatio = (stopTime.Hour * 3600 + stopTime.Minute * 60 + stopTime.Second) / 86400f;
        }
      }

      heightRatio = stopRatio - startRatio;
      return new GridLength(heightRatio, GridUnitType.Star);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class RestTimePos : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is DateTime))
        return new GridLength(0, GridUnitType.Star);

      DateTime stopTime = (DateTime)value;

      if (stopTime.Subtract(ScheduleItem.NullDateTime).TotalSeconds > 0)
      {
        double stopRatio = (stopTime.Hour * 3600 + stopTime.Minute * 60 + stopTime.Second) / 86400f;
        return new GridLength(1 - stopRatio, GridUnitType.Star);
      }
      else
      {
        return new GridLength(0, GridUnitType.Star);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class NormalTagVisibility : MarkupExtension, IMultiValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length != 2)
        return Visibility.Hidden;

      if (!(values[0] is DateTime) || !(values[1] is DateTime))
        return Visibility.Hidden;

      return ((DateTime)values[0]).Subtract(ScheduleItem.NullDateTime).TotalSeconds > 0 && 
             ((DateTime)values[1]).Subtract(ScheduleItem.NullDateTime).TotalSeconds > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class InvertVisibility : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is Visibility))
        return Visibility.Hidden;

      return (Visibility)value != Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class DateRangeText : MarkupExtension, IMultiValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length != 2)
        return "오류";

      string strStart = "";
      string strStop = "";

      if (values[0] != DependencyProperty.UnsetValue)
      {
        if (((DateTime)values[0]).Subtract(ScheduleItem.NullDateTime).TotalSeconds > 0)
          strStart = $"{values[0]:HH:mm:ss}";
      }

      if (values[1] != DependencyProperty.UnsetValue)
      {
        if (((DateTime)values[1]).Subtract(ScheduleItem.NullDateTime).TotalSeconds > 0)
          strStop = $"{values[1]:HH:mm:ss}";
      }

      return $"{strStart}~{strStop}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class AbnormalBindingPosition : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is DateTime))
        return VerticalAlignment.Top;

      DateTime startTime = (DateTime)value;

      if (startTime.Subtract(ScheduleItem.NullDateTime).TotalSeconds > 0)
      {
        return VerticalAlignment.Top;
      }
      else
      {
        return VerticalAlignment.Bottom;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class SelectedNormalBackground : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is bool))
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B066FFCC"));

      return (bool)value ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B088FFEE")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B066FFCC"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class SelectedAbnormalBackground : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is bool))
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B0CCCC66"));

      return (bool)value ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B0CCEE88")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B0CCCC66"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}

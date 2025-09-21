using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace iCos5.CSPGateway
{
  public class EnumDescriptionTypeConverter : EnumConverter
  {
    public EnumDescriptionTypeConverter(Type type) : base(type)
    {

    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        if (value != null)
        {
          FieldInfo fi = value.GetType().GetField(value.ToString());

          if (fi != null)
          {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
          }
        }

        return string.Empty;
      }

      return base.ConvertTo(context, culture, value, destinationType);
    }
  }

  [TypeConverter(typeof(EnumDescriptionTypeConverter))]
  public enum BellSounds
  {
    [Description("벨소리 0")]
    Bell00,
    [Description("벨소리 1")]
    Bell01,
    [Description("벨소리 2")]
    Bell02,
  }

  [TypeConverter(typeof(EnumDescriptionTypeConverter))]
  public enum BellPlayTypes
  {
    [Description("연속재생")]
    Continuous,
    [Description("한번만")]
    Once,
    [Description("재생안함")]
    None,
  }
}

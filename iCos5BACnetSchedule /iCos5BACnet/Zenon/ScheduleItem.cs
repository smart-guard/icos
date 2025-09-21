using iCos5.BACnet.MVVM;
using System;
using System.Collections.Generic;

namespace iCos5.BACnet.Zenon
{
  public class ScheduleItem : NotifyBase
  {
    public static readonly DateTime NullDateTime = new DateTime(1899, 1, 1, 0, 0, 0);

    private DateTime _time = DateTime.Now;
    public DateTime Time 
    { 
      get
      {
        return _time;
      }
      set
      {
        _time = value;
        OnPropertyChanged(nameof(Time));
      }
    }

    private BACnetTagType _tagType;
    public BACnetTagType TagType 
    {
      get 
      { 
        return _tagType; 
      }
      set
      {
        _tagType = value;
        OnPropertyChanged(nameof(TagType));
      }
    }

    private object _tagValue = 0x00;
    public object TagValue 
    { 
      get
      {
        return _tagValue;
      }
      set
      {
        _tagValue = value;
        OnPropertyChanged(nameof(TagValue));
      }
    }

    public ScheduleItem(BACnetTagType type)
    {
      TagType = type;
    }

    public ScheduleItem(DateTime dateTime, BACnetTagType type, object value)
    {
      Time = dateTime;
      TagType = type;
      TagValue = value;
    }

    public ScheduleItem(BACnetTagNode timeNode)
    {
      Time = (DateTime)timeNode.TagValue;
    }

    public void SetValue(BACnetTagNode valueNode)
    {
      TagType = valueNode.TagType;
      TagValue = valueNode.TagValue;
    }

    public List<BACnetTagNode> GetBACnetTagNodes()
    {
      List<BACnetTagNode> tagNodes = new List<BACnetTagNode>
      {
        new BACnetTagNode(BACnetTagClass.Application, BACnetTagType.Time, Time),
        new BACnetTagNode(BACnetTagClass.Application, TagType, TagValue)
      };

      return tagNodes;
    }
  }
}

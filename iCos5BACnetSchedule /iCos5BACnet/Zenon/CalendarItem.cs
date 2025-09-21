using iCos5.BACnet.MVVM;
using System;
using System.Collections.Generic;

namespace iCos5.BACnet.Zenon
{
  public class CalendarWeekNDay : NotifyBase
  {
    private int _month = 255;
    public int Month 
    { 
      get
      {
        return _month;
      }
      set
      {
        _month = value;
        OnPropertyChanged(nameof(Month));
      }
    }

    private int _weekOfMonth = 255;
    public int WeekOfMonth 
    { 
      get
      {
        return _weekOfMonth;
      }
      set
      {
        _weekOfMonth = value; 
        OnPropertyChanged(nameof(WeekOfMonth));
      }
    }

    private int _weekDay = 255;
    public int WeekDay 
    { 
      get
      {
        return _weekDay;
      }
      set
      {
        _weekDay = value;
        OnPropertyChanged(nameof(WeekDay));
      }
    }

    public CalendarWeekNDay()
    {

    }

    public CalendarWeekNDay(BACnetTagNode tagNode)
    {
      if (tagNode.TagClass != BACnetTagClass.ContextSpecific ||
          tagNode.TagType != BACnetTagType.UInt)
      {
        return;
      }

      string[] strNums = tagNode.TagValue.ToString().Split('.');

      if (strNums.Length != 3)
      {
        return;
      }

      Month = Convert.ToInt32(strNums[0]);
      WeekOfMonth = Convert.ToInt32(strNums[1]);
      WeekDay = Convert.ToInt32(strNums[2]);
    }

    public string GetBACnetTagNodeValue()
    {
      return $"{Month}.{WeekOfMonth}.{WeekDay}";
    }
  }

  public class CalendarEntry : NotifyBase
  {
    private BACnetCalendarEntryType _entryType = BACnetCalendarEntryType.Date;
    public BACnetCalendarEntryType EntryType 
    { 
      get
      {
        return _entryType;
      }
      set
      {
        _entryType = value; 
        OnPropertyChanged(nameof(EntryType));
      }
    }

    private DateTime _startDate = new DateTime(1900, 1, 1, 0, 0, 0);
    public DateTime StartDate 
    { 
      get
      {
        return _startDate;
      }
      set
      {
        _startDate = value; 
        OnPropertyChanged(nameof(StartDate));
      }
    }

    private DateTime _stopDate = new DateTime(1900, 1, 1, 0, 0, 0);
    public DateTime StopDate 
    { 
      get
      {
        return _stopDate;
      }
      set
      {
        _stopDate = value;
        OnPropertyChanged(nameof(StopDate));
      }
    }

    private CalendarWeekNDay _weekDay = new CalendarWeekNDay();
    public CalendarWeekNDay WeekDay 
    { 
      get
      {
        return _weekDay;
      }
      set
      {
        _weekDay = value;
        OnPropertyChanged(nameof(WeekDay));
      }
    }

    public CalendarEntry()
    {

    }

    public CalendarEntry(BACnetTagNode timeNode)
    {
      if (timeNode.TagClass != BACnetTagClass.ContextConstucted ||
          timeNode.TagType != BACnetTagType.Null)
      {
        return;
      }

      BACnetTagNode tagValue = ((List<BACnetTagNode>)timeNode.TagValue)[0];

      switch (tagValue.TagClass)
      {
        case BACnetTagClass.ContextConstucted:
          if (tagValue.TagType == BACnetTagType.Bool)
          {
            EntryType = BACnetCalendarEntryType.DateRange;
          }

          break;
        case BACnetTagClass.ContextSpecific:
          if (tagValue.TagType == BACnetTagType.Null)
          {
            EntryType = BACnetCalendarEntryType.Date;
          }
          else if (tagValue.TagType == BACnetTagType.UInt)
          {
            EntryType = BACnetCalendarEntryType.WeekNDay;
          }

          break;
        case BACnetTagClass.Application:
          break;
        default:
          break;
      }

      switch (EntryType)
      {
        case BACnetCalendarEntryType.Date:
          StartDate = getDateFromNode(tagValue);
          break;
        case BACnetCalendarEntryType.DateRange:
          List<BACnetTagNode> tagNodes = (List<BACnetTagNode>)tagValue.TagValue;
          StartDate = (DateTime)tagNodes[0].TagValue;
          StopDate = (DateTime)tagNodes[1].TagValue;
          break;
        case BACnetCalendarEntryType.WeekNDay:
          WeekDay = new CalendarWeekNDay(tagValue);
          break;
        default:
          break;
      }
    }

    public List<BACnetTagNode> GetNode()
    {
      BACnetTagNode tagNode = new BACnetTagNode();
      tagNode.TagClass = BACnetTagClass.ContextSpecific;
      tagNode.TagType = (BACnetTagType)EntryType;

      switch (EntryType)
      {
        case BACnetCalendarEntryType.Date:
          tagNode.TagValue = getBACnetDate(StartDate);
          break;
        case BACnetCalendarEntryType.DateRange:
          tagNode.TagClass = BACnetTagClass.ContextConstucted; 
          tagNode.TagValue = new List<BACnetTagNode>()
          {
            new BACnetTagNode(BACnetTagClass.Application, BACnetTagType.Date, StartDate),
            new BACnetTagNode(BACnetTagClass.Application, BACnetTagType.Date, StopDate)
          };
          break;
        case BACnetCalendarEntryType.WeekNDay:
          tagNode.TagValue = WeekDay.GetBACnetTagNodeValue();
          break;
      }

      return new List<BACnetTagNode>() { tagNode };
    }

    private DateTime getDateFromNode(BACnetTagNode tagValue)
    {
      string[] strDate = tagValue.TagValue.ToString().Trim().Split('.');
      return new DateTime(Convert.ToInt32(strDate[0]) + 1900, Convert.ToInt32(strDate[1]), Convert.ToInt32(strDate[2]));
    }

    private string getBACnetDate(DateTime date)
    {
      int dayOfWeek = date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
      return $"{date.Year - 1900}.{date.Month}.{date.Day}.{dayOfWeek}";
    }
  }
}

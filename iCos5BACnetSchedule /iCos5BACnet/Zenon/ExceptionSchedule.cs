using iCos5.BACnet.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace iCos5.BACnet.Zenon
{
  public class ExceptionScheduleSet : IBACnetScheduleSet
  {
    public ObservableCollection<BooleanScheduleItemPair>[] ScheduleItemPairs { get; set; }

    public ExceptionScheduleSet()
    {
      ScheduleItemPairs = new ObservableCollection<BooleanScheduleItemPair>[] {
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
      };
    }

    public ExceptionScheduleSet(ObservableCollection<BooleanScheduleItemPair>[] scheduleItemPairs)
    {
      ScheduleItemPairs = scheduleItemPairs;
    }
  }

  public class ExceptionScheduleItem : NotifyBase
  {
    private int _priority = 255;
    public int Priority 
    { 
      get
      {
        return _priority;
      }
      set
      {
        _priority = value;
        OnPropertyChanged(nameof(Priority));
      }
    }

    private CalendarEntry _calendar = new CalendarEntry();
    public CalendarEntry Calendar 
    { 
      get
      {
        return _calendar;
      }
      set
      {
        _calendar = value;
        OnPropertyChanged(nameof(Calendar));
      }
    }

    private ObservableCollection<BooleanScheduleItemPair> _scheduleItemPairs = new ObservableCollection<BooleanScheduleItemPair>();
    public ObservableCollection<BooleanScheduleItemPair> ScheduleItemPairs 
    { 
      get
      {
        return _scheduleItemPairs;
      }
      set
      {
        _scheduleItemPairs = value;
        OnPropertyChanged(nameof(ScheduleItemPairs));
      }
    }

    public ExceptionScheduleItem(int priority)
    {
      Priority = priority;
    }

    public ExceptionScheduleItem(BACnetTagNode calendarNode)
    {
      Calendar = new CalendarEntry(calendarNode);
    }

    public ExceptionScheduleItem(BACnetTagNode calendarNode, BACnetTagNode scheduleNode, BACnetTagNode priorityNode)
    {
      Calendar = new CalendarEntry(calendarNode);
      SetTagValueSets(scheduleNode);
      SetPriority(priorityNode);
    }

    public void SetTagValueSets(BACnetTagNode scheduleNode)
    {
      ScheduleItemPairs = new ObservableCollection<BooleanScheduleItemPair>();

      List<ScheduleItem> scheduleItems = new List<ScheduleItem>();
      ScheduleItem scheduleItem = null;

      foreach (BACnetTagNode tagNode in (List<BACnetTagNode>)scheduleNode.TagValue)
      {
        if (tagNode.TagType == BACnetTagType.Time)
        {
          scheduleItem = new ScheduleItem(tagNode);
          scheduleItems.Add(scheduleItem);
        }
        else
        {
          scheduleItem.SetValue(tagNode);
        }
      }

      scheduleItems.Sort((x, y) => x.Time.CompareTo(y.Time));

      BooleanScheduleItemPair booleanItemPair = null;

      foreach (ScheduleItem item in scheduleItems)
      {
        if (booleanItemPair == null)
        {
          if (Convert.ToBoolean(item.TagValue))
          {
            booleanItemPair = new BooleanScheduleItemPair(item, true);
          }
          else
          {
            ScheduleItemPairs.Add(new BooleanScheduleItemPair(item, false));
          }
        }
        else
        {
          if (Convert.ToBoolean(item.TagValue))
          {
            ScheduleItemPairs.Add(booleanItemPair);
            booleanItemPair = new BooleanScheduleItemPair(item, true);
          }
          else
          {
            booleanItemPair.StopItem = item;
            ScheduleItemPairs.Add(booleanItemPair);
            booleanItemPair = null;
          }
        }
      }

      if (booleanItemPair != null)
      {
        ScheduleItemPairs.Add(booleanItemPair);
      }
    }

    public void SetPriority(BACnetTagNode priorityNode)
    {
      Priority = Convert.ToInt32(priorityNode.TagValue);
    }
  }

  public class ExceptionSchedule : IBACnetSchedule
  {
    private BACnetTagNode _rootBACnetNode;

    public IBACnetScheduleSet ScheduleSet { get; private set; }
    public ObservableCollection<ExceptionScheduleItem> ExceptionScheduleItems { get; set; }

    public ExceptionSchedule()
    {
      _rootBACnetNode = new BACnetTagNode();
    }

    public ExceptionSchedule(byte[] bytes)
    {

    }

    public ExceptionSchedule(string code) : this(code, 13)
    {

    }

    public ExceptionSchedule(string code, int defaultPriority)
    {
      _rootBACnetNode = new BACnetTagNode(code, BACnetCodeType.OnlyValue);
      ExceptionScheduleItems = new ObservableCollection<ExceptionScheduleItem>();

      BACnetTagNode calendarNode = null;
      BACnetTagNode scheduleNode = null;
      BACnetTagNode priorityNode = null;

      foreach (BACnetTagNode tagNode in (List<BACnetTagNode>)_rootBACnetNode.TagValue)
      {
        switch (tagNode.TagType)
        {
          case BACnetTagType.Null:
            calendarNode = tagNode;
            break;
          case BACnetTagType.UInt:
            scheduleNode = tagNode;
            break;
          case BACnetTagType.Int:
            priorityNode = tagNode;

            if (calendarNode != null && scheduleNode != null && priorityNode != null)
            {
              ExceptionScheduleItems.Add(new ExceptionScheduleItem(calendarNode, scheduleNode, priorityNode));

              calendarNode = null;
              scheduleNode = null;
              priorityNode = null;
            }

            break;
        }
      }

      for (int i = ExceptionScheduleItems.Count; i < 5; i++)
      {
        ExceptionScheduleItems.Add(new ExceptionScheduleItem(defaultPriority));
      }

      ObservableCollection<BooleanScheduleItemPair>[] booleanScheduleItemPairs = new ObservableCollection<BooleanScheduleItemPair>[5]
      {
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>()
      };

      for (int i = 0; i < ExceptionScheduleItems.Count; i++)
      {
        if (i > 4)
        {
          break;
        }

        booleanScheduleItemPairs[i] = ExceptionScheduleItems[i].ScheduleItemPairs;
      }

      ScheduleSet = new ExceptionScheduleSet(booleanScheduleItemPairs);
    }

    public string GetCode()
    {
      List<BACnetTagNode> tagNodes = new List<BACnetTagNode>();

      for (int i = 0; i < ExceptionScheduleItems.Count; i++)
      {
        List<BACnetTagNode> scheduleNodes = new List<BACnetTagNode>();

        foreach (BooleanScheduleItemPair booleanItemPair in ExceptionScheduleItems[i].ScheduleItemPairs)
        {
          scheduleNodes.AddRange(booleanItemPair.GetBACnetTagNodes());
        }

        if (scheduleNodes.Count > 0)
        {
          tagNodes.Add(new BACnetTagNode(BACnetTagClass.ContextConstucted, BACnetTagType.Null, ExceptionScheduleItems[i].Calendar.GetNode()));
          tagNodes.Add(new BACnetTagNode(BACnetTagClass.ContextConstucted, BACnetTagType.UInt, scheduleNodes));
          tagNodes.Add(new BACnetTagNode(BACnetTagClass.ContextSpecific, BACnetTagType.Int, ExceptionScheduleItems[i].Priority));
        }
      }

      _rootBACnetNode.TagValue = tagNodes;
      return _rootBACnetNode.GetCodeOnlyValues();
    }

    public byte[] GetBytes()
    {
      return new byte[] { 0 };
    }

    public void ClearScheduleSet()
    {
      foreach (ObservableCollection<BooleanScheduleItemPair> scheduleItemPair in ScheduleSet.ScheduleItemPairs)
      {
        scheduleItemPair?.Clear();
      }
    }
  }
}

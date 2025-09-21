using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace iCos5.BACnet.Zenon
{
  public class WeeklyScheduleSet : IBACnetScheduleSet
  {
    public ObservableCollection<BooleanScheduleItemPair>[] ScheduleItemPairs { get; set; }

    public WeeklyScheduleSet()
    {
      ScheduleItemPairs = new ObservableCollection<BooleanScheduleItemPair>[] {
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>(),
        new ObservableCollection<BooleanScheduleItemPair>()
      };
    }

    public WeeklyScheduleSet(ObservableCollection<BooleanScheduleItemPair>[] scheduleItemPairs)
    {
      ScheduleItemPairs = scheduleItemPairs;
    }
  }

  public class WeeklySchedule : IBACnetSchedule
  {
    private BACnetTagNode _rootBACnetNode;
    private ObservableCollection<BooleanScheduleItemPair>[] _scheduleItemPairs = new ObservableCollection<BooleanScheduleItemPair>[7];

    public IBACnetScheduleSet ScheduleSet { get; private set; }

    public WeeklySchedule()
    {
      _rootBACnetNode = new BACnetTagNode();
      ScheduleSet = new WeeklyScheduleSet();
    }

    public WeeklySchedule(byte[] bytes)
    {

    }

    public WeeklySchedule(string code)
    {
      _rootBACnetNode = new BACnetTagNode(code, BACnetCodeType.OnlyValue);
      List<BACnetTagNode> weekNodes = (List<BACnetTagNode>)_rootBACnetNode.TagValue;

      for (int i = 0; i < weekNodes.Count; i++)
      {
        _scheduleItemPairs[i] = new ObservableCollection<BooleanScheduleItemPair>();

        ObservableCollection<BooleanScheduleItemPair> booleanItemPairs = _scheduleItemPairs[i];
        List<ScheduleItem> scheduleItems = new List<ScheduleItem>();
        ScheduleItem scheduleItem = null;

        foreach (BACnetTagNode tagNode in (List<BACnetTagNode>)weekNodes[i].TagValue)
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
              booleanItemPairs.Add(new BooleanScheduleItemPair(item, false));
            }
          }
          else
          {
            if (Convert.ToBoolean(item.TagValue))
            {
              booleanItemPairs.Add(booleanItemPair);
              booleanItemPair = new BooleanScheduleItemPair(item, true);
            }
            else
            {
              booleanItemPair.StopItem = item;
              booleanItemPairs.Add(booleanItemPair);
              booleanItemPair = null;
            }
          }
        }

        if (booleanItemPair != null)
        {
          booleanItemPairs.Add(booleanItemPair);
        }
      }

      ScheduleSet = new WeeklyScheduleSet(_scheduleItemPairs);
    }

    public string GetCode()
    {
      List<BACnetTagNode> weekNodes = (List<BACnetTagNode>)_rootBACnetNode.TagValue;

      for (int i = 0; i < weekNodes.Count; i++)
      {
        List<BACnetTagNode> tagNodes = new List<BACnetTagNode>();

        foreach (BooleanScheduleItemPair booleanItemPair in _scheduleItemPairs[i])
        {
          tagNodes.AddRange(booleanItemPair.GetBACnetTagNodes());
        }

        ((List<BACnetTagNode>)_rootBACnetNode.TagValue)[i].TagValue = tagNodes;
      }

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

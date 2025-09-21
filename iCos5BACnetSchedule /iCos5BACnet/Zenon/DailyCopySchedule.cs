using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace iCos5.BACnet.Zenon
{
  public class DailyCopyScheduleSet : IBACnetScheduleSet
  {
    public ObservableCollection<BooleanScheduleItemPair>[] ScheduleItemPairs { get; set; }

    public DailyCopyScheduleSet(ObservableCollection<BooleanScheduleItemPair>[] booleanScheduleItemPairs)
    {
      ScheduleItemPairs = booleanScheduleItemPairs;
    }
  }

  public class DailyCopySchedule : IBACnetSchedule
  {
    private BACnetTagNode _rootBACnetNode;
    private ObservableCollection<BooleanScheduleItemPair>[] _booleanScheduleItemPairs;

    public IBACnetScheduleSet ScheduleSet { get; }

    public DailyCopySchedule(ObservableCollection<BooleanScheduleItemPair>[] booleanScheduleItemPairs)
    {
      _rootBACnetNode = new BACnetTagNode();
      _booleanScheduleItemPairs = booleanScheduleItemPairs;
    }

    public DailyCopySchedule(string code)
    {
      _rootBACnetNode = new BACnetTagNode(code, BACnetCodeType.OnlyValue);
      List<BACnetTagNode> dailyTagNodes = (List<BACnetTagNode>)_rootBACnetNode.TagValue;
      List<ObservableCollection<BooleanScheduleItemPair>> booleanScheduleItemPairs = new List<ObservableCollection<BooleanScheduleItemPair>>();

      for (int i = 0; i < dailyTagNodes.Count; i++)
      {
        ObservableCollection<BooleanScheduleItemPair> booleanItemPairs = new ObservableCollection<BooleanScheduleItemPair>();
        List<ScheduleItem> scheduleItems = new List<ScheduleItem>();
        ScheduleItem scheduleItem = null;

        foreach (BACnetTagNode tagNode in (List<BACnetTagNode>)dailyTagNodes[i].TagValue)
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

        booleanScheduleItemPairs.Add(booleanItemPairs);
      }

      ScheduleSet = new DailyCopyScheduleSet(booleanScheduleItemPairs.ToArray());
    }

    public string GetCode()
    {
      List<BACnetTagNode> dailyTagNodes = new List<BACnetTagNode>();

      for (int i = 0; i < _booleanScheduleItemPairs.Length; i++)
      {
        List<BACnetTagNode> tagNodes = new List<BACnetTagNode>();

        foreach (BooleanScheduleItemPair booleanItemPair in _booleanScheduleItemPairs[i])
        {
          tagNodes.AddRange(booleanItemPair.GetBACnetTagNodes());
        }

        dailyTagNodes.Add(new BACnetTagNode(BACnetTagClass.ContextConstucted, BACnetTagType.Null, tagNodes));
      }

      _rootBACnetNode = new BACnetTagNode(BACnetTagClass.ContextConstucted, BACnetTagType.Real, dailyTagNodes); ;

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
        scheduleItemPair.Clear();
      }
    }
  }
}

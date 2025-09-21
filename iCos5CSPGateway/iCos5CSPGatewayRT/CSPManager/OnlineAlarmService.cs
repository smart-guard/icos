using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.AlarmMessageList;
using System;
using System.Collections.Generic;
using System.Timers;

namespace iCos5CSPGatewayRT.Manager
{
  public class OnlineAlarmService
  {
    private IProject _zenonProject;
    private List<OnlineAlarm> _alarmEntryQueue;
    private Timer _alarmEntryQueueUpdateTimer;
    private readonly int alarmEntryQueueUpdatePeriodMilliseconds = 100;
    private object _alarmEntryQueueLock = new object();
    private event EventHandler<AlarmServiceUpdatedEventArgs> updated;
    private EventHandler<AlarmServiceUpdatedEventArgs> _updatedEventHandler;
    private Timer _retryTimer;
    private ElapsedEventHandler _retryElapsedEventHandler;

    public OnlineAlarmService(IProject zenonProject,
                              EventHandler<AlarmServiceUpdatedEventArgs> updatedEventHandler,
                              int retryMilliSeconds,
                              ElapsedEventHandler retryElapsedEventHandler)
    {
      _zenonProject = zenonProject;
      _updatedEventHandler = updatedEventHandler;
      updated += _updatedEventHandler;

      _alarmEntryQueue = new List<OnlineAlarm>();
      _alarmEntryQueueUpdateTimer = new Timer(alarmEntryQueueUpdatePeriodMilliseconds);
      _alarmEntryQueueUpdateTimer.Elapsed += alarmEntryQueueUpdateTimer_Elapsed;
      _alarmEntryQueueUpdateTimer.Start();

      _zenonProject.AlarmMessageList.AlarmEntryReceived += alarmMessageList_AlarmEntryReceived;
      _zenonProject.AlarmMessageList.AlarmEntryCleared += alarmMessageList_AlarmEntryCleared;

      _retryTimer = new Timer(retryMilliSeconds);
      _retryElapsedEventHandler = retryElapsedEventHandler;
      _retryTimer.Elapsed += _retryElapsedEventHandler;
      _retryTimer.Start();
    }

    private void alarmEntryQueueUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      if (_alarmEntryQueue.Count > 0)
      {
        lock (_alarmEntryQueueLock)
        {
          onUpdated(_alarmEntryQueue.ToArray());
          _alarmEntryQueue.Clear();
        }
      }
    }

    private void onUpdated(OnlineAlarm[] onlineAlarms)
    {
      updated?.Invoke(this, new AlarmServiceUpdatedEventArgs(onlineAlarms));
    }

    private void alarmMessageList_AlarmEntryReceived(object sender, AlarmEntryReceivedEventArgs e)
    {
      lock (_alarmEntryQueueLock)
      {
        _alarmEntryQueue.Add(new OnlineAlarm(e.Item, OnlineAlarmReasonType.Received));
      }
    }

    private void alarmMessageList_AlarmEntryCleared(object sender, AlarmEntryClearedEventArgs e)
    {
      lock (_alarmEntryQueueLock)
      {
        _alarmEntryQueue.Add(new OnlineAlarm(e.Item, OnlineAlarmReasonType.Cleared));
      }
    }

    public void BeforeFree()
    {
      _retryTimer.Stop();
      _retryTimer.Elapsed -= _retryElapsedEventHandler;
      _retryTimer.Dispose();
      _retryTimer = null;

      _zenonProject.AlarmMessageList.AlarmEntryReceived -= alarmMessageList_AlarmEntryReceived;
      _zenonProject.AlarmMessageList.AlarmEntryCleared -= alarmMessageList_AlarmEntryCleared;

      _alarmEntryQueueUpdateTimer.Stop();
      _alarmEntryQueueUpdateTimer.Elapsed -= alarmEntryQueueUpdateTimer_Elapsed;
      _alarmEntryQueueUpdateTimer.Dispose();
      _alarmEntryQueueUpdateTimer = null;

      if (_alarmEntryQueue.Count > 0)
      {
        onUpdated(_alarmEntryQueue.ToArray());
        _alarmEntryQueue.Clear();
      }

      updated -= _updatedEventHandler;
    }
  }
}

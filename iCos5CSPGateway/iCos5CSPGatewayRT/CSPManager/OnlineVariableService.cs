using PASoft.Zenon.Addins.Extension;
using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.Variable;
using System;
using System.Collections.Generic;
using System.Timers;

namespace iCos5CSPGatewayRT.Manager
{
  public class OnlineVariableService
  {
    private readonly bool isActivatedBulkMode = true;

    public bool IsRunning { get; private set; } = false;

    private IProject _zenonProject;
    private IOnlineVariableContainer _onlineContainer;
    private List<string> _variables = new List<string>();
    private string[][] _variableSets;
    private bool _isLocalTime;
    private int _maxVariableCount;
    private bool _useDriverUpdateTime;
    private Dictionary<string, OnlineValue> _onlineValues;
    private Dictionary<string, AlarmMessageInfo> _dicAlarmMessageInfos;
    private Timer _delayTimer;
    private int _quotientSeq;

    public OnlineVariableService(IProject zenonProject,
                                 string onlineContainerName,
                                 bool isLocalTime,
                                 int delayMilliSeconds,
                                 int maxVariableCount,
                                 bool useDriverUpdateTime,
                                 ref Dictionary<string, OnlineValue> onlineValues,
                                 ref Dictionary<string, AlarmMessageInfo> dicAlarmMessageInfos)
    {
      _zenonProject = zenonProject;
      _isLocalTime = isLocalTime;

      if (_zenonProject.OnlineVariableContainerCollection[onlineContainerName] != null)
      {
        _zenonProject.OnlineVariableContainerCollection.Delete(onlineContainerName);
      }

      _onlineContainer = zenonProject.OnlineVariableContainerCollection.Create(onlineContainerName);

      if (isActivatedBulkMode)
      {
        _onlineContainer.BulkChanged += onlineContainer_BulkChanged;
        _onlineContainer.ActivateBulkMode();
      }
      else
      {
        _onlineContainer.Changed += onlineContainer_Changed;
      }

      _delayTimer = new Timer(delayMilliSeconds);
      _delayTimer.Elapsed += delayTimer_Elapsed;
      _delayTimer.AutoReset = false;

      _maxVariableCount = maxVariableCount;
      _useDriverUpdateTime = useDriverUpdateTime;
      _onlineValues = onlineValues;
      _dicAlarmMessageInfos = dicAlarmMessageInfos;
    }

    ~OnlineVariableService()
    {
      _delayTimer.Stop();
      _delayTimer.Elapsed -= delayTimer_Elapsed;
      _delayTimer.Dispose();
      _delayTimer = null;

      _onlineContainer.Deactivate();

      if (isActivatedBulkMode)
      {
        _onlineContainer.BulkChanged -= onlineContainer_BulkChanged;
      }
      else
      {
        _onlineContainer.Changed -= onlineContainer_Changed;
      }

      _zenonProject.OnlineVariableContainerCollection.Delete(_onlineContainer.Name);
      _onlineContainer = null;
    }

    public void AddVariable(IVariable variable)
    {
      AddVariable(variable.Name);
    }

    public void AddVariable(string variableName)
    {
      if (!_onlineValues.ContainsKey(variableName))
      {
        _variables.Add(variableName);
        _onlineValues.Add(variableName, new OnlineValue());
      }
    }

    public void RemoveVariable(IVariable variable)
    {
      RemoveVariable(variable.Name);
    }

    public void RemoveVariable(string variableName)
    {
      _variables.RemoveAll(x => x == variableName);

      if (!_onlineValues.ContainsKey(variableName))
      {
        _onlineValues.Remove(variableName);
      }
    }

    public void Initialization()
    {
      List<string[]> varSets = new List<string[]>();
      int quotient = _variables.Count / _maxVariableCount;
      int remainder = _variables.Count % _maxVariableCount;

      for (int i = 0; i < quotient; i++)
      {
        varSets.Add(_variables.GetRange(_maxVariableCount * i, _maxVariableCount).ToArray());
      }

      if (remainder > 0)
      {
        varSets.Add(_variables.GetRange(_maxVariableCount * quotient, remainder).ToArray());
      }

      _variableSets = varSets.ToArray();
    }

    private void onlineContainer_Changed(object sender, ChangedEventArgs e)
    {
      updateOnlineVariable(e.Variable);
      checkOnlineContainer();
    }

    private void onlineContainer_BulkChanged(object sender, BulkChangedEventArgs e)
    {
      foreach (IVariable variable in e.Variables)
      {
        updateOnlineVariable(variable);
      }

      checkOnlineContainer();
    }

    private void updateOnlineVariable(IVariable variable)
    {
      string varName = variable.Name;
      object value = variable.GetValue(0);
      _onlineValues[varName].LastUpdateTime = _useDriverUpdateTime && !_dicAlarmMessageInfos.ContainsKey(varName) ?
                                              variable.Get_LastUpdateTimeWithMilliSeconds(_isLocalTime) :
                                              _isLocalTime ? DateTime.Now : DateTime.UtcNow;
      _onlineValues[varName].StatusValue = variable.Get_StatusValue();

      if ((_onlineValues[varName].StatusValue & 0x40000) != 0 || !value.ToString().Contains("-2147483638"))
      {
        _onlineValues[varName].Value = value;
      }

      _onlineContainer.Remove(varName);
    }

    private void checkOnlineContainer()
    {
      if (_onlineContainer.Count == 0)
      {
        _onlineContainer.Deactivate();
        _quotientSeq++;

        if (_quotientSeq < _variableSets.Length)
        {
          _delayTimer.Start();
        }
        else
        {
          IsRunning = false;
        }
      }
    }

    private void delayTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      _delayTimer.Stop();

      _onlineContainer.AddVariable(_variableSets[_quotientSeq]);
      _onlineContainer.Activate();
    }

    public void Update()
    {
      if (IsRunning)
      {
        return;
      }

      IsRunning = true;

      _onlineContainer.Deactivate();
      _onlineContainer.RemoveAll();

      _quotientSeq = 0;
      _onlineContainer.AddVariable(_variableSets[_quotientSeq]);
      _onlineContainer.Activate();
    }
  }
}

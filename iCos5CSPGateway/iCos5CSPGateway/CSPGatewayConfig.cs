using iCos5.CSPGateway.AWS;
using iCos5.CSPGateway.DB;
using iCos5.CSPGateway.MVVM;
using iCos5.CSPGateway.SMB;
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;

namespace iCos5.CSPGateway
{
  public partial class GatewayConfig : NotifyBase
  {
    public static class Constants
    {
      public static readonly string RootName = "HDCLabs";
      public static readonly string SolutionNewName = "Cloud Gateway";
      public static readonly string SolutionName = "CSPGateway";
      public static readonly string SolutionVersion = "1.7";
      public static readonly string CSPOnlineContainer = "CSPGatewayOnlineContainer";
      public static readonly string DialogueVariable = "CSPGatewayDialogVariable";
      public static readonly string DialogueOnlineContainer = "CSPGatewayDialogueOnlineContainer";
      public static readonly string DialogueFunction = "CSPGatewayDialogFunction";
      public static readonly string DialogueFunctionParam = "CSPGatewayFunctionParam";
      public static readonly string BACnetDriverName = "BACNETNG";
      public static readonly char MultiBuildingDelimiter = '.';
    }

    private bool _cspDebugMode = false;
    public bool CSPDebugMode
    {
      get { return _cspDebugMode; }
      set
      {
        _cspDebugMode = value;
        OnPropertyChanged("CSPDebugMode");
      }
    }

    // Cloud ---------------------------------------------------------------------------------
    private bool _activeInsite = true;
    public bool ActiveInsite
    {
      get { return _activeInsite; }
      set
      {
        _activeInsite = value;
        OnPropertyChanged("ActiveInsite");
      }
    }

    private AWSConfig _connectionInfo = new AWSConfig();
    public AWSConfig ConnectionInfo
    {
      get { return _connectionInfo; }
      set
      {
        _connectionInfo = value;
        OnPropertyChanged("ConnectionInfo");
      }
    }

    private bool _isSingleBuilding = true;
    public bool IsSingleBuilding
    {
      get { return _isSingleBuilding; }
      set
      {
        _isSingleBuilding = value;
        OnPropertyChanged("IsSingleBuilding");
      }
    }

    private int _buildingID = 0;
    public int BuildingID
    {
      get { return _buildingID; }
      set
      {
        _buildingID = value;
        OnPropertyChanged("BuildingID");
      }
    }

    private bool _activeInbase = false;
    public bool ActiveInbase
    {
      get { return _activeInbase; }
      set
      {
        _activeInbase = value;
        OnPropertyChanged("ActiveInbase");
      }
    }

    private AWSConfig _connectionInfoInbase = new AWSConfig();
    public AWSConfig ConnectionInfoInbase
    {
      get { return _connectionInfoInbase; }
      set
      {
        _connectionInfoInbase = value;
        OnPropertyChanged("ConnectionInfoInbase");
      }
    }

    private string _buildingIDInbase = string.Empty;
    public string BuildingIDInbase
    {
      get { return _buildingIDInbase; }
      set
      {
        _buildingIDInbase = value;
        OnPropertyChanged("BuildingIDInbase");
      }
    }

    private bool _activeBEMS = false;
    public bool ActiveBEMS
    {
      get { return _activeBEMS; }
      set
      {
        _activeBEMS = value;
        OnPropertyChanged("ActiveBEMS");
      }
    }

    private SMBConfig _connectionInfoBEMS = new SMBConfig();
    public SMBConfig ConnectionInfoBEMS
    {
      get { return _connectionInfoBEMS; }
      set
      {
        _connectionInfoBEMS = value;
        OnPropertyChanged("ConnectionInfoBEMS");
      }
    }

    private NpgsqlConfig _connectionInfoBEMSdb = new NpgsqlConfig();
    public NpgsqlConfig ConnectionInfoBEMSdb
    {
      get { return _connectionInfoBEMSdb; }
      set
      {
        _connectionInfoBEMSdb = value;
        OnPropertyChanged("ConnectionInfoBEMSdb");
      }
    }

    private string _buildingIDBEMS = string.Empty;
    public string BuildingIDBEMS
    {
      get { return _buildingIDBEMS; }
      set
      {
        _buildingIDBEMS = value;
        OnPropertyChanged("BuildingIDBEMS");
      }
    }

    // Common ---------------------------------------------------------------------------------
    private bool _isLocalTime = true;
    public bool IsLocalTime
    {
      get { return _isLocalTime; }
      set
      {
        _isLocalTime = value;
        OnPropertyChanged("IsLocalTime");
      }
    }

    private int _retransmissionCount = 5;
    public int RetransmissionCount
    {
      get { return _retransmissionCount; }
      set
      {
        _retransmissionCount = value;
        OnPropertyChanged("RetransmissionCount");
      }
    }

    private int _retransmissionInterval = 100;
    public int RetransmissionInterval
    {
      get { return _retransmissionInterval; }
      set
      {
        _retransmissionInterval = value;
        OnPropertyChanged("RetransmissionInterval");
      }
    }

    private bool _enableAllTagSelection = false;
    public bool EnableAllTagSelection
    {
      get { return _enableAllTagSelection; }
      set
      {
        _enableAllTagSelection = value;
        OnPropertyChanged("EnableAllTagSelection");
      }
    }

    private bool _enableCEL = true;
    public bool EnableCEL
    {
      get { return _enableCEL; }
      set
      {
        _enableCEL = value;
        OnPropertyChanged("EnableCEL");
      }
    }

    private int _celClassPvID = -1;
    public int CELClassPvID
    {
      get { return _celClassPvID; }
      set
      {
        _celClassPvID = value;
        OnPropertyChanged("CELClassPvID");
      }
    }

    private string _celGroupName = "None";
    public string CELGroupName
    {
      get { return _celGroupName; }
      set
      {
        _celGroupName = value;
        OnPropertyChanged("CELGroupName");
      }
    }

    private int _celGroupPvID = -1;
    public int CELGroupPvID
    {
      get { return _celGroupPvID; }
      set
      {
        _celGroupPvID = value;
        OnPropertyChanged("CELGroupPvID");
      }
    }

    private bool _detailLogging = false;
    public bool DetailLogging
    {
      get { return _detailLogging; }
      set
      {
        _detailLogging = value;
        OnPropertyChanged("DetailLogging");
      }
    }

    private string _tempPath = @"c:\iCos5\CloudGateway\";
    public string TempPath
    {
      get { return _tempPath; }
      set
      {
        _tempPath = value;
        OnPropertyChanged("TempPath");
      }
    }

    // Value ----------------------------------------------------------------------------------
    private int _periodInterval = 5;
    public int PeriodInterval
    {
      get { return _periodInterval; }
      set
      {
        _periodInterval = value;
        OnPropertyChanged("PeriodInterval");
      }
    }

    private int _periodOffset = 0;
    public int PeriodOffset
    {
      get { return _periodOffset; }
      set
      {
        _periodOffset = value;
        OnPropertyChanged("PeriodOffset");
      }
    }

    private int _maximumCount = 1000;
    public int MaximumCount
    {
      get { return _maximumCount; }
      set
      {
        _maximumCount = value;
        OnPropertyChanged("MaximumCount");
      }
    }

    private int _sequenceInterval = 100;
    public int SequenceInterval
    {
      get { return _sequenceInterval; }
      set
      {
        _sequenceInterval = value;
        OnPropertyChanged("SequenceInterval");
      }
    }

    private bool _initialUpdate = false;
    public bool InitialUpdate
    {
      get { return _initialUpdate; }
      set
      {
        _initialUpdate = value;
        OnPropertyChanged("InitialUpdate");
      }
    }

    private bool _useDriverUpdateTime = false;
    public bool UseDriverUpdateTime
    {
      get { return _useDriverUpdateTime; }
      set
      {
        _useDriverUpdateTime = value;
        OnPropertyChanged("UseDriverUpdateTime");
      }
    }

    // Alarm ----------------------------------------------------------------------------------
    private bool _isAlarmRetry = false;
    public bool IsAlarmRetry
    {
      get { return _isAlarmRetry; }
      set
      {
        _isAlarmRetry = value;
        OnPropertyChanged("IsAlarmRetry");
      }
    }

    private int _alarmIgnoreMinutes = 10;
    public int AlarmIgnoreMinutes
    {
      get { return _alarmIgnoreMinutes; }
      set
      {
        _alarmIgnoreMinutes = value;
        OnPropertyChanged("AlarmIgnoreMinutes");
      }
    }

    private int _alarmRetryInterval = 60;
    public int AlarmRetryInterval
    {
      get { return _alarmRetryInterval; }
      set
      {
        _alarmRetryInterval = value;
        OnPropertyChanged("AlarmRetryInterval");
      }
    }

    // Control --------------------------------------------------------------------------------
    private bool _isUseControl = false;
    public bool IsUseControl
    {
      get { return _isUseControl; }
      set
      {
        _isUseControl = value;
        OnPropertyChanged("IsUseControl");
      }
    }

    private int _controlFlushPeriod = 60;
    public int ControlFlushPeriod
    {
      get { return _controlFlushPeriod; }
      set
      {
        _controlFlushPeriod = value;
        OnPropertyChanged("ControlFlushPeriod");
      }
    }

    private int _controlUpdatePeriod = 10;
    public int ControlUpdatePeriod
    {
      get { return _controlUpdatePeriod; }
      set
      {
        _controlUpdatePeriod = value;
        OnPropertyChanged("ControlUpdatePeriod");
      }
    }

    private int _controlTimeout = 5;
    public int ControlTimeout
    {
      get { return _controlTimeout; }
      set
      {
        _controlTimeout = value;
        OnPropertyChanged("ControlTimeout");
      }
    }

    private bool _isMannedControl = false;
    public bool IsMannedControl
    {
      get { return _isMannedControl; }
      set
      {
        _isMannedControl = value;
        OnPropertyChanged("IsMannedControl");
      }
    }

    private int _holdOnTime = 3;
    public int HoldOnTime
    {
      get { return _holdOnTime; }
      set
      {
        _holdOnTime = value;
        OnPropertyChanged("HoldOnTime");
      }
    }

    private BellSounds _bellSound = BellSounds.Bell00;
    public BellSounds BellSound
    {
      get { return _bellSound; }
      set
      {
        _bellSound = value;
        OnPropertyChanged("BellSound");
      }
    }

    private BellPlayTypes _bellPlayType = BellPlayTypes.Continuous;
    public BellPlayTypes BellPlayType
    {
      get { return _bellPlayType; }
      set
      {
        _bellPlayType = value;
        OnPropertyChanged("BellPlayType");
      }
    }

    // ScriptIgnore ---------------------------------------------------------------------------
    [ScriptIgnore]
    public ObservableCollection<string> CELGroups { get; set; } = new ObservableCollection<string>();

    public void Reset()
    {
      ActiveInsite = false;
      ConnectionInfo = new AWSConfig();
      IsSingleBuilding = true;
      BuildingID = 0;
      ActiveInbase = false;
      ConnectionInfoInbase = new AWSConfig();
      BuildingIDInbase = string.Empty;
      ActiveBEMS = false;
      ConnectionInfoBEMS = new SMBConfig();
      ConnectionInfoBEMSdb = new NpgsqlConfig();
      BuildingIDBEMS = string.Empty;

      IsLocalTime = true;
      RetransmissionCount = 5;
      RetransmissionInterval = 100;
      EnableAllTagSelection = false;
      EnableCEL = true;
      CELClassPvID = -1;
      CELGroupName = "None";
      CELGroupPvID = -1;
      DetailLogging = true;
      TempPath = @"c:\iCos5\CSPGateway\";

      PeriodInterval = 5;
      PeriodOffset = 0;
      MaximumCount = 1000;
      SequenceInterval = 100;

      IsAlarmRetry = false;
      AlarmIgnoreMinutes = 10;
      AlarmRetryInterval = 60;

      IsUseControl = false;
      ControlUpdatePeriod = 10;
      ControlTimeout = 5;
      IsMannedControl = false;
      HoldOnTime = 3;
      BellSound = BellSounds.Bell00;
      BellPlayType = BellPlayTypes.Continuous;
    }
  }
}

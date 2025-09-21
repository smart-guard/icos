namespace iCos5.BACnet.Zenon
{
  public enum BACnetCodeType
  {
    Full,
    WithoutBraces,
    OnlyValue,
  }

  public enum BACnetTagParserState
  {
    Ready,
    OpenTag,
    TagType,
    SkipSpace,
    TagValue,
    GenValue,
    SkipComma,
  }

  public enum BACnetTagClass
  {
    ContextConstucted = 0,
    ContextSpecific   = 1,
    Application       = 2,
  }

  public enum BACnetCalendarEntryType
  {
    Date      = 0,
    DateRange = 1,
    WeekNDay  = 2,
  }

  public enum BACnetTagType
  {
    Null        = 0,
    Bool        = 1,
    UInt        = 2,
    Int         = 3,
    Real        = 4,
    Double      = 5,
    OctetString = 6,
    CharString  = 7,
    BitString   = 8,
    Enum        = 9,
    Date        = 10,
    Time        = 11,
    ObjectID    = 12,
  }

  public enum BACnetReliability
  {
    NoFaultDetected       = 0,
    NoSensor              = 1,
    OverRange             = 2,
    UnderRange            = 3,
    OpenLoop              = 4,
    ShortedLoop           = 5,
    NoOutput              = 6,
    UnreliableOther       = 7,
    ProcessError          = 8,
    MultiStateFault       = 9,
    ConfigurationError    = 10,
    MemberFault           = 11,
    CommunicationFailure  = 12,
    Tripped               = 13,
  }

  public enum BACnetObjectTypes
  {
    AnalogInput           = 0,
    AnalogOutput          = 1,
    AnalogValue           = 2,
    BinaryInput           = 3,
    BinaryOutput          = 4,
    BinaryValue           = 5,
    Calendar              = 6,
    Command               = 7,
    Device                = 8,
    EventEnrollment       = 9,
    File                  = 10,
    Group                 = 11,
    Loop                  = 12,
    MultiStateInput       = 13,
    MultiStateOutput      = 14,
    NotificationClass     = 15,
    Program               = 16,
    Schedule              = 17,
    Averaging             = 18,
    MultiStateValue       = 19,
    Trendlog              = 20,
    LifeSafetyPoint       = 21,
    LifeSafetyZone        = 22,
    Accumulator           = 23,
    PulseConverter        = 24,
    EventLog              = 25,
    GlobalGroup           = 26,
    TrendLogMultiple      = 27,
    LoadControl           = 28,
    StructuredView        = 29,
    AccessDoor            = 30,
    Timer                 = 31,
    AccessCredential      = 32,
    AccessPoint           = 33,
    AccessRights          = 34,
    AccessUser            = 35,
    AccessZone            = 36,
    CredentialDataInput   = 37,
    NetworkSecurity       = 38,
    BitstringValue        = 39,
    CharacterstringValue  = 40,
    DatePatternValue      = 41,
    DateValue             = 42,
    DatetimePatternValue  = 43,
    DatetimeValue         = 44,
    IntegerValue          = 45,
    LargeAnalogValue      = 46,
    OctetstringValue      = 47,
    PositiveIntegerValue  = 48,
    TimePatternValue      = 49,
    TimeValue             = 50,
    NotificationForwarder = 51,
    AlertEnrollment       = 52,
    Channel               = 53,
    LightingOutput        = 54,
    BinaryLightingOutput  = 55,
    NetworkPort           = 56,
    ElevatorGroup         = 57,
    Escalator             = 58,
    Lift                  = 59,
    Staging               = 60,
    AuditLog              = 61,
    AuditReporter         = 62,
    Color                 = 63,
    ColorTemperature      = 64,
  }

  public enum BACnetPropertyID
  {
    DateList                        = 23,
    Description                     = 28,
    EffectivePeriod                 = 32,
    ExeptionSchedule                = 38,
    ListOfObjectPropertyReferences  = 54,
    ObjectIdentifier                = 75,
    ObjectName                      = 77,
    ObjectType                      = 79,
    OutOfService                    = 81,
    PresentValue                    = 85,
    PriorityForWriting              = 88,
    Reliability                     = 103,
    StatusFlags                     = 111,
    WeeklySchedule                  = 123,
    ScheduleDefault                 = 174,
    PropertyList                    = 371,
  }
}

using iCos5.CSPGateway.MVVM;

namespace iCos5.CSPGateway.AWS
{
  public class AWSConfig : NotifyBase
  {
    private string _s3ServiceUrl = "";

    public string S3ServiceUrl
    {
      get { return _s3ServiceUrl; }
      set
      {
        _s3ServiceUrl = value;
        OnPropertyChanged("S3ServiceUrl");
      }
    }

    private string _bucketName = "";

    public string BucketName
    {
      get { return _bucketName; }
      set
      {
        _bucketName = value;
        OnPropertyChanged("BucketName");
      }
    }

    private string _valueBucketFolder = "";

    public string ValueBucketFolder
    {
      get { return _valueBucketFolder; }
      set
      {
        _valueBucketFolder = value;
        OnPropertyChanged("ValueBucketFolder");
      }
    }

    private string _alarmBucketFolder = "";

    public string AlarmBucketFolder
    {
      get { return _alarmBucketFolder; }
      set
      {
        _alarmBucketFolder = value;
        OnPropertyChanged("AlarmBucketFolder");
      }
    }

    private string _accessKeyID = "";

    public string AccessKeyID
    {
      get { return _accessKeyID; }
      set
      {
        _accessKeyID = value;
        OnPropertyChanged("AccessKeyID");
      }
    }

    private string _secretAccessKey = "";

    public string SecretAccessKey
    {
      get { return _secretAccessKey; }
      set
      {
        _secretAccessKey = value;
        OnPropertyChanged("SecretAccessKey");
      }
    }

    private string _alarmEventApiUrl = "";

    public string AlarmEventApiUrl
    {
      get { return _alarmEventApiUrl; }
      set
      {
        _alarmEventApiUrl = value;
        OnPropertyChanged("AlarmEventApiUrl");
      }
    }

    private string _remoteControlApiUrl = "";

    public string RemoteControlApiUrl
    {
      get { return _remoteControlApiUrl; }
      set
      {
        _remoteControlApiUrl = value;
        OnPropertyChanged("RemoteControlApiUrl");
      }
    }

    private string _awsApiKey = "";

    public string AwsApiKey
    {
      get { return _awsApiKey; }
      set
      {
        _awsApiKey = value;
        OnPropertyChanged("AwsApiKey");
      }
    }
  }
}

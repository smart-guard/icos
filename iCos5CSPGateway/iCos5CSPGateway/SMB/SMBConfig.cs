using iCos5.CSPGateway.MVVM;
using System.Web.Script.Serialization;

namespace iCos5.CSPGateway.SMB
{
  public class SMBConfig : NotifyBase
  {
    private string _hostName = "localhost";
    public string HostName
    {
      get { return _hostName; }
      set
      {
        _hostName = value;
        OnPropertyChanged("HostName");
        OnPropertyChanged("RepositoryFullPath");
      }
    }

    private string _storePath = "";
    public string StorePath
    {
      get { return _storePath; }
      set
      {
        _storePath = value;
        OnPropertyChanged("StorePath");
        OnPropertyChanged("RepositoryFullPath");
      }
    }

    private string _userName = "";
    public string UserName
    {
      get { return _userName; }
      set
      {
        _userName = value;
        OnPropertyChanged("UserName");
      }
    }

    private string _password = "";
    public string Password
    {
      get { return _password; }
      set
      {
        _password = value;
        OnPropertyChanged("Password");
      }
    }

    [ScriptIgnore]
    public string RepositoryFullPath { get => $@"\\{HostName}\{StorePath.Replace('/', '\\').Trim(new char[] { ' ', '\\' })}"; }
  }
}

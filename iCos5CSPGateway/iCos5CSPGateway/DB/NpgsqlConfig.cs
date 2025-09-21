using iCos5.CSPGateway.MVVM;
using PASoft.Common.Encryption;
using System.Web.Script.Serialization;

namespace iCos5.CSPGateway.DB
{
  public class NpgsqlConfig : NotifyBase
  {
    private string _hostName = "localhost";
    public string HostName
    {
      get { return _hostName; }
      set
      {
        _hostName = value;
        OnPropertyChanged("HostName");
        OnPropertyChanged("ConnectionStringView");
      }
    }

    private int _port = 5432;
    public int Port
    {
      get { return _port; }
      set
      {
        _port = value;
        OnPropertyChanged("Port");
        OnPropertyChanged("ConnectionStringView");
      }
    }

    private string _database = "postgres";
    public string Database
    {
      get { return _database; }
      set
      {
        _database = value;
        OnPropertyChanged("Database");
        OnPropertyChanged("ConnectionStringView");
      }
    }

    private bool _isIntegrated = false;
    public bool IsIntegrated
    {
      get { return _isIntegrated; }
      set
      {
        _isIntegrated = value;
        OnPropertyChanged("IsIntegrated");
        OnPropertyChanged("ConnectionStringView");
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
        OnPropertyChanged("ConnectionStringView");
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

    private string _requestProcedure = "SELECT * FROM sp_bems_batc_16_data_prmt_ovst_list()";
    public string RequestProcedure
    {
      get { return _requestProcedure; }
      set
      {
        _requestProcedure = value;
        OnPropertyChanged("RequestProcedure");
      }
    }

    private string _replyProcedure = "SELECT sp_bems_batc_17_data_prmt_ovst_rslt_ins('ID', 'TIME', 'MESSAGE')";
    public string ReplyProcedure
    {
      get { return _replyProcedure; }
      set
      {
        _replyProcedure = value;
        OnPropertyChanged("ReplyProcedure");
      }
    }

    [ScriptIgnore]
    public string ConnectionStringView { get => $"Server={HostName};Port={Port};Database={Database};" + 
                                                (IsIntegrated ? "Integrated Security=true;" 
                                                              : $"User Id={UserName};Password=****;"); }

    [ScriptIgnore]
    public string ConnectionString
    {
      get => $"Server={HostName};Port={Port};Database={Database};" +
                                            (IsIntegrated ? "Integrated Security=true;"
                                                          : $"User Id={UserName};Password={AESCryptor.Decoding256(Password)};");
    }
  }
}

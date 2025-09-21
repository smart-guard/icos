using System;
using System.IO;
using System.Net;
using System.Text;

namespace iCos5CSPGateway.AWS
{
  public enum HTTPMethod
  {
    Get,
    Post,
    Put,
    Delete
  }

  public class ServiceAPI
  {
    public static string InvokeAPI(string url, HTTPMethod method, string key, string message)
    {
      string responseFromServer = string.Empty;
      byte[] bytes = Encoding.UTF8.GetBytes(message);

      WebRequest request = WebRequest.Create(url);
      request.Method = Enum.GetName(typeof(HTTPMethod), method);
      request.ContentType = "application/json; charset=utf-8";
      request.ContentLength = bytes.Length;
      request.Headers.Add("x-api-key", key);
      ((HttpWebRequest)request).UserAgent = "iCos5/1.1";

      using (Stream reqStream = request.GetRequestStream())
      {
        reqStream.Write(bytes, 0, bytes.Length);
      }

      using (WebResponse response = request.GetResponse())
      {
        using (Stream dataStream = response.GetResponseStream())
        {
          using (StreamReader reader = new StreamReader(dataStream))
          {
            responseFromServer = reader.ReadToEnd();
          }
        }
      }

      return responseFromServer;
    }

    private string _urlAlarm;
    private string _urlControl;
    private string _key;

    public ServiceAPI(string urlAlarm, string urlControl, string key)
    {
      _urlAlarm = urlAlarm;
      _urlControl = urlControl;
      _key = key;
    }

    public string InvokeAlarm(string message)
    {
      return InvokeAPI(_urlAlarm, HTTPMethod.Post, _key, message);
    }

    public string InvokeGetControl(string message)
    {
      return InvokeAPI(_urlControl, HTTPMethod.Post, _key, message);
    }

    public string InvokeSetControl(string message)
    {
      return InvokeAPI(_urlControl, HTTPMethod.Put, _key, message);
    }
  }
}

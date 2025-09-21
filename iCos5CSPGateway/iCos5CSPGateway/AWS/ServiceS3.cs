using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;

namespace iCos5CSPGateway.AWS
{
  public partial class ServiceS3
  {
    public bool IsConnected 
    { 
      get
      {
        try
        {
          if (_s3Client == null)
          {
            return false;
          }

          return AmazonS3Util.DoesS3BucketExistV2(_s3Client, _bucketName);
        }
        catch (Exception)
        {
          return false;
        }
      }
    }

    private AmazonS3Client _s3Client;
    private string _bucketName;

    public ServiceS3(string accessKey, string secretKey, string url, string bucketName)
    {
      _s3Client = GetNewAmazonS3Client(accessKey, secretKey, url);
      _bucketName = bucketName;
    }

    ~ServiceS3()
    {
      _s3Client?.Dispose();
    }

    public List<S3Object> ListBucketContents()
    {
      return ListBucketContents(_s3Client, _bucketName);
    }

    public List<string> ListBucketContentKeys(string prefix = "")
    {
      List<string> keys = new List<string>();

      foreach (S3Object s3Object in ListBucketContents(_s3Client, _bucketName, prefix))
      {
        keys.Add(s3Object.Key);
      }

      return keys;
    }

    public bool UploadFile(string objectName, string filePath)
    {
      return UploadFile(_s3Client, _bucketName, objectName, filePath);
    }

    public bool DownloadObjectFromBucket(string objectName, string filePath)
    {
      return DownloadObjectFromBucket(_s3Client, _bucketName, objectName, filePath);
    }

    public bool DeleteFile(string objectName)
    {
      return DeleteFile(_s3Client, _bucketName, objectName);
    }
  }
}

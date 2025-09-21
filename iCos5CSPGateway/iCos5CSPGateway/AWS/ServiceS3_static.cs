using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace iCos5CSPGateway.AWS
{
  public partial class ServiceS3
  {
    public static AmazonS3Client GetNewAmazonS3Client(string accessKey, string secretKey, string url)
    {
      AmazonS3Config config = new AmazonS3Config
      {
        MaxErrorRetry = 3,
        ReadWriteTimeout = TimeSpan.FromSeconds(10),
        RetryMode = RequestRetryMode.Standard,
        ServiceURL = url,
        Timeout = TimeSpan.FromSeconds(10),
      };
      return new AmazonS3Client(accessKey, secretKey, config);
    }

    public static bool CheckConnection(string accessKey, string secretKey, string url, string bucketName)
    {
      try
      {
        AmazonS3Config config = new AmazonS3Config();
        config.ServiceURL = url;
        AmazonS3Client ns3Client = new AmazonS3Client(accessKey, secretKey, config);

        return AmazonS3Util.DoesS3BucketExistV2(ns3Client, bucketName);
      }
      catch (Exception)
      {
        return false;
      }
    }

    public static List<S3Object> ListBucketContents(IAmazonS3 client, string bucketName, string prefix = "")
    {
      ListObjectsV2Request request = new ListObjectsV2Request
      {
        BucketName = bucketName,
        Prefix = prefix,
        MaxKeys = 5,
      };

      List<S3Object> s3Objects = new List<S3Object>();
      ListObjectsV2Response response;

      do
      {
        response = client.ListObjectsV2(request);
        s3Objects.AddRange(response.S3Objects);
        request.ContinuationToken = response.NextContinuationToken;
      }
      while (response.IsTruncated);

      return s3Objects;
    }

    public static async Task<List<S3Object>> ListBucketContentsAsync(IAmazonS3 client, string bucketName)
    {
      ListObjectsV2Request request = new ListObjectsV2Request
      {
        BucketName = bucketName,
        MaxKeys = 5,
      };

      List<S3Object> s3Objects = new List<S3Object>();
      ListObjectsV2Response response;

      do
      {
        response = await client.ListObjectsV2Async(request);
        s3Objects.AddRange(response.S3Objects);
        request.ContinuationToken = response.NextContinuationToken;
      }
      while (response.IsTruncated);

      return s3Objects;
    }

    public static bool UploadFile(IAmazonS3 client, string bucketName, string objectName, string filePath)
    {
      PutObjectRequest request = new PutObjectRequest
      {
        BucketName = bucketName,
        Key = objectName,
        FilePath = filePath,
      };

      PutObjectResponse response = client.PutObject(request);
      return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public static async Task<bool> UploadFileAsync(IAmazonS3 client, string bucketName, string objectName, string filePath)
    {
      PutObjectRequest request = new PutObjectRequest
      {
        BucketName = bucketName,
        Key = objectName,
        FilePath = filePath,
      };

      PutObjectResponse response = await client.PutObjectAsync(request);
      return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public static bool DownloadObjectFromBucket(IAmazonS3 client, string bucketName, string objectName, string filePath)
    {
      GetObjectRequest request = new GetObjectRequest
      {
        BucketName = bucketName,
        Key = objectName,
      };

      GetObjectResponse response = client.GetObject(request);
      response.WriteResponseStreamToFile(filePath);
      return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public static async Task<bool> DownloadObjectFromBucketAsync(IAmazonS3 client, string bucketName, string objectName, string filePath)
    {
      GetObjectRequest request = new GetObjectRequest
      {
        BucketName = bucketName,
        Key = objectName,
      };

      GetObjectResponse response = await client.GetObjectAsync(request);
      await response.WriteResponseStreamToFileAsync(filePath, false, CancellationToken.None);
      return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public static bool DeleteFile(IAmazonS3 client, string bucketName, string objectName)
    {
      DeleteObjectRequest request = new DeleteObjectRequest
      {
        BucketName = bucketName,
        Key = objectName,
      };

      DeleteObjectResponse response = client.DeleteObject(request);
      return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public static async Task<bool> DeleteFileAsync(IAmazonS3 client, string bucketName, string objectName)
    {
      DeleteObjectRequest request = new DeleteObjectRequest
      {
        BucketName = bucketName,
        Key = objectName,
      };

      DeleteObjectResponse response = await client.DeleteObjectAsync(request);
      return response.HttpStatusCode == HttpStatusCode.OK;
    }
  }
}

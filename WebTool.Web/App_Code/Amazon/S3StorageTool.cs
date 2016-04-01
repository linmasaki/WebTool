using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;


public class S3StorageTool
{
    private static string DefaultBucket = "mybucket";

    public S3StorageTool()
    {

    }

    //取得目前該金鑰權限下的所有Bucket列表
    public static List<string> GetBucketList()
    {
        var buckets = new List<string>();
        var credentials = new StoredProfileAWSCredentials("developer"); //若config檔裡有指定profileName屬性,則此段可省略,下面改寫為 var client = new AmazonS3Client()

        using (var client = new AmazonS3Client(credentials))
        {
            try
            {
                ListBucketsResponse response = client.ListBuckets();

                foreach (S3Bucket bucket in response.Buckets)
                {
                    buckets.Add(bucket.BucketName);
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    LogTools.Error("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");

                }
                else
                {
                    LogTools.Error(string.Format("An Error, number {0}, occurred when listing buckets with the message '{1}", amazonS3Exception.ErrorCode, amazonS3Exception.Message));
                }
            }
        }

        return buckets;
    }

    //取得指定Bucket根目錄下的檔案列表
    public static List<string> GetBucketRootList(string bucket)
    {
        var files = new List<string>();
        var credentials = new StoredProfileAWSCredentials("developer");

        using (var client = new AmazonS3Client(credentials))
        {
            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = bucket;
                ListObjectsResponse response = client.ListObjects(request);

                do
                {
                    response = client.ListObjects(request);
                    foreach (S3Object entry in response.S3Objects.Where(r => r.Key.Count(k => k == '/') <= 1))
                    {
                        files.Add(entry.Key);
                    }

                    if (response.IsTruncated)
                    {
                        request.Marker = response.NextMarker;
                    }
                    else
                    {
                        request = null;
                    }
                } while (request != null);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    LogTools.Error("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    LogTools.Error(string.Format("An error occurred with the message '{0}' when listing objects", amazonS3Exception.Message));
                }
            }
        }

        return files;
    }

    //取得指定目錄之檔案列表
    public static List<string> GetFileList(string bucket, string path = "")
    {
        var files = new List<string>();
        var credentials = new StoredProfileAWSCredentials("developer");

        using (var client = new AmazonS3Client(credentials))
        {
            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = bucket;
                request.Prefix = path;
                ListObjectsResponse response = client.ListObjects(request);

                do
                {
                    response = client.ListObjects(request);
                    foreach (S3Object entry in response.S3Objects)
                    {
                        files.Add(entry.Key);
                    }

                    if (response.IsTruncated)
                    {
                        request.Marker = response.NextMarker;
                    }
                    else
                    {
                        request = null;
                    }
                } while (request != null);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    LogTools.Error("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    LogTools.Error(string.Format("An error occurred with the message '{0}' when listing objects", amazonS3Exception.Message));
                }
            }
        }

        return files;
    }


    //取得指定Bucket下符合指定副檔名的檔案列表
    public static List<string> GetFileListWithExt(string bucket, string ext)
    {
        var files = new List<string>();
        var credentials = new StoredProfileAWSCredentials("developer");

        using (var client = new AmazonS3Client(credentials))
        {
            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = bucket;
                ListObjectsResponse response = client.ListObjects(request);

                do
                {
                    response = client.ListObjects(request);
                    foreach (S3Object entry in response.S3Objects.Where(r => r.Key.EndsWith(ext)))
                    {
                        files.Add(entry.Key);
                    }

                    if (response.IsTruncated)
                    {
                        request.Marker = response.NextMarker;
                    }
                    else
                    {
                        request = null;
                    }
                } while (request != null);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    LogTools.Error("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    LogTools.Error(string.Format("An error occurred with the message '{0}' when listing objects", amazonS3Exception.Message));
                }
            }
        }

        return files;
    }

    //取得檔案內容
    public static string GetFileContent(string bucket, string path)
    {
        var credentials = new StoredProfileAWSCredentials("developer");
        IAmazonS3 client = new AmazonS3Client(credentials);

        GetObjectRequest request = new GetObjectRequest()
        {
            BucketName = bucket,
            Key = path
        };

        using (GetObjectResponse response = client.GetObject(request))
        {
            StreamReader reader = new StreamReader(response.ResponseStream);
            return reader.ReadToEnd();
        }
    }

}


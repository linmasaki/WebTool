<%@ WebHandler Language="C#" Class="GetS3BucketList" %>

using System;
using System.Web;


public class GetS3BucketList : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        try
        {
            var s3Buckets = S3StorageTool.GetBucketList();
            context.ReturnPackage(s3Buckets);
        }
        catch (Exception ex)
        {
           context.ReturnPackage(ex.Message);
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}
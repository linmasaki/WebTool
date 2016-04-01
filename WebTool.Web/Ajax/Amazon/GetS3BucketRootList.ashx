<%@ WebHandler Language="C#" Class="GetS3BucketRootList" %>

using System;
using System.Web;

public class GetS3BucketRootList : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        var bucket = context.Request["buk"].NullTrim();
        
        if (string.IsNullOrWhiteSpace(bucket))
        {
            context.ReturnPackage("Please input bucket name！");
            return;
        }

        try
        {
            var s3BucketFiles = S3StorageTool.GetBucketRootList(bucket);

            context.ReturnPackage(s3BucketFiles);
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
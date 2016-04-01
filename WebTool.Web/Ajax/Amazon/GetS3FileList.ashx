<%@ WebHandler Language="C#" Class="GetS3FileList" %>

using System;
using System.Web;

public class GetS3FileList : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        var bucket = context.Request["buk"].NullTrim();
        var path = context.Request["path"].NullTrim();

        if (string.IsNullOrWhiteSpace(bucket))
        {
            context.ReturnPackage("Please input bucket name！");
            return;
        }

        try
        {
            var s3BucketFiles = S3StorageTool.GetFileList(bucket, path);

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
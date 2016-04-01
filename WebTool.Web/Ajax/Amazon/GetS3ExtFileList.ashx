<%@ WebHandler Language="C#" Class="GetS3ExtFileList" %>

using System;
using System.Web;

public class GetS3ExtFileList : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        var bucket = context.Request["buk"].NullTrim();
        var ext = context.Request["ext"].NullTrim();
        if (string.IsNullOrWhiteSpace(bucket) ||string.IsNullOrWhiteSpace(ext))
        {
            context.ReturnPackage("Please input bucket and extension name！");
            return;
        }

        if (!ext.StartsWith("."))
        {
            ext = "." + ext;
        }

        try
        {
            var s3BucketFiles = S3StorageTool.GetFileListWithExt(bucket, ext);

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
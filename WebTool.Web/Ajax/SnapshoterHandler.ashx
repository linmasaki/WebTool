<%@ WebHandler Language="C#" Class="SnapshoterHandler" %>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using WebTool.Common;

public class SnapshoterHandler : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        var url = context.Request["url"].NullTrim();

        if (string.IsNullOrEmpty(url))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        url = HttpUtility.UrlDecode(url);
        var fileName = Convert.ToBase64String(Encoding.UTF8.GetBytes(url)).TrimEnd('=');

        if (!Directory.Exists(context.Server.MapPath("~/Image")))
        {
            Directory.CreateDirectory(context.Server.MapPath("~/Image"));
        }
        
        var filePath = context.Server.MapPath(string.Format("~/Image/{0}.jpg", fileName));
        if (!File.Exists(filePath))
        {
            try
            {
                Snapshoter.ListenForURL(url, filePath);

                var now = DateTime.Now;
                while (!File.Exists(filePath))
                {
                    Thread.Sleep(600);
                    if ((DateTime.Now - now).TotalSeconds > 20)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
        }

        var cacheFilePath = context.Server.MapPath(string.Format("~/Image/{0}_cache.jpg", fileName));

        try
        {
            Image.FromFile(filePath).Resize(256, 192, ImageTool.ResizeMode.Stretch).Save(cacheFilePath, ImageFormat.Jpeg);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return;
        }

        context.Response.ContentType = "image/jpeg";
        context.Response.WriteFile(cacheFilePath);
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}
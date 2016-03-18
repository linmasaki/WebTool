<%@ WebHandler Language="C#" Class="FileHandler" %>

using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using WebTool.Common;

public class FileHandler : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        var dirPath = context.Request["path"].NullTrim();
        var showSub = context.Request["sub"].AsBool(true);
        var showHidden = context.Request["hidden"].AsBool(true);

        if (string.IsNullOrWhiteSpace(dirPath))
        {
            context.ReturnPackage("資料夾路徑尚未填寫！");
            return;
        }

        var serverPath = context.Server.MapPath(dirPath);
        if (!Directory.Exists(serverPath))
        {
            context.ReturnPackage("目標資料夾不存在！");
            return;
        }

        var fileList = FileTool.ListFiles(serverPath, showSub, showHidden);

        context.ReturnPackage(fileList);
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}
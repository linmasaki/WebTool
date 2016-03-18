<%@ WebHandler Language="C#" Class="ImageHandler" %>

using System;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.WebPages;
using WebTool.Common;

public class ImageHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) 
    {
        var mode = context.Request["mode"].AsInt(1);
        var path = "~" + context.Request.Params["path"].NullTrim();
        var serverPath = context.Server.MapPath(path);
        var width = context.Request["width"].AsInt(300);
        var height = context.Request["height"].AsInt(300);
        var startX = context.Request["x"].AsInt(0);
        var startY = context.Request["y"].AsInt(0);
        var quality = context.Request["quality"].AsInt(80);
        
        try
        {
            if (context.Request.Files.Count == 0)
            {
                context.ReturnPackage("無檔案上傳！");
                return;
            }
            
            if (!Directory.Exists(serverPath))
            {
                context.ReturnPackage("存檔路徑不存在！");
                return;
            }
            
            HttpPostedFile postedFile = context.Request.Files[0];
            if (!CheckImage(postedFile))
            {
                context.ReturnPackage("不正確的檔案格式(非圖片檔)！");
                return;
            }

            var fileName = postedFile.FileName;
            var savePath = Path.Combine(serverPath, fileName);
            var image = Image.FromStream(postedFile.InputStream, true, true);

            switch (mode)
            {
                case 1:
                    image = image.Resize(width, height, ImageTool.ResizeMode.Normal);
                    image.Save(savePath);    //將上傳的圖片儲存
                    break;
                case 2:
                    image = image.Resize(width, height, ImageTool.ResizeMode.Stretch);
                    image.Save(savePath);
                    break;
                case 3:
                    image = image.Resize(width, height, ImageTool.ResizeMode.Crop);
                    image.Save(savePath);
                    break;
                case 4:
                    image = image.Resize(width, height, ImageTool.ResizeMode.Fill);
                    image.Save(savePath);
                    break;
                case 5:
                    image = image.Resize(width, height, ImageTool.ResizeMode.Designation, startX, startY);
                    image.Save(savePath);
                    break;
                case 6:
                    image.SaveVaryQualityLevel(savePath, quality);   //壓縮圖片
                    break;
            }

            context.ReturnPackage(fileName, true);
        }
        catch (Exception ex)
        {
            context.ReturnPackage(ex.Message);
        }
    }

    //判斷上傳檔案是否為圖檔
    public bool CheckImage(HttpPostedFile file)
    {
        var sr = file.InputStream;
        var buffer = new Byte[10];

        sr.Read(buffer, 0, 10);
        var header = ((int)buffer[0]).ToString("X2") + ((int)buffer[1]).ToString("X2");

        switch (header)
        {
            case "FFD8":    //jpeg
            case "4949":    //tif
            case "8950":    //png
            case "424D":    //bmp
            case "4749":    //gif
                return true;
            default:
                return false;
        }
    }
    
    public bool IsReusable {
        get {
            return false;
        }
    }
}
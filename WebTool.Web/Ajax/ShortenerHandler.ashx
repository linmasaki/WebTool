<%@ WebHandler Language="C#" Class="ShortenerHandler" %>

using System;
using System.Web;
using WebTool.Google;

public class ShortenerHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context)
    {
        var url = context.Request["url"].NullTrim();
        
        //產生短網址
        var urlShortener = new UrlShortener("AIzaSyDUAjdOjEP6aaPhkC8Q1fkRnAcFysWbEK8");
        var shortUrl = urlShortener.GetShorten(url);
        
        context.ReturnPackage(shortUrl, true);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}
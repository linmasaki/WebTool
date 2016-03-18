using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;

/// <summary>
/// Summary description for CommonExtension
/// </summary>
public static class CommonExtension
{
    public static int NullLength<T>(this T[] self)
    {
        if (self == null) return 0;
        return self.Length;
    }

    #region Time Extensions

    // 以下是取得 "台北標準時區" 的標準寫法
    private static TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");

    public static DateTime ToTaiwanTime(this DateTime self)
    {
        // 判斷本機的時區設定是否為 UTC 時間，如果是，就要進行轉換，否則就直接顯示本地時間
        //if (TimeZoneInfo.Local == TimeZoneInfo.Utc)
        //if (TimeZoneInfo.Local.Equals(tzi)) return self.ToLocalTime();
        // 依據取得的時區進行時間轉換
        return TimeZoneInfo.ConvertTime(self, tzi);
        //return self.AddHours(8);
    }

    private static TimeSpan _utcZeroTimeSpan = new System.TimeSpan(System.DateTime.Parse("1/1/1970").Ticks);
    public static long ToJsTimestamp(this System.DateTime self)
    {
        var time = self.Subtract(_utcZeroTimeSpan);
        return (long)(time.Ticks / 10000);
    }
    //public static DateTime FormTaiwanDate(this string self)
    //{
    //    try
    //    {
    //        return DateTime.ParseExact(self, "yyyy-M-d", null, DateTimeStyles.AssumeUniversal).AddHours(-8);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //public static DateTime FormTaiwanTime(this string self)
    //{
    //    try
    //    {
    //        return DateTime.ParseExact(self, "yyyy-M-d HH:m:s", null, DateTimeStyles.AssumeUniversal).AddHours(-8);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    #endregion

    #region HttpContext Extensions

    public static string RequestContent(this HttpContext self)
    {
        var body = self.Request.InputStream;
        var encoding = self.Request.ContentEncoding;
        using (var reader = new System.IO.StreamReader(body, encoding))
        {
            return reader.ReadToEnd();
        }
    }

    public static void ReturnPackage(this HttpContext self, string message, bool ok)
    {
        self.Response.ContentType = string.IsNullOrEmpty(self.Request["callback"])
            ? "text/javascript"
            : "application/json";
        var json = JsonConvert.SerializeObject(new Package() { ok = ok, msg = message });
        self.Response.Write(string.IsNullOrEmpty(self.Request["callback"])
            ? json
            : string.Format("{0}({1});", self.Request["callback"], json));
    }

    public static void ReturnPackage(this HttpContext self, string error = null)
    {
        self.Response.ContentType = string.IsNullOrEmpty(self.Request["callback"])
            ? "text/javascript"
            : "application/json";
        var json = JsonConvert.SerializeObject(error == null ? new Package() : new Package(error));

        self.Response.Write(string.IsNullOrEmpty(self.Request["callback"])
            ? json
            : string.Format("{0}({1});", self.Request["callback"], json));
    }

    public static void ReturnPackage<T>(this HttpContext self, T package, string message = null, bool ok = true)
    {
        self.Response.ContentType = string.IsNullOrEmpty(self.Request["callback"])
            ? "text/javascript"
            : "application/json";
        //var pkg = message == null ? new Package<T>(package) : new Package<T>(package) { msg = message };
        var json = JsonConvert.SerializeObject(new Package<T>(package) { ok = ok, msg = message });

        self.Response.Write(string.IsNullOrEmpty(self.Request["callback"])
            ? json
            : string.Format("{0}({1});", self.Request["callback"], json));
    }

    public static string GetClientIp(this HttpContext self)
    {
        var real = self.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (!string.IsNullOrEmpty(real))
        {
            var ip = real.Split(',').LastOrDefault();
            IPAddress address;
            if (!string.IsNullOrEmpty(ip) && IPAddress.TryParse(ip, out address))
                return address.ToString();
        }
        return self.Request.ServerVariables["REMOTE_ADDR"];
    }

    #endregion

    #region String Extensions

    public static string NullToLower(this string self)
    {
        if (string.IsNullOrEmpty(self)) return null;
        return self.ToLower();
    }

    public static string NullTrim(this string self)
    {
        if (string.IsNullOrEmpty(self)) return null;
        return self.Trim();
    }

    public static string NullTrimToLower(this string self)
    {
        return self.NullTrim().NullToLower();
    }

    public static string ToStringN(this Guid self)
    {
        return self.ToString("N");
    }

    public static bool IsValidEmail(this string email)
    {
        return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"); 
    }

    public static string[] NullSplitNewLine(this string self)
    {
        if (string.IsNullOrEmpty(self)) return null;
        var replace = self.Replace("\r\n", "\n");
        return replace.Split('\n');
    }

    public static string Right(this string self, int maxLength)
    {
        //Check if the value is valid
        if (string.IsNullOrEmpty(self))
        {
            //Set valid empty string as string could be null
            self = string.Empty;
        }
        else if (self.Length > maxLength)
        {
            //Make the string no longer than the max length
            self = self.Substring(self.Length - maxLength, maxLength);
        }

        //Return the string
        return self;
    }

    #endregion
}
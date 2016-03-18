using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


namespace WebTool.Google
{
    /// <summary>
    /// http://www.dotblogs.com.tw/larrynung/archive/2011/08/03/32506.aspx
    /// </summary>
    public class UrlShortener
    {
        const String BASE_API_URL = @"https://www.googleapis.com/urlshortener/v1/url";
        const String SHORTENER_URL_PATTERN = BASE_API_URL + @"?key={0}";
        const String EXPAND_URL_PATTERN =  BASE_API_URL + @"?shortUrl={0}";
        private String _apiKey;
  
        public UrlShortener(string googleApiKey)
        {
            if (string.IsNullOrEmpty(googleApiKey))
            {
                throw new ArgumentNullException("googleApiKey");
            }

            this.ApiKey = googleApiKey;    
        }

        private String ApiKey
        {
            get
            {
                return _apiKey ?? string.Empty;
            }
            set
            {
                _apiKey = value;
            }
        }

        private string GetHTMLSourceCode(string url)
        {
            var request = (WebRequest.Create(url)) as HttpWebRequest;
            var response = request.GetResponse() as HttpWebResponse;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Get Short URL
        /// </summary>
        /// <param name="url">Source Url</param>
        /// <returns></returns>
        public string GetShorten(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (ApiKey.Length == 0)
            {
                throw new Exception("APIKey not set!");
            }

            const string POST_PATTERN = @"{{""longUrl"": ""{0}""}}";
            const string MATCH_PATTERN = @"""id"": ?""(?<id>.+)""";
            var post = string.Format(POST_PATTERN, url);
            var request = (HttpWebRequest)WebRequest.Create(string.Format(SHORTENER_URL_PATTERN, ApiKey));

            request.Method = "POST";
            request.ContentLength = post.Length;
            request.ContentType = "application/json";
            request.Headers.Add("Cache-Control", "no-cache");

            using (var requestStream = request.GetRequestStream())
            {
                var buffer = Encoding.ASCII.GetBytes(post);
                requestStream.Write(buffer, 0, buffer.Length);
            }

            try
            {
                using (var responseStream = request.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(responseStream))
                    {
                        return Regex.Match(sr.ReadToEnd(), MATCH_PATTERN).Groups["id"].Value;
                    }
                }
            }
            catch (WebException webEx)
            {
                string responseData;
                using (var reader = new StreamReader(webEx.Response.GetResponseStream()))
                {
                    responseData = reader.ReadToEnd();
                }

                throw new Exception(webEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        /// <summary>
        /// Get Source URL
        /// </summary>
        /// <param name="url">Short Url</param>
        /// <returns></returns>
        public String GetExpand(string url)
        {
            const string MATCH_PATTERN = @"""longUrl"": ?""(?<longUrl>.+)""";
            var expandUrl = string.Format(EXPAND_URL_PATTERN, url);
            var response = GetHTMLSourceCode(expandUrl);
        
            return Regex.Match(response, MATCH_PATTERN).Groups["longUrl"].Value;
        }
    }
}

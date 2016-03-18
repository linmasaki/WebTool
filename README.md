# Webtool

* Google Tool 
* Common Tool
* ...


## Google Tool
### Google Url Shortening

使用此API時,您必須先向Google申請相關的<a href="https://console.developers.google.com" target="_blank">憑證</a>

To convert longUrl to shortUrl:
```C#
using WebTool.Google;

//Generate short URLs
var urlShortener = new UrlShortener("Your Google Api Key");
var shortUrl = urlShortener.GetShorten("Souece URL");
```

To convert shortUrl to sourceUrl:
```C#
using WebTool.Google;

//Generate short URLs
var urlShortener = new UrlShortener("Your Google Api Key");
var sourceUrl = urlShortener.GetExpand("Short URL");
```


## Common Tool
### Image Tool

Convert image to new size image:
```C#
using WebTool.Common;

//Resize target image
System.Drawing.Image image = Image.FromStream(postedFile.InputStream, true, true);
image = image.Resize(width, height, ImageTool.ResizeMode.Normal);
```
* ResizeMode :<br/>
1.Normal : 維持原本的圖片比例,以指定寬高中比例較小的一方為基準去做等比例放大或縮小<br/>
2.Stretch : 以指定的寬高來放大或縮小圖片<br/>
3.Crop : 以指定寬高中比例較大的一方為基準去做等比例縮放後(維持原本的圖片比例),再做剪裁<br/>
4.Fill : 以指定寬高中比例較小的一方為基準去做等比例縮放後(維持原本的圖片比例),不足部分將以空白補上<br/>
5.Designation : 以指定的寬高及位置來對圖片做剪裁<br/>

Add watermark text to image:


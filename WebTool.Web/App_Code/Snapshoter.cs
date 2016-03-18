using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Awesomium.Core;

/// <summary>
/// Summary description for Snapshoter
/// </summary>
public class Snapshoter
{
    public static volatile bool webCoreStarted;
    public static volatile int savingSnapshots;

    public Snapshoter()
    {
    }

    public static void AwesomiumThread()
    {
        // Initialize the WebCore with some configuration settings.
        WebCore.Initialize(new WebConfig()
        {
            //UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.76 Safari/537.36",
            //LogPath = Environment.CurrentDirectory + "/awesomium.log",
            //LogLevel = LogLevel.Verbose,
        });

        WebCore.CreateWebSession(new WebPreferences() { CustomCSS = "::-webkit-scrollbar { visibility: hidden; }" });

        // Check if the WebCore is already automatically updating.
        // A background thread has no message loop and synchronization context.
        if (WebCore.UpdateState != WebCoreUpdateState.NotUpdating)
            return;

        // Tell the WebCore to create an Awesomium-specific
        // synchronization context and start an update loop.
        // The current thread will be blocked until WebCore.Shutdown
        // is called. For details about the new auto-updating and 
        // synchronization model of Awesomium.NET, read the documentation
        // of WebCore.Run.
        WebCore.Run((s, e) => webCoreStarted = true);
    }

    public static void ListenForURL(string url, string savePath)
    {
        // Get the URL. ToUri is a String extension provided
        // by Awesomium.Core.Utilities that can help you easily
        // convert strings to a URL.
        Uri uri = url.ToUri();

        // ToUri is errors-free. If an invalid string is specified,
        // a blank URI (about:blank) will be returned that can be
        // checked with IsBlank, a helper Uri extension.
        if (uri.IsBlank())
        {
            return;
        }

        // We demonstrate using WebCore.QueueWork to queue work to be executed on Awesomium's thread.
        WebCore.QueueWork(() => NavigateAndTakeSnapshots(uri, savePath));
    }

    // This is executed on Awesomium's thread.
    private static void NavigateAndTakeSnapshots(Uri url, string savePath)
    {
        savingSnapshots++;

        // Create a view that uses our WebSession.
        WebView view = WebCore.CreateWebView(1024, 768, WebCore.Sessions.Last());

        // Prevent new windows.
        view.ShowCreatedWebView += (s, e) => e.Cancel = true;

        // Respond to failures.
        view.LoadingFrameFailed += (s, e) =>
        {
            // We do not mind about child frames.
            if (!e.IsMainFrame)
                return;
        };

        // This event is fired when a frame in the page finishes loading.
        view.LoadingFrameComplete += (s, e) =>
        {
            // The main frame usually finishes loading last for a given page load.
            if (!e.IsMainFrame)
                return;

            // Take snapshots of the page.
            TakeSnapshots((WebView)s, savePath);
        };

        // Load the URL.
        view.Source = url;
    }

    // This is executed on Awesomium's thread.
    static void TakeSnapshots(WebView view, string filePath)
    {
        if (!view.IsLive)
        {
            // Dispose the view.
            view.Dispose();
            return;
        }

        // A BitmapSurface is assigned by default to all WebViews.
        BitmapSurface surface = (BitmapSurface)view.Surface;

        // Save the buffer to a PNG image.
        surface.SaveToJPEG(filePath, 75);

        view.Dispose();
        savingSnapshots--;
        return;
    }
}
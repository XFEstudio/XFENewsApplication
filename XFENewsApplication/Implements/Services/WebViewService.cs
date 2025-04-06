using XFENewsApplication.Interface.Services;

namespace XFENewsApplication.Implements.Services;

public class WebViewService : IWebViewService
{
    private WebView2? webView;
    public WebView2 WebView { get => webView ?? throw new NullReferenceException(); set => webView = value; }
    public bool IsWebViewLoaded { get => WebView.IsLoaded; }
}

using Windows.Foundation;

namespace XFENewsApplication.Interface.Services;

public interface IWebViewService
{
    public WebView2 WebView { get; set; }
    public bool IsWebViewLoaded { get; }
    public Uri? Url { get => WebView.Source; set => WebView.Source = value; }

    public IAsyncOperation<string> ExecuteScriptAsync(string javascriptCode) => WebView.ExecuteScriptAsync(javascriptCode);

    public void Reload() => WebView.Reload();

    public void GoForward() => WebView.GoForward();

    public void GoBack() => WebView.GoBack();

    public void NavigateToString(string htmlContent) => WebView.NavigateToString(htmlContent);

    public void Close() => WebView.Close();

    public void Initialize(WebView2 webView2) => WebView = webView2;
}

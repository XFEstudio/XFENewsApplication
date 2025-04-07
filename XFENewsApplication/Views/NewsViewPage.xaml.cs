using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Navigation;

namespace XFENewsApplication.Views;

/// <summary>
/// 新闻展示页面
/// </summary>
public sealed partial class NewsViewPage : Page
{
    public string? Parameter { get; set; }
    public static NewsViewPage? Current { get; set; }
    public NewsViewPageViewModel ViewModel { get; set; } = new();
    public NewsViewPage()
    {
        Current = this;
        this.InitializeComponent();
        ViewModel.NavigationParameterService.Initialize(this);
        ViewModel.NewsListViewService.Initialize(newsSourceListView);
        ViewModel.ContentListViewService.Initialize(contentListView);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.NavigationParameterService.OnParameterChange(e.Parameter);
    }
}

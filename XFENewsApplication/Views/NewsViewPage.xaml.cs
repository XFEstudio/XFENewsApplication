using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using XFENewsApplication.Models;

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
        NavigationCacheMode = NavigationCacheMode.Enabled;
        ViewModel.NavigationParameterService.Initialize(this);
        ViewModel.NewsListViewService.Initialize(newsSourceListView);
        ViewModel.ContentListViewService.Initialize(contentListView);
        ViewModel.DialogService.RegisterDialog(previewImageContentDialog);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.NavigationParameterService.OnParameterChange(e.Parameter);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        ViewModel.TokenSource?.Cancel();
        if (ViewModel.NavigationViewService is not null)
            ViewModel.NavigationViewService.ContentMargin = new(56, 24, 56, 0);
    }

    private void Border_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (sender is Border border)
            border.Opacity = 0.3;
    }

    private void Border_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (sender is Border border)
            border.Opacity = 1;
    }

    private void imageFlipView_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement image && image.DataContext is FlipImage imageSource)
        {
            ViewModel.PreviewImageUrl = imageSource.ImageUrl;
            ViewModel.DialogService.ShowDialog("previewImageContentDialog");
        }
    }
}

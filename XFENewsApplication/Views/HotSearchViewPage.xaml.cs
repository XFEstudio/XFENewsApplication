using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

namespace XFENewsApplication.Views;

/// <summary>
/// 热搜页面
/// </summary>
public sealed partial class HotSearchViewPage : Page
{
    public string? Parameter { get; set; }
    public static HotSearchViewPage? Current { get; set; }
    public HotSearchViewPageViewModel ViewModel { get; set; } = new();
    public HotSearchViewPage()
    {
        Current = this;
        this.InitializeComponent();
        ViewModel.NavigationParameterService.Initialize(this);
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
    }

    private void ListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {

    }
}

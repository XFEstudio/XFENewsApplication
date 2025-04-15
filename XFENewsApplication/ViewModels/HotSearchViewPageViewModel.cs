using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using XFEExtension.NetCore.WinUIHelper.Interface.Services;
using XFEExtension.NetCore.WinUIHelper.Utilities;
using XFENewsApplication.Core.Models;
using XFENewsApplication.Core.Utilities.Helpers;

namespace XFENewsApplication.ViewModels;

public partial class HotSearchViewPageViewModel : ViewModelBase
{
    [ObservableProperty]
    int selectedIndex = -1;
    private List<NewsSource> hotSearchSource = [];
    public ObservableCollection<NewsSource> HotSearchSource { get; } = [];
    public CancellationTokenSource? TokenSource { get; set; }
    public INavigationParameterService<string> NavigationParameterService { get; set; } = ServiceManager.GetService<INavigationParameterService<string>>();
    public IListViewService ListViewService { get; set; } = ServiceManager.GetService<IListViewService>();
    public IMessageService? MessageService { get; set; } = ServiceManager.GetGlobalService<IMessageService>();
    public INavigationViewService? NavigationViewService { get; set; } = ServiceManager.GetGlobalService<INavigationViewService>();

    public HotSearchViewPageViewModel()
    {
        NavigationParameterService.ParameterChange += NavigationParameterService_ParameterChange;
    }

    private void ListView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        switch (NavigationParameterService.Parameter)
        {
            case "Weibo":
                Helper.OpenInBrowser($"https://m.weibo.cn/search?containerid=100103type%3D1%26t%3D10%26q%3D%23{HotSearchSource[SelectedIndex].Title}%23&stream_entry_id=31&isnewpage=1&extparam=seat%3D1%26cate%3D5001%26lcate%3D5001%26stream_entry_id%3D31%26q%3D%2523%25E6%2582%25AC%25E8%25B5%258F%25E9%2580%259A%25E7%25BC%25893%25E5%2590%258D%25E7%25BE%258E%25E5%259B%25BD%25E7%2589%25B9%25E5%25B7%25A5%2523%26dgr%3D0%26band_rank%3D1%26realpos%3D1%26pos%3D0%26c_type%3D31%26flag%3D0%26filter_type%3Drealtimehot%26display_time%3D1744725202%26pre_seqid%3D17447252029660411768217&luicode=10000011&lfid=106003type%3D25%26t%3D3%26disable_hot%3D1%26filter_type%3Drealtimehot");
                break;
            case "Bilibili":
                Helper.OpenInBrowser($"https://search.bilibili.com/all?vt=24215469&keyword={HotSearchSource[SelectedIndex].Title}&from_source=webtop_search&spm_id_from=333.1007&search_source=4");
                break;
            default:
                break;
        }
    }

    private async void NavigationParameterService_ParameterChange(object? sender, string? e)
    {
        ListViewService.ListView.DoubleTapped += ListView_DoubleTapped;
        var tokenSource = new CancellationTokenSource();
        TokenSource = tokenSource;
        var result = await GetSource(e, TokenSource.Token);
        if (result.Success)
            LoadHotSearch(result.NewsSourceList);
        else
            MessageService?.ShowMessage(result.Message ?? "请检查网络连接", "无法获取", InfoBarSeverity.Warning);
    }

    private static async Task<NewsResult> GetSource(string? source, CancellationToken cancellationToken) => source switch
    {
        "Weibo" => await ClimbHelper.ClimbWeiboHotSearch(cancellationToken),
        "Bilibili" => await ClimbHelper.ClimbBilibiliHotSearch(cancellationToken),
        _ => new()
    };

    public void LoadHotSearch(IEnumerable<NewsSource> newsSources)
    {
        HotSearchSource.Clear();
        foreach (var source in newsSources)
            HotSearchSource.Add(source);
    }
}

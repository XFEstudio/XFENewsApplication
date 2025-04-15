using System.Collections.ObjectModel;
using XFEExtension.NetCore.WinUIHelper.Interface.Services;
using XFEExtension.NetCore.WinUIHelper.Utilities;
using XFENewsApplication.Core.Models;
using XFENewsApplication.Core.Utilities.Helpers;

namespace XFENewsApplication.ViewModels;

public partial class HotSearchViewPageViewModel : ViewModelBase
{
    private List<NewsSource> hotSearchSource = [];
    public ObservableCollection<NewsSource> HotSearchSource { get; } = [];
    public CancellationTokenSource? TokenSource { get; set; }
    public INavigationParameterService<string> NavigationParameterService { get; set; } = ServiceManager.GetService<INavigationParameterService<string>>();
    public IMessageService? MessageService { get; set; } = ServiceManager.GetGlobalService<IMessageService>();
    public INavigationViewService? NavigationViewService { get; set; } = ServiceManager.GetGlobalService<INavigationViewService>();

    public HotSearchViewPageViewModel() => NavigationParameterService.ParameterChange += NavigationParameterService_ParameterChange;

    private async void NavigationParameterService_ParameterChange(object? sender, string? e)
    {
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

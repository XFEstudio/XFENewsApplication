using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using XFEExtension.NetCore.WinUIHelper.Interface.Services;
using XFEExtension.NetCore.WinUIHelper.Utilities;
using XFENewsApplication.Core.Models;
using XFENewsApplication.Core.Utilities.Helpers;
using XFENewsApplication.Models;
using XFENewsApplication.Profiles.CrossVersionProfiles;
using XFENewsApplication.Utilities.Helper;

namespace XFENewsApplication.ViewModels;

public partial class NewsViewPageViewModel : ViewModelBase
{
    [ObservableProperty]
    int selectedIndex = -1;
    [ObservableProperty]
    double sideBarWidth = SystemProfile.SideBarWidth;
    private List<NewsSource> newsList = [];
    public INavigationParameterService<string> NavigationParameterService { get; set; } = ServiceManager.GetService<INavigationParameterService<string>>();
    public INavigationViewService? NavigationViewService { get; set; } = ServiceManager.GetGlobalService<INavigationViewService>();
    public IListViewService NewsListViewService { get; set; } = ServiceManager.GetService<IListViewService>();
    public IListViewService ContentListViewService { get; set; } = ServiceManager.GetService<IListViewService>();
    public IDialogService DialogService { get; set; } = ServiceManager.GetService<IDialogService>();
    public ObservableCollection<NewsSource> NewsList { get; } = [];
    public ObservableCollection<ArticlePart> ContentList { get; } = [];

    public NewsViewPageViewModel()
    {
        NavigationParameterService.ParameterChange += NavigationParameterService_ParameterChange;
        NewsListViewService.SelectionChanged += ListViewService_SelectionChanged;
    }

    private async void ListViewService_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ContentList.Clear();
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36 Edg/129.0.0.0");
        int reTryTimes = 0;
        Exception? exception = null;
        while (reTryTimes < 5)
        {
            if (NewsListViewService.ListView.SelectedItem is NewsSource newsSource)
            {
                try
                {
                    var result = await client.GetStringAsync($"https://assets.msn.cn/content/view/v2/Detail/zh-cn/{newsSource.ID}");
                    LoadContentList(ArticleDisplayHelper.ConvertToArticlePart(result));
                    return;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }
        }
        ServiceManager.GetGlobalService<IMessageService>()?.ShowMessage(exception?.Message?.ToString() ?? "未知原因", "无法获取新闻", InfoBarSeverity.Warning);
    }

    private async void NavigationParameterService_ParameterChange(object? sender, string? e)
    {
        switch (e)
        {
            case "MSN":
                await GetNews();
                SelectedIndex = 0;
                break;
            default:
                break;
        }
    }

    private void LoadNewsSource(IEnumerable<NewsSource> newsSources)
    {
        foreach (var news in newsSources)
            NewsList.Add(news);
    }

    private void LoadContentList(IEnumerable<ArticlePart> articleParts)
    {
        ContentList.Clear();
        foreach (var article in articleParts)
            ContentList.Add(article);
    }

    [RelayCommand]
    async Task GetNews()
    {
        await Task.Run(async () =>
        {
            newsList = await ClimbHelper.ClimbMSNNews();
        });
        LoadNewsSource(newsList);
    }

    [RelayCommand]
    void OpenInBrowser()
    {
        if (NewsListViewService.ListView.SelectedItem is NewsSource newsSource)
            Process.Start(new ProcessStartInfo(newsSource.Url) { UseShellExecute = true });
    }
}

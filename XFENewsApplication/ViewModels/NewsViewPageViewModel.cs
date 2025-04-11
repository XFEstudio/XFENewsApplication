using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.WinUIHelper.Interface.Services;
using XFEExtension.NetCore.WinUIHelper.Utilities;
using XFENewsApplication.Core.Models;
using XFENewsApplication.Core.Utilities.Helpers;
using XFENewsApplication.Models;
using XFENewsApplication.Profiles.CrossVersionProfiles;
using XFENewsApplication.Utilities;
using XFENewsApplication.Utilities.Helper;

namespace XFENewsApplication.ViewModels;

public partial class NewsViewPageViewModel : ViewModelBase
{
    [ObservableProperty]
    int selectedIndex = -1;
    [ObservableProperty]
    double sideBarWidth = SystemProfile.SideBarWidth;
    [ObservableProperty]
    bool isFavoriteArticle = false;
    [ObservableProperty]
    bool isSideBarLoadingVisible = true;
    [ObservableProperty]
    bool isContentLoadingVisible = true;
    [ObservableProperty]
    bool isSideBarMoreContentLoadingVisible = false;
    [ObservableProperty]
    string searchText = string.Empty;
    [ObservableProperty]
    string previewImageUrl = string.Empty;
    private List<NewsSource> newsList = [];
    private List<NewsSource> lastNewsList = [];
    private string? currentNewsId;
    private string? currentNewsContent;
    private string? lastNewsSource;
    private bool isLoadingNews = false;
    private int skip = 0;
    private int servedCardCount = 0;
    private int lastCardCount = 0;
    private int lastSelectedNews = 0;
    public INavigationParameterService<string> NavigationParameterService { get; set; } = ServiceManager.GetService<INavigationParameterService<string>>();
    public IListViewService NewsListViewService { get; set; } = ServiceManager.GetService<IListViewService>();
    public IListViewService ContentListViewService { get; set; } = ServiceManager.GetService<IListViewService>();
    public IDialogService DialogService { get; set; } = ServiceManager.GetService<IDialogService>();
    public INavigationViewService? NavigationViewService { get; set; } = ServiceManager.GetGlobalService<INavigationViewService>();
    public IMessageService? MessageService { get; set; } = ServiceManager.GetGlobalService<IMessageService>();
    public ObservableCollection<NewsSource> NewsList { get; } = [];
    public ObservableCollection<ArticlePart> ContentList { get; } = [];
    public HistoryManager<string, NewsSource> HistoryManager { get; set; } = new();
    public CancellationTokenSource? TokenSource { get; set; }

    public NewsViewPageViewModel()
    {
        NavigationParameterService.ParameterChange += NavigationParameterService_ParameterChange;
        NewsListViewService.SelectionChanged += ListViewService_SelectionChanged;
    }

    private async void NavigationParameterService_ParameterChange(object? sender, string? e)
    {
        NewsListViewService.ListView.Loaded += ListView_Loaded;
        HistoryManager = new(UserProfile.HistoryKeepCount);
        for (int i = UserProfile.HistoryArticleList.Count - 1; i >= 0; i--)
        {
            HistoryManager.Visit(UserProfile.HistoryArticleList[i].Url, UserProfile.HistoryArticleList[i]);
        }
        if (!NavigationParameterService.SameAsLast)
        {
            IsSideBarLoadingVisible = true;
            IsContentLoadingVisible = true;
            NewsList.Clear();
            ContentList.Clear();
            SearchText = string.Empty;
            currentNewsId = string.Empty;
            newsList.Clear();
        }
        switch (e)
        {
            case "MSN":
                if (NavigationViewService is not null)
                {
                    NavigationViewService.ContentMargin = new(0, 24, 0, 0);
                    var stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 24
                    };
                    stackPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new("https://www.msn.com/favicon.ico"))
                    });
                    stackPanel.Children.Add(new TextBlock
                    {
                        Text = "MSN新闻",
                        FontSize = 28,
                        FontWeight = FontWeights.SemiBold
                    });
                    NavigationViewService.Header = stackPanel;
                }
                if (lastNewsList.Count > 0 && lastNewsSource == "MSN")
                {
                    newsList = lastNewsList;
                    LoadNewsSource(newsList);
                    NewsListViewService.ListView.ScrollIntoView(NewsList[lastSelectedNews], ScrollIntoViewAlignment.Leading);
                }
                else
                {
                    await GetNews();
                }
                SelectedIndex = lastSelectedNews;
                IsSideBarMoreContentLoadingVisible = true;
                lastNewsSource = "MSN";
                break;
            case "Favorite":
                IsSideBarMoreContentLoadingVisible = false;
                if (NavigationViewService is not null)
                    NavigationViewService.ContentMargin = new(0, 24, 0, 0);
                newsList = [.. UserProfile.FavoriteArticleList.Select(favorite => favorite.NewsSource)];
                LoadNewsSource(newsList);
                if (UserProfile.FavoriteArticleList.Count > 0)
                    SelectedIndex = 0;
                IsContentLoadingVisible = false;
                break;
            case "History":
                IsSideBarMoreContentLoadingVisible = false;
                if (NavigationViewService is not null)
                    NavigationViewService.ContentMargin = new(0, 24, 0, 0);
                newsList = [.. UserProfile.HistoryArticleList];
                LoadNewsSource(newsList);
                if (UserProfile.FavoriteArticleList.Count > 0)
                    SelectedIndex = 0;
                IsContentLoadingVisible = false;
                break;
            default:
                break;
        }
        IsSideBarLoadingVisible = false;
    }

    private void ListView_Loaded(object sender, RoutedEventArgs e)
    {
        NewsListViewService.ListViewScrollViewer!.ViewChanged += ListViewScrollViewer_ViewChanged;
    }

    private async void ListViewScrollViewer_ViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            switch (NavigationParameterService.Parameter)
            {
                case "MSN":
                    if ((!isLoadingNews || TokenSource is null || TokenSource.IsCancellationRequested) && SearchText.IsNullOrEmpty())
                        if (scrollViewer.ScrollableHeight - scrollViewer.VerticalOffset < 300)
                            await GetNews();
                    break;
                default:
                    break;
            }
        }
    }

    private async void ListViewService_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        IsFavoriteArticle = NewsListViewService.ListView.SelectedItem is NewsSource source && UserProfile.FavoriteArticleList.Any(favorite => favorite.NewsSource.Url == source.Url);
        switch (NavigationParameterService.Parameter)
        {
            case "Favorite":
                FavoriteChanged();
                break;
            default:
                await NewsChanged();
                break;
        }
    }

    partial void OnSideBarWidthChanged(double value) => SystemProfile.SideBarWidth = value;

    partial void OnSearchTextChanged(string value) => SearchNews(value);

    private void SearchNews(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            IsSideBarMoreContentLoadingVisible = true;
            NewsList.Clear();
            LoadNewsSource(newsList);
        }
        else
        {
            IsSideBarMoreContentLoadingVisible = false;
            NewsList.Clear();
            LoadNewsSource(newsList.Where(news => news.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) || news.Abstract is not null && news.Abstract.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
        }
    }

    private void FavoriteChanged()
    {
        if (NewsListViewService.ListView.SelectedItem is NewsSource newsSource && UserProfile.FavoriteArticleList.FirstOrDefault(favorite => favorite.NewsSource.Url == newsSource.Url) is Article article && SelectedIndex != -1)
        {
            ContentList.Clear();
            currentNewsId = newsSource.ID;
            LoadContentList(ArticleDisplayHelper.ConvertToArticlePart(article.ArticleContent));
        }
    }

    private async Task NewsChanged()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36 Edg/129.0.0.0");
        int reTryTimes = 0;
        Exception? exception = null;
        if (NewsListViewService.ListView.SelectedItem is NewsSource newsSource && SelectedIndex != -1)
        {
            IsContentLoadingVisible = true;
            ContentList.Clear();
            currentNewsId = newsSource.ID;
            while (reTryTimes < 5)
            {
                try
                {
                    switch (newsSource.Source)
                    {
                        case "MSN":
                            if (NavigationParameterService.Parameter == "MSN")
                                lastSelectedNews = SelectedIndex;
                            var result = await client.GetStringAsync($"https://assets.msn.cn/content/view/v2/Detail/zh-cn/{newsSource.ID}");
                            currentNewsContent = result;
                            if (!CheckCurrentNewsSource(newsSource.ID))
                                return;
                            var articleParts = ArticleDisplayHelper.ConvertToArticlePart(result);
                            if (!CheckCurrentNewsSource(newsSource.ID))
                                return;
                            LoadContentList(articleParts);
                            HistoryManager.Visit(newsSource.Url, newsSource);
                            UserProfile.HistoryArticleList = [.. HistoryManager.Export()];
                            IsContentLoadingVisible = false;
                            return;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }
            ServiceManager.GetGlobalService<IMessageService>()?.ShowMessage(exception?.Message?.ToString() ?? "未知原因", "无法获取新闻", InfoBarSeverity.Warning);
        }
    }

    private bool CheckCurrentNewsSource(string? id) => currentNewsId is not null && id is not null && currentNewsId == id;

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
        isLoadingNews = true;
        int totalCount = 0;
        var tokenSource = new CancellationTokenSource();
        TokenSource = tokenSource;
        while (!tokenSource.IsCancellationRequested)
        {
            var result = await ClimbHelper.ClimbMSNNews(tokenSource.Token, skip, lastCardCount, servedCardCount);
            var finalResult = result.NewsSourceList.Where(newsSource => !newsList.Any(news => news.Url == newsSource.Url)).ToList();
            if (result.Success)
            {
                if (finalResult.Count > 0)
                {
                    totalCount += finalResult.Count;
                    newsList.AddRange(finalResult);
                    lastNewsList.AddRange(finalResult);
                    skip++;
                    servedCardCount = result.Count;
                    lastCardCount += result.Count;
                    if (!tokenSource.IsCancellationRequested)
                        LoadNewsSource(finalResult);
                    if (totalCount >= 10)
                        break;
                }
            }
            else
            {
                MessageService?.ShowMessage(result.Message ?? "未知原因", "无法获取新闻", InfoBarSeverity.Warning);
            }
        }
        isLoadingNews = false;
    }

    [RelayCommand]
    void OpenInBrowser()
    {
        if (NewsListViewService.ListView.SelectedItem is NewsSource newsSource)
            Process.Start(new ProcessStartInfo(newsSource.Url) { UseShellExecute = true });
    }

    [RelayCommand]
    void Share()
    {
        var dataTransferManager = DataTransferManagerHelper.GetForWindow();
        dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        DataTransferManagerHelper.ShowShareUIForWindow(WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow));
    }

    private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        if (args.Request is DataRequest dataRequest)
        {
            if (NewsListViewService.ListView.SelectedItem is NewsSource newsSource)
            {
                dataRequest.Data.Properties.Title = newsSource.Title;
                dataRequest.Data.Properties.Description = newsSource.Abstract;
                dataRequest.Data.SetWebLink(new Uri(newsSource.Url));
            }
            else
            {
                dataRequest.FailWithDisplayText("没有选中任何新闻");
            }
        }
    }

    [RelayCommand]
    void Favorite()
    {
        if (IsFavoriteArticle)
        {
            if (NewsListViewService.ListView.SelectedItem is NewsSource newsSource)
            {
                UserProfile.FavoriteArticleList.AddFirst(new Article()
                {
                    NewsSource = newsSource,
                    ArticleContent = currentNewsContent ?? string.Empty
                });
                UserProfile.SaveProfile();
                MessageService?.ShowMessage("已添加到收藏夹", "收藏成功", InfoBarSeverity.Success);
            }
        }
        else
        {
            if (NewsListViewService.ListView.SelectedItem is NewsSource newsSource)
            {
                UserProfile.FavoriteArticleList.Remove(UserProfile.FavoriteArticleList.First(favorite => favorite.NewsSource.Url == newsSource.Url));
                UserProfile.SaveProfile();
                MessageService?.ShowMessage("已从收藏夹移除", "取消收藏");
            }
        }
    }
}

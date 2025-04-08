using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.WinUIHelper.Utilities;
using XFENewsApplication.Core.Models;
using XFENewsApplication.Models;

namespace XFENewsApplication.Profiles.CrossVersionProfiles;

public partial class UserProfile : XFEProfile
{
    public UserProfile() => ProfilePath = $@"{AppPathHelper.LocalProfile}\{nameof(UserProfile)}";
    /// <summary>
    /// 历史记录储存数量
    /// </summary>
    [ProfileProperty]
    private int historyKeepCount = 100;
    /// <summary>
    /// MSN新闻的当前用户ID
    /// </summary>
    [ProfileProperty]
    private string msnUserId = string.Empty;
    /// <summary>
    /// 历史记录存储模式
    /// </summary>
    [ProfileProperty]
    private HistorySaveMode historySaveMode = HistorySaveMode.OneHundred;
    /// <summary>
    /// 收藏的文章
    /// </summary>
    [ProfileProperty]
    private LinkedList<Article> favoriteArticleList = [];
    /// <summary>
    /// 历史记录
    /// </summary>
    [ProfileProperty]
    [ProfilePropertyAddGet("Current.historyArticleList.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current.historyArticleList")]
    private ProfileList<NewsSource> historyArticleList = [];

    static partial void SetHistorySaveModeProperty(ref HistorySaveMode value)
    {
        switch (value)
        {
            case HistorySaveMode.OneHundred:
                HistoryKeepCount = 100;
                break;
            case HistorySaveMode.OneThousand:
                HistoryKeepCount = 1000;
                break;
            case HistorySaveMode.Unlimited:
                HistoryKeepCount = int.MaxValue;
                break;
            default:
                break;
        }
    }
}

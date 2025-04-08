using XFENewsApplication.Core.Models;

namespace XFENewsApplication.Models;

public class Article
{
    public required NewsSource NewsSource { get; set; }
    public string ArticleContent { get; set; } = string.Empty;
}
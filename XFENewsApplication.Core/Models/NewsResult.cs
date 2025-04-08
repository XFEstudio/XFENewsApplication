namespace XFENewsApplication.Core.Models;

public class NewsResult
{
    public List<NewsSource> NewsSourceList { get; set; } = [];
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int Count { get; set; }
}

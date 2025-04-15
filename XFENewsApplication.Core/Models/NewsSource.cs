namespace XFENewsApplication.Core.Models;

public class NewsSource
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? ID { get; set; }
    public string? Authors { get; set; }
    public string? PublishTime { get; set; }
    public string? Abstract { get; set; }
    public string? ImageUrl { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    public int Index { get; set; }
    public string Source { get; set; } = string.Empty;
}

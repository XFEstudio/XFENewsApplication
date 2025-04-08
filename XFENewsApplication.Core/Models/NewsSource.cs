namespace XFENewsApplication.Core.Models;

public class NewsSource
{
    public required string Title { get; set; }
    public required string Url { get; set; }
    public string? ID { get; set; }
    public string? Authors { get; set; }
    public string? PublishTime { get; set; }
    public string? Abstract { get; set; }
    public string? ImageUrl { get; set; }
    public required string Source { get; set; }
}

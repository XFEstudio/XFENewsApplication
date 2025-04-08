namespace XFENewsApplication.Models;

public class AuthorArticlePart : ArticlePart
{
    public string? AuthorName { get; set; }
    public string? ProviderName { get; set; }
    public string? PublishDateTime { get; set; }
    public string? ReadTime { get; set; }
}
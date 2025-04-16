using HtmlAgilityPack;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Documents;
using System.Text.Json;
using XFEExtension.NetCore.StringExtension;
using XFENewsApplication.Models;

namespace XFENewsApplication.Utilities.Helper;

public static class ArticleDisplayHelper
{
    public static List<ArticlePart> ConvertToArticlePart(string newsDocument)
    {
        var articleParts = new List<ArticlePart>();
        var imageDictionary = new Dictionary<string, string>();
        var document = JsonDocument.Parse(newsDocument);
        var title = document.RootElement.GetProperty("title").ToString();
        articleParts.Add(new TextArticlePart
        {
            FontSize = 40,
            FontWeight = FontWeights.Bold,
            Content = title
        });
        if (document.RootElement.TryGetProperty("provider", out var provider))
        {
            var providerName = provider.GetProperty("name").ToString();
            articleParts.Add(new AuthorArticlePart
            {
                Content = provider.GetProperty("logo").GetProperty("url").ToString(),
                PublishDateTime = GetRelativeTimeDescription(document.RootElement.GetProperty("publishedDateTime").ToString()),
                ReadTime = document.RootElement.TryGetProperty("readTimeMin", out var readTime) && readTime.GetInt32() > 0 ? readTime.ToString() : string.Empty,
                ProviderName = providerName,
                AuthorName = document.RootElement.TryGetProperty("authors", out var authors) && authors.EnumerateArray().Any() ? string.Join(", ", authors.EnumerateArray().Select(x => x.GetProperty("name").ToString())) : providerName
            });
        }
        if (document.RootElement.TryGetProperty("slides", out var slides) && slides.EnumerateArray().Any())
        {
            var flipImages = new List<FlipImage>();
            foreach (var slide in slides.EnumerateArray())
            {
                var imageProperty = slide.GetProperty("image");
                flipImages.Add(new()
                {
                    ImageUrl = imageProperty.GetProperty("url").ToString(),
                    Description = imageProperty.TryGetProperty("caption", out var description) ? description.ToString() : string.Empty,
                    Attribution = imageProperty.TryGetProperty("attribution", out var attribution) ? attribution.ToString() : string.Empty,
                    Title = slide.TryGetProperty("title", out var titleProperty) ? titleProperty.ToString() : string.Empty
                });
            }
            articleParts.Add(new FlipImageArticlePart
            {
                Images = flipImages
            });
        }
        foreach (var imageNode in document.RootElement.GetProperty("imageResources").EnumerateArray())
            imageDictionary.Add(imageNode.GetProperty("cmsId").ToString(), imageNode.GetProperty("url").ToString());
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(document.RootElement.GetProperty("body").ToString());
        AnalysisMSNDocument(articleParts, title, imageDictionary, htmlDocument.DocumentNode.ChildNodes, htmlDocument.DocumentNode);
        return articleParts;
    }

    private static CustomTextArticlePart CheckLastArticlePart(List<ArticlePart> articleParts, HtmlNode parentNode, Inline inline)
    {
        if (articleParts.LastOrDefault() is not CustomTextArticlePart customTextArticlePart)
        {
            customTextArticlePart = new CustomTextArticlePart
            {
                Content = parentNode.GetHashCode().ToString()
            };
            articleParts.Add(customTextArticlePart);
        }
        if (customTextArticlePart.Content != parentNode.GetHashCode().ToString())
        {
            customTextArticlePart.Content = parentNode.GetHashCode().ToString();
            customTextArticlePart.CustomTextBlock.Inlines.Add(new LineBreak());
            customTextArticlePart.CustomTextBlock.Inlines.Add(new LineBreak());
        }
        customTextArticlePart.CustomTextBlock.Inlines.Add(inline);
        return customTextArticlePart;
    }

    private static void AnalysisMSNDocument(List<ArticlePart> articleParts, string title, Dictionary<string, string> imageDictionary, HtmlNodeCollection htmlNodes, HtmlNode parentNode)
    {
        foreach (var node in htmlNodes)
        {
            switch (node.Name)
            {
                case "img":
                    articleParts.Add(new ImageArticlePart
                    {
                        Content = imageDictionary[node.GetAttributeValue("data-document-id", string.Empty).Replace("\"", string.Empty)],
                        From = title
                    });
                    break;
                //case "video":
                //    articleParts.Add(new )
                case "#text":
                    switch (parentNode.Name)
                    {
                        case "p":
                            CheckLastArticlePart(articleParts, parentNode, new Run
                            {
                                Text = node.InnerText
                            });
                            break;
                        case "span":
                            CheckLastArticlePart(articleParts, parentNode.ParentNode, new Run
                            {
                                Text = node.InnerText
                            });
                            break;
                        case "strong":
                            var bold = new Bold();
                            bold.Inlines.Add(new Run
                            {
                                Text = node.InnerText
                            });
                            CheckLastArticlePart(articleParts, parentNode.ParentNode, bold);
                            break;
                        case "blockquote":
                            if (node.InnerText.IsNullOrWhiteSpace())
                                continue;
                            articleParts.Add(new QuoteArticlePart
                            {
                                Content = node.InnerText,
                                FontSize = 36,
                                FontWeight = FontWeights.SemiBold
                            });
                            break;
                        case "h1":
                            articleParts.Add(new TitleArticlePart
                            {
                                Content = node.InnerText,
                                FontSize = 68,
                                FontWeight = FontWeights.SemiBold
                            });
                            break;
                        case "h2":
                            articleParts.Add(new TitleArticlePart
                            {
                                Content = node.InnerText,
                                FontSize = 48,
                                FontWeight = FontWeights.SemiBold
                            });
                            break;
                        case "h3":
                            articleParts.Add(new TitleArticlePart
                            {
                                Content = node.InnerText,
                                FontSize = 36,
                                FontWeight = FontWeights.SemiBold
                            });
                            break;
                        case "h4":
                            articleParts.Add(new TitleArticlePart
                            {
                                Content = node.InnerText,
                                FontSize = 28,
                                FontWeight = FontWeights.SemiBold
                            });
                            break;
                        case "h5":
                            articleParts.Add(new TitleArticlePart
                            {
                                Content = node.InnerText,
                                FontSize = 20,
                                FontWeight = FontWeights.SemiBold
                            });
                            break;
                        case "h6":
                            articleParts.Add(new TitleArticlePart
                            {
                                Content = node.InnerText,
                                FontSize = 16,
                                FontWeight = FontWeights.SemiBold
                            });
                            break;
                        default:
                            if (node.InnerText.IsNullOrWhiteSpace())
                                continue;
                            articleParts.Add(new TextArticlePart
                            {
                                Content = node.InnerText
                            });
                            break;
                    }
                    break;
                default:
                    if (node.HasChildNodes)
                        AnalysisMSNDocument(articleParts, title, imageDictionary, node.ChildNodes, node);
                    break;
            }
        }
    }

    public static string GetRelativeTimeDescription(DateTime time)
    {
        var now = DateTime.Now;
        var span = now - time;

        if (span.TotalSeconds < 60)
        {
            return "现在";
        }
        else if (span.TotalMinutes < 60)
        {
            int minutes = (int)span.TotalMinutes;
            return $"{minutes} 分钟前";
        }
        else if (span.TotalHours < 24)
        {
            int hours = (int)span.TotalHours;
            return $"{hours} 小时前";
        }
        else if (span.TotalDays < 7)
        {
            int days = (int)span.TotalDays;
            return $"{days} 天前";
        }
        else
        {
            return time.ToString("yyyy年MM月dd日 HH:mm");
        }
    }

    public static string GetRelativeTimeDescription(string time)
    {
        if (DateTime.TryParse(time, out var dateTime))
        {
            return GetRelativeTimeDescription(dateTime);
        }
        return "暂无时间";
    }
}

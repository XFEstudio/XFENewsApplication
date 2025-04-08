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
        articleParts.Add(new TextArticlePart
        {
            FontSize = 40,
            FontWeight = FontWeights.Bold,
            Content = document.RootElement.GetProperty("title").ToString()
        });
        foreach (var imageNode in document.RootElement.GetProperty("imageResources").EnumerateArray())
            imageDictionary.Add(imageNode.GetProperty("cmsId").ToString(), imageNode.GetProperty("url").ToString());
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(document.RootElement.GetProperty("body").ToString());
        AnalysisMSNDocument(articleParts, imageDictionary, htmlDocument.DocumentNode.ChildNodes, htmlDocument.DocumentNode);
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

    private static void AnalysisMSNDocument(List<ArticlePart> articleParts, Dictionary<string, string> imageDictionary, HtmlNodeCollection htmlNodes, HtmlNode parentNode)
    {
        foreach (var node in htmlNodes)
        {
            switch (node.Name)
            {
                case "img":
                    articleParts.Add(new ImageArticlePart
                    {
                        Content = imageDictionary[node.GetAttributeValue("data-document-id", string.Empty).Replace("\"", string.Empty)]
                    });
                    break;
                case "#text":
                    switch (parentNode.Name)
                    {
                        case "p":
                            var customTextArticlePart = CheckLastArticlePart(articleParts, parentNode, new Run
                            {
                                Text = node.InnerText
                            });
                            break;
                        case "strong":
                            if (parentNode.ParentNode.Name == "p")
                            {
                                var bold = new Bold();
                                bold.Inlines.Add(new Run
                                {
                                    Text = node.InnerText
                                });
                                CheckLastArticlePart(articleParts, parentNode.ParentNode, bold);
                            }
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
                        AnalysisMSNDocument(articleParts, imageDictionary, node.ChildNodes, node);
                    break;
            }
        }
    }
}

using HtmlAgilityPack;
using Microsoft.UI.Text;
using System.Text.Json;
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
        foreach (var node in htmlDocument.DocumentNode.Descendants())
        {
            if (node.Name == "img")
                articleParts.Add(new ImageArticlePart
                {
                    Content = imageDictionary[node.GetAttributeValue("data-document-id", string.Empty).Replace("\"", string.Empty)]
                });
            if (node.HasChildNodes)
                continue;
            articleParts.Add(new TextArticlePart
            {
                Content = node.InnerText
            });
        }
        return articleParts;
    }
}

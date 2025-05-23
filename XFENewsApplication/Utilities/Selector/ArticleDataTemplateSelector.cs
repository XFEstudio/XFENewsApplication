﻿
using XFENewsApplication.Models;

namespace XFENewsApplication.Utilities.Selector;

public partial class ArticleDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? DefaultDataTemplate { get; set; }
    public DataTemplate? TextDataTemplate { get; set; }
    public DataTemplate? AuthorDataTemplate { get; set; }
    public DataTemplate? TitleDataTemplate { get; set; }
    public DataTemplate? QuoteDataTemplate { get; set; }
    public DataTemplate? CustomTextDataTemplate { get; set; }
    public DataTemplate? ImageDataTemplate { get; set; }
    public DataTemplate? FlipImageDataTemplate { get; set; }
    protected override DataTemplate? SelectTemplateCore(object item)
    {
        if (item is ArticlePart articlePart)
        {
            if (articlePart is TextArticlePart)
            {
                if (articlePart is TitleArticlePart)
                    return TitleDataTemplate;
                if (articlePart is QuoteArticlePart)
                    return QuoteDataTemplate;
                if (articlePart is CustomTextArticlePart)
                    return CustomTextDataTemplate;
                return TextDataTemplate;
            }
            if (articlePart is ImageArticlePart)
                return ImageDataTemplate;
            if (articlePart is AuthorArticlePart)
                return AuthorDataTemplate;
            if (articlePart is FlipImageArticlePart)
                return FlipImageDataTemplate;
            return TextDataTemplate;
        }
        return DefaultDataTemplate;
    }
}

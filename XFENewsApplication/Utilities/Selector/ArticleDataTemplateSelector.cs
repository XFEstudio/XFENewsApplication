
using XFENewsApplication.Models;

namespace XFENewsApplication.Utilities.Selector;

public partial class ArticleDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? DefaultDataTemplate { get; set; }
    public DataTemplate? TextDataTemplate { get; set; }
    public DataTemplate? ImageDataTemplate { get; set; }
    protected override DataTemplate? SelectTemplateCore(object item)
    {
        if (item is TextArticlePart)
            return TextDataTemplate;
        if (item is ImageArticlePart)
            return ImageDataTemplate;
        return DefaultDataTemplate;
    }
}

namespace XFENewsApplication.Models;

public class CustomTextArticlePart : TextArticlePart
{
    public TextBlock CustomTextBlock { get; set; } = new()
    {
        Margin = new(0, 10, 0, 10),
        FontSize = 18,
        TextWrapping = TextWrapping.Wrap,
        IsTextSelectionEnabled = true
    };
}

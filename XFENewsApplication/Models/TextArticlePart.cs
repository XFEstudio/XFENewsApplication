using Microsoft.UI.Text;
using Windows.UI.Text;

namespace XFENewsApplication.Models;

public class TextArticlePart : ArticlePart
{
    public double FontSize { get; set; } = 14;
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;
}
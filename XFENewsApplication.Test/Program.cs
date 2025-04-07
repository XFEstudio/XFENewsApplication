using HtmlAgilityPack;

class Program
{
    [SMTest("""
        <p>1、《夏日之恋》</p><p>小说改编，小镇压抑女孩和富家女，拍的又清新又燥热，结尾sadie和河边很厉害，人家只是假日消遣过家家，你却死去活来以为真，古早的艾米莉布朗特。清澈，简洁，真实，晃动的镜头。也许因为是英音—— 中文的字幕错误多的一塌糊涂，全不得要领，一定要下载英文的字幕，不然会直接给电影拉低两颗星。 结尾的反转残酷也现实。</p><div><img data-reference=\"image\" data-document-id=\"cms/api/amp/image/AA1AXNbb\"></div><p>2、《火车上的女孩》</p><p>两位金发的女演员令人脸盲症发作，有点分不清楚，导演故作玄虚的剪辑更让人观影难度增加。结尾草率匆忙。不知道是否忠于原著，应该多点铺垫和人物行为逻辑。三个女演员非常捧。缺点是既忠于原著又不得不修改，因为原著的文学性加分，在电影上反而容易减分，不能直接用文学悬疑手法，电影应该增加悬疑度。线索也有点乱。总之看在主角表演不错。</p><div><img data-reference=\"image\" data-document-id=\"cms/api/amp/image/AA1AXPFO\"></div><p>3、《寂静之地》</p><p>很不错的一部影片，全片没几句台词，在寂静中烘托出了恐怖的氛围，全程紧张氛围满满，在恐怖的氛围下影片塑造的一家人同甘共苦的亲情也是让人感觉温暖，丈夫和妻子带着耳机翩翩起舞，父亲为女儿制作声音放大器耳麦等等，最后为了拯救孩子，父亲打着手语表达爱意都让人泪奔。</p><div><img data-reference=\"image\" data-document-id=\"cms/api/amp/image/AA1AXPFY\"></div><p>4、《明日边缘》</p><p>很有游戏感，循环死亡刷经验，打掉大boss获得自主读档的能力。“人”味的一面是主角的成长和爱情，但说真的，这种题材里的爱情线实在是千篇一律。动作戏好看，虽然是很老套的剧情，但是爆米花你能要求什么，不过汤姆 克鲁斯确实，这种硬汉很帅，结局的温情戏感觉没烘托出来，不过也算是好看的。</p><div><img data-reference=\"image\" data-document-id=\"cms/api/amp/image/AA1AXPG3\"></div><p>5、《边境杀手》</p><p>节奏把控很好，一直让人摸不透又悬着心。镜头很讲究，最后一场“地道战”在美丽的夕阳下，黑色的人影慢慢没入漆黑的大地，很棒！整体给人一种总有你想不到的黑暗隐藏在日常生活的表象下的感觉，就好像片头的尸体都藏在墙里。一定的留白给了人感慨和喘息的空间。</p><div><img data-reference=\"image\" data-document-id=\"cms/api/amp/image/AA1AXS3L\"></div><p>艾米莉·布朗特主演的5部电影，每部都值得细细品味！</p>
        """)]
    public static void TestMethod(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        foreach (var node in doc.DocumentNode.Descendants())
        {
            if (node.Name == "img")
                Console.WriteLine($"图片：{node.GetAttributeValue("data-document-id", string.Empty)}");
            if (node.HasChildNodes)
                continue;
            Console.WriteLine(node.InnerText);
        }
    }
}
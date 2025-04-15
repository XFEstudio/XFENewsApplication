using System.Text.RegularExpressions;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.XFETransform.JsonConverter;

class Program
{
    //[SMTest]
    public static async Task TestWeiboHotSearch()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36 Edg/135.0.0.0");
        QueryableJsonNode jsonNode = await client.GetStringAsync("https://m.weibo.cn/api/container/getIndex?containerid=106003type%3D25%26t%3D3%26disable_hot%3D1%26filter_type%3Drealtimehot&luicode=20000061&lfid=5070140584495876");
        foreach (var node in jsonNode["data"]["cards"].GetChildNodes().First()["card_group"]["package:list", "desc", "desc_extr", "icon"].PackageInListObject())
        {
            if (node.TryGetValue("desc_extr", out var desc_extr) && !desc_extr.ToString().IsNullOrWhiteSpace())
                Console.WriteLine($"{Regex.Unescape(node["desc"].ToString())}\t  {Regex.Unescape(desc_extr.ToString())}");
        }
    }

    [SMTest]
    public static async Task TestBilibiliHotSearch()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36 Edg/135.0.0.0");
        QueryableJsonNode jsonNode = await client.GetStringAsync("https://api.bilibili.com/x/web-interface/wbi/search/square?limit=50&platform=web&web_location=333.1007&w_rid=2247c88d4bf6d1fbd138018aa2c530d5&wts=1744698017");
        foreach (var node in jsonNode["data"]["trending"]["list"]["package:list", "keyword", "heat_score", "icon"].PackageInListObject())
        {
            Console.WriteLine($"{node["keyword"]}\t {node["heat_score"]}");
        }
    }
}
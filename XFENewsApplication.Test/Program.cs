using System.Text.RegularExpressions;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.XFETransform.JsonConverter;

class Program
{
    //[SMTest(1)]
    public static async Task TestCCTVClimb(int index)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36 Edg/135.0.0.0");
        QueryableJsonNode jsonNode = await client.GetStringAsync($"https://news.cctv.com/2019/07/gaiban/cmsdatainterface/page/news_{index}.jsonp?cb=news");
        foreach (var node in jsonNode["data"]["list"]["package:list", "brief", "title", "url"].PackageInListObject())
        {
            Console.WriteLine($"标题：{node["title"]}({node["url"]})\n{node["brief"]}\n");
        }
    }

    //[SMTest]
    public static async Task TestM3u8()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36 Edg/135.0.0.0");
        Console.WriteLine(await client.GetStringAsync($"https://vdn.apps.cntv.cn/api/getHttpVideoInfo.do?pid=87b7efc3c7d84b45a288336e510e880e&client=flash&im=0&tsp=1744734329&vn=2049&vc=&uid=&wlan="));
    }

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
        QueryableJsonNode jsonNode = await client.GetStringAsync($"https://api.bilibili.com/x/web-interface/wbi/search/square?limit=50&platform=web&web_location=333.1007&w_rid=&wts={DateTimeOffset.Now.ToUnixTimeSeconds()}");
        foreach (var node in jsonNode["data"]["trending"]["list"]["package:list", "keyword", "heat_score", "icon"].PackageInListObject())
        {
            Console.WriteLine($"{node["keyword"]}\t {node["heat_score"]}");
        }
    }
}
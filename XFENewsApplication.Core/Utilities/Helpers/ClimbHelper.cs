using System.Text.Json;
using System.Text.RegularExpressions;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.XFETransform.JsonConverter;
using XFENewsApplication.Core.Models;

namespace XFENewsApplication.Core.Utilities.Helpers;

public static class ClimbHelper
{
    public static string? MSNAPI { get; set; }
    public static string? ActiveId { get; set; }
    public static string? UserId { get; set; }

    public static async Task<string> GetMSNAPI(CancellationToken cancellationToken)
    {
        string result = string.Empty;
        int retryTimes = 0;
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36 Edg/129.0.0.0");
        await Task.Run(async () =>
        {
            while (retryTimes < 5)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    QueryableJsonNode authJsonNode = await client.GetStringAsync("https://www.msn.cn/resolver/api/resolve/v3/config/?expType=AppConfig&expInstance=default&apptype=homePage&v=20250401.183&targetScope={%22audienceMode%22:%22adult%22,%22browser%22:{%22browserType%22:%22edgeChromium%22,%22version%22:%22135%22,%22ismobile%22:%22false%22},%22deviceFormFactor%22:%22desktop%22,%22domain%22:%22www.msn.cn%22,%22locale%22:{%22content%22:{%22language%22:%22zh%22,%22market%22:%22cn%22},%22display%22:{%22language%22:%22zh%22,%22market%22:%22cn%22}},%22os%22:%22windows%22,%22platform%22:%22web%22,%22pageType%22:%22hp%22,%22pageExperiments%22:[]}", cancellationToken);
                    result = authJsonNode["configs"]["StripeWC/default"]["properties"]["apikey"];
                    break;
                }
                catch
                {
                    retryTimes++;
                    try
                    {
                        await Task.Delay(500, cancellationToken);
                    }
                    catch { }
                }
            }
        }, cancellationToken);
        if (retryTimes >= 5)
            throw new Exception("无法获取新闻，请检查网络连接");
        return result;
    }

    public static async Task<NewsResult> ClimbMSNNews(CancellationToken cancellationToken, int skip = 0, int lastCardCount = 0, int servedCardCount = 0)
    {
        if (MSNAPI.IsNullOrWhiteSpace())
            MSNAPI = await GetMSNAPI(cancellationToken);
        if (lastCardCount == 0 || skip == 0)
            return await ClimbMSNNews($"https://assets.msn.com/service/news/feed/pages/weblayout?User=&activityId=&adoffsets=c1:-1,c2:-1&apikey={MSNAPI}&audienceMode=adult&cm=zh-cn&colstatus=c1:0,c2:0&column=c2&colwidth=300&contentType=article&it=edgeid&l3v=2&layout=c2&memory=8&newsSkip=0&newsTop=48&ocid=hponeservicefeed&pgc=11&private=1&scn=APP_ANON&timeOut=1000&vpSize=721x674&wposchema=byregion", cancellationToken);
        else
            return await ClimbMSNNews($"https://assets.msn.com/service/news/feed/pages/weblayout?$skip={skip}&User=&activityId=&adoffsets=c1:304,c2:912,c3:-1&adsTimeout=600&alladoffsets=c1:1824|c2:1520,608|c3:304,912,3041|c4:2129,304,608,2129|c5:1825,304,1216,608,1825|&allcoloffsets=c1:0|c2:0,0|c3:0,0,0|c4:0,0,0,0|c5:0,0,0,0,0|&alltwocoffsets=c1:7601|c2:304,304|c3:1824,304,304|c4:1216,1216,1520,1520|c5:0,0,304,304,1825|&apikey={MSNAPI}&audienceMode=adult&cardsServed={servedCardCount}&cm=zh-cn&colstatus=c1:0,c2:0,c3:0&column=c3&colwidth=300&contentType=article&cookieWallPresent=false&l3v=2&lastcardrank={lastCardCount}&layout=c3&memory=8&mobile=false&ocid=hponeservicefeed&private=1&renderedSegments=35734127939585&timeOut=2000&vpSize=1097x1317&wpoCmsAdServed=0&wposchema=byregion", cancellationToken);
    }

    public static async Task<NewsResult> ClimbMSNNews(string requestUrl, CancellationToken cancellationToken)
    {
        var resultList = new List<NewsSource>();
        int retryTimes = 0;
        int totalCount = 0;
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36 Edg/129.0.0.0");
        await Task.Run(async () =>
        {
            while (retryTimes < 5)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    var result = await client.GetStringAsync(requestUrl, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    using var document = JsonDocument.Parse(result);
                    int i = 0;
                    foreach (var subNode in document.RootElement.GetProperty("sections").EnumerateArray().ElementAt(0).GetProperty("cards").EnumerateArray())
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;
                        totalCount++;
                        if (i++ == 0)
                            continue;
                        try
                        {
                            var id = subNode.GetProperty("id").GetString();
                            if (id is null || id.Length > 8)
                                continue;
                            resultList.Add(new NewsSource
                            {
                                Title = subNode.GetProperty("title").GetString()!,
                                Url = subNode.GetProperty("url").GetString()!,
                                ID = id,
                                Source = "MSN",
                                Abstract = subNode.GetProperty("abstract").GetString()
                            });
                        }
                        catch { }
                    }
                    break;
                }
                catch
                {
                    retryTimes++;
                    resultList.Clear();
                    totalCount = 0;
                    try
                    {
                        await Task.Delay(500, cancellationToken);
                    }
                    catch { }
                }
            }
        }, cancellationToken);
        if (retryTimes >= 5)
            return new()
            {
                Success = false,
                Message = "无法获取新闻，请检查网络连接"
            };
        return new()
        {
            NewsSourceList = resultList,
            Success = true,
            Message = "Success",
            Count = totalCount
        };
    }

    public static async Task<NewsResult> ClimbWeiboHotSearch(CancellationToken cancellationToken)
    {
        int index = 1;
        string message = "Success";
        var resultList = new List<NewsSource>();
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36 Edg/135.0.0.0");
            QueryableJsonNode jsonNode = await client.GetStringAsync("https://m.weibo.cn/api/container/getIndex?containerid=106003type%3D25%26t%3D3%26disable_hot%3D1%26filter_type%3Drealtimehot&luicode=20000061&lfid=5070140584495876", cancellationToken);
            foreach (var node in jsonNode["data"]["cards"].GetChildNodes().First()["card_group"]["package:list", "desc", "desc_extr", "icon"].PackageInListObject())
            {
                if (node.TryGetValue("desc_extr", out var desc_extr) && !desc_extr.ToString().IsNullOrWhiteSpace())
                    resultList.Add(new NewsSource
                    {
                        Title = Regex.Unescape(node["desc"].ToString()),
                        Abstract = Regex.Unescape(desc_extr.ToString()),
                        Source = "微博热搜",
                        ImageWidth = 30,
                        ImageHeight = 30,
                        Index = index++,
                        ImageUrl = node.TryGetValue("icon", out var icon) ? Regex.Unescape(icon.ToString()) : string.Empty
                    });
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }
        return new()
        {
            NewsSourceList = resultList,
            Success = resultList.Count > 0,
            Message = message,
            Count = resultList.Count
        };
    }

    public static async Task<NewsResult> ClimbBilibiliHotSearch(CancellationToken cancellationToken)
    {
        int index = 1;
        string message = "Success";
        var resultList = new List<NewsSource>();
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36 Edg/135.0.0.0");
            QueryableJsonNode jsonNode = await client.GetStringAsync("https://api.bilibili.com/x/web-interface/wbi/search/square?limit=50&platform=web&web_location=333.1007&w_rid=2247c88d4bf6d1fbd138018aa2c530d5&wts=1744698017", cancellationToken);
            foreach (var node in jsonNode["data"]["trending"]["list"]["package:list", "keyword", "heat_score", "icon"].PackageInListObject())
            {
                resultList.Add(new NewsSource
                {
                    Title = node["keyword"].ToString(),
                    Abstract = node["heat_score"].ToString(),
                    Source = "B站热搜",
                    ImageWidth = 18,
                    ImageHeight = 18,
                    Index = index++,
                    ImageUrl = node.TryGetValue("icon", out var icon) ? Regex.Unescape(icon.ToString()) : string.Empty
                });
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }
        return new()
        {
            NewsSourceList = resultList,
            Success = resultList.Count > 0,
            Message = message,
            Count = resultList.Count
        };
    }
}

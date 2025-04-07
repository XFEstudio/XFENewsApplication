using System.Text.Json;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.XFETransform.JsonConverter;
using XFENewsApplication.Core.Models;

namespace XFENewsApplication.Core.Utilities.Helpers;

public static class ClimbHelper
{
    public static string? MSNAPI { get; set; }
    public static async Task<List<NewsSource>> ClimbMSNNews()
    {
        var resultList = new List<NewsSource>();
        int reTryTimes = 0;
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36 Edg/129.0.0.0");
        while (reTryTimes < 5)
            try
            {
                if (MSNAPI.IsNullOrWhiteSpace())
                {
                    QueryableJsonNode authJsonNode = await client.GetStringAsync("https://www.msn.cn/resolver/api/resolve/v3/config/?expType=AppConfig&expInstance=default&apptype=homePage&v=20250401.183&targetScope={%22audienceMode%22:%22adult%22,%22browser%22:{%22browserType%22:%22edgeChromium%22,%22version%22:%22135%22,%22ismobile%22:%22false%22},%22deviceFormFactor%22:%22desktop%22,%22domain%22:%22www.msn.cn%22,%22locale%22:{%22content%22:{%22language%22:%22zh%22,%22market%22:%22cn%22},%22display%22:{%22language%22:%22zh%22,%22market%22:%22cn%22}},%22os%22:%22windows%22,%22platform%22:%22web%22,%22pageType%22:%22hp%22,%22pageExperiments%22:[]}");
                    MSNAPI = authJsonNode["configs"]["StripeWC/default"]["properties"]["apikey"];
                }
                var result = await client.GetStringAsync($"https://assets.msn.com/service/news/feed/pages/weblayout?User=&activityId=&adoffsets=c1:-1,c2:-1&apikey={MSNAPI}&audienceMode=adult&cm=zh-cn&colstatus=c1:0,c2:0&column=c2&colwidth=300&contentType=article&it=edgeid&l3v=2&layout=c2&memory=8&newsSkip=0&newsTop=48&ocid=hponeservicefeed&pgc=11&private=1&scn=APP_ANON&timeOut=1000&vpSize=721x674&wposchema=byregion");
                using var document = JsonDocument.Parse(result);
                int i = 0;
                foreach (var subNode in document.RootElement.GetProperty("sections").EnumerateArray().ElementAt(0).GetProperty("cards").EnumerateArray())
                {
                    try
                    {
                        if (i++ == 0)
                            continue;
                        resultList.Add(new NewsSource
                        {
                            Title = subNode.GetProperty("title").GetString()!,
                            Url = subNode.GetProperty("url").GetString()!,
                            ID = subNode.GetProperty("id").GetString(),
                            Abstract = subNode.GetProperty("abstract").GetString()
                        });
                    }
                    catch { }
                }
                break;
            }
            catch
            {
                reTryTimes++;
            }
        if (reTryTimes >= 5)
            throw new Exception("无法获取新闻，请检查网络连接");
        return resultList;
    }
}

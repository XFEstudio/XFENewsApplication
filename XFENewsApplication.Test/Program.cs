using System.Text.Json;
using XFEExtension.NetCore.XFETransform.JsonConverter;

class Program
{
    [SMTest]
    public static async Task ClimbTest()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36 Edg/129.0.0.0");
        QueryableJsonNode authJsonNode = await client.GetStringAsync("https://www.msn.cn/resolver/api/resolve/v3/config/?expType=AppConfig&expInstance=default&apptype=homePage&v=20250401.183&targetScope={%22audienceMode%22:%22adult%22,%22browser%22:{%22browserType%22:%22edgeChromium%22,%22version%22:%22135%22,%22ismobile%22:%22false%22},%22deviceFormFactor%22:%22desktop%22,%22domain%22:%22www.msn.cn%22,%22locale%22:{%22content%22:{%22language%22:%22zh%22,%22market%22:%22cn%22},%22display%22:{%22language%22:%22zh%22,%22market%22:%22cn%22}},%22os%22:%22windows%22,%22platform%22:%22web%22,%22pageType%22:%22hp%22,%22pageExperiments%22:[]}");
        var result = await client.GetStringAsync($"https://assets.msn.com/service/news/feed/pages/weblayout?User=&activityId=&adoffsets=c1:-1,c2:-1&apikey={authJsonNode["configs"]["StripeWC/default"]["properties"]["apikey"]}&audienceMode=adult&cm=zh-cn&colstatus=c1:0,c2:0&column=c2&colwidth=300&contentType=article,video,slideshow,webcontent&it=edgeid&l3v=2&layout=c2&memory=8&newsSkip=0&newsTop=48&ocid=hponeservicefeed&pgc=11&private=1&scn=APP_ANON&timeOut=1000&vpSize=721x674&wposchema=byregion");
        using var document = JsonDocument.Parse(result);
        int i = 0;
        foreach (var subNode in document.RootElement.GetProperty("sections").EnumerateArray().ElementAt(0).GetProperty("cards").EnumerateArray())
        {
            if (i++ == 0)
                continue;
            Console.WriteLine($"{subNode.GetProperty("title").GetString()}\n{subNode.GetProperty("abstract").GetString()}\n{subNode.GetProperty("url").GetString()}\n");
        }
    }
}
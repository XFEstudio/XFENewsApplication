using XFEExtension.NetCore.XFETransform.JsonConverter;

class Program
{
    //[SMTest]
    public static string TestMethod()
    {
        return Path.GetExtension("adwada.png");
    }

    [SMTest("attitude")]
    public static async Task<List<string>> TestBaiduTranslate(string word)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36 Edg/135.0.0.0");
        var result = await client.PostAsync("https://fanyi.baidu.com/sug", new StringContent($"kw={word}"));
        result.EnsureSuccessStatusCode();
        QueryableJsonNode jsonNode = await result.Content.ReadAsStringAsync();
        return [.. jsonNode["data"]["package:list", "k", "v"].PackageInListObject().Select(wordInfo => wordInfo["v"].ToString())];
    }
}
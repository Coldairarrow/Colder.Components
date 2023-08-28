using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Colder.Common;

/// <summary>
/// 
/// </summary>
public class MiraiClient
{
    private readonly string _baseUrl;
    private readonly string _verifyKey;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// 
    /// </summary>
    public MiraiClient(IHttpClientFactory httpClientFactory, string baseUrl, string verifyKey)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = baseUrl;
        _verifyKey = verifyKey;
    }

    private string GetSessionKey(string qqNumber)
    {
        var body = new
        {
            verifyKey = _verifyKey
        };

        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_baseUrl);

        var responseJson = AsyncHelper.RunSync(() => httpClient.PostJson("verify", JsonConvert.SerializeObject(body)));
        var responseObj = JObject.Parse(responseJson);

        var sessionKey = responseObj["session"].ToString();

        AsyncHelper.RunSync(() => httpClient.PostJson("bind", ToJson(new
        {
            sessionKey,
            qq = qqNumber
        })));

        return sessionKey;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="qqNumber"></param>
    /// <param name="groupNumber"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendGroupMessage(string qqNumber, string groupNumber, string message)
    {
        var body = new
        {
            sessionKey = GetSessionKey(qqNumber),
            target = groupNumber,
            messageChain = new object[] {
                new
                {
                    type="Plain",
                    text=message
                }
            }
        };

        await Request("sendGroupMessage", body);
    }

    private async Task<JObject> Request(string path, object body)
    {
        var jsonObj = JObject.FromObject(body);

        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_baseUrl);

        var responseJson = await httpClient.PostJson(path, ToJson(jsonObj));
        var responseObj = JObject.Parse(responseJson);

        return responseObj;
    }

    private static string ToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}
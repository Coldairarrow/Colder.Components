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
    private readonly string _qqNumber;
    private readonly string _baseUrl;
    private readonly string _verifyKey;
    private readonly IHttpClientFactory _httpClientFactory;
    private string _sessionKey;
    private readonly object _lockObj = new object();

    /// <summary>
    /// 
    /// </summary>
    public MiraiClient(IHttpClientFactory httpClientFactory, string qqNumber, string baseUrl, string verifyKey)
    {
        _httpClientFactory = httpClientFactory;
        _qqNumber = qqNumber;
        _baseUrl = baseUrl;
        _verifyKey = verifyKey;
    }

    private void RefreshSessionKey()
    {
        var body = new
        {
            verifyKey = _verifyKey
        };

        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_baseUrl);

        var responseJson = AsyncHelper.RunSync(() => httpClient.PostJson("verify", JsonConvert.SerializeObject(body)));
        var responseObj = JObject.Parse(responseJson);

        _sessionKey = responseObj["session"].ToString();

        AsyncHelper.RunSync(() => httpClient.PostJson("bind", ToJson(new
        {
            sessionKey = _sessionKey,
            qq = _qqNumber
        })));
    }

    private string GetSessionKey()
    {
        if (string.IsNullOrEmpty(_sessionKey))
        {
            lock (_lockObj)
            {
                if (string.IsNullOrEmpty(_sessionKey))
                {
                    RefreshSessionKey();
                }
            }
        }

        return _sessionKey;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="groupNumber"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendGroupMessage(string groupNumber, string message)
    {
        var body = new
        {
            sessionKey = GetSessionKey(),
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

        if ((int)responseObj["code"] != 0)
        {
            RefreshSessionKey();

            jsonObj["sessionKey"] = GetSessionKey();
            responseJson = await httpClient.PostJson(path, ToJson(jsonObj));
            responseObj = JObject.Parse(responseJson);
        }

        return responseObj;
    }

    private static string ToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}

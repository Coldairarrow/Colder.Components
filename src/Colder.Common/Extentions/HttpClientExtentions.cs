using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Colder.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpClientExtentions
    {
        /// <summary>
        /// Post请求并获取Json
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static async Task<string> PostJson(this HttpClient httpClient, string url, string body)
        {
            HttpContent content = new StringContent(body);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}

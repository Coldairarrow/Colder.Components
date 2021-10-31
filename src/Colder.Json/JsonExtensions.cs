using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Linq;

namespace Colder.Json
{
    /// <summary>
    /// Json拓展
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Iso配置，时间格式为ISO-8601
        /// </summary>
        /// <remarks>系统内部需要可读性时采用此序列化，例如缓存、日志打印等</remarks>
        public static readonly JsonSerializerSettings IsoSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            Converters = new JsonConverter[] { new IsoDateTimeConverter() }
        };

        /// <summary>
        /// 时间戳配置
        /// </summary>
        /// <remarks>接口数据传输的时候使用此序列化</remarks>
        public static readonly JsonSerializerSettings TimestampSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            Converters = new JsonConverter[] { new TimestampConverter() }
        };

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="timestamp">是否序列化成时间戳，默认true</param>
        /// <param name="settings">自定义配置</param>
        /// <returns></returns>
        public static string ToJson(this object obj, bool timestamp = true, JsonSerializerSettings settings = null)
        {
            settings = settings ?? (timestamp ? TimestampSettings : IsoSettings);
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="timestamp">是否序列化成时间戳，默认true</param>
        /// <param name="settings">自定义配置</param>
        /// <returns></returns>
        public static T ToObject<T>(this string json, bool timestamp = true, JsonSerializerSettings settings = null)
        {
            settings = settings ?? (timestamp ? TimestampSettings : IsoSettings);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// josn中属性排序
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string SortJson(string json)
        {
            var jObj = JsonConvert.DeserializeObject<JObject>(json);
            Sort(jObj);

            return JsonConvert.SerializeObject(jObj);

            void Sort(JObject jObj)
            {
                var props = jObj.Properties().ToList();
                foreach (var prop in props)
                {
                    prop.Remove();
                }

                foreach (var prop in props.OrderBy(p => p.Name))
                {
                    jObj.Add(prop);
                    if (prop.Value is JObject)
                        Sort((JObject)prop.Value);
                    if (prop.Value is JArray)
                    {
                        int iCount = prop.Value.Count();
                        for (int iIterator = 0; iIterator < iCount; iIterator++)
                            if (prop.Value[iIterator] is JObject)
                                Sort((JObject)prop.Value[iIterator]);
                    }
                }
            }
        }
    }
}

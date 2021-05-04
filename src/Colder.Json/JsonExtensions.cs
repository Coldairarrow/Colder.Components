using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Colder.Json
{
    /// <summary>
    /// Json拓展
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// 默认配置
        /// </summary>
        public static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver()
        };

        /// <summary>
        /// 时间戳配置
        /// </summary>
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
            settings = settings ?? (timestamp ? TimestampSettings : DefaultSettings);
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
            settings = settings ?? (timestamp ? TimestampSettings : DefaultSettings);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}

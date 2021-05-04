using Newtonsoft.Json;
using System;

namespace Colder.Json
{
    internal class TimestampConverter : JsonConverter
    {
        private static readonly long _minTimestamp = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();
        private static readonly long _maxTimestamp = DateTimeOffset.MaxValue.ToUnixTimeMilliseconds();

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime)
                || objectType == typeof(DateTime?)
                || objectType == typeof(DateTimeOffset)
                || objectType == typeof(DateTimeOffset?)
                ;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return reader.Value;
            }

            if (reader.TokenType != JsonToken.Integer)
            {
                throw new Exception($"序列化失败：必须为时间戳 {reader.Value}");
            }

            var timestamp = (long)reader.Value;
            //小于最小值或大于最大值，做兼容，防止抛异常
            if (timestamp < _minTimestamp)
            {
                timestamp = _minTimestamp;
            }
            else if (timestamp > _maxTimestamp)
            {
                timestamp = _maxTimestamp;
            }

            var date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);

            if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
            {
                return date.LocalDateTime;
            }
            else
            {
                return date;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();

                return;
            }

            var type = value.GetType();
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                var timeValue = (DateTime)value;
                if (timeValue.Kind == DateTimeKind.Unspecified)
                {
                    timeValue = new DateTime(timeValue.Ticks, DateTimeKind.Local);
                }

                writer.WriteValue(new DateTimeOffset(timeValue).ToUnixTimeMilliseconds());
            }
            else
            {
                writer.WriteValue(((DateTimeOffset)value).ToUnixTimeMilliseconds());
            }
        }
    }
}

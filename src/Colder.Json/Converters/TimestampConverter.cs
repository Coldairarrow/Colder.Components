﻿using Colder.Common;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Colder.Json
{
    /// <summary>
    /// 
    /// </summary>
    public class TimestampConverter : JsonConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime)
                || objectType == typeof(DateTime?)
                || objectType == typeof(DateTimeOffset)
                || objectType == typeof(DateTimeOffset?)
                ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return reader.Value;
            }

            long timestamp = 0;
            if (reader.TokenType == JsonToken.Integer)
            {
                timestamp = (long)reader.Value;
            }
            else if (long.TryParse(reader.Value.ToString(), out long t))
            {
                timestamp = t;
            }
            else if (DateTime.TryParse(reader.Value.ToString(), null, DateTimeStyles.AssumeLocal, out DateTime dt))
            {
                timestamp = new DateTimeOffset(dt).ToUnixTimeMilliseconds();
            }

            var date = TimestampHelper.FromUnixTimeMilliseconds(timestamp);

            if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
            {
                return date.LocalDateTime;
            }
            else
            {
                return date;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
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

                writer.WriteValue(TimestampHelper.ToUnixTimeMilliseconds(timeValue));
            }
            else
            {
                writer.WriteValue(((DateTimeOffset)value).ToUnixTimeMilliseconds());
            }
        }
    }
}
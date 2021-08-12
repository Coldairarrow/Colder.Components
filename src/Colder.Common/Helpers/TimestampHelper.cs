using System;

namespace Colder.Common
{
    /// <summary>
    /// 时间戳帮助类
    /// </summary>
    public static class TimestampHelper
    {
        private static readonly long _minTimestamp = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();
        private static readonly long _maxTimestamp = DateTimeOffset.MaxValue.ToUnixTimeMilliseconds();

        /// <summary>
        /// 转为Unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToUnixTimeMilliseconds(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Local);
            }

            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Unix时间戳转为DateTimeOffset
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTimeOffset FromUnixTimeMilliseconds(long timestamp)
        {
            //小于最小值或大于最大值，做兼容，防止抛异常
            if (timestamp < _minTimestamp)
            {
                timestamp = _minTimestamp;
            }
            else if (timestamp > _maxTimestamp)
            {
                timestamp = _maxTimestamp;
            }

            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
        }
    }
}

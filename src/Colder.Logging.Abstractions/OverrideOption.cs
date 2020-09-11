using Microsoft.Extensions.Logging;

namespace Colder.Logging.Abstractions
{
    /// <summary>
    /// 重写
    /// </summary>
    public class OverrideOption
    {
        /// <summary>
        /// 源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 最低级别
        /// </summary>
        public LogLevel MinLevel { get; set; }
    }
}

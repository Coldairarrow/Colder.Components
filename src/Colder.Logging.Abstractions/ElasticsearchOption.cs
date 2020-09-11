using System.Collections.Generic;

namespace Colder.Logging.Abstractions
{
    /// <summary>
    /// ES配置
    /// </summary>
    public class ElasticsearchOption : EnableOption
    {
        /// <summary>
        /// ES节点
        /// </summary>
        public List<string> Nodes { get; set; } = new List<string>();

        /// <summary>
        /// 索引格式:custom-index-{0:yyyy.MM}
        /// </summary>
        public string IndexFormat { get; set; }
    }
}

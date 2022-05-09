using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Colder.Logging.Abstractions
{
    /// <summary>
    /// 日志配置项
    /// </summary>
    public class LogOptions
    {
        private string _instance;

        /// <summary>
        /// 实例名,默认为机器名
        /// </summary>
        public string Instance
        {
            get => string.IsNullOrEmpty(_instance) ? Environment.MachineName : _instance;
            set => _instance = value;
        }

        /// <summary>
        /// 最低日志输出级别
        /// </summary>
        public LogLevel MinLevel { get; set; }

        /// <summary>
        /// 输出到控制台
        /// </summary>
        public EnableOption Console { get; set; } = new EnableOption();

        /// <summary>
        /// 输出到调试
        /// </summary>
        public EnableOption Debug { get; set; } = new EnableOption();

        /// <summary>
        /// 输出到文件
        /// </summary>
        public EnableOption File { get; set; } = new EnableOption();

        /// <summary>
        /// 输出到ES
        /// </summary>
        public ElasticsearchOption Elasticsearch { get; set; } = new ElasticsearchOption();

        /// <summary>
        /// 输出到Kafka
        /// </summary>
        public KafkaOption Kafka { get; set; } = new KafkaOption();

        /// <summary>
        /// 重写日志级别 
        /// </summary>
        public List<OverrideOption> Overrides { get; set; } = new List<OverrideOption>();
    }
}

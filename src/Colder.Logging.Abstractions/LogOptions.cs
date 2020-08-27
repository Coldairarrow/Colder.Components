using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Colder.Logging.Abstractions
{
    public class LogOptions
    {
        public LogLevel MinLevel { get; set; }
        public EnableOption Console { get; set; } = new EnableOption();
        public EnableOption Debug { get; set; } = new EnableOption();
        public EnableOption File { get; set; } = new EnableOption();
        public ElasticsearchOption Elasticsearch { get; set; } = new ElasticsearchOption();
        public List<OverrideOption> Overrides { get; set; } = new List<OverrideOption>();
    }
}

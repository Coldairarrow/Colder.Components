using System.Collections.Generic;

namespace Colder.Logging.Abstractions
{
    public class ElasticsearchOption : EnableOption
    {
        public List<string> Nodes { get; set; } = new List<string>();
        public string IndexFormat { get; set; }
    }
}

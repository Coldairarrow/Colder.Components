using Microsoft.Extensions.Logging;

namespace Colder.Logging.Abstractions
{
    public class OverrideOption
    {
        public string Source { get; set; }
        public LogLevel MinLevel { get; set; }
    }
}

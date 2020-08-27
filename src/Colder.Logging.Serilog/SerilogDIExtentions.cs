using Colder.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Colder.Logging.Serilog
{
    /// <summary>
    /// 注入拓展
    /// </summary>
    public static class SerilogDIExtentions
    {
        /// <summary>
        /// 配置日志
        /// </summary>
        /// <param name="hostBuilder">建造者</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureLoggingDefaults(this IHostBuilder hostBuilder)
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var path = Path.Combine(rootPath, "logs", "log.txt");

            return hostBuilder.UseSerilog((hostingContext, serilogConfig) =>
            {
                var envConfig = hostingContext.Configuration;

                LogOptions logConfig = new LogOptions();
                envConfig.GetSection("log").Bind(logConfig);

                logConfig.Overrides.ForEach(aOverride =>
                {
                    serilogConfig
                        .MinimumLevel
                        .Override(aOverride.Source, (LogEventLevel)aOverride.MinLevel);
                });

                serilogConfig.MinimumLevel.Is((LogEventLevel)logConfig.MinLevel);
                if (logConfig.Console.Enabled)
                {
                    serilogConfig.WriteTo.Console();
                }
                if (logConfig.Debug.Enabled)
                {
                    serilogConfig.WriteTo.Debug();
                }
                if (logConfig.File.Enabled)
                {
                    string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3} {SourceContext:l}] {Message:lj}{NewLine}{Exception}";

                    serilogConfig.WriteTo.File(
                        path,
                        outputTemplate: template,
                        rollingInterval: RollingInterval.Day,
                        shared: true,
                        fileSizeLimitBytes: 10 * 1024 * 1024,
                        rollOnFileSizeLimit: true
                        );
                }
                if (logConfig.Elasticsearch.Enabled)
                {
                    var uris = logConfig.Elasticsearch.Nodes.Select(x => new Uri(x)).ToList();

                    serilogConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(uris)
                    {
                        IndexFormat = logConfig.Elasticsearch.IndexFormat,
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    });
                }
            });
        }
    }
}

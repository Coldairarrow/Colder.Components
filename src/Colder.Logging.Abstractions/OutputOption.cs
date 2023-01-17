namespace Colder.Logging.Abstractions;

/// <summary>
/// 带模板输出日志
/// </summary>
public class OutputOption: EnableOption
{
    /// <summary>
    /// 日志模板配置
    /// </summary>
    public string Template { get; set; } = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3} {SourceContext:l}] {Message:lj}{NewLine}{Exception}";
}

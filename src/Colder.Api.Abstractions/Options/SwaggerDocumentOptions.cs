using System;

namespace Colder.Api.Abstractions.Options;

/// <summary>
/// Swagger 文档配置。
/// </summary>
public class SwaggerDocumentOptions
{
    /// <summary>
    /// 获取或设置 Swagger 标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 获取或设置文档名称。
    /// </summary>
    public string DocumentName { get; set; }

    /// <summary>
    /// 获取或设置 API 分组名称。
    /// </summary>
    public string[] ApiGroupNames { get; set; } = Array.Empty<string>();
}

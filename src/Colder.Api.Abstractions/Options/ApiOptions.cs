using System;

namespace Colder.Api.Abstractions.Options;

/// <summary>
/// 
/// </summary>
public class ApiOptions
{
    /// <summary>
    /// 启用Jwt
    /// </summary>
    public bool EnableJwt { get; set; } = true;

    /// <summary>
    /// Jwt密钥
    /// </summary>
    public string JwtSecret { get; set; }

    /// <summary>
    /// 启用Swagger
    /// </summary>
    public bool EnableSwagger { get; set; } = true;

    /// <summary>
    /// 文档分组
    /// </summary>
    public SwaggerDocumentOptions[] Documents { get; set; } = Array.Empty<SwaggerDocumentOptions>();
}

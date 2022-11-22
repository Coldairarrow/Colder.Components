namespace Colder.Api.Abstractions;

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
    public string JwtSecret { get; set; } = "08LTxQVb4Z0Zlfio";

    /// <summary>
    /// 启用Swagger
    /// </summary>
    public bool EnableSwagger { get; set; } = true;
}

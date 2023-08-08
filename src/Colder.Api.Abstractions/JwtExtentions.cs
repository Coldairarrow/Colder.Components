using Colder.Api.Abstractions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Colder.Api.Abstractions;

/// <summary>
/// 
/// </summary>
public static class JwtExtentions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="apiOption"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwt(this IServiceCollection services, ApiOptions  apiOption)
    {
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = false;
            //Token Validation Parameters
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,//到时间立即过期
                ValidateIssuerSigningKey = true,
                //获取或设置要使用的Microsoft.IdentityModel.Tokens.SecurityKey用于签名验证。
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.
                GetBytes(apiOption.JwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        return services;
    }
}
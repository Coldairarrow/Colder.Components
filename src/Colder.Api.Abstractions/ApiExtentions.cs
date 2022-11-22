using Colder.DistributedId;
using Colder.Json;
using Colder.Logging.Serilog;
using Logistics.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using System;
using System.Linq;

namespace Colder.Api.Abstractions;

/// <summary>
/// Api拓展
/// </summary>
public static class ApiExtentions
{
    /// <summary>
    /// 注入Api日志
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApiLog(this IServiceCollection services)
    {
        return services.AddScoped<RequestInfo>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureWebApiDefaults(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureLoggingDefaults();
        hostBuilder.ConfigureDistributedIdDefaults();
        hostBuilder.ConfigureServices((host, services) =>
        {
            services.AddOptions();
            services.AddOptions<ApiOptions>();

            var theConfig = host.Configuration.GetChildren().Where(x => x.Key.ToLower() == "api").FirstOrDefault();
            ApiOptions apiOption = new ApiOptions();
            if (theConfig == null)
            {
                services.Configure<ApiOptions>(x =>
                {
                    x.EnableSwagger = apiOption.EnableSwagger;
                    x.EnableJwt = apiOption.EnableJwt;
                    x.JwtSecret = apiOption.JwtSecret;
                });
            }
            else
            {
                services.Configure<ApiOptions>(theConfig);
                apiOption = theConfig.Get<ApiOptions>();
            }

            services.AddApiLog();

            services.AddControllers(options =>
            {
                options.Filters.Add<ExceptionFilter>();
            }).AddControllersAsServices().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.GetType().GetProperties().ToList().ForEach(aProperty =>
                {
                    var value = aProperty.GetValue(JsonExtensions.TimestampSettings);
                    aProperty.SetValue(options.SerializerSettings, value);
                });
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            //跨域
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .DisallowCredentials()
                           .WithExposedHeaders("Content-Disposition")
                           ;
                });
            });

            //swagger
            services.AddOpenApiDocument(settings =>
            {
                settings.AllowReferencesWithProperties = true;
                settings.AddSecurity("身份认证Token", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
                {
                    Scheme = "bearer",
                    Description = "Authorization:Bearer {your JWT token}",
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Type = OpenApiSecuritySchemeType.Http
                });
            });

            //jwt
            services.AddJwt(apiOption);
            services.AddHttpClient();
            services.AddHttpContextAccessor();
        });

        return hostBuilder;
    }

    /// <summary>
    /// 使用Api日志
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseApiLog(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestInfoMiddleware>()
            .UseMiddleware<RequestLogMiddleware>();
    }

    /// <summary>
    /// 使用WebApi默认配置
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseWebApiDefaults(this IApplicationBuilder app)
    {
        app.UseApiLog();

        app.UseCors();

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseOpenApi()//添加swagger生成api文档（默认路由文档 /swagger/v1/swagger.json）
            .UseSwaggerUi3();//添加Swagger UI到请求管道中(默认路由: /swagger).

        return app;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceBuilder"></param>
    /// <param name="builder"></param>
    /// <param name="appBuilder"></param>
    public static void RunWebApiDefaults(this WebApplicationBuilder builder, Action<IServiceCollection> serviceBuilder = null, Action<WebApplication> appBuilder = null)
    {
        var host = builder.Host;
        var services = builder.Services;
        var configuration = builder.Configuration;

        host.ConfigureWebApiDefaults();

        serviceBuilder?.Invoke(services);

        var app = builder.Build();
        app.UseWebApiDefaults();
        appBuilder?.Invoke(app);

        app.Run();
    }
}

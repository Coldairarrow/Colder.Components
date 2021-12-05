using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Colder.Api.Abstractions
{
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
        /// 使用Api日志
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestInfoMiddleware>()
                .UseMiddleware<RequestLogMiddleware>();
        }
    }
}

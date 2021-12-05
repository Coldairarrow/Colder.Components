using Colder.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Threading.Tasks;

namespace Colder.Api.Abstractions
{
    internal class RequestInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if ((context.Request.ContentType ?? string.Empty).Contains("application/json"))
            {
                context.Request.EnableBuffering();
                string body = await context.Request.Body?.ReadToStringAsync(Encoding.UTF8);
                context.RequestServices.GetService<RequestInfo>().RequestBody = body;
            }

            await _next(context);
        }
    }
}

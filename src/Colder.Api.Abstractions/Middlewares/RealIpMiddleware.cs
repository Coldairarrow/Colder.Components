using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Net;

namespace Colder.Api.Abstractions.Middlewares;

internal class RealIpMiddleware
{
    private readonly RequestDelegate _next;

    public RealIpMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        var headers = context.Request.Headers;
        if (headers.ContainsKey("X-Forwarded-For"))
        {
            context.Connection.RemoteIpAddress = IPAddress.Parse(headers["X-Forwarded-For"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)[0]);
        }
        return _next(context);
    }
}
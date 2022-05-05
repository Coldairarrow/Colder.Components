using Colder.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Colder.Api.Abstractions
{
    internal class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLogMiddleware(RequestDelegate next, ILogger<RequestLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            Stopwatch watch = Stopwatch.StartNew();
            string responseBody = string.Empty;

            //返回Body需特殊处理
            Stream originalResponseBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;
            Exception theEx = null;

            try
            {
                await _next(context);

                memStream.Position = 0;
                responseBody = new StreamReader(memStream).ReadToEnd();

                memStream.Position = 0;
                await memStream.CopyToAsync(originalResponseBody);
            }
            catch (Exception ex)
            {
                theEx = ex;
                throw;
            }
            finally
            {
                context.Response.Body = originalResponseBody;

                watch.Stop();

                if (responseBody?.Length > 1024)
                {
                    responseBody = responseBody.Substring(0, 1024);
                    responseBody += "......内容太长已忽略";
                }
                var requestBody = context.RequestServices.GetService<RequestInfo>().RequestBody;
                if (requestBody?.Length > 1024)
                {
                    requestBody = requestBody.Substring(0, 1024);
                    requestBody += "......内容太长已忽略";
                }

                string log =
            @"方向:请求本系统
Url:{Url}
ElapsedMilliseconds:{ElapsedMilliseconds}ms
Method:{Method}
ContentType:{ContentType}
Header:{Header}
Body:{Body}
StatusCode:{StatusCode}

Response:{Response}
";
                _logger.Log(
                    theEx == null ? LogLevel.Information : LogLevel.Error,
                    theEx,
                    log,
                    context.Request.Path,
                    (int)watch.ElapsedMilliseconds,
                    context.Request.Method,
                    context.Request.ContentType,
                    context.Request.Headers.ToJson(false),
                    requestBody,
                    context.Response.StatusCode,
                    responseBody
                    );
            }
        }
    }
}

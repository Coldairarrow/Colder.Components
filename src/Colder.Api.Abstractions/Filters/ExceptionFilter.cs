using Colder.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Logistics.Api;

/// <summary>
/// 
/// </summary>
public class ExceptionFilter : IAsyncExceptionFilter
{
    readonly ILogger _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        Exception ex = context.Exception;

        if (ex is MsgException busEx)
        {
            _logger.LogInformation(busEx.Message);
            context.Result = new JsonResult(new ApiResult { Msg = ex.Message, Code = 1 });
        }
        else
        {
            _logger.LogError(ex, ex.Message);
            context.Result = new JsonResult(new ApiResult { Msg = "系统异常", Code = 1 });
        }

        await Task.CompletedTask;
    }
}

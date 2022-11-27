using Colder.Common;
using Microsoft.AspNetCore.Mvc;

namespace Colder.Api.Abstractions.Controllers;

/// <summary>
/// Api控制器
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// 返回成功
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="data">数据</param>
    /// <returns></returns>
    protected ApiResult<T> Success<T>(T data)
    {
        return new ApiResult<T> { Data = data, Code = 200 };
    }

    /// <summary>
    /// 返回成功
    /// </summary>
    /// <returns></returns>
    protected ApiResult Success()
    {
        return new ApiResult { Code = 200 };
    }

    /// <summary>
    /// 返回失败
    /// </summary>
    /// <param name="msg">错误消息</param>
    /// <param name="code">错误码</param>
    /// <returns></returns>
    protected ApiResult Failure(string msg, int code = 1)
    {
        return new ApiResult { Msg = msg, Code = code };
    }

    /// <summary>
    /// 返回失败
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="msg">错误消息</param>
    /// <param name="data">数据</param>
    /// <param name="code">错误码</param>
    /// <returns></returns>
    protected ApiResult<T> Failure<T>(string msg, T data = default, int code = 1)
    {
        return new ApiResult<T> { Data = data, Msg = msg, Code = code };
    }
}

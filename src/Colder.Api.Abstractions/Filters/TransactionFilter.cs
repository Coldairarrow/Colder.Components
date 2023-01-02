using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Colder.Api.Abstractions.Filters;

/// <summary>
/// 
/// </summary>
public class TransactionFilter : Attribute, IAsyncActionFilter
{
    private readonly Type _dbContextType;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContextType"></param>
    /// <exception cref="Exception"></exception>
    public TransactionFilter(Type dbContextType)
    {
        if (!dbContextType.IsAssignableTo(typeof(DbContext)))
        {
            throw new Exception("dbContextType必须为DbContext");
        }
        _dbContextType = dbContextType;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        IServiceProvider serviceProvider = context.HttpContext.RequestServices;
        var dbContext = serviceProvider.GetRequiredService(_dbContextType) as DbContext;

        using var transaction = await dbContext.Database.BeginTransactionAsync();

        var excuted = await next();

        if (excuted.Exception == null)
        {
            await transaction.CommitAsync();
        }
    }
}

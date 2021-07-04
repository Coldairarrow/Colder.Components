using Colder.Common;
using Colder.OpenService.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Colder.OpenService.Hosting
{
    internal class RPCResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Controller is IOpenService)
            {
                if (context.Result is EmptyResult)
                {
                    Success(null);

                    return;
                }
                else if (context.Result is ObjectResult objectResult)
                {
                    if (objectResult.Value is ApiResult)
                    {
                        return;
                    }
                    else
                    {
                        Success(objectResult.Value);
                    }
                }
            }

            void Success(object data)
            {
                ApiResult<object> res = new ApiResult<object>
                {
                    Data = data
                };

                context.Result = new ObjectResult(res);
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {

        }
    }
}

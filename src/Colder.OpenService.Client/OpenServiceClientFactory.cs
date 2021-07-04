using Castle.DynamicProxy;
using Colder.Common;
using Colder.Json;
using Colder.OpenService.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Colder.OpenService.Client
{
    /// <summary>
    /// 客户端工厂
    /// </summary>
    public class OpenServiceClientFactory
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly IServiceProvider _internelServiceProvider;
        static OpenServiceClientFactory()
        {
            _internelServiceProvider = new ServiceCollection()
                .AddHttpClient()
                .BuildServiceProvider();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public OpenServiceClientFactory(IHttpClientFactory httpClientFactory = null)
        {
            _httpClientFactory = httpClientFactory ?? _internelServiceProvider.GetService<IHttpClientFactory>();
        }

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <typeparam name="TService">服务接口</typeparam>
        /// <param name="openServiceOption">参数</param>
        /// <returns></returns>
        public TService GetClient<TService>(OpenServiceOptions openServiceOption) where TService : class, IOpenService
        {
            return GetClient(typeof(TService), openServiceOption) as TService;
        }

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="openServiceOption">参数</param>
        /// <returns></returns>
        internal object GetClient(Type interfaceType, OpenServiceOptions openServiceOption)
        {
            return _generator.CreateInterfaceProxyWithoutTarget(interfaceType, new ApiRPCClientProxyInterceptor(_httpClientFactory, openServiceOption, interfaceType).ToInterceptor());
        }

        private class ApiRPCClientProxyInterceptor : AsyncInterceptorBase
        {
            private readonly IHttpClientFactory _httpClientFactory;
            private readonly OpenServiceOptions _openServiceOption;
            private readonly Type _interfaceType;
            public ApiRPCClientProxyInterceptor(IHttpClientFactory httpClientFactory, OpenServiceOptions openServiceOption, Type interfaceType)
            {
                _httpClientFactory = httpClientFactory;
                _openServiceOption = openServiceOption;
                _interfaceType = interfaceType;
            }

            private async Task<object> InternelInterceptAsync(IInvocation invocation)
            {
                string url = string.Empty;
                string requestBody = string.Empty;
                string responseBody = string.Empty;

                try
                {
                    var httpClient = _httpClientFactory.CreateClient();

                    var paramterTypes = invocation.Method.GetParameters().Select(x => x.ParameterType).ToArray();
                    var path = Helper.GetRoute(_interfaceType, invocation.Method.Name, paramterTypes);
                    url = _openServiceOption.BaseUrl + path;
                    if (!Helper.IsJsonParamter(invocation.Method))
                    {
                        throw new Exception($"{invocation.Method.DeclaringType.Name}.{invocation.Method.Name}只能有一个DTO参数");
                    }

                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    requestBody = invocation.Arguments[0].ToJson();
                    HttpContent content = new StringContent(requestBody);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();

                    responseBody = await response.Content.ReadAsStringAsync();
                    ApiResult<JToken> apiResult = responseBody.ToObject<ApiResult<JToken>>();
                    if (apiResult.Code != 200)
                    {
                        throw new MsgException(apiResult.Msg);
                    }

                    var returnType = invocation.Method.ReturnType;
                    if (returnType == typeof(void) || returnType == typeof(Task))
                    {
                        return null;
                    }

                    else if (typeof(Task).IsAssignableFrom(returnType) && returnType.IsGenericType)
                    {
                        var dataType = returnType.GetGenericArguments()[0];

                        object returnData = apiResult.Data.ToObject(dataType);
                        invocation.ReturnValue = Task.FromResult(returnData);
                        return returnData;
                    }
                    else if (!typeof(Task).IsAssignableFrom(returnType))
                    {
                        var dataType = returnType;

                        object returnData = apiResult.Data.ToObject(dataType);
                        invocation.ReturnValue = returnData;
                        return returnData;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Exception innerEx = new Exception($"请求{url}失败,请求Body:{requestBody},返回Body:{responseBody}");

                    if (ex is MsgException msgException)
                    {
                        throw new MsgException(msgException.Message, msgException.Code, innerEx);
                    }
                    else
                    {
                        throw new Exception(ex.Message, innerEx);
                    }
                }
            }

            protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
            {
                await InternelInterceptAsync(invocation);
            }

            protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
            {
                var data = await InternelInterceptAsync(invocation);

                return await Task.FromResult((TResult)data);
            }
        }
    }
}

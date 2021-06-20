using Colder.OpenService.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Colder.OpenService.Hosting
{
    /// <summary>
    /// 依赖注入扩展
    /// </summary>
    public static class OpenServiceHostDependencyInjectionExtentions
    {
        private static void Validate()
        {
            //校验,每个接口与方法必须有Route特性
            //每个实现类只能实现一个接口
            //接口中方法名不能重复,路由不能重复
            var implements = Assembly.GetEntryAssembly().GetTypes().Where(x => x.IsWebApiRPCImplement()).ToList();
            implements.ForEach(aImplement =>
            {
                var interfaces = aImplement.GetInterfaces().Where(x => x.IsWebApiRPCInterface()).ToList();
                if (interfaces.Count > 1)
                {
                    throw new Exception($"{aImplement}禁止实现多个接口");
                }

                if (interfaces.Count == 0)
                {
                    throw new Exception($"{aImplement}未实现接口");
                }

                var theInterface = interfaces.FirstOrDefault();
                if (theInterface.GetCustomAttribute<Abstractions.RouteAttribute>() == null)
                {
                    throw new Exception($"{theInterface}必须定义RouteAttribute");
                }

                var methods = theInterface.GetMethods().ToList();
                methods.ForEach(aMethod =>
                {
                    if (aMethod.GetCustomAttribute<Abstractions.RouteAttribute>() == null)
                    {
                        throw new Exception($"{theInterface}.{aMethod.Name}必须定义RouteAttribute");
                    }
                });

                if (methods.Count !=
                    methods.Select(x => x.GetCustomAttribute<Abstractions.RouteAttribute>().Template).Distinct().Count())
                {
                    throw new Exception($"{theInterface}禁止路由重复");
                }
            });
        }

        /// <summary>
        /// 注入OpenService服务端
        /// </summary>
        /// <param name="mvcBuilder">IMvcBuilder</param>
        /// <param name="routePrefix">路由前缀,默认为api</param>
        /// <returns></returns>
        public static IMvcBuilder AddOpenServiceHost(this IMvcBuilder mvcBuilder, string routePrefix = "api")
        {
            Validate();

            Constant.RoutePrefix = routePrefix;

            mvcBuilder.PartManager.FeatureProviders.Add(new WebApiRPCProvider());

            mvcBuilder.Services.Configure<MvcOptions>(o =>
            {
                o.Conventions.Add(new WebApiRPCConvention());

                //统一使用ApiResult格式化返回
                o.Filters.Add<RPCResultFilter>();
            });

            return mvcBuilder;
        }
    }
}

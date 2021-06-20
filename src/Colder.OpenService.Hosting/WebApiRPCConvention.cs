using Colder.OpenService.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Colder.OpenService.Hosting
{
    internal class WebApiRPCConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var aController in application.Controllers.ToList())
            {
                var type = aController.ControllerType.AsType();
                if (!typeof(IOpenService).IsAssignableFrom(aController.ControllerType))
                {
                    continue;
                }

                var theInterface = type.GetInterfaces().Where(x => x.IsWebApiRPCInterface()).FirstOrDefault();

                //Action
                foreach (var aAction in aController.Actions)
                {
                    var actionName = aAction.ActionMethod.Name;

                    //统一POST
                    var actionSelectorModel = aAction.Selectors[0];
                    actionSelectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { "POST" }));
                    actionSelectorModel.EndpointMetadata.Add(new HttpPostAttribute());

                    var paramterTypes = aAction.ActionMethod.GetParameters().Select(x => x.ParameterType).ToArray();

                    //不存在接口方法，则为自定义接口
                    if (Helper.GetInterfaceMethod(theInterface, actionName, paramterTypes) == null)
                    {
                        continue;
                    }

                    //路由
                    foreach (var selector in aAction.Selectors)
                    {
                        string route = Helper.GetRoute(theInterface, actionName, paramterTypes);
                        if (!Constant.RoutePrefix.IsNullOrEmpty())
                        {
                            route = $"{Constant.RoutePrefix}/{route}".BuildUrl();
                        }

                        selector.AttributeRouteModel = new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(route));
                    }

                    //JSON参数,只有一个参数并且为复杂类型
                    if (Helper.IsJsonParamter(aAction.ActionMethod))
                    {
                        aAction.Parameters[0].BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                    }
                    //Form参数
                    else
                    {
                        aAction.Parameters.ToList().ForEach(aParamter =>
                        {
                            aParamter.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromFormAttribute() });
                        });
                    }
                }
            }
        }
    }
}

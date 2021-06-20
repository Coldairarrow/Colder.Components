using System;
using System.Linq;
using System.Reflection;

namespace Colder.OpenService.Abstractions
{
    internal static class Helper
    {
        public static string GetRoute(Type interfaceType, string methodName, Type[] paramterTypes)
        {
            var controllerRoute = interfaceType.GetCustomAttribute<RouteAttribute>();
            if (controllerRoute == null)
            {
                throw new Exception($"{interfaceType.Name}缺少Route");
            }

            var method = GetInterfaceMethod(interfaceType, methodName, paramterTypes);

            var actionRoute = method.GetCustomAttribute<RouteAttribute>();
            if (actionRoute == null || string.IsNullOrEmpty(actionRoute.Template))
            {
                throw new Exception($"{method.DeclaringType.Name}.{method.Name}缺少Route");
            }

            string fullRoute = $"{controllerRoute.Template}/{actionRoute.Template}";

            return fullRoute.BuildUrl();
        }

        public static MethodInfo GetInterfaceMethod(Type interfaceType, string methodName, Type[] paramterTypes)
        {
            return interfaceType.GetInterfaces().ToList().Concat(new Type[] { interfaceType })
                .Where(x => x.GetMethod(methodName, paramterTypes) != null)
                .Select(x => x.GetMethod(methodName, paramterTypes))
                .FirstOrDefault();
        }

        public static bool IsJsonParamter(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().Length == 1
                && !methodInfo.GetParameters()[0].ParameterType.IsSimpleType();
        }
    }
}

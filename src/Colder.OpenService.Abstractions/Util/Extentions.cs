using System;
using System.Linq;

namespace Colder.OpenService.Abstractions
{
    internal static class Extentions
    {
        public static bool IsSimpleType(this Type type)
        {
            return
                type.IsPrimitive ||
                new Type[]
                {
                    typeof(string),
                    typeof(decimal),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid)
                }.Contains(type) ||
                type.IsEnum ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                ;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsWebApiRPCInterface(this Type type)
        {
            return typeof(IOpenService).IsAssignableFrom(type) && type.IsInterface && type != typeof(IOpenService);
        }

        public static bool IsWebApiRPCImplement(this Type type)
        {
            return typeof(IOpenService).IsAssignableFrom(type)
                && !type.IsAbstract
                && !type.IsInterface;
        }

        public static string BuildUrl(this string url)
        {
            while (url.Contains("//"))
            {
                url = url.Replace("//", "/");
            }

            return url;
        }
    }
}

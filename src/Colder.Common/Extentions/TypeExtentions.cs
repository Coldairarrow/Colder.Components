using System;

namespace Colder.Common
{
    /// <summary>
    /// Type拓展
    /// </summary>
    public static class TypeExtentions
    {
        /// <summary>
        /// 是否为简单类型，即JSON序列化时直接ToString
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsSimple(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(type.GetGenericArguments()[0]);
            }
            return type.IsPrimitive
              || type.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal))
              || type.Equals(typeof(DateTime))
              || type.Equals(typeof(DateTimeOffset))
              || type.Equals(typeof(Guid))
              ;
        }
    }
}

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace Colder.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectExtentions
    {
        /// <summary>
        /// 判断是否为Null或者空
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object obj)
        {
            if (obj == null)
                return true;
            else
            {
                string objStr = obj.ToString();
                return string.IsNullOrEmpty(objStr);
            }
        }

        /// <summary>
        /// 改变类型
        /// </summary>
        /// <param name="obj">原对象</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static object ChangeTypeByConvert(this object obj, Type targetType)
        {
            object resObj;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                NullableConverter newNullableConverter = new NullableConverter(targetType);
                resObj = newNullableConverter.ConvertFrom(obj);
            }
            else
            {
                resObj = Convert.ChangeType(obj, targetType);
            }

            return resObj;
        }

        /// <summary>
        /// 判断是否有效
        /// 注：不为null，不为空Guid，不为空集合
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsValid(this object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is Guid?)
            {
                return (Guid?)obj != Guid.Empty;
            }
            if (obj is Guid guid)
            {
                return guid != Guid.Empty;
            }
            if (obj is string str)
            {
                return !string.IsNullOrEmpty(str);
            }
            if (obj is IEnumerable enumerable)
            {
                return enumerable.Cast<object>().Count() >= 1;
            }

            return true;
        }
    }
}

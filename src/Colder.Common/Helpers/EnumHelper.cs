using System;
using System.ComponentModel;

namespace Colder.Common.Helpers;

/// <summary>
/// 枚举帮助类
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// 将枚举描述值转换成枚举值。
    /// </summary>
    /// <typeparam name="T">枚举类型。</typeparam>
    /// <param name="enumDescription">枚举值描述<see cref="DescriptionAttribute"/></param>
    /// <returns></returns>
    public static T ToEnumByDescription<T>(this string enumDescription)
        where T : struct, IConvertible
    {
        var enumType = typeof(T);

        if (!enumType.IsEnum)
        {
            throw new InvalidOperationException($"该类型（{enumType}）并不是一个枚举类型！");
        }

        foreach (var field in enumType.GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (attribute.Description == enumDescription)
                {
                    return (T)field.GetRawConstantValue();
                }
            }
        }

        throw new Exception($"{enumDescription} 转换为{typeof(T).FullName} 失败");
    }
}

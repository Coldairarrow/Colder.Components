using Colder.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Colder.Common;

/// <summary>
/// 跟踪帮助类
/// </summary>
public static class TrackingHelper
{
    private static readonly BindingFlags _bindingFlags
        = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

    /// <summary>
    /// 自动跟踪实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbData">表数据</param>
    /// <param name="newData">新数据</param>
    /// <param name="add">添加</param>
    /// <param name="remove">删除</param>
    public static (List<object> added, List<object> updated, List<object> removed) Tracking<TEntity>(TEntity dbData, TEntity newData, Action<object> add, Action<object> remove)
    {
        return InternleTracking(dbData, newData, add, remove, new List<object>(), dbData);
    }

    private static (List<object> added, List<object> updated, List<object> removed) InternleTracking<TEntity>(TEntity dbData, TEntity newData, Action<object> add, Action<object> remove, List<object> accessedObjs, object partentEntity)
    {
        if (accessedObjs.Contains(dbData))
        {
            return (new List<object>(), new List<object>(), new List<object>());
        }

        if (dbData == null)
        {
            throw new Exception($"{nameof(dbData)}不能为Null");
        }
        if (newData == null)
        {
            throw new Exception($"{nameof(newData)}不能为Null");
        }

        var added = new List<object>();
        var updated = new List<object>
    {
        dbData
    };
        accessedObjs.Add(dbData);
        var removed = new List<object>();

        //普通属性跟踪
        var properties = dbData.GetType().GetPropertyOrFields().Where(x =>
            NeedTracking(x.MemberInfo)
            && !IsEntityClass(x.PropertyType)
            && !IsEntityCollection(x.PropertyType)
            ).ToList();
        properties.ForEach(aProperty =>
        {
            aProperty.SetValue(dbData, aProperty.GetValue(newData));
        });

        //类属性跟踪
        properties = dbData.GetType().GetPropertyOrFields().Where(x =>
            NeedTracking(x.MemberInfo)
            && IsEntityClass(x.PropertyType)
            ).ToList();

        properties.ForEach(aProperty =>
        {
            var dbValue = aProperty.GetValue(dbData);
            var newValue = aProperty.GetValue(newData);

            //添加
            if (dbValue == null && newValue != null)
            {
                var addValue = newValue.DeepClone();
                aProperty.SetValue(dbData, addValue);

                var children = GetAllChildren(addValue, accessedObjs);
                children.ForEach(item => add?.Invoke(item));

                added.AddRange(children);
            }
            //删除
            else if (dbValue != null && newValue == null)
            {
                var children = GetAllChildren(dbValue, accessedObjs);
                children.Reverse();
                children.ForEach(item => remove?.Invoke(item));

                aProperty.SetValue(dbData, null);

                removed.Add(children);
            }
            //修改
            else if (dbValue != null && newValue != null)
            {
                //递归
                var res = InternleTracking(dbValue, newValue, add, remove, accessedObjs, dbData);
                added.AddRange(res.added);
                updated.AddRange(res.updated);
                removed.AddRange(res.removed);
            }

            //若该类型为主表实体
            if (aProperty.PropertyType == partentEntity.GetType())
            {
                aProperty.SetValue(dbData, partentEntity);
            }
        });

        //集合属性跟踪
        properties = dbData.GetType().GetPropertyOrFields().Where(x =>
            NeedTracking(x.MemberInfo)
            && IsEntityCollection(x.PropertyType)
            ).ToList();
        properties.ForEach(aProperty =>
        {
            var itemType = aProperty.PropertyType.GetGenericArguments()[0];
            var idField = GetKeyField(itemType);

            var dbProperty = aProperty.GetValue(dbData);
            var newProperty = aProperty.GetValue(newData);
            var dbCollection = (dbProperty as IEnumerable).Cast<object>().ToList();
            var newCollection = (newProperty as IEnumerable).Cast<object>().ToList();

            //添加
            newCollection
                .Where(x => !dbCollection.Any(y => GetPropertyOrFieldValue(x, idField)?.ToString() == GetPropertyOrFieldValue(y, idField)?.ToString()))
                .ToList()
                .ForEach(aItem =>
                {
                    var addItem = DeepCloneExtensions.DeepClone(aItem);
                    var addMethod = aProperty.PropertyType.GetMethod("Add", new Type[] { itemType });

                    addMethod.Invoke(dbProperty, new object[] { addItem });

                    var children = GetAllChildren(addItem, accessedObjs);
                    children.ForEach(item => add?.Invoke(item));

                    added.AddRange(children);
                });

            //删除
            dbCollection
                .Where(x => !newCollection.Any(y => GetPropertyOrFieldValue(x, idField)?.ToString() == GetPropertyOrFieldValue(y, idField)?.ToString()))
                .ToList()
                .ForEach(aItem =>
                {
                    var children = GetAllChildren(aItem, accessedObjs);
                    children.Reverse();
                    children.ForEach(item => remove?.Invoke(item));

                    var removeMethod = aProperty.PropertyType.GetMethod("Remove", new Type[] { itemType });
                    removeMethod.Invoke(dbProperty, new object[] { aItem });

                    removed.AddRange(children);
                });

            //修改
            dbCollection
                .Where(x => newCollection.Any(y => GetPropertyOrFieldValue(x, idField)?.ToString() == GetPropertyOrFieldValue(y, idField)?.ToString()))
                .ToList()
                .ForEach(aItem =>
                {
                    var itemId = GetPropertyOrFieldValue(aItem, idField).ToString();
                    var newItem = newCollection.Where(x => GetPropertyOrFieldValue(x, idField)?.ToString() == itemId).FirstOrDefault();

                    //递归
                    var res = InternleTracking(aItem, newItem, add, remove, accessedObjs, dbData);
                    added.AddRange(res.added);
                    updated.AddRange(res.updated);
                    removed.AddRange(res.removed);
                });
        });

        return (added, updated, removed);
    }

    private static List<object> GetAllChildren(object obj, List<object> accessedObjs)
    {
        if (accessedObjs.Contains(obj))
        {
            return new List<object>();
        }

        var childrenList = new List<object>()
    {
        obj
    };
        accessedObjs.Add(obj);

        //类属性跟踪
        var properties = obj.GetType().GetPropertyOrFields().Where(x =>
            NeedTracking(x.MemberInfo)
            && IsEntityClass(x.PropertyType)
            ).ToList();
        properties.ForEach(aPropperty =>
        {
            var value = aPropperty.GetValue(obj);
            if (value != null)
            {
                childrenList.AddRange(GetAllChildren(value, accessedObjs));
            }
        });

        //集合属性跟踪
        properties = obj.GetType().GetPropertyOrFields().Where(x =>
            NeedTracking(x.MemberInfo)
            && IsEntityCollection(x.PropertyType)
            ).ToList();
        properties.ForEach(aProperty =>
        {
            var value = (IEnumerable)aProperty.GetValue(obj);
            if (value != null)
            {
                value.Cast<object>().ToList().ForEach(aItem =>
                {
                    childrenList.AddRange(GetAllChildren(aItem, accessedObjs));
                });
            }
        });

        return childrenList;
    }

    private static object GetPropertyOrFieldValue(object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName, _bindingFlags)?.GetValue(obj)
            ?? obj.GetType().GetField(propertyName, _bindingFlags)?.GetValue(obj);
    }
    private static bool NeedTracking(MemberInfo type)
    {
        var needTracking = type.GetCustomAttribute<NotMappedAttribute>() == null
            && type.GetCustomAttribute<ConcurrencyCheckAttribute>() == null;

        if (type is PropertyInfo property)
        {
            needTracking = needTracking && property.CanWrite;
        }

        return needTracking;
    }
    private static bool IsEntityClass(Type type)
    {
        return type.GetCustomAttribute<TableAttribute>() != null;
    }
    private static bool IsEntityCollection(Type type)
    {
        return type.GetInterfaces().Concat(new Type[] { type }).Any(x =>
            x.IsGenericType
            && x.GetGenericTypeDefinition() == typeof(ICollection<>)
            && x.GetGenericArguments()[0].GetCustomAttribute<TableAttribute>() != null);
    }
    private static string GetKeyField(Type type)
    {
        return type.GetProperties().Where(x => x.GetCustomAttribute<KeyAttribute>() != null).FirstOrDefault()?.Name ?? "Id";
    }
    private static PropertyOrField[] GetPropertyOrFields(this Type type)
    {
        return type.GetProperties(_bindingFlags).Select(x => new PropertyOrField { MemberInfo = x })
            .Concat(type.GetFields(_bindingFlags).Select(x => new PropertyOrField { MemberInfo = x }))
            .Where(x => !x.MemberInfo.IsDefined(typeof(CompilerGeneratedAttribute), false))//去除私有BackingField
            .ToArray();
    }
    private class PropertyOrField
    {
        public MemberInfo MemberInfo { get; set; }
        public Type PropertyType
        {
            get
            {
                if (MemberInfo is PropertyInfo property)
                {
                    return property.PropertyType;
                }
                else if (MemberInfo is FieldInfo field)
                {
                    return field.FieldType;
                }
                else
                {
                    throw new Exception("MemberInfo必须为属性或字段");
                }
            }
        }
        public void SetValue(object obj, object value)
        {
            if (MemberInfo is PropertyInfo property)
            {
                if (property.CanWrite)
                {
                    property.SetValue(obj, value);
                }
            }
            else if (MemberInfo is FieldInfo field)
            {
                field.SetValue(obj, value);
            }
            else
            {
                throw new Exception("memberInfo必须为属性或字段");
            }
        }
        public object GetValue(object obj)
        {
            if (MemberInfo is PropertyInfo property)
            {
                return property.GetValue(obj);
            }
            else if (MemberInfo is FieldInfo field)
            {
                return field.GetValue(obj);
            }
            else
            {
                throw new Exception("memberInfo必须为属性或字段");
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Colder.CommonUtil
{
    /// <summary>
    /// 跟踪帮助类
    /// </summary>
    public static class TrackingHelper
    {
        private static readonly BindingFlags _bindingFlags
                    = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// 自动跟踪实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="dbData">表数据</param>
        /// <param name="newData">新数据</param>
        public static (List<object> added, List<object> updated, List<object> removed) Tracking<TEntity>(TEntity dbData, TEntity newData)
        {
            List<object> added = new List<object>();
            List<object> updated = new List<object>();
            List<object> removed = new List<object>();

            //普通属性跟踪
            var properties = typeof(TEntity).GetProperties(_bindingFlags).Where(x =>
                x.CanWrite
                && NeedTracking(x.PropertyType)
                && (!IsEntityClass(x.PropertyType))
                && (!IsEntityCollection(x.PropertyType))
                ).ToList();
            properties.ForEach(aProperty =>
            {
                aProperty.SetValue(dbData, aProperty.GetValue(newData));
            });

            //类属性跟踪
            properties = typeof(TEntity).GetProperties().Where(x =>
                x.CanWrite
                && NeedTracking(x.PropertyType)
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

                    added.AddRange(SetAdded(addValue));
                }
                //删除
                else if (dbValue != null && newValue == null)
                {
                    aProperty.SetValue(dbData, null);

                    removed.Add(dbValue);
                }
                //修改
                else
                {
                    //递归
                    Tracking(dbValue, newValue);

                    updated.Add(dbValue);
                }
            });

            //集合属性跟踪
            properties = typeof(TEntity).GetProperties().Where(x =>
                x.CanWrite
                && NeedTracking(x.PropertyType)
                && IsEntityCollection(x.PropertyType)
                ).ToList();
            properties.ForEach(aProperty =>
            {
                Type itemType = aProperty.PropertyType.GetGenericArguments()[0];
                object dbProperty = aProperty.GetValue(dbData);
                object newProperty = aProperty.GetValue(newData);
                var dbCollection = (dbProperty as IEnumerable).Cast<object>();
                var newCollection = (newProperty as IEnumerable).Cast<object>();

                List<object> allValues = new List<object>();
                allValues.AddRange(dbCollection);
                allValues.AddRange(newCollection);
                allValues.ForEach(aItem =>
                {
                    string idField = GetKeyField(aItem.GetType());

                    var itemId = GetPropertyValue(aItem, idField);

                    var dbItem = dbCollection.Where(x => GetPropertyValue(x, idField) == itemId).FirstOrDefault();
                    var newItem = newCollection.Where(x => GetPropertyValue(x, idField) == itemId).FirstOrDefault();
                    //添加
                    if (dbItem == null && newItem != null)
                    {
                        var addItem = DeepCloneExtensions.DeepClone(aItem);
                        var addMethod = aProperty.PropertyType.GetMethod("Add", new Type[] { itemType });

                        addMethod.Invoke(dbProperty, new object[] { addItem });

                        added.AddRange(SetAdded(addItem));
                    }
                    //删除
                    else if (dbItem != null && newItem == null)
                    {
                        var removeMethod = aProperty.PropertyType.GetMethod("Remove", new Type[] { itemType });

                        removeMethod.Invoke(dbProperty, new object[] { dbItem });

                        removed.Add(dbItem);
                    }
                    //修改
                    else
                    {
                        //递归
                        Tracking(dbItem, newItem);

                        updated.Add(dbItem);
                    }
                });
            });

            return (added, updated, removed);
        }

        private static List<object> SetAdded(object obj)
        {
            List<object> addedList = new List<object>()
            {
                obj
            };

            //类属性跟踪
            var properties = obj.GetType().GetProperties().Where(x =>
                x.CanWrite
                && NeedTracking(x.PropertyType)
                && IsEntityClass(x.PropertyType)
                ).ToList();
            properties.ForEach(aPropperty =>
            {
                var value = aPropperty.GetValue(obj);
                if (value != null)
                {
                    addedList.AddRange(SetAdded(value));
                }
            });

            //集合属性跟踪
            properties = obj.GetType().GetProperties().Where(x =>
                x.CanWrite
                && NeedTracking(x.PropertyType)
                && IsEntityCollection(x.PropertyType)
                ).ToList();
            properties.ForEach(aProperty =>
            {
                var value = (IEnumerable)aProperty.GetValue(obj);
                if (value != null)
                {
                    value.Cast<object>().ToList().ForEach(aItem =>
                    {
                        addedList.AddRange(SetAdded(aItem));
                    });
                }
            });

            return addedList;
        }
        private static object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName, _bindingFlags).GetValue(obj);
        }
        private static bool NeedTracking(Type type)
        {
            return type.GetCustomAttribute<NotMappedAttribute>() == null
                && type.GetCustomAttribute<ConcurrencyCheckAttribute>() == null;
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
    }
}

using DynamicExpresso;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Colder.Common.Util
{
    /// <summary>
    /// Linq操作帮助类
    /// </summary>
    public static class LinqHelper
    {
        static LinqHelper()
        {
            try
            {
                EFCore = Assembly.Load("Microsoft.EntityFrameworkCore");
                EFCoreNpgsql = Assembly.Load("Npgsql.EntityFrameworkCore.PostgreSQL");
            }
            catch
            {

            }
        }


        private static readonly Assembly EFCore;
        private static readonly Assembly EFCoreNpgsql;

        /// <summary>
        /// 创建初始条件为True的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return x => true;
        }

        /// <summary>
        /// 创建初始条件为False的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>()
        {
            return x => false;
        }

        /// <summary>
        /// 构建动态表达式
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, TResult>> BuildDynamicExpression<T, TResult>(string[] paramterNames, string expressionStr, params (string name, object value)[] variables)
        {
            var interpreter = new Interpreter();
            if (EFCore != null)
            {
                interpreter.Reference(EFCore.GetType("Microsoft.EntityFrameworkCore.EF"));
                interpreter.Reference(EFCore.GetType("Microsoft.EntityFrameworkCore.DbFunctionsExtensions"));
            }
            if (EFCoreNpgsql != null)
            {
                interpreter.Reference(EFCoreNpgsql.GetType("Microsoft.EntityFrameworkCore.NpgsqlDbFunctionsExtensions"));
            }
            foreach (var (name, value) in variables)
            {
                interpreter.SetVariable(name, value);
            }

            return interpreter.ParseAsExpression<Func<T, TResult>>(expressionStr, paramterNames);
        }
    }
}

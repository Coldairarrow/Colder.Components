using Colder.Dependency;
using EFCore.Sharding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colder.Extentions
{
    /// <summary>
    /// 使用事务包裹
    /// 注：必须使用在接口实现
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TransactionalAttribute : BaseAOPAttribute
    {
        private readonly IsolationLevel _isolationLevel;
        private readonly Type[] _dbImplementTypes;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbImplementTypes">数据库实现类型，必须继承于DbContext或IDbAccessor</param>
        /// <param name="isolationLevel">事务级别，默认ReadCommitted</param>
        public TransactionalAttribute(Type[] dbImplementTypes, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (dbImplementTypes.Any(x => !typeof(DbContext).IsAssignableFrom(x) && !typeof(IDbAccessor).IsAssignableFrom(x)))
            {
                throw new Exception("dbImplementTypes必须继承于DbContext或IDbAccessor");
            }

            _dbImplementTypes = dbImplementTypes;
            _isolationLevel = isolationLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Befor(IAOPContext context)
        {
            var container = context.ServiceProvider.GetService<TransactionContainer>();
            if (container == null)
            {
                throw new Exception("请调用AddAOPTransaction注入事务");
            }

            container.Depth++;

            if (!container.TransactionOpened)
            {
                container.TransactionOpened = true;

                container.DbContexts = _dbImplementTypes
                    .Where(x => typeof(DbContext).IsAssignableFrom(x))
                    .Select(x => (DbContext)context.ServiceProvider.GetService(x))
                    .ToList();

                container.DbAccessors = _dbImplementTypes
                    .Where(x => typeof(IDbAccessor).IsAssignableFrom(x))
                    .Select(x => (IDbAccessor)context.ServiceProvider.GetService(x))
                    .ToList();

                foreach (var aDbContext in container.DbContexts)
                {
                    await aDbContext.Database.BeginTransactionAsync(_isolationLevel);
                }
                foreach (var aDbAccessor in container.DbAccessors)
                {
                    await aDbAccessor.BeginTransactionAsync(_isolationLevel);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task After(IAOPContext context)
        {
            var container = context.ServiceProvider.GetService<TransactionContainer>();
            container.Depth--;

            try
            {
                if (container.TransactionOpened && container.Depth == 0)
                {
                    foreach (var aDbContext in container.DbContexts)
                    {
                        await aDbContext.Database.CurrentTransaction.CommitAsync();
                    }
                    foreach (var aDbAccessor in container.DbAccessors)
                    {
                        aDbAccessor.CommitTransaction();
                    }

                    container.TransactionOpened = false;
                }
            }
            catch
            {
                foreach (var aDbContext in container.DbContexts)
                {
                    await aDbContext.Database.CurrentTransaction.RollbackAsync();
                }
                foreach (var aDbAccessor in container.DbAccessors)
                {
                    aDbAccessor.RollbackTransaction();
                }

                throw;
            }
        }
    }

    internal class TransactionContainer
    {
        public bool TransactionOpened { get; set; }
        public List<DbContext> DbContexts { get; set; }
        public List<IDbAccessor> DbAccessors { get; set; }
        public int Depth { get; set; }
    }
}

using Colder.Common;
using Colder.Common.Helpers;
using Colder.Domain;
using EFCore.Sharding;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Colder.Infrastructure
{
    /// <summary>
    /// 数据库仓基类
    /// </summary>
    /// <typeparam name="TAggregateRoot">聚合根</typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class DbRepositoryBase<TAggregateRoot, TKey> : IRepositoryBase<TAggregateRoot, TKey>
        where TAggregateRoot : class, IAggregateRoot<TKey>
    {
        /// <summary>
        /// 数据库
        /// </summary>
        protected readonly IDbAccessor Db;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db">指定数据库</param>
        protected DbRepositoryBase(IDbAccessor db)
        {
            Db = db;
        }

        /// <summary>
        /// 主键字段
        /// </summary>
        protected virtual string KeyField => "Id";

        /// <summary>
        /// 获取IQueryable
        /// </summary>
        /// <returns></returns>
        protected virtual IQueryable<TAggregateRoot> GetQuery()
        {
            return Db.GetIQueryable<TAggregateRoot>().AsTracking().AsSingleQuery();
        }

        /// <summary>
        /// 从数据库获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual async Task<TAggregateRoot> GetDbData(TKey key)
        {
            return await GetQuery().Where($"{KeyField} == @0", key).AsTracking().FirstOrDefaultAsync();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<TAggregateRoot> Get(TKey key)
        {
            var dbData = await GetDbData(key);

            return dbData.DeepClone();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="aggregateRoot"></param>
        /// <returns></returns>
        public virtual async Task Add(TAggregateRoot aggregateRoot)
        {
            var dbData = aggregateRoot.DeepClone();
            await Db.InsertAsync(dbData, true);

            TrackingHelper.Tracking(aggregateRoot, dbData, null, null);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="aggregateRoot"></param>
        /// <returns></returns>
        public virtual async Task Update(TAggregateRoot aggregateRoot)
        {
            var dbData = await GetDbData(aggregateRoot.Id);

            TrackingHelper.Tracking(
                dbData,
                aggregateRoot,
                obj => Db.Entry(obj).State = EntityState.Added,
                obj => Db.Entry(obj).State = EntityState.Deleted
                );

            await Db.SaveChangesAsync();

            TrackingHelper.Tracking(aggregateRoot, dbData, null, null);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual async Task Remove(params TKey[] keys)
        {
            var deleteList = await GetQuery().Where($"@0.Contains({KeyField})", keys).ToListAsync();

            await Db.DeleteAsync(deleteList);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task Remove(params TAggregateRoot[] items)
        {
            await Db.DeleteAsync(items.ToList());
        }
    }
}

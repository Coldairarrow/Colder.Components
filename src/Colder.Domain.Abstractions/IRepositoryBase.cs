using System.Threading.Tasks;

namespace Colder.Domain
{
    /// <summary>
    /// 仓储基接口
    /// </summary>
    /// <typeparam name="TAggregateRoot">聚合根类型</typeparam>
    /// <typeparam name="TKey">聚合根标志类型</typeparam>
    public interface IRepositoryBase<TAggregateRoot, TKey> where TAggregateRoot : IAggregateRoot<TKey>
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key">唯一标志</param>
        /// <returns></returns>
        Task<TAggregateRoot> Get(TKey key);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="aggregateRoot">聚合根</param>
        /// <returns></returns>
        Task Add(TAggregateRoot aggregateRoot);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="aggregateRoot">聚合根</param>
        /// <returns></returns>
        Task Update(TAggregateRoot aggregateRoot);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">唯一标志</param>
        /// <returns></returns>
        Task Remove(params TKey[] id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="items">聚合根集合</param>
        /// <returns></returns>
        Task Remove(params TAggregateRoot[] items);
    }
}

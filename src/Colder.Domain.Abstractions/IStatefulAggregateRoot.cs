using System.Threading.Tasks;

namespace Colder.Domain
{
    /// <summary>
    /// 有状态聚合根
    /// </summary>
    /// <typeparam name="TState">状态</typeparam>
    /// <typeparam name="TKey">标志类型</typeparam>
    public interface IStatefulAggregateRoot<TState, TKey> where TState : IAggregateRoot<TKey>
    {
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <returns></returns>
        Task<TState> GetState();

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task SetState(TState state);
    }
}

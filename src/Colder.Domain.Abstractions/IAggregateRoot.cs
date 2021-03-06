namespace Colder.Domain
{
    /// <summary>
    /// 聚合根标志
    /// </summary>
    public interface IAggregateRoot<TKey>
    {
        /// <summary>
        /// 唯一标志
        /// </summary>
        public TKey Id { get; set; }
    }
}

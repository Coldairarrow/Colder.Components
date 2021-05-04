using System;

namespace Colder.DistributedId
{
    /// <summary>
    /// 分布式Id
    /// </summary>
    public interface IDistributedId
    {
        /// <summary>
        /// 生成有序Guid
        /// </summary>
        /// <returns></returns>
        public Guid NewGuid();
    }
}

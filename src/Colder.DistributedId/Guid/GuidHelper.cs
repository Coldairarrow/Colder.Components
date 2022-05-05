using System;
using System.Security.Cryptography;

namespace Colder.DistributedId
{
    /// <summary>
    /// Guid帮助类
    /// </summary>
    public static class GuidHelper
    {
        private static readonly RNGCryptoServiceProvider _randomGenerator = new RNGCryptoServiceProvider();

        /// <summary>
        /// 获取有序Guid
        /// 注:默认使用尾部序列(适用SQLServer),其它数据库请使用AtBegin
        /// </summary>
        /// <param name="guidType">序列类型</param>
        /// <returns></returns>
        public static Guid NewGuid(SequentialGuidType guidType = SequentialGuidType.AtEnd)
        {
            byte[] randomBytes = new byte[10];
            _randomGenerator.GetBytes(randomBytes);

            long timestamp = DateTime.UtcNow.Ticks / 10000L;

            // Then get the bytes
            byte[] timestampBytes = BitConverter.GetBytes(timestamp);

            // Since we're converting from an Int64, we have to reverse on
            // little-endian systems.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            byte[] guidBytes = new byte[16];

            switch (guidType)
            {
                case SequentialGuidType.AtBegin:
                    // For string and byte-array version, we copy the timestamp first, followed
                    // by the random data.
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                    // If formatting as a string, we have to compensate for the fact
                    // that .NET regards the Data1 and Data2 block as an Int32 and an Int16,
                    // respectively.  That means that it switches the order on little-endian
                    // systems.  So again, we have to reverse.
                    if (guidType == SequentialGuidType.AtBegin && BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(guidBytes, 0, 4);
                        Array.Reverse(guidBytes, 4, 2);
                    }

                    break;

                case SequentialGuidType.AtEnd:

                    // For sequential-at-the-end versions, we copy the random data first,
                    // followed by the timestamp.
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                    break;
            }

            return new Guid(guidBytes);
        }
    }

    /// <summary>
    /// 序列类型
    /// </summary>
    public enum SequentialGuidType
    {
        /// <summary>
        /// 首部排序,适用于非SQLServer
        /// </summary>
        AtBegin,

        /// <summary>
        /// 尾部排序,适用于SQLServer
        /// </summary>
        AtEnd
    }
}

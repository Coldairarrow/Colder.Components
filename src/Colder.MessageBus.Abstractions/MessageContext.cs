using System;
using System.Collections.Generic;

namespace Colder.MessageBus.Abstractions
{
    /// <summary>
    /// 消息上下文
    /// </summary>
    public class MessageContext
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public Guid? MessageId { get; set; }

        /// <summary>
        /// 当前容器
        /// </summary>
        public IServiceProvider  ServiceProvider { get; set; }

        /// <summary>
        /// 源地址
        /// </summary>
        public Uri SourceAddress { get; set; }

        /// <summary>
        /// 源机器名
        /// </summary>
        public string SourceMachineName { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        public Uri DestinationAddress { get; set; }

        /// <summary>
        /// 返回地址
        /// </summary>
        public Uri ResponseAddress { get; set; }

        /// <summary>
        /// 失败地址
        /// </summary>
        public Uri FaultAddress { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? SentTime { get; set; }

        /// <summary>
        /// 头部
        /// </summary>
        public Dictionary<string, object> Headers { get; set; }

        /// <summary>
        /// 消息体
        /// </summary>
        public string MessageBody { get; set; }

        /// <summary>
        /// 返回消息（仅用于请求）
        /// </summary>
        public object Response { get; set; }
    }
}

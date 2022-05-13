using System;
using System.Reflection;

namespace Colder.MessageBus.Abstractions
{
    /// <summary>
    /// 消息总线配置
    /// </summary>
    public class MessageBusOptions
    {
        /// <summary>
        /// 传输介质
        /// </summary>
        public TransportType Transport { get; set; } = TransportType.InMemory;

        /// <summary>
        /// 地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 发送消息超时时间,单位(秒)
        /// </summary>
        public int SendMessageTimeout { get; set; } = 30;

        /// <summary>
        /// 终结点，默认为入口程序集名
        /// </summary>
        public string Endpoint { get; set; } = Assembly.GetEntryAssembly().GetName().Name;

        /// <summary>
        /// 失败重试次数，默认3
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// 重试等待间隔（毫秒），默认1000
        /// </summary>
        public int RetryIntervalMilliseconds { get; set; } = 1000;

        private int _concurrency;

        /// <summary>
        /// 并发处理数（默认根据逻辑处理器数量自动分配,cpu线程数*2）
        /// </summary>
        public int Concurrency
        {
            get => _concurrency == 0 ? Environment.ProcessorCount * 2 : _concurrency;
            set => _concurrency = value;
        }

        /// <summary>
        /// IMessageHandler所在程序集，默认为入口程序集，作用于全局
        /// </summary>
        public static Assembly[] Assemblies { get; set; } = new Assembly[] { Assembly.GetEntryAssembly() };
    }
}

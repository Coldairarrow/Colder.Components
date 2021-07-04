using Colder.Common;
using Orleans.Configuration;
using System.Reflection;

namespace Colder.Orleans.Hosting
{
    /// <summary>
    /// Orelans参数
    /// </summary>
    public class OrleansOptions
    {
        private static readonly string _entryAssemblyName = Assembly.GetEntryAssembly().GetName().Name;

        /// <summary>
        /// 提供类型
        /// </summary>
        public ProviderTypes Provider { get; set; } = ProviderTypes.InMemory;

        /// <summary>
        /// AdoNet提供器
        /// </summary>
        public string AdoNetInvariant { get; set; }

        /// <summary>
        /// AdoNet链接字符串
        /// </summary>
        public string AdoNetConString { get; set; }

        private string _clusterId;
        /// <summary>
        /// 集群Id，默认入口程序集名
        /// </summary>
        public string ClusterId { get => string.IsNullOrEmpty(_clusterId) ? _entryAssemblyName : _clusterId; set => _clusterId = value; }

        private string _serviceId;
        /// <summary>
        /// 服务Id，默认入口程序集名
        /// </summary>
        public string ServiceId { get => string.IsNullOrEmpty(_serviceId) ? _entryAssemblyName : _serviceId; set => _serviceId = value; }

        private string _ip;
        /// <summary>
        /// 本机Ip，默认自动扫描获取本机Ip
        /// </summary>
        public string Ip { get => string.IsNullOrEmpty(_ip) ? IpHelper.GetLocalIp() : _ip; set => _ip = value; }

        /// <summary>
        /// Silo端口
        /// </summary>
        public int Port { get; set; } = EndpointOptions.DEFAULT_SILO_PORT;
    }
}

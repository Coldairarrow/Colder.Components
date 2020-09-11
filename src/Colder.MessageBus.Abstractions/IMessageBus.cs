﻿using System;
using System.Threading.Tasks;

namespace Colder.MessageBus.Abstractions
{
    /// <summary>
    /// 消息总线接口
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// 发布事件(广播)
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="message">消息</param>
        /// <returns></returns>
        Task Publish<T>(T message) where T : IEvent;

        /// <summary>
        /// 发送命令(单播)
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="message">消息</param>
        /// <param name="destination">指定消费节点</param>
        /// <returns></returns>
        Task Send<T>(T message, Uri destination) where T : ICommand;

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <typeparam name="TRequest">请求数据类型</typeparam>
        /// <typeparam name="TResponse">返回数据类型</typeparam>
        /// <param name="message">消息</param>
        /// <param name="destination">指定消费节点</param>
        /// <returns></returns>
        Task<TResponse> Request<TRequest, TResponse>(TRequest message, Uri destination)
            where TRequest : class, ICommand where TResponse : class;
    }
}

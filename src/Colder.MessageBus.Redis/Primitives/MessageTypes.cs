namespace Colder.MessageBus.Redis
{
    internal enum MessageTypes
    {
        /// <summary>
        /// 事件（广播）
        /// </summary>
        Event = 0,

        /// <summary>
        /// 命令（单播）
        /// </summary>
        Command = 1,

        /// <summary>
        /// 请求响应
        /// </summary>
        Response = 2
    }
}

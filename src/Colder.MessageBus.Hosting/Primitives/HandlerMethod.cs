using System;
using System.Linq;
using System.Reflection;

namespace Colder.MessageBus.Hosting.Primitives
{
    internal class HandlerMethod
    {
        public HandlerMethod(HandlerClass handlerClass, MethodInfo method)
        {
            HandlerClass = handlerClass;
            Method = method;
            MessageType = method.GetParameters()[0].ParameterType.GetGenericArguments()[0];
            ChildrenMessageTypes = Assembly.GetEntryAssembly().GetTypes()
                .Where(x => MessageType.IsAssignableFrom(x) && x != MessageType)
                .ToArray();

            AllMessageTypes = ChildrenMessageTypes.Concat(new Type[] { MessageType }).ToArray();
        }
        public HandlerClass HandlerClass { get; set; }
        public MethodInfo Method { get; }
        public Type MessageType { get; }
        public Type[] ChildrenMessageTypes { get; }
        public Type[] AllMessageTypes { get; set; }
    }
}

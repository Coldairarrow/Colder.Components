using Colder.CommonUtil;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting.Primitives;
using System;
using System.Linq;
using System.Reflection;

namespace Colder.MessageBus.Hosting
{
    internal static class Cache
    {
        static Cache()
        {
            Handlers = AssemblyHelper.AllTypes.Where(x =>
                 x.IsClass
                 && !x.IsAbstract
                 && x.GetInterfaces().Any(y =>
                     y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                ).Select(x => new HandlerClass(x))
                .ToArray();

            var handlerCounts = Handlers.SelectMany(x => x.HandlerMethods.Select(y => y.MessageType))
                .GroupBy(x => x)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToList();
            var repeatHandlers = handlerCounts.Where(x => x.Count > 1).ToList();
            if (repeatHandlers.Count > 0)
            {
                throw new Exception($"消息{string.Join(",", repeatHandlers.Select(x => x.Key.FullName))}有多个订阅者");
            }

            MessageTypes = Handlers.SelectMany(x => x.HandlerMethods.Select(y => y.MessageType)).ToArray();
            AllMessageTypes = Handlers.SelectMany(x => x.HandlerMethods.SelectMany(y => y.AllMessageTypes)).ToArray();
        }

        public static readonly HandlerClass[] Handlers;
        public static readonly Type[] MessageTypes;
        public static readonly Type[] AllMessageTypes;

        public static (Type realMessageType, Type handleMessageType, Type handlerType, MethodInfo handleMethod)
            GetHandler(string messageType)
        {
            var theMethod = Handlers.SelectMany(x => x.HandlerMethods)
                .Where(y => y.AllMessageTypes.Any(z => z.FullName == messageType))
                .FirstOrDefault();

            if (theMethod == null)
            {
                return default;
            }
            else
            {
                return (theMethod.AllMessageTypes.Where(x => x.FullName == messageType).FirstOrDefault(),
                    theMethod.MessageType, theMethod.HandlerClass.HanlderType, theMethod.Method);
            }
        }
    }
}

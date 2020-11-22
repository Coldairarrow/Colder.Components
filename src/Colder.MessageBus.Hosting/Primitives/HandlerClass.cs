using Colder.MessageBus.Abstractions;
using System;
using System.Linq;

namespace Colder.MessageBus.Hosting.Primitives
{
    internal class HandlerClass
    {
        public HandlerClass(Type hanlderType)
        {
            HanlderType = hanlderType;

            HandlerMethods = hanlderType.GetMethods().Where(x =>
                      x.Name == "Handle"
                      && x.GetParameters().Length == 1
                      && x.GetParameters()[0].ParameterType.IsGenericType
                      && x.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(MessageContext<>)
                    ).Select(x => new HandlerMethod(this, x))
                    .ToArray();
        }

        public Type HanlderType { get; }
        public HandlerMethod[] HandlerMethods { get; }
    }
}

﻿using Colder.MessageBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Colder.MessageBus.Hosting
{
    internal static class Cache
    {
        static Cache()
        {
            HanlderTypes = Assembly.GetEntryAssembly().GetTypes().Where(x =>
                 x.IsClass
                 && !x.IsAbstract
                 && x.GetInterfaces().Any(y =>
                     y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                ).ToList();

            MessageTypes = HanlderTypes
                .SelectMany(x => x.GetInterfaces())
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                .Select(x => x.GetGenericArguments()[0])
                .Distinct()
                .ToList();

            MessageTypes.ForEach(aMessageType =>
            {
                var interfaceType = typeof(IMessageHandler<>).MakeGenericType(aMessageType);
                var handlers = HanlderTypes.Where(x => interfaceType.IsAssignableFrom(x))
                    .ToArray();
                if (handlers.Length > 1)
                {
                    throw new Exception($"消息{aMessageType.Name}有多个订阅者:{string.Join(",", handlers.Select(x => x.Name))}");
                }
                Message2Handler.Add(aMessageType, handlers[0]);
            });
        }

        public static readonly List<Type> HanlderTypes = new List<Type>();
        public static readonly List<Type> MessageTypes = new List<Type>();
        public static readonly Dictionary<Type, Type> Message2Handler = new Dictionary<Type, Type>();
    }
}
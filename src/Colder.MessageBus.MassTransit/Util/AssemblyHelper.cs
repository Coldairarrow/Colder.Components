using Colder.MessageBus.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Colder.MessageBus.MassTransit
{
    internal static class AssemblyHelper
    {
        static AssemblyHelper()
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var assemblies = Directory.GetFiles(rootPath, "*.dll")
                .Where(x => !new FileInfo(x).Name.StartsWith("System")
                    && !new FileInfo(x).Name.StartsWith("Microsoft"))
                .Select(x => Assembly.LoadFrom(x))
                .Where(x => !x.IsDynamic)
                .ToList();

            assemblies.ForEach(aAssembly =>
            {
                try
                {
                    AllTypes.AddRange(aAssembly.GetTypes());
                }
                catch
                {

                }
            });


            HanlderTypes = AllTypes.Where(x =>
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
        }

        public static readonly List<Type> AllTypes = new List<Type>();

        public static readonly List<Type> HanlderTypes = new List<Type>();

        public static readonly List<Type> MessageTypes = new List<Type>();
    }
}

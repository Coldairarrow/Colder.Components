using Colder.MessageBus.Abstractions;
using System;
using System.Reflection;

namespace Colder.MessageBus.Hosting
{
    internal static class MessageBusFactory
    {
        public static IMessageBus GetBusInstance(IServiceProvider serviceProvider, MessageBusOptions options)
        {
            AbstractProvider provider;

            string assemblyName = $"Colder.MessageBus.{options.Transport}";
            try
            {
                Assembly assembly = Assembly.Load(assemblyName);

                var type = assembly.GetType($"{assemblyName}.{options.Transport}Provider");

                provider = Activator.CreateInstance(type, new object[] { serviceProvider, options }) as AbstractProvider;
            }
            catch
            {
                throw new Exception($"请安装nuget包:{assemblyName}");
            }

            return provider.GetBusInstance();
        }
    }
}

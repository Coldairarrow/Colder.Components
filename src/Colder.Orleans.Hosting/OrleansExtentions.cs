using Colder.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Reflection;

namespace Colder.Orleans.Hosting
{
    /// <summary>
    /// Orleans拓展
    /// </summary>
    public static class OrleansExtentions
    {
        /// <summary>
        /// 配置Orleans
        /// </summary>
        /// <param name="hostBuilder">建造者</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureOrleansDefaults(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .UseOrleans((hostContext, builder) =>
                {
                    OrleansOptions orleansOptions = hostContext.Configuration.GetSection("orleans").Get<OrleansOptions>();

                    switch (orleansOptions.Provider)
                    {
                        case ProviderTypes.InMemory:
                            {
                                builder
                                   .UseLocalhostClustering()
                                   .AddMemoryGrainStorageAsDefault();
                            }
                            break;
                        case ProviderTypes.AdoNet:
                            {
                                builder
                                    .UseAdoNetClustering(options =>
                                    {
                                        options.ConnectionString = orleansOptions.AdoNetConString;
                                        options.Invariant = orleansOptions.AdoNetInvariant;
                                    })
                                    .AddAdoNetGrainStorageAsDefault(options =>
                                    {
                                        options.ConnectionString = orleansOptions.AdoNetConString;
                                        options.Invariant = orleansOptions.AdoNetInvariant;
                                    });
                            }; break;
                        default: throw new Exception($"{orleansOptions.Provider}无效");
                    }

                    builder
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = orleansOptions.ClusterId;
                            options.ServiceId = orleansOptions.ServiceId;
                        })
                        .Configure<ClusterMembershipOptions>(options =>
                        {
                            //及时下线
                            options.ProbeTimeout = TimeSpan.FromSeconds(3);
                            options.IAmAliveTablePublishTimeout = TimeSpan.FromSeconds(3);
                            options.NumVotesForDeathDeclaration = 1;
                        })
                        .Configure<EndpointOptions>(options =>
                        {
                            if (orleansOptions.Provider != ProviderTypes.InMemory)
                            {
                                options.AdvertisedIPAddress = IPAddress.Parse(orleansOptions.Ip);
                            }

                            options.SiloPort = orleansOptions.Port;
                        })
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(Assembly.GetEntryAssembly()).WithReferences());
                });
        }
    }
}

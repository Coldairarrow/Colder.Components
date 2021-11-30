using Colder.WebSockets.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Demo.WebSockets.Server
{
    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddWebSocketServer(x =>
            {
                x.OnConnected = async (serviceProvider, connection) =>
                {
                    connection.Id = Guid.NewGuid().ToString();
                    await Task.CompletedTask;
                };
                x.OnReceive = async (serviceProvider, connection, msg) =>
                {
                    var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger(GetType());
                    logger.LogInformation("{Time} 收到来自 {Id} 的消息:{Msg}", DateTime.Now, connection.Id, msg);
                    await Task.CompletedTask;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSocketServer();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

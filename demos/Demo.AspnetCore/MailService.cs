using Colder.Mail;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.AspnetCore
{
    public class MailService : BackgroundService
    {
        private readonly ILoggerFactory _loggerFactory;
        public MailService(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IdleClient idleClient = new IdleClient("imap.qq.com", 993, "abc@qq.com", "zbxhsiwwywlrbfhh", _loggerFactory);
            idleClient.OnNewMessage = async uids =>
            {
                Console.WriteLine(string.Join(",", uids));
            };
            await idleClient.RunAsync();
        }
    }
}
